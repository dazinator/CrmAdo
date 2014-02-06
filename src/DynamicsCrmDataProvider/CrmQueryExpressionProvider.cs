using System;
using System.Collections;
using System.Collections.Generic;
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
            GuardSelectBuilder(builder);

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
            return query;
        }

        private void ProcessInFilter(QueryExpression query, ConditionExpression condition, InFilter filter)
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
                    AppendColumnCondition(condition, conditionOperator, filter, leftcolumn, inValues);
                    query.Criteria.Conditions.Add(condition);
                    return;
                }
                throw new ArgumentException("The values list for the IN expression is null");
            }
            throw new NotSupportedException();
        }

        private void ProcessNullFilter(QueryExpression query, ConditionExpression condition, NullFilter nullFilter)
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

            AppendColumnCondition(condition, conditionOperator, nullFilter, leftcolumn);
            query.Criteria.Conditions.Add(condition);
            return;
        }

        private void ProcessOrderFilter(QueryExpression query, ConditionExpression condition, OrderFilter filter)
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
                AppendColumnConditionWithValue(condition, ConditionOperator.Equal, filter, firstColumn, isColumnLeft);
                query.Criteria.Conditions.Add(condition);
                return;
            }

            // Support Not Equals
            var notEqualTo = filter as NotEqualToFilter;
            if (notEqualTo != null)
            {
                AppendColumnConditionWithValue(condition, ConditionOperator.NotEqual, filter, firstColumn, isColumnLeft);
                query.Criteria.Conditions.Add(condition);
                return;
            }

            // Support Greater Than
            var greaterThan = filter as GreaterThanFilter;
            if (greaterThan != null)
            {
                AppendColumnConditionWithValue(condition, ConditionOperator.GreaterThan, filter, firstColumn, isColumnLeft);
                query.Criteria.Conditions.Add(condition);
                return;
            }

            // Support Greater Than Equal
            var greaterEqual = filter as GreaterThanEqualToFilter;
            if (greaterEqual != null)
            {
                AppendColumnConditionWithValue(condition, ConditionOperator.GreaterEqual, filter, firstColumn, isColumnLeft);
                query.Criteria.Conditions.Add(condition);
                return;
            }

            // Support Less Than
            var lessThan = filter as LessThanFilter;
            if (lessThan != null)
            {
                AppendColumnConditionWithValue(condition, ConditionOperator.LessThan, filter, firstColumn, isColumnLeft);
                query.Criteria.Conditions.Add(condition);
                return;
            }

            // Support Less Than Equal
            var lessThanEqual = filter as LessThanEqualToFilter;
            if (lessThanEqual != null)
            {
                AppendColumnConditionWithValue(condition, ConditionOperator.LessEqual, filter, firstColumn, isColumnLeft);
                query.Criteria.Conditions.Add(condition);
                return;
            }

        }

        private void ProcessLikeFilter(QueryExpression query, ConditionExpression condition, LikeFilter filter)
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

            var conditionOperator = ConditionOperator.Like;
            if (filter.Not)
            {
                conditionOperator = ConditionOperator.NotLike;
            }

            AppendColumnCondition(condition, conditionOperator, filter, leftcolumn, filter.RightHand.Value);
            query.Criteria.Conditions.Add(condition);
            return;
        }

        private void AppendColumnConditionWithValue(ConditionExpression condition, ConditionOperator conditionOperator, OrderFilter greaterThan, Column column, bool isColumnLeft)
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
                AppendColumnCondition(condition, conditionOperator, greaterThan, column, litVal);
                return;
            }

            throw new NotSupportedException();
        }

        private void AppendColumnCondition(ConditionExpression condition, ConditionOperator conditionOperator, Filter filter, Column column, params object[] values)
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
                return;
            }
        }

        // ReSharper disable UnusedParameter.Local
        // This method is solely for pre condition checking.
        private void GuardSelectBuilder(SelectBuilder builder)
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

        //private void AppendColumnNotEqualTo(ConditionExpression condition, NotEqualToFilter notEqualTo, Column column, bool isColumnLeft)
        //{
        //    condition.Operator = ConditionOperator.NotEqual;
        //    if (column == null)
        //    {
        //        throw new InvalidOperationException("The query contains a WHERE clause with an Not Equals condition, however one side of the Not Equals condition must refer to an attribute name.");
        //    }
        //    Literal lit = null;
        //    if (isColumnLeft)
        //    {
        //        lit = notEqualTo.RightHand as Literal;
        //    }
        //    else
        //    {
        //        lit = notEqualTo.LeftHand as Literal;
        //    }

        //    if (lit != null)
        //    {
        //        object litVal = GitLiteralValue(lit);
        //        condition.Values.Add(litVal);
        //        return;
        //    }

        //    throw new NotSupportedException();
        //}

        //private void AppendColumnEqualTo(ConditionExpression condition, EqualToFilter equalTo, Column column, bool isColumnLeft)
        //{
        //    condition.Operator = ConditionOperator.Equal;
        //    if (column == null)
        //    {
        //        throw new InvalidOperationException("The query contains a WHERE clause with an Equals condition, however one side of the Equals condition must refer to an attribute name.");
        //    }
        //    Literal lit = null;
        //    if (isColumnLeft)
        //    {
        //        lit = equalTo.RightHand as Literal;
        //    }
        //    else
        //    {
        //        lit = equalTo.LeftHand as Literal;
        //    }
        //    if (lit != null)
        //    {
        //        object litVal = GitLiteralValue(lit);
        //        condition.Values.Add(litVal);
        //        return;
        //    }

        //    throw new NotSupportedException();
        //}

        private object GitLiteralValue(Literal lit)
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




    }


}