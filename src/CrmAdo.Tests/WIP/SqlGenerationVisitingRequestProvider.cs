using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLGeneration.Generators;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using Microsoft.Xrm.Sdk.Messages;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using System.Data.Common;
using CrmAdo.Dynamics;

namespace CrmAdo.Tests.WIP
{

    public class OrganizationRequestBuilderVisitor : BuilderVisitor
    {
        private int _Level;
        public int Level
        {
            get { return _Level; }
            protected set { _Level = value; }
        }

        protected readonly CommandType _CommandType;
        protected enum CommandType
        {
            Unknown,
            Select,
            Insert,
            Update,
            Delete,
            Batch
        }

        protected class VisitorSubCommandContext : IDisposable
        {
            public VisitorSubCommandContext(OrganizationRequestBuilderVisitor visitor)
            {
                Visitor = visitor;
                Visitor.Level = Visitor.Level + 1;
            }

            public void Dispose()
            {
                Visitor.Level = Visitor.Level - 1;
            }

            public OrganizationRequestBuilderVisitor Visitor { get; set; }

        }

        protected VisitorSubCommandContext GetSubCommand()
        {
            return new VisitorSubCommandContext(this);
        }

        protected void VisitEach(IEnumerable<IVisitableBuilder> builders)
        {
            foreach (var item in builders)
            {
                using (var ctx = GetSubCommand())
                {
                    // IVisitableBuilder first = builders.First();
                    item.Accept(ctx.Visitor);
                }
            }
        }

        protected string GetTableLogicalEntityName(Table table)
        {
            return table.Name.ToLower();
        }

        protected string GetColumnLogicalAttributeName(Column column)
        {
            return column.Name.ToLower();
        }       

        protected object GitLiteralValue(Literal lit)
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
                return stringLit.Value;
            }

            numberLiteral = lit as NumericLiteral;
            if (numberLiteral != null)
            {
                // cast down from double to int if possible..
                checked
                {
                    try
                    {
                        if ((numberLiteral.Value % 1) == 0)
                        {
                            int intValue = (int)numberLiteral.Value;
                            if (intValue == numberLiteral.Value)
                            {
                                return intValue;
                            }
                        }

                        // can we return a decimal instead?
                        var decVal = Convert.ToDecimal(numberLiteral.Value);
                        return decVal;
                    }
                    catch (OverflowException)
                    {
                        //   can't down cast to int so remain as double.
                        return numberLiteral.Value;
                    }
                }

            }

            var nullLiteral = lit as NullLiteral;
            if (nullLiteral != null)
            {
                return null;
            }

