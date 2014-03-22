using System;
using System.Collections.Generic;
using System.Linq;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;

namespace CrmAdo
{
    public class BetterCrmQueryExpressionProvider : ICrmQueryExpressionProvider
    {

        #region NewStuff

        public static QueryExpression FromCommand(ICommand command)
        {
            var query = new QueryExpression();
            //  query.EntityName
            if (command is SelectBuilder)
            {
                var selCommand = command as SelectBuilder;
                AddFrom(selCommand.From, query);
                AddColumns(selCommand.Projection, query);
            }
            return query;
        }

        public static void AddFrom(IEnumerable<IJoinItem> @from, QueryExpression query)
        {
            foreach (var f in from)
            {
                if (f != null)
                {
                    if (f is Join)
                    {
                        AddJoin(f as Join, query);
                    }
                    else if (f is AliasedSource)
                    {
                        // only reached if no Joins used in select query.
                        var aliasedSource = f as AliasedSource;
                        if (aliasedSource.Source.IsTable)
                        {
                            var table = aliasedSource.Source as Table;
                            query.EntityName = GetTableLogicalEntityName(table);
                        }
                        else
                        {
                            throw new NotSupportedException("The From keyword must be used in conjunction with a table / entity name. Subqueries not supported.");
                        }

                    }
                }
            }
        }
        
        public static void AddJoin(Join @join, QueryExpression query)
        {
            if (join != null)
            {

                var binaryJoin = join as BinaryJoin;
                var linkEntity = new LinkEntity();
                bool isFirstFrom = false;
                if (binaryJoin != null)
                {
                    // Recurse to the left..

                    if (binaryJoin.LeftHand != null)
                    {
                        if (@join.GetType().Name == "JoinStart")
                        {
                            // this is the very far left hand the join line!
                            isFirstFrom = true;
                        }

                        if (!isFirstFrom)
                        {
                            // There is another join expression to the left - recurse to process that one first..

                            AddJoin(binaryJoin.LeftHand, query);
                        }
                        else
                        {
                            // the first one is special..

                        }
                    }

                    // Are we an inner or outer join?
                    var innerJoin = binaryJoin as InnerJoin;
                    if (innerJoin != null)
                    {
                        linkEntity.JoinOperator = JoinOperator.Inner;
                    }
                    else
                    {
                        var outer = binaryJoin as LeftOuterJoin;
                        if (outer != null)
                        {
                            linkEntity.JoinOperator = JoinOperator.LeftOuter;
                        }
                        else
                        {
                            throw new NotSupportedException("Join type not supported.");
                        }
                    }

                    if (binaryJoin.RightHand != null)
                    {
                        // This is what we are joining on to..
                        AliasedSource asource = binaryJoin.RightHand;
                        if (!string.IsNullOrEmpty(asource.Alias))
                        {
                            linkEntity.EntityAlias = asource.Alias;
                        }
                    }

                }
                var filteredJoin = join as FilteredJoin;
                if (filteredJoin != null)
                {
                    //   stringBuilder.AppendLine(string.Format("{0} On filters:", indent));

                    Column leftColumn = null;
                    Column rightColumn = null;
                    //string leftTableAlias = leftTable
                    Table leftTable = null;
                    Table rightTable = null;
                    int onCount = 0;
                    string leftEntityName = string.Empty;

                    foreach (var on in filteredJoin.OnFilters)
                    {
                        onCount++;
                        // Support Equals
                        var equalTo = on as EqualToFilter;
                        if (equalTo != null)
                        {
                            leftColumn = equalTo.LeftHand as Column;
                            rightColumn = equalTo.RightHand as Column;
                            GuardOnColumn(leftColumn);
                            GuardOnColumn(rightColumn);

                            leftTable = leftColumn.Source.Source as Table;
                            rightTable = rightColumn.Source.Source as Table;
                            leftEntityName = GetTableLogicalEntityName(leftTable);

                            if (isFirstFrom)
                            {
                                // The left table of the first from statement is the main entity
                                query.EntityName = leftEntityName;
                                // detect all columns..
                                // query.ColumnSet = GetColumnSet(projectionItems);
                            }
                            else
                            {
                                linkEntity.LinkFromEntityName = leftEntityName;
                                linkEntity.LinkToEntityName = GetTableLogicalEntityName(rightTable);
                                linkEntity.LinkFromAttributeName = GetColumnLogicalAttributeName(leftColumn);
                                linkEntity.LinkToAttributeName = GetColumnLogicalAttributeName(rightColumn);
                            }
                        }
                        else
                        {
                            throw new NotSupportedException("When using an ON condition, only Equal To operator is supported. For example: INNER JOIN X ON Y.ID = X.ID");
                        }
                        //  stringBuilder.AppendLine(string.Format("{0}  Filter Type: {1}", indent, on.GetType().FullName));
                    }

                    if (onCount > 1)
                    {
                        throw new NotSupportedException("You can only use one ON condition. For example: INNER JOIN X ON Y.ID = X.ID is supported, but INNER JOIN X ON Y.ID = X.ID AND Y.NAME = X.NAME is not supported.");
                    }

                    // add the right columns to the link entities


                    if (!isFirstFrom)
                    {
                        // need to add the link entity onto the query - or onto the appropriate link entity in the chain..
                        // if the left entity name has a link entity in the query, then 
                        LinkEntity existing = null;
                        if (!query.LinkEntities.Any())
                        {
                            if (query.EntityName != linkEntity.LinkFromEntityName)
                            {
                                throw new InvalidOperationException("The first JOIN in the query must link from the main entity which in your case is " + linkEntity + " but the first join in your query links from: " + linkEntity.LinkFromEntityName + " which is an unknown entity,");
                            }
                            query.LinkEntities.Add(linkEntity);
                        }
                        else
                        {

                            bool isAliased = !string.IsNullOrEmpty(leftColumn.Source.Alias);
                            var match = string.IsNullOrEmpty(leftColumn.Source.Alias)
                                               ? leftEntityName
                                               : leftColumn.Source.Alias;


                            var leftLink = query.FindLinkEntity(match, isAliased);
                            if (leftLink == null)
                            {
                                throw new InvalidOperationException("Could not perform join, as " + match + " is an unknown entity.");
                            }

                            leftLink.LinkEntities.Add(linkEntity);
                        }
                    }
                }
            }
        }

