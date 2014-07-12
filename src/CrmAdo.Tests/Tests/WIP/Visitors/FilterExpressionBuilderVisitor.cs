using CrmAdo.Tests.WIP;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.Dynamics.Metadata;
using CrmAdo.Dynamics;

namespace CrmAdo.Tests.Tests.WIP.Visitors
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds the <see cref="FilterExpression"/> for a <see cref="QueryExpression"/> when it visits an <see cref="FilterGroup"/> 
    /// </summary>
    public class FilterExpressionBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public QueryExpression QueryExpression { get; set; }
        public FilterExpression FilterExpression { get; set; }
        public DbParameterCollection Parameters { get; set; }

        public bool NegateOperator { get; set; }

        public FilterExpressionBuilderVisitor(QueryExpression queryExpression, DbParameterCollection parameters)
        {
            FilterExpression = null;
            QueryExpression = queryExpression;
            Parameters = parameters;
        }

        protected override void VisitFilterGroup(FilterGroup item)
        {
            if (item.HasFilters)
            {

                var newFilterExpression = new FilterExpression();
                var conjunction = item.Conjunction;
                if (conjunction == Conjunction.Or)
                {
                    newFilterExpression.FilterOperator = LogicalOperator.Or;
                }
                else
                {
                    newFilterExpression.FilterOperator = LogicalOperator.And;
                }

                var existingFilter = this.FilterExpression;
                this.FilterExpression = newFilterExpression;

                //TODO: Should only be one where clause?
                foreach (var where in item.Filters)
                {
                    where.Accept(this);
                }

                this.FilterExpression = existingFilter;

                // if there is a filter expression, chain this filter to that one..
                if (existingFilter != null)
                {
                    existingFilter.AddFilter(newFilterExpression);
                }
                else
                {
                    // this is top level filter expression, add it directly to query.
                    QueryExpression.Criteria.AddFilter(newFilterExpression);
                }

            }
        }

        #region Order Filter

        protected override void VisitEqualToFilter(EqualToFilter item)
        {
            VisitOrderFilter(item);
        }

        protected override void VisitGreaterThanEqualToFilter(GreaterThanEqualToFilter item)
        {
            VisitOrderFilter(item);
        }

        protected override void VisitGreaterThanFilter(GreaterThanFilter item)
        {
            VisitOrderFilter(item);
        }

        protected override void VisitLessThanEqualToFilter(LessThanEqualToFilter item)
        {
            VisitOrderFilter(item);
        }

        protected override void VisitLessThanFilter(LessThanFilter item)
        {
            VisitOrderFilter(item);
        }

        protected override void VisitLikeFilter(LikeFilter item)
        {
            VisitOrderFilter(item);
        }

        protected override void VisitNotEqualToFilter(NotEqualToFilter item)
        {
            VisitOrderFilter(item);
        }

        protected virtual void VisitOrderFilter(OrderFilter filter)
        {
            bool isLeft;
            Column attColumn;
            var condition = GetCondition(filter, out attColumn, out isLeft);
            if (this.NegateOperator)
            {
                condition.NegateOperator();
            }
            AddFilterCondition(condition, attColumn);
            return;
        }

        #endregion

        #region Null Filter

        protected override void VisitNullFilter(NullFilter item)
        {
            Column attColumn;
            var condition = GetCondition(item, out attColumn);
            if (this.NegateOperator)
            {
                condition.NegateOperator();
            }
            AddFilterCondition(condition, attColumn);
            return;
        }

        #endregion

        #region InFilter

        protected override void VisitInFilter(InFilter item)
        {
            Column attColumn;
            var condition = GetCondition(item, out attColumn);
            if (this.NegateOperator)
            {
                condition.NegateOperator();
            }
            AddFilterCondition(condition, attColumn);
            return;
        }

        #endregion

        #region Function Filter

        protected override void VisitFunction(Function item)
        {
            Column attColumn;
            var condition = GetCondition(item, out attColumn);
            if (this.NegateOperator)
            {
                condition.NegateOperator();
            }
            AddFilterCondition(condition, attColumn);
            return;
        }

        #endregion

        #region Not Filter

        protected override void VisitNotFilter(NotFilter item)
        {
            var negatedFilter = item.Filter;
            var currentNegate = this.NegateOperator;
            this.NegateOperator = true;
            item.Filter.Accept(this);
            this.NegateOperator = currentNegate;
            return;
        }

        #endregion

        #region TODO revisit these methods

        private void AddFilterCondition(ConditionExpression condition, Column attColumn)
        {

            bool isAlias;
            LinkEntity link = null;
            var sourceEntityName = GetEntityNameOrAliasForSource(attColumn.Source, out isAlias, out link);
            condition.EntityName = sourceEntityName;

            // if filter expression present, add it to that.
            if (FilterExpression != null)
            {
                FilterExpression.AddCondition(condition);
            }
            else
            {
                if (link == null)
                {
                    QueryExpression.Criteria.Conditions.Add(condition);
                }
                else
                {
                    link.LinkCriteria.Conditions.Add(condition);
                }
            }
        }

        private ConditionExpression GetCondition(Function functionFilter, out Column attColumn)
        {
            switch (functionFilter.Name.ToLower())
            {
                case "contains":

                    var condition = new ConditionExpression();
                    var argCount = functionFilter.Arguments.Count();
                    if (argCount != 2)
                    {
                        throw new NotSupportedException("contains function should have exactly 2 arguments.");
                    }

                    //the first argument should be the column name.
                    var args = functionFilter.Arguments.ToArray();

                    attColumn = args[0] as Column;
                    // default attribute name for the filter condition.
                    if (attColumn != null)
                    {
                        condition.AttributeName = attColumn.Name.ToLower();
                    }
                    else
                    {
                        throw new NotSupportedException("the contains function should have a column name as its first argument.");
                    }
                    condition.Operator = ConditionOperator.Contains;

                    var searchArg = args[1];
                    var stringLiteral = searchArg as StringLiteral;
                    if (stringLiteral != null)
                    {
                        condition.Values.Add(stringLiteral.Value);
                        return condition;
                    }

                    // perhaps its a placeholder?
                    var placeholder = searchArg as Placeholder;
                    if (placeholder != null)
                    {
                        var paramVal = GetParamaterValue<object>(placeholder.Value);
                        condition.Values.Add(paramVal);
                        return condition;
                    }


                    throw new NotSupportedException("The second argument of the contains function should be either a string literal value, or a parameter");
                //if (functionFilter.Name)
                //{
                //    condition.Operator = ConditionOperator.NotNull;
                //}


                default:

                    throw new NotSupportedException("Unsupported function: '" + functionFilter.Name + "'");
            }
        }

        private ConditionExpression GetCondition(InFilter filter, out Column attColumn)
        {
            var condition = new ConditionExpression();

            var left = filter.LeftHand;
            attColumn = left as Column;

            // defaullt attribute name for the filter condition.
            if (attColumn != null)
            {
                condition.AttributeName = attColumn.Name.ToLower();
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
                    return condition;
                }
                throw new ArgumentException("The values list for the IN expression is null");
            }
            throw new NotSupportedException();
        }

        private ConditionExpression GetCondition(NullFilter filter, out Column attColumn)
        {
            var condition = new ConditionExpression();

            var left = filter.LeftHand;
            attColumn = left as Column;

            // default attribute name for the filter condition.
            if (attColumn != null)
            {
                condition.AttributeName = attColumn.Name.ToLower();
            }
            else
            {
                throw new NotSupportedException("Null operator only works agains a column value.");
            }

            condition.Operator = ConditionOperator.Null;
            if (filter.Not)
            {
                condition.Operator = ConditionOperator.NotNull;
            }
            return condition;
        }

        private ConditionExpression GetCondition(OrderFilter filter, out Column attColumn, out bool isLeft)
        {
            var condition = new ConditionExpression();
            attColumn = GetAttributeColumn(filter, out isLeft);
            if (attColumn != null)
            {
                condition.AttributeName = attColumn.Name.ToLower();
            }

            ConditionOperator con;
            var equalTo = filter as EqualToFilter;
            if (equalTo != null)
            {
                con = ConditionOperator.Equal;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Not Equals
            var notEqualTo = filter as NotEqualToFilter;
            if (notEqualTo != null)
            {
                con = ConditionOperator.NotEqual;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Greater Than
            var greaterThan = filter as GreaterThanFilter;
            if (greaterThan != null)
            {
                con = ConditionOperator.GreaterThan;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Greater Than Equal
            var greaterEqual = filter as GreaterThanEqualToFilter;
            if (greaterEqual != null)
            {
                con = ConditionOperator.GreaterEqual;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Less Than
            var lessThan = filter as LessThanFilter;
            if (lessThan != null)
            {
                con = ConditionOperator.LessThan;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Less Than Equal
            var lessThanEqual = filter as LessThanEqualToFilter;
            if (lessThanEqual != null)
            {
                con = ConditionOperator.LessEqual;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Like
            var likeFilter = filter as LikeFilter;
            if (likeFilter != null)
            {
                var conditionValue = GetFilterValue<string>(likeFilter, isLeft);
                ConditionOperator likeOperator;
                var newLike = GetLikeString(conditionValue, likeFilter.Not, out likeOperator);
                SetConditionExpressionValue(condition, likeOperator, newLike);
                return condition;
            }

            throw new NotSupportedException();

        }

        private Column GetAttributeColumn(OrderFilter filter, out bool isColumnLeftSide)
        {
            var left = filter.LeftHand;
            var right = filter.RightHand;
            var leftcolumn = left as Column;
            var rightcolumn = right as Column;
            Column attributeColumn = null;
            isColumnLeftSide = false;
            if (leftcolumn != null)
            {
                attributeColumn = leftcolumn;
                isColumnLeftSide = true;
            }
            else if (rightcolumn != null)
            {
                attributeColumn = rightcolumn;
            }
            if (attributeColumn == null)
            {
                throw new InvalidOperationException("The query contains a WHERE clause, however one side of the where condition must refer to an attribute name.");
            }
            return attributeColumn;
        }

        private void SetConditionExpressionValue(ConditionExpression condition, ConditionOperator conditionOperator, params object[] values)
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

        private T GetParamaterValue<T>(string paramName)
        {
            if (!Parameters.Contains(paramName))
            {
                throw new InvalidOperationException("Missing parameter value for parameter named: " + paramName);
            }
            var param = Parameters[paramName];
            return (T)param.Value;
        }

        private T GetFilterValue<T>(OrderFilter filter, bool isLeft)
        {
            // check for literals..
            Literal lit = null;
            if (isLeft)
            {
                lit = filter.RightHand as Literal;
            }
            else
            {
                lit = filter.LeftHand as Literal;
            }

            if (lit != null)
            {
                return (T)GitLiteralValue(lit);
            }

            // check for placeholders..
            Placeholder placeholder = null;
            if (isLeft)
            {
                placeholder = filter.RightHand as Placeholder;
            }
            else
            {
                placeholder = filter.LeftHand as Placeholder;
            }

            if (placeholder != null)
            {
                return GetParamaterValue<T>(placeholder.Value);
            }

            throw new NotSupportedException("Could not get value of type: " + typeof(T).FullName);

        }

        private string GetLikeString(string likeString, bool not, out ConditionOperator conditionOperator)
        {
            // detect paramater
            if (string.IsNullOrEmpty(likeString))
            {
                throw new ArgumentException("likestring cannot be null or empty");
            }

            bool startsWith = likeString.EndsWith("%");
            bool endsWith = likeString.StartsWith("%");

            if (startsWith)
            {
                if (endsWith)
                {
                    // contains
                    if (not)
                    {
                        // Does Not contain operator not recognised by Xrm sdk??? 
                        // conditionoperator = ConditionOperator.DoesNotContain;
                        conditionOperator = ConditionOperator.NotLike;
                    }
                    else
                    {
                        //  Contains operator causes Xrm organisation servuce to throw "Generic SQL error"??? 
                        conditionOperator = ConditionOperator.Like;
                    }
                    // val = filter.RightHand.Value.Trim('%');
                }
                else
                {
                    // starts with
                    likeString = likeString.TrimEnd('%');
                    if (not)
                    {
                        conditionOperator = ConditionOperator.DoesNotBeginWith;
                    }
                    else
                    {
                        conditionOperator = ConditionOperator.BeginsWith;
                    }
                }
            }
            else if (endsWith)
            {
                // ends with;
                // contains
                likeString = likeString.TrimStart('%');
                if (not)
                {
                    conditionOperator = ConditionOperator.DoesNotEndWith;
                }
                else
                {
                    conditionOperator = ConditionOperator.EndsWith;
                }
            }
            else
            {
                if (not)
                {
                    conditionOperator = ConditionOperator.NotLike;
                }
                else
                {
                    conditionOperator = ConditionOperator.Like;
                }
            }

            return likeString;
        }

        protected string GetEntityNameOrAliasForSource(AliasedSource source, out bool isAlias, out LinkEntity linkEntity)
        {
            if (source != null)
            {
                isAlias = !string.IsNullOrEmpty(source.Alias);
                var sourceName = string.IsNullOrEmpty(source.Alias) ? GetTableLogicalEntityName(source.Source as Table) : source.Alias;
                linkEntity = this.QueryExpression.FindLinkEntity(sourceName, isAlias);
                if (linkEntity == null)
                {
                    // If this is for the main entity - it doesn;t support alias name..
                    isAlias = false;
                    return QueryExpression.EntityName;
                }
                return sourceName;
            }
            throw new NotSupportedException("A condition in the WHERE clause contains refers to an unknown table / entity.");
        }

        #endregion

    }
}