            throw new NotSupportedException("Unknown Literal");

        }

    }

    public class LinkEntityBuilderVisitor : OrganizationRequestBuilderVisitor
    {

        public LinkEntity LinkEntity { get; set; }
        public EqualToFilter EqualToFilter = null;

        public Column LeftColumn { get; set; }
        public Column RightColumn { get; set; }

        public Table LeftTable { get; set; }
        public Table RightTable { get; set; }

        // public string LeftEntityName { get; set; }

        public LinkEntityBuilderVisitor(LinkEntity linkEntity)
        {
            LinkEntity = linkEntity;
        }

        protected override void VisitEqualToFilter(EqualToFilter item)
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

            LinkEntity.LinkFromEntityName = GetTableLogicalEntityName(LeftTable);
            LinkEntity.LinkToEntityName = GetTableLogicalEntityName(RightTable);
            LinkEntity.LinkFromAttributeName = GetColumnLogicalAttributeName(LeftColumn);
            LinkEntity.LinkToAttributeName = GetColumnLogicalAttributeName(RightColumn);
        }

    }

    public class RetrieveMultipleRequestBuilderVisitor : OrganizationRequestBuilderVisitor
    {

        public int SourceTableLevel = 0;
        public AliasedSource MainSource = null;
        public Table MainSourceTable = null;

        public RetrieveMultipleRequestBuilderVisitor()
        {
            Request = new RetrieveMultipleRequest();
            QueryExpression = new QueryExpression();
            Request.Query = QueryExpression;
        }

        public RetrieveMultipleRequest Request { get; set; }
        public QueryExpression QueryExpression { get; set; }
        public DbParameterCollection Parameters { get; set; }

        public RetrieveMultipleRequestBuilderVisitor(DbParameterCollection parameters)
        {
            Parameters = parameters;
        }


        #region Visit Methods
        protected override void VisitSelect(SelectBuilder item)
        {
            this.QueryExpression = new QueryExpression();
            Request.Query = this.QueryExpression;
            if (item.From.Any())
            {
                VisitEach(item.From);
            }
            if (item.Projection.Any())
            {
                AddProjections(item.Projection);
            }
            if (item.WhereFilterGroup != null)
            {
                AddFilterGroup(item.WhereFilterGroup, null);
            }
            else
            {
                AddFilters(item.Where);
            }

        }

        #region Join
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
            if (this.Level > SourceTableLevel)
            {
                SourceTableLevel = this.Level;
                MainSource = aliasedSource;
                MainSourceTable = aliasedSource.Source as Table;
                // source should be a table.                 
                QueryExpression.EntityName = GetTableLogicalEntityName(MainSourceTable);
            }
        }

        protected override void VisitColumn(Column item)
        {
            bool isAliasedSource = !string.IsNullOrEmpty(item.Source.Alias);
            var sourceName = !isAliasedSource
                                 ? GetTableLogicalEntityName(item.Source.Source as Table)
                                 : item.Source.Alias;
            var linkItem = QueryExpression.FindLinkEntity(sourceName, isAliasedSource);
            var columnAttributeName = GetColumnLogicalAttributeName(item);
            if (linkItem == null)
            {
                QueryExpression.ColumnSet.AddColumn(columnAttributeName);
                // wehn we can;t find the source, assume default entity..
                //   throw new InvalidOperationException("Could not find source named: " + sourceName);
            }
            else
            {
                linkItem.Columns.AddColumn(columnAttributeName);
            }
        }

        protected override void VisitAllColumns(AllColumns item)
        {
            this.QueryExpression.IncludeAllColumns();
        }

        #endregion
        #endregion

        #region Add Methods

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

            var linkEntity = CreateLinkEntity(item, jointype);

            int onFilterCount = item.OnFilters.Count();
            if (onFilterCount != 1)
            {
                throw new NotSupportedException("You must use exactly one ON condition in a join. For example: INNER JOIN X ON Y.ID = X.ID is supported, but INNER JOIN X ON Y.ID = X.ID AND Y.NAME = X.NAME is not supported.");
            }
            var singleFilter = item.OnFilters.Single();
            var linkEntityBuilder = new LinkEntityBuilderVisitor(linkEntity);
            singleFilter.Accept(linkEntityBuilder);
            if (linkEntityBuilder.EqualToFilter == null)
            {
                throw new NotSupportedException("When using an ON condition in a join, only Equal To operator is supported. For example: INNER JOIN X ON Y.ID = X.ID");
            }

            // throw new NotImplementedException();

            if (!QueryExpression.LinkEntities.Any())
            {
                if (QueryExpression.EntityName != linkEntity.LinkFromEntityName)
                {
                    throw new InvalidOperationException("The first JOIN in the query must link from the main entity which in your case is " +
                        linkEntity + " but the first join in your query links from: " +
                        linkEntity.LinkFromEntityName + " which is an unknown entity,");
                }
                QueryExpression.LinkEntities.Add(linkEntity);
            }
            else
            {

                bool isAliased = !string.IsNullOrEmpty(linkEntityBuilder.LeftColumn.Source.Alias);
                var match = string.IsNullOrEmpty(linkEntityBuilder.LeftColumn.Source.Alias)
                                   ? linkEntityBuilder.LinkEntity.LinkFromEntityName
                                   : linkEntityBuilder.LeftColumn.Source.Alias;


                var leftLink = QueryExpression.FindLinkEntity(match, isAliased);
                if (leftLink == null)
                {
                    throw new InvalidOperationException("Could not perform join, as " + match + " is an unknown entity.");
                }
                leftLink.LinkEntities.Add(linkEntity);
            }

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

        #region Helper Methods

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

        #endregion

        #region Projections

        private void AddProjections(IEnumerable<AliasedProjection> projections)
        {
            foreach (AliasedProjection a in projections)
            {
                using (var ctx = GetSubCommand())
                {
                    string colAlias = a.Alias;
                    if (!string.IsNullOrEmpty(colAlias))
                    {
                        throw new NotSupportedException("Column aliases are not supported.");
                    }
                    a.ProjectionItem.Accept(ctx.Visitor);
                }
            }
        }

        #endregion

        #region Filters

        private void AddFilters(IEnumerable<IFilter> whereFilter, FilterExpression filterExpression = null)
        {
            // do where clause..
            if (whereFilter != null && whereFilter.Any())
            {
                //TODO: Should only be one where clause?
                foreach (var where in whereFilter)
                {
                    ProcessFilter(where, filterExpression);
                }
            }
        }

        private void AddFilterGroup(FilterGroup filterGroup, FilterExpression filterExpression)
        {
            if (filterGroup.HasFilters)
            {
                var filter = new FilterExpression();

                // add each filter in this as a condition to the filter..
                AddFilters(filterGroup.Filters, filter);

                // if there is a filter expression, chain this filter to that one..
                if (filterExpression != null)
                {
                    filterExpression.AddFilter(filter);
                }
                else
                {
                    // this is top level filter expression, add it directly to query.
                    QueryExpression.Criteria.AddFilter(filter);
                }

                var conjunction = filterGroup.Conjunction;
                if (conjunction == Conjunction.Or)
                {
                    filter.FilterOperator = LogicalOperator.Or;
                }
                else
                {
                    filter.FilterOperator = LogicalOperator.And;
                }
            }
        }

        private void ProcessFilter(IFilter filter, FilterExpression filterExpression = null, bool negateOperator = false)
        {

            var filterGroup = filter as FilterGroup;
            if (filterGroup != null)
            {
                AddFilterGroup(filterGroup, filterExpression);
                return;
            }

            var orderFilter = filter as OrderFilter;
            if (orderFilter != null)
            {
                bool isLeft;
                Column attColumn;
                var condition = GetCondition(orderFilter, out attColumn, out isLeft);
                if (negateOperator)
                {
                    condition.NegateOperator();
                }
                AddFilterCondition(filterExpression, condition, attColumn);
                return;
            }

            var nullFilter = filter as NullFilter;
            if (nullFilter != null)
            {
                Column attColumn;
                var condition = GetCondition(nullFilter, out attColumn);
                if (negateOperator)
                {
                    condition.NegateOperator();
                }
                AddFilterCondition(filterExpression, condition, attColumn);
                return;
            }

            var inFilter = filter as InFilter;
            if (inFilter != null)
            {
                Column attColumn;
                var condition = GetCondition(inFilter, out attColumn);
                if (negateOperator)
                {
                    condition.NegateOperator();
                }
                AddFilterCondition(filterExpression, condition, attColumn);
                return;
            }

            var functionFilter = filter as Function;
            if (functionFilter != null)
            {
                Column attColumn;
                var condition = GetCondition(functionFilter, out attColumn);
                if (negateOperator)
                {
                    condition.NegateOperator();
                }
                AddFilterCondition(filterExpression, condition, attColumn);
                return;
            }

            var notFilter = filter as NotFilter;
            if (notFilter != null)
            {
                var negatedFilter = notFilter.Filter;
                ProcessFilter(negatedFilter, filterExpression, true);
                return;
            }

            throw new NotSupportedException();

        }

        private void AddFilterCondition(FilterExpression filterExpression, ConditionExpression condition, Column attColumn)
        {

            bool isAlias;
            LinkEntity link = null;
            var sourceEntityName = GetEntityNameOrAliasForSource(attColumn.Source, out isAlias, out link);
            condition.EntityName = sourceEntityName;

            // if filter expression present, add it to that.
            if (filterExpression != null)
            {
                filterExpression.AddCondition(condition);
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

        #endregion


    }

    public class SqlGenerationVisitingRequestProvider : OrganizationRequestBuilderVisitor
    {
        public const string ParameterToken = "@";

        public OrganizationRequest OrganizationRequest { get; set; }

        protected override void VisitSelect(SQLGeneration.Builders.SelectBuilder item)
        {
            var selectVisitorBuilder = new RetrieveMultipleRequestBuilderVisitor();
            IVisitableBuilder builder = item;
            builder.Accept(selectVisitorBuilder);
            OrganizationRequest = selectVisitorBuilder.Request;

            //  throw new NotImplementedException();                  
            //if (selCommand.Top != null)
            //{
            //    AddTop(selCommand.Top, query);
            //}
            //if (selCommand.Top == null)
            //{
            //    // xrm wont let you use paging and top for some reason..
            //    if (query.PageInfo == null)
            //    {
            //        query.PageInfo = new PagingInfo();
            //    }
            //    query.PageInfo.PageNumber = 1;
            //    //todo take this from the connection string..
            //    query.PageInfo.Count = 500;
            //    query.PageInfo.ReturnTotalRecordCount = true;
            //}
            //if (selCommand.OrderBy != null)
            //{
            //    AddOrderBy(selCommand.OrderBy, query);
            //}


            //return query;
        }

    }
}