        private static void GuardOnColumn(Column column)
        {
            if (column == null)
            {
                throw new NotSupportedException("The ON operator used in the Join statement must have a column name on it's left and right side.");
            }
            if (column.Source == null || column.Source.Source == null)
            {
                throw new NotSupportedException("No column source found for column: " + column.Name + " do you need to prefix that with the table name?");
            }
            if (!column.Source.Source.IsTable)
            {
                throw new NotSupportedException("The ON operator used in the Join statement has a column name that is not from an entity table. Column Name: " + column.Name);
            }
        }

        private static string GetColumnLogicalAttributeName(Column column)
        {
            return column.Name.ToLower();
        }

        private static string GetTableLogicalEntityName(Table table)
        {
            return table.Name.ToLower();
        }

        private static void AddColumns(IEnumerable<AliasedProjection> projection, QueryExpression query)
        {
            if (projection.Count() == 1)
            {
                var aliasedColumn = projection.First().ProjectionItem;
                if (aliasedColumn is AllColumns)
                {
                    query.IncludeAllColumns();
                    return;
                }
            }
            // var projItemsBySource = projection.Where(p=>p.ProjectionItem is Column && ((Column)p.ProjectionItem).Source. ).Where(p=>(.Source)
            foreach (var projItem in projection)
            {
                AddColumn(projItem, query);
            }
        }

        private static void AddColumn(AliasedProjection projection, QueryExpression query)
        {
            string colAlias = projection.Alias;
            if (!string.IsNullOrEmpty(colAlias))
            {
                throw new NotSupportedException("Column aliases are not supported.");
            }

            var col = projection.ProjectionItem as Column;
            if (col != null)
            {
                bool isAliasedSource = !string.IsNullOrEmpty(col.Source.Alias);
                var sourceName = string.IsNullOrEmpty(col.Source.Alias)
                                     ? GetTableLogicalEntityName(col.Source.Source as Table)
                                     : col.Source.Alias;
                var linkItem = query.FindLinkEntity(sourceName, isAliasedSource);
                if (linkItem == null)
                {
                    throw new InvalidOperationException("Could not find source named: " + sourceName);
                }
                linkItem.Columns.AddColumn(GetColumnLogicalAttributeName(col));
            }
            else
            {
                throw new NotSupportedException("Only columns that are attribute names are supported.");
            }
        }

        #endregion

        public QueryExpression CreateQueryExpression(CrmDbCommand command)
        {
            throw new NotImplementedException();
        }
    }
}