using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace DynamicsCrmDataProvider
{
    public class CrmQueryExpressionProvider : ICrmQueryExpressionProvider
    {
        /// <summary>
        /// Creates a QueryExpression from the given Select command.
        /// </summary>
        /// <returns></returns>
        public QueryExpression CreateQueryExpression(CrmDbCommand command)
        {
            var commandText = command.CommandText;
            var commandBuilder = new CommandBuilder();
            var builder = commandBuilder.GetCommand(commandText) as SelectBuilder;
            GuardSelectBuilder(builder);
            var query = FromCommand(builder);
            return query;
        }

        public static QueryExpression FromCommand(ICommand command)
        {
            var query = new QueryExpression();
            //  query.EntityName
            if (command is SelectBuilder)
            {
                var selCommand = command as SelectBuilder;
                AddFrom(selCommand.From, query);
                AddColumns(selCommand.Projection, query);
                AddWhere(selCommand.Where, query);
            }
            return query;
        }

        #region Where and Filters

        private static void AddWhere(IEnumerable<IFilter> whereFilter, QueryExpression query)
        {
            // do where clause..
            if (whereFilter != null && whereFilter.Any())
            {
                //TODO: Should only be one where clause?
                foreach (var where in whereFilter)
                {
                    var condition = new ConditionExpression();
                    var orderFilter = where as OrderFilter;
                    if (orderFilter != null)
                    {
                        ProcessOrderFilter(query, condition, orderFilter);
                        continue;
                    }

                    var nullFilter = where as NullFilter;
                    if (nullFilter != null)
                    {
                        ProcessNullFilter(query, condition, nullFilter);
                        continue;
                    }

                    var likeFilter = where as LikeFilter;
                    if (likeFilter != null)
                    {
                        ProcessLikeFilter(query, condition, likeFilter);
                        continue;
                    }

                    var inFilter = where as InFilter;
                    if (inFilter != null)
                    {
                        ProcessInFilter(query, condition, inFilter);
                        continue;
                    }

                    throw new NotSupportedException();
                }
            }
        }

        private static void ProcessInFilter(QueryExpression query, ConditionExpression condition, InFilter filter)
        {
            // Support Like
            var left = filter.LeftHand;
            var leftcolumn = left as Column;

            // defaullt attribute name for the filter condition.
            if (leftcolumn != null)
            {
                condition.AttributeName = leftcolumn.Name.ToLower();
            }
            else
            {
                throw new NotSupportedException("IN operator only works agains a column value.");
            }

            var conditionOperator = ConditionOperator.In;
            if (filter.Not)
            {
                conditionOperator = ConditionOperator.NotIn;
            }
            var values = filter.Values;

            if (values.IsValueList)
            {
                var valuesList = values as ValueList;

                if (valuesList != null)
                {
                    var inValues = new object[valuesList.Values.Count()];
                    int index = 0;
                    foreach (var item in valuesList.Values)
                    {
                        var literal = item as Literal;
                        if (literal == null)
                        {
                            throw new ArgumentException("The values list must contain literals.");
                        }
                        inValues[index] = GitLiteralValue(literal);
                        index++;
                    }
                    SetConditionExpressionValue(condition, conditionOperator, inValues);
                    AddConditionExpressionToQuery(leftcolumn.Source, query, condition);
                    return;
                }
                throw new ArgumentException("The values list for the IN expression is null");
            }
            throw new NotSupportedException();
        }

        private static void ProcessNullFilter(QueryExpression query, ConditionExpression condition, NullFilter nullFilter)
        {

            var left = nullFilter.LeftHand;
            var leftcolumn = left as Column;

            // defaullt attribute name for the filter condition.
            if (leftcolumn != null)
            {
                condition.AttributeName = leftcolumn.Name.ToLower();
            }
            else
            {
                throw new NotSupportedException("Null operator only works agains a column value.");
            }

            var conditionOperator = ConditionOperator.Null;
            if (nullFilter.Not)
            {
                conditionOperator = ConditionOperator.NotNull;
            }

            SetConditionExpressionValue(condition, conditionOperator);
            AddConditionExpressionToQuery(leftcolumn.Source, query, condition);
        }

        private static void ProcessOrderFilter(QueryExpression query, ConditionExpression condition, OrderFilter filter)
        {
            bool isColumnLeft = false;

            var left = filter.LeftHand;
            var right = filter.RightHand;
            var leftcolumn = left as Column;
            var rightcolumn = right as Column;

            if (leftcolumn != null)
            {
                isColumnLeft = true;
            }

            Column firstColumn = leftcolumn ?? rightcolumn;

            // defaullt attribute name for the filter condition.
            if (firstColumn != null)
            {
                condition.AttributeName = firstColumn.Name.ToLower();
            }

            // Support Equals
            var equalTo = filter as EqualToFilter;
            if (equalTo != null)
            {
                SetColumnCondition(condition, ConditionOperator.Equal, filter, firstColumn, isColumnLeft);
                AddConditionExpressionToQuery(firstColumn.Source, query, condition);
                return;
            }

            // Support Not Equals
            var notEqualTo = filter as NotEqualToFilter;
            if (notEqualTo != null)
            {
                SetColumnCondition(condition, ConditionOperator.NotEqual, filter, firstColumn, isColumnLeft);
                AddConditionExpressionToQuery(firstColumn.Source, query, condition);
                return;
            }

            // Support Greater Than
            var greaterThan = filter as GreaterThanFilter;
            if (greaterThan != null)
            {
                SetColumnCondition(condition, ConditionOperator.GreaterThan, filter, firstColumn, isColumnLeft);
                AddConditionExpressionToQuery(firstColumn.Source, query, condition);
                return;
            }

            // Support Greater Than Equal
            var greaterEqual = filter as GreaterThanEqualToFilter;
            if (greaterEqual != null)
            {
                SetColumnCondition(condition, ConditionOperator.GreaterEqual, filter, firstColumn, isColumnLeft);
                AddConditionExpressionToQuery(firstColumn.Source, query, condition);
                return;
            }

            // Support Less Than
            var lessThan = filter as LessThanFilter;
            if (lessThan != null)
            {
                SetColumnCondition(condition, ConditionOperator.LessThan, filter, firstColumn, isColumnLeft);
                AddConditionExpressionToQuery(firstColumn.Source, query, condition);
                return;
            }

            // Support Less Than Equal
            var lessThanEqual = filter as LessThanEqualToFilter;
            if (lessThanEqual != null)
            {
                SetColumnCondition(condition, ConditionOperator.LessEqual, filter, firstColumn, isColumnLeft);
                AddConditionExpressionToQuery(firstColumn.Source, query, condition);
                return;
            }

        }

        private static void ProcessLikeFilter(QueryExpression query, ConditionExpression condition, LikeFilter filter)
        {

            // Support Like
            var left = filter.LeftHand;
            var leftcolumn = left as Column;

            // defaullt attribute name for the filter condition.
            if (leftcolumn != null)
            {
                condition.AttributeName = leftcolumn.Name.ToLower();
            }
            else
            {
                throw new NotSupportedException("Null operator only works agains a column value.");
            }

            // detect like expressions for begins with, ends with and contains..

            var val = filter.RightHand.Value;
            bool startsWith = val.EndsWith("%");
            bool endsWith = val.StartsWith("%");

            ConditionOperator conditionoperator;

            if (startsWith)
            {
                if (endsWith)
                {
                    // contains
                    if (filter.Not)
                    {
                        // Does Not contain operator not recognised by Xrm sdk??? 
                       // conditionoperator = ConditionOperator.DoesNotContain;
                        conditionoperator = ConditionOperator.NotLike;
                    }
                    else
                    {
                       //  Contains operator causes Xrm organisation servuce to throw "Generic SQL error"??? 
                        conditionoperator = ConditionOperator.Like;
                    }
                   // val = filter.RightHand.Value.Trim('%');
                }
                else
                {
                    // starts with
                    val = filter.RightHand.Value.TrimEnd('%');
                    if (filter.Not)
                    {
                        conditionoperator = ConditionOperator.DoesNotBeginWith;
                    }
                    else
                    {
                        conditionoperator = ConditionOperator.BeginsWith;
                    }
                }
            }
            else if (endsWith)
            {
                // ends with;
                // contains
                val = filter.RightHand.Value.TrimStart('%');
                if (filter.Not)
                {
                    conditionoperator = ConditionOperator.DoesNotEndWith;
                }
                else
                {
                    conditionoperator = ConditionOperator.EndsWith;
                }
            }
            else
            {
                if (filter.Not)
                {
                    conditionoperator = ConditionOperator.NotLike;
                }
                else
                {
                    conditionoperator = ConditionOperator.Like;
                }

            }
            SetConditionExpressionValue(condition, conditionoperator, val);
            AddConditionExpressionToQuery(leftcolumn.Source, query, condition);
        }


        #endregion

        #region From and Joins
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
                        if (binaryJoin.LeftHand.ToString() == "SQLGeneration.Builders.JoinStart")
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

                            linkEntity.LinkFromEntityName = leftEntityName;
                            linkEntity.LinkToEntityName = GetTableLogicalEntityName(rightTable);
                            linkEntity.LinkFromAttributeName = GetColumnLogicalAttributeName(leftColumn);
                            linkEntity.LinkToAttributeName = GetColumnLogicalAttributeName(rightColumn);

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

                    // need to add the link entity onto the query - or onto the appropriate link entity in the chain..
                    // if the left entity name has a link entity in the query, then 
                    LinkEntity existing = null;
                    if (!query.LinkEntities.Any() || isFirstFrom)
                    {
                        if (query.EntityName != linkEntity.LinkFromEntityName)
                        {
                            throw new InvalidOperationException(
                                "The first JOIN in the query must link from the main entity which in your case is " +
                                linkEntity + " but the first join in your query links from: " +
                                linkEntity.LinkFromEntityName + " which is an unknown entity,");
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
        #endregion

        #region Select and Projections
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
                    query.ColumnSet.AddColumn(GetColumnLogicalAttributeName(col));
                    // wehn we can;t find the source, assume default entity..
                    //   throw new InvalidOperationException("Could not find source named: " + sourceName);
                }
                else
                {
                    linkItem.Columns.AddColumn(GetColumnLogicalAttributeName(col));
                }

            }
            else
            {
                throw new NotSupportedException("Only columns that are attribute names are supported.");
            }
        }

        #endregion

        #region Util

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

        // ReSharper disable UnusedParameter.Local
        // This method is solely for pre condition checking.
        private static void GuardSelectBuilder(SelectBuilder builder)
        // ReSharper restore UnusedParameter.Local
        {
            if (builder == null)
            {
                throw new InvalidOperationException("Command Text must be a Select statement.");
            }
            if (!builder.From.Any())
            {
                throw new InvalidOperationException("The select statement must include a From clause.");
            }
            if (builder.From.Count() > 1)
            {
                throw new NotSupportedException("The select statement must select from a single entity.");
            }
            if (!builder.Projection.Any())
            {
                throw new InvalidOperationException("The select statement must select atleast 1 attribute.");
            }
        }

        private static object GitLiteralValue(Literal lit)
        {
            // Support string literals.
            StringLiteral stringLit = null;
            NumericLiteral numberLiteral = null;

            stringLit = lit as StringLiteral;
            if (stringLit != null)
            {
                // cast to GUID?
                Guid val;
                if (Guid.TryParse(stringLit.Value, out val))
                {
                    return val;
                }
                else
                {
                    return stringLit.Value;
                }
            }

            numberLiteral = lit as NumericLiteral;
            if (numberLiteral != null)
            {
                // cast down from double if possible..
                checked
                {
                    try
                    {
                        int intValue = (int)numberLiteral.Value;
                        return intValue;
                    }
                    catch (OverflowException)
                    {
                        //   can't down cast to int so remain as double.
                        return numberLiteral.Value;
                    }
                }

            }

            throw new NotSupportedException("Unknown Literal");

        }

        private static void AddConditionExpressionToQuery(AliasedSource source, QueryExpression query, ConditionExpression condition)
        {
            if (source != null)
            {
                bool isAliasedSource = !string.IsNullOrEmpty(source.Alias);
                var sourceName = string.IsNullOrEmpty(source.Alias) ? GetTableLogicalEntityName(source.Source as Table) : source.Alias;
                var linkItem = query.FindLinkEntity(sourceName, isAliasedSource);
                if (linkItem == null)
                {
                    // Assume criteria is not for link entity but for main entity.
                    query.Criteria.Conditions.Add(condition);
                }
                else
                {
                    linkItem.LinkCriteria.Conditions.Add(condition);
                }
            }
            else
            {
                throw new NotSupportedException("A condition in the WHERE clause contains refers to an unknown table / entity.");
            }
        }

        private static void SetColumnCondition(ConditionExpression condition, ConditionOperator conditionOperator, OrderFilter greaterThan, Column column, bool isColumnLeft)
        {

            //  condition.Operator = conditionOperator;
            if (column == null)
            {
                throw new InvalidOperationException("The query contains a WHERE clause, however one side of the where condition must refer to an attribute name.");
            }
            Literal lit = null;
            if (isColumnLeft)
            {
                lit = greaterThan.RightHand as Literal;
            }
            else
            {
                lit = greaterThan.LeftHand as Literal;
            }

            if (lit != null)
            {
                object litVal = GitLiteralValue(lit);
                SetConditionExpressionValue(condition, conditionOperator, litVal);
                return;
            }

            throw new NotSupportedException();
        }

        private static void SetConditionExpressionValue(ConditionExpression condition, ConditionOperator conditionOperator, params object[] values)
        {
            condition.Operator = conditionOperator;
            if (values != null)
            {
                // is the literal a 
                foreach (var value in values)
                {
                    if (value is Array)
                    {
                        foreach (var o in value as Array)
                        {
                            condition.Values.Add(o);
                        }
                    }
                    else
                    {
                        condition.Values.Add(value);
                    }
                }
            }
        }
        
        #endregion

    }
}