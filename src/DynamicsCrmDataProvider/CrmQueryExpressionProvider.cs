using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;

namespace DynamicsCrmDataProvider
{
    public class CrmQueryExpressionProvider : ICrmQueryExpressionProvider
    {
        /// <summary>
        /// Creates a QueryExpression from the given Select command.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public QueryExpression CreateQueryExpression(SelectBuilder builder)
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

            // This is the entity being selected.
            var fromTable = (Table)((AliasedSource)builder.From.First()).Source;
            var firstEntityName = fromTable.Name.ToLower();

            // detect all columns..
            var query = new QueryExpression(firstEntityName);
            query.ColumnSet = new ColumnSet();
            if (builder.Projection.Count() == 1)
            {
                var column = builder.Projection.First().ProjectionItem;
                if (column is AllColumns)
                {
                    query.ColumnSet.AllColumns = true;
                }
                else
                {
                    query.ColumnSet.AddColumn(column.GetProjectionName());
                }
            }
            else
            {
                foreach (var projection in builder.Projection)
                {
                    query.ColumnSet.AddColumn(projection.ProjectionItem.GetProjectionName());
                }
            }

            // do where clause..
            if (builder.Where != null && builder.Where.Any())
            {
                //TODO: Should only be one where clause?
                foreach (var where in builder.Where)
                {

                    Column column = null;
                    var condition = new ConditionExpression();
                    bool isColumnLeft = false;

                    var filter = where as OrderFilter;
                    if (filter != null)
                    {
                        var left = filter.LeftHand;
                        var right = filter.RightHand;
                        column = left as Column;
                        if (column != null)
                        {
                            isColumnLeft = true;
                            condition.AttributeName = column.Name.ToLower();
                        }
                        else
                        {
                            column = right as Column;
                            if (column != null)
                            {
                                condition.AttributeName = column.Name.ToLower();
                            }
                        }

                        // Support Equals
                        var equalTo = where as EqualToFilter;
                        if (equalTo != null)
                        {
                            condition.Operator = ConditionOperator.Equal;
                            if (column == null)
                            {
                                throw new InvalidOperationException("The query contains a WHERE clause with an Equals condition, however one side of the Equals condition must refer to an attribute name.");
                            }

                            Literal lit = null;
                            if (isColumnLeft)
                            {
                                lit = right as Literal;
                            }
                            else
                            {
                                lit = left as Literal;
                            }

                            if (lit != null)
                            {
                                object litVal = GitLiteralValue(lit);
                                condition.Values.Add(litVal);
                                query.Criteria.Conditions.Add(condition);
                                continue;
                            }

                            throw new NotSupportedException();

                        }

                        // Support Not Equals
                        var notEqualTo = where as NotEqualToFilter;
                        if (notEqualTo != null)
                        {
                            condition.Operator = ConditionOperator.NotEqual;
                            if (column == null)
                            {
                                throw new InvalidOperationException("The query contains a WHERE clause with an Equals condition, however one side of the Equals condition must refer to an attribute name.");
                            }

                            Literal lit = null;
                            if (isColumnLeft)
                            {
                                lit = right as Literal;
                            }
                            else
                            {
                                lit = left as Literal;
                            }

                            if (lit != null)
                            {
                                object litVal = GitLiteralValue(lit);
                                condition.Values.Add(litVal);
                                query.Criteria.Conditions.Add(condition);
                                continue;
                            }

                            throw new NotSupportedException();

                        }

                    }

                    throw new NotSupportedException();
                }
            }
            return query;
        }

        private object GitLiteralValue(Literal lit)
        {
            // Support string literals.
            StringLiteral stringLit = null;
            NumericLiteral numberLiteral = null;

            stringLit = lit as StringLiteral;
            if (stringLit != null)
            {
                return stringLit.Value;
            }

            numberLiteral = lit as NumericLiteral;
            if (numberLiteral != null)
            {
                return numberLiteral.Value;
            }

            throw new NotSupportedException("Unknown Literal");

        }

    }
}