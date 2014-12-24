using Microsoft.Xrm.Sdk.Messages;
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
using CrmAdo.Metadata;
using CrmAdo.Core;

namespace CrmAdo.Visitor
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="RetrieveMultipleRequest"/> when it visits a <see cref="SelectBuilder"/> 
    /// </summary>
    public class RetrieveMultipleRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        private ICrmMetaDataProvider _MetadataProvider;

        public enum VisitMode
        {
            From = 0,
            Projection = 1,
            Filter = 2,
            Top = 3,
            OrderBy = 4
        }

        public RetrieveMultipleRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
            : this(null, metadataProvider)
        {

        }

        public RetrieveMultipleRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider)
        {
            Parameters = parameters;
            Request = new RetrieveMultipleRequest();
            QueryExpression = new QueryExpression();
            Request.Query = QueryExpression;
            _MetadataProvider = metadataProvider;
            EntityMetadata = new Dictionary<string, CrmEntityMetadata>();
            ColumnMetadata = new List<ColumnMetadata>();
        }

        public RetrieveMultipleRequest Request { get; set; }
        public QueryExpression QueryExpression { get; set; }
        public DbParameterCollection Parameters { get; set; }
        public bool IsSingleSource { get; set; }
        public AliasedSource SingleSource { get; set; }

        private Dictionary<string, CrmEntityMetadata> EntityMetadata { get; set; }
        public List<ColumnMetadata> ColumnMetadata { get; set; }

        public VisitMode Mode { get; set; }

        #region From Context
        public int SourceTableLevel = 0;
        public AliasedSource MainSource = null;
        public Table MainSourceTable = null;

        #region Join Context
        private LinkEntity LinkEntity { get; set; }
        private EqualToFilter EqualToFilter { get; set; }
        private Column LeftColumn { get; set; }
        private Column RightColumn { get; set; }
        private Table LeftTable { get; set; }
        private Table RightTable { get; set; }
        #endregion

        #endregion

        #region Filter Context
        public FilterExpression FilterExpression { get; set; }
        public bool NegateOperator { get; set; }
        #endregion

        #region Visit Methods

        protected override void VisitSelect(SelectBuilder item)
        {
            this.QueryExpression = new QueryExpression();
            Request.Query = this.QueryExpression;
            if (!item.From.Any())
            {
                throw new InvalidOperationException("The query does not have a valid FROM clause");
            }
            Mode = VisitMode.From;
            if (item.From.Count() == 1)
            {
                IsSingleSource = true;
            }
            VisitEach(item.From);

            Mode = VisitMode.Projection;
            NavigateProjections(item.Projection);

            Mode = VisitMode.Filter;
            if (item.WhereFilterGroup != null)
            {
                IFilter where = item.WhereFilterGroup;
                where.Accept(this);
            }
            else
            {
                //TODO: Should only be one where clause?
                foreach (IVisitableBuilder where in item.Where)
                {
                    where.Accept(this);
                }
            }

            if (item.Top != null)
            {
                Mode = VisitMode.Top;
                IVisitableBuilder top = item.Top;
                top.Accept(this);
            }
            else
            {
                // xrm wont let you use paging and top for some reason..
                if (QueryExpression.PageInfo == null)
                {
                    QueryExpression.PageInfo = new PagingInfo();
                }
                QueryExpression.PageInfo.PageNumber = 1;
                //todo take this from the connection string..
                QueryExpression.PageInfo.Count = 500;
                QueryExpression.PageInfo.ReturnTotalRecordCount = true;
            }

            if (item.OrderBy != null)
            {
                Mode = VisitMode.OrderBy;
                foreach (IVisitableBuilder order in item.OrderBy)
                {
                    order.Accept(this);
                }
            }

        }

        #region From
        protected override void VisitCrossJoin(CrossJoin item)
        {
            this.AddJoin(item);
        }
        protected override void VisitFullOuterJoin(FullOuterJoin item)
        {
            this.AddJoin(item);
        }
        protected override void VisitInnerJoin(InnerJoin item)
        {
            this.AddJoin(item);
        }
        protected override void VisitLeftOuterJoin(LeftOuterJoin item)
        {
            this.AddJoin(item);
        }
        protected override void VisitRightOuterJoin(RightOuterJoin item)
        {
            this.AddJoin(item);
        }
        protected override void VisitAliasedSource(AliasedSource aliasedSource)
        {
            // We want the top aliased source (furthest source to the left) as this is the main entity.
            if (IsSingleSource)
            {
                SingleSource = aliasedSource;
            }
            if (this.Level > SourceTableLevel)
            {
                SourceTableLevel = this.Level;
                MainSource = aliasedSource;
                MainSourceTable = aliasedSource.Source as Table;
                // source should be a table.                 
                QueryExpression.EntityName = MainSourceTable.GetTableLogicalEntityName();
            }
        }
        #endregion

        #region Projection
        protected override void VisitColumn(Column item)
        {
            bool isAliasedSource = !string.IsNullOrEmpty(item.Source.Alias);
            var sourceTable = item.Source.Source as Table;
            var sourceName = !isAliasedSource
                                 ? sourceTable.GetTableLogicalEntityName()
                                 : item.Source.Alias;
            var linkItem = QueryExpression.FindLinkEntity(sourceName, isAliasedSource);
            var attributeName = item.GetColumnLogicalAttributeName();
            string entityName = null;
            string alias = null;
            if (linkItem == null)
            {
                QueryExpression.ColumnSet.AddColumn(attributeName);
                entityName = MainSourceTable.GetTableLogicalEntityName();
            }
            else
            {
                linkItem.Columns.AddColumn(attributeName);
                alias = linkItem.EntityAlias;
                entityName = linkItem.LinkToEntityName.ToLower();
            }

            AddColumnMetadata(entityName, alias, attributeName);

        }

        protected override void VisitAllColumns(AllColumns item)
        {
            this.QueryExpression.IncludeAllColumns();

            var aliasedSource = IsSingleSource ? SingleSource : item.Source;
            var sourceTable = aliasedSource.Source as Table;

            var entityName = sourceTable.GetTableLogicalEntityName();
            var alias = aliasedSource.Alias;
            AddAllColumnMetadata(entityName, alias);
        }

        #endregion

        #region Top
        protected override void VisitTop(Top item)
        {
            if (item.IsPercent)
            {
                throw new NotSupportedException("TOP X PERCENT is not supported.");
            }
            var topCount = item.Expression as NumericLiteral;
            if (topCount == null)
            {
                throw new ArgumentException("The TOP Clause should specify the number of records to limit the resultset to as a numeric literal.");
            }
            var topCountInt = (int)topCount.GitLiteralValue();
            if (topCountInt > 5000)
            {
                //TODO: To get around the fact that dynamics wont support TOP that is greater than 5000,
                // we could actually set up paging, and then in the result set, limit the max page number / record number to
                // coincide with the TOP value.
                throw new NotSupportedException("Dynamics will not allow a TOP value that is greater than 5000. A work around will be implemented in this provider at a later date.");
            }
            QueryExpression.TopCount = topCountInt;
        }
        #endregion

        #region Order By
        protected override void VisitOrderBy(OrderBy item)
        {
            var orderType = OrderType.Ascending;
            if (item.Order == Order.Descending)
            {
                orderType = OrderType.Descending;
            }
            var column = item.Projection.ProjectionItem as Column;
            if (column == null)
            {
                throw new InvalidOperationException("Can only apply Order By clause to a column.");
            }
            var attName = column.GetColumnLogicalAttributeName();
            QueryExpression.AddOrder(attName, orderType);
        }
        #endregion

        #region Filter

        #region Order Filter

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

        protected override void VisitEqualToFilter(EqualToFilter item)
        {
            if (this.Mode == VisitMode.Filter)
            {
                VisitOrderFilter(item);
            }
            else if (Mode == VisitMode.From)
            {
                EqualToFilter = item;

                //TODO: Tidy this up use more of visitor pattern?   
                LeftColumn = item.LeftHand as Column;
                RightColumn = item.RightHand as Column;
                // GuardOnColumn(leftColumn);
                //  GuardOnColumn(rightColumn);

                LeftTable = LeftColumn.Source.Source as Table;
                RightTable = RightColumn.Source.Source as Table;
                //  LeftEntityName = 

                LinkEntity.LinkFromEntityName = LeftTable.GetTableLogicalEntityName();
                LinkEntity.LinkToEntityName = RightTable.GetTableLogicalEntityName();
                LinkEntity.LinkFromAttributeName = LeftColumn.GetColumnLogicalAttributeName();
                LinkEntity.LinkToAttributeName = RightColumn.GetColumnLogicalAttributeName();
            }
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

        #endregion

        #endregion

        #region Helper Methods

        #region Joins

        private void AddJoin(InnerJoin item)
        {
            AddJoin(item, JoinOperator.Inner);
        }
        private void AddJoin(LeftOuterJoin item)
        {
            AddJoin(item, JoinOperator.LeftOuter);
        }
        private void AddJoin(FilteredJoin item, JoinOperator jointype)
        {
            NavigateBinaryJoins(item);

            LinkEntity = CreateLinkEntity(item, jointype);

            int currentFilterIndex = 0;
            foreach (IVisitableBuilder onFilter in item.OnFilters)
            {
                onFilter.Accept(this);
                currentFilterIndex++;
            }

            if (currentFilterIndex != 1)
            {
                throw new NotSupportedException("You must use exactly one ON condition in a join. For example: INNER JOIN X ON Y.ID = X.ID is supported, but INNER JOIN X ON Y.ID = X.ID AND Y.NAME = X.NAME is not supported.");
            }

            if (EqualToFilter == null)
            {
                throw new NotSupportedException("When using an ON condition in a join, only Equal To operator is supported. For example: INNER JOIN X ON Y.ID = X.ID");
            }

            if (!QueryExpression.LinkEntities.Any())
            {
                if (QueryExpression.EntityName != LinkEntity.LinkFromEntityName)
                {
                    throw new InvalidOperationException("The first JOIN in the query must link from the main entity which in your case is " +
                        QueryExpression.EntityName + " but the first join in your query links from: " +
                        LinkEntity.LinkFromEntityName + " which is an unknown entity,");
                }
                QueryExpression.LinkEntities.Add(LinkEntity);
            }
            else
            {

                bool isAliased = !string.IsNullOrEmpty(LeftColumn.Source.Alias);
                var match = string.IsNullOrEmpty(LeftColumn.Source.Alias)
                                   ? LinkEntity.LinkFromEntityName
                                   : LeftColumn.Source.Alias;


                var leftLink = QueryExpression.FindLinkEntity(match, isAliased);
                if (leftLink == null)
                {
                    throw new InvalidOperationException("Could not perform join, as " + match + " is an unknown entity.");
                }
                leftLink.LinkEntities.Add(LinkEntity);
            }


            LinkEntity = null;
            EqualToFilter = null;
            LeftColumn = null;
            RightColumn = null;
            LeftTable = null;
            RightTable = null;

        }

        #region NotSupported
        private void AddJoin(CrossJoin item)
        {
            throw new NotSupportedException();
        }
        private void AddJoin(FullOuterJoin item)
        {
            throw new NotSupportedException();
        }
        private void AddJoin(RightOuterJoin item)
        {
            throw new NotSupportedException();
        }
        #endregion

        private void NavigateBinaryJoins(BinaryJoin item)
        {
            // visit left side first.            
            using (var ctx = GetSubCommand())
            {
                IJoinItem leftHand = item.LeftHand;
                leftHand.Accept(ctx.Visitor);
            }
        }

        private LinkEntity CreateLinkEntity(BinaryJoin item, JoinOperator jointype)
        {
            var linkEntity = new LinkEntity();
            linkEntity.JoinOperator = jointype;
            // This is what we are joining on to..
            AliasedSource asource = item.RightHand;
            if (!string.IsNullOrEmpty(asource.Alias))
            {
                linkEntity.EntityAlias = asource.Alias;
            }
            return linkEntity;
        }

        #endregion

        #region Projections

        private void NavigateProjections(IEnumerable<AliasedProjection> projections)
        {
            foreach (AliasedProjection a in projections)
            {
                //using (var ctx = GetSubCommand())
                //{
                string colAlias = a.Alias;
                if (!string.IsNullOrEmpty(colAlias))
                {
                    throw new NotSupportedException("Column aliases are not supported.");
                }
                a.ProjectionItem.Accept(this);
                // }
            }
        }

        #endregion

        #region Filter

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
                        inValues[index] = literal.GitLiteralValue();
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
                return (T)lit.GitLiteralValue();
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
                var sourceTable = source.Source as Table;
                var sourceName = string.IsNullOrEmpty(source.Alias) ? sourceTable.GetTableLogicalEntityName() : source.Alias;
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

        #endregion

        #endregion

        private void AddAllColumnMetadata(string entityName, string entityAlias)
        {
            // Add the metadata for this column.
            var entityMetadata = GetEntityMetadata(entityName);
            if (entityMetadata != null)
            {
                // Populate metadata for these columns.
                ColumnMetadata.AddRange((from c in entityMetadata.Attributes orderby c.LogicalName select new ColumnMetadata(c, entityAlias)));
            }
            else
            {
                // Could throw an exceptiton as no metadata found for this entity.
            }
        }

        private void AddColumnMetadata(string entityName, string entityAlias, string attributeName)
        {
            // Add the metadata for this column.
            var entityMetadata = GetEntityMetadata(entityName);
            if (entityMetadata != null)
            {
                var colMeta = entityMetadata.Attributes.FirstOrDefault(c => c.LogicalName == attributeName);
                ColumnMetadata columnMetadata = null;
                if (colMeta == null)
                {
                    // could throw an exception as no metadata found for this attribute?
                    //  throw new ArgumentException("Unknown column: " + columnAttributeName);
                    columnMetadata = new ColumnMetadata(attributeName, entityAlias);

                }
                else
                {
                    columnMetadata = new ColumnMetadata(colMeta, entityAlias);
                }
                ColumnMetadata.Add(columnMetadata);
            }
            else
            {
                // Could throw an exceptiton as no metadata found for this entity.
            }
        }

        private CrmEntityMetadata GetEntityMetadata(string entityName)
        {
            if (!EntityMetadata.ContainsKey(entityName))
            {
                if (_MetadataProvider == null)
                {
                    return null;
                }
                EntityMetadata[entityName] = _MetadataProvider.GetEntityMetadata(entityName);
            }
            var entMeta = EntityMetadata[entityName];
            return entMeta;
        }

    }
}
