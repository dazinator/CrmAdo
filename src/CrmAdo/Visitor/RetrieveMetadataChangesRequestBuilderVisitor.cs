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
using Microsoft.Xrm.Sdk.Metadata.Query;
using CrmAdo.Metadata;

namespace CrmAdo.Visitor
{
    public enum MetadataFilterType
    {
        Entity = 0,
        Attribute = 1,
        Relationship = 2

    }

    public class MetadataFilterGroupExpression
    {
        public MetadataFilterGroupExpression(MetadataFilterType filterType)
        {
            Filter = new MetadataFilterExpression();

        }
        public MetadataFilterExpression Filter { get; set; }
        public MetadataFilterType FilterType { get; set; }
    }


    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="RetrieveMetadataChangesRequest"/> when it visits a <see cref="SelectBuilder"/> 
    /// </summary>
    public class RetrieveMetadataChangesRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {
        public const string EntityMetadadataTableLogicalName = "entitymetadata";
        public const string AttributeMetadadataTableLogicalName = "attributemetadata";
        public const string OneToManyRelationshipMetadadataTableLogicalName = "onetomanyrelationshipmetadata";
        public const string ManyToManyRelationshipMetadadataTableLogicalName = "manytomanyrelationshipmetadata";



        private ICrmMetaDataProvider _MetadataProvider;

        public enum VisitMode
        {
            From = 0,
            Projection = 1,
            Filter = 2,
            Top = 3,
            OrderBy = 4
        }

        public RetrieveMetadataChangesRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
            : this(null, metadataProvider)
        {

        }

        public RetrieveMetadataChangesRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider)
        {
            Parameters = parameters;
            Request = new RetrieveMetadataChangesRequest();
            _MetadataProvider = metadataProvider;
            EntityMetadata = new Dictionary<string, CrmEntityMetadata>();
            ColumnMetadata = new List<ColumnMetadata>();
            //  QueryExpression = new EntityQueryExpression();          
            // Request.Query = QueryExpression;
        }

        public RetrieveMetadataChangesRequest Request { get; set; }
        public EntityQueryExpression QueryExpression { get; set; }
        public DbParameterCollection Parameters { get; set; }

        public bool IsSingleSource { get; set; }
        public AliasedSource SingleSource { get; set; }
        public VisitMode Mode { get; set; }
        private Dictionary<string, CrmEntityMetadata> EntityMetadata { get; set; }
        public List<ColumnMetadata> ColumnMetadata { get; set; }

        #region From Context
        //  public int EntityMetadataTableLevel = 0;
        //  public AliasedSource EntityMetadataSource = null;
        //  public Table EntityMetadataTable = null;

        #region Join Context
        // private LinkEntity LinkEntity { get; set; }
        private EqualToFilter EqualToFilter { get; set; }
        private Column LeftColumn { get; set; }
        private Column RightColumn { get; set; }
        private Table LeftTable { get; set; }
        private Table RightTable { get; set; }
        #endregion

        #endregion

        #region Filter Context
        public MetadataFilterGroupExpression FilterExpression { get; set; }
        public bool NegateOperator { get; set; }
        #endregion

        #region Visit Methods

        protected override void VisitSelect(SelectBuilder item)
        {
            this.QueryExpression = CreateDefaultQueryExpression();
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

            if (item.OrderBy != null)
            {
                Mode = VisitMode.OrderBy;
                foreach (IVisitableBuilder order in item.OrderBy)
                {
                    order.Accept(this);
                }
            }

        }

        private EntityQueryExpression CreateDefaultQueryExpression()
        {
            var query = new EntityQueryExpression();
            query.Properties = new MetadataPropertiesExpression();
            query.Properties.AllProperties = false;
            query.Properties.PropertyNames.Add("Attributes");
            query.Properties.PropertyNames.Add("ManyToManyRelationships");
            query.Properties.PropertyNames.Add("ManyToOneRelationships");
            query.Properties.PropertyNames.Add("OneToManyRelationships");

            // We need to ensure that attribute metadata is excluded until included.
            query.AttributeQuery = new AttributeQueryExpression();
            query.AttributeQuery.Properties = new MetadataPropertiesExpression();
            query.AttributeQuery.Properties.AllProperties = false;
            query.AttributeQuery.Criteria = new MetadataFilterExpression();
            query.AttributeQuery.Criteria.Conditions.Add(new MetadataConditionExpression("SchemaName", MetadataConditionOperator.Equals, "ThisWillNeverExist"));


            query.RelationshipQuery = new RelationshipQueryExpression();
            query.RelationshipQuery.Properties = new MetadataPropertiesExpression();
            query.RelationshipQuery.Properties.AllProperties = false;
            //  query.RelationshipQuery.Properties.PropertyNames.Add("RelationshipType");

            query.RelationshipQuery.Criteria.Conditions.Add(new MetadataConditionExpression("SchemaName", MetadataConditionOperator.Equals, "ThisWillNeverExist"));
            return query;
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
            if (IsSingleSource)
            {
                SingleSource = aliasedSource;
            }
            aliasedSource.Source.Accept(this);

            //// We want the root aliased source (furthest source to the left) as this is the entity metadata.
            //// from which other things will join.
            //if (this.Level > EntityMetadataTableLevel)
            //{
            //    EntityMetadataTableLevel = this.Level;
            //    EntityMetadataSource = aliasedSource;
            //    EntityMetadataTable = aliasedSource.Source as Table;
            //    // source should be a table.  
            //    //  QueryExpression.EntityName = EntityMetadataTable.GetTableLogicalEntityName();
            //}
        }

        public void InitialiseAttributeQuery()
        {
            this.QueryExpression.AttributeQuery.Properties = new MetadataPropertiesExpression();
            this.QueryExpression.AttributeQuery = new AttributeQueryExpression();
        }

        protected override void VisitTable(Table item)
        {
            switch (item.GetTableLogicalEntityName())
            {
                case EntityMetadadataTableLogicalName:
                    //  this.QueryExpression.Properties = new MetadataPropertiesExpression();
                    break;
                case AttributeMetadadataTableLogicalName:
                    InitialiseAttributeQuery();
                    break;
                case OneToManyRelationshipMetadadataTableLogicalName:
                    if (this.QueryExpression.RelationshipQuery == null)
                    {
                        this.QueryExpression.RelationshipQuery = new RelationshipQueryExpression();
                        this.QueryExpression.RelationshipQuery.Properties = new MetadataPropertiesExpression();
                    }
                    break;
                case ManyToManyRelationshipMetadadataTableLogicalName:
                    if (this.QueryExpression.RelationshipQuery == null)
                    {
                        this.QueryExpression.RelationshipQuery = new RelationshipQueryExpression();
                        this.QueryExpression.RelationshipQuery.Properties = new MetadataPropertiesExpression();
                    }
                    break;
            }

            base.VisitTable(item);
        }
        #endregion

        #region Projection
        protected override void VisitColumn(Column item)
        {
            //   bool isAliasedSource = !string.IsNullOrEmpty(item.Source.Alias);
            var aliasedSource = IsSingleSource ? SingleSource : item.Source;
            var sourceTable = aliasedSource.Source as Table;

            //var sourceTable = item.Source.Source as Table;
            var sourceName = sourceTable.GetTableLogicalEntityName();
            switch (sourceName)
            {
                case EntityMetadadataTableLogicalName:
                    this.QueryExpression.Properties.PropertyNames.Add(item.Name);
                    break;
                case AttributeMetadadataTableLogicalName:
                    if (this.QueryExpression.AttributeQuery.Properties == null)
                    {
                        this.QueryExpression.AttributeQuery.Properties = new MetadataPropertiesExpression();
                    }
                    this.QueryExpression.AttributeQuery.Properties.PropertyNames.Add(item.Name);
                    break;
                case OneToManyRelationshipMetadadataTableLogicalName:
                    this.QueryExpression.RelationshipQuery.Properties.PropertyNames.Add(item.Name);
                    break;
                case ManyToManyRelationshipMetadadataTableLogicalName:
                    this.QueryExpression.RelationshipQuery.Properties.PropertyNames.Add(item.Name);
                    break;
            }

            AddColumnMetadata(sourceName, aliasedSource.Alias, item.GetColumnLogicalAttributeName());
        }

        protected override void VisitAllColumns(AllColumns item)
        {
            var aliasedSource = IsSingleSource ? SingleSource : item.Source;
            var sourceTable = aliasedSource.Source as Table;
            var sourceName = sourceTable.GetTableLogicalEntityName();
            switch (sourceName)
            {
                case EntityMetadadataTableLogicalName:
                    this.QueryExpression.Properties.AllProperties = true;
                    break;
                case AttributeMetadadataTableLogicalName:
                    this.QueryExpression.AttributeQuery.Properties.AllProperties = true;
                    break;
                case OneToManyRelationshipMetadadataTableLogicalName:
                    this.QueryExpression.RelationshipQuery.Properties.AllProperties = true;
                    break;
                case ManyToManyRelationshipMetadadataTableLogicalName:
                    this.QueryExpression.RelationshipQuery.Properties.AllProperties = true;
                    break;
            }
            AddAllColumnMetadata(sourceName, aliasedSource.Alias);
        }
        #endregion

        #region Not Supported

        #region Top
        protected override void VisitTop(Top item)
        {
            throw new NotSupportedException("Due to Dynamics Crm SDK Limitations, TOP is not supported with Metadata Queries.");
        }
        #endregion

        #region Order By
        protected override void VisitOrderBy(OrderBy item)
        {
            throw new NotSupportedException("Due to Dynamics Crm SDK Limitations, Order By is not supported with Metadata Queries.");
        }
        #endregion

        #region Cross Join

        private void AddJoin(CrossJoin item)
        {
            this.IsSingleSource = false;
            throw new NotSupportedException();
        }

        #endregion

        #region Full Outer Join
        private void AddJoin(FullOuterJoin item)
        {
            this.IsSingleSource = false;
            throw new NotSupportedException();
        }
        #endregion

        #region Right Outer Join
        private void AddJoin(RightOuterJoin item)
        {
            this.IsSingleSource = false;
            throw new NotSupportedException();
        }
        #endregion
        #region Left Outer Join
        private void AddJoin(LeftOuterJoin item)
        {
            this.IsSingleSource = false;
            throw new NotSupportedException();
            //  AddJoin(item, JoinOperator.LeftOuter);
        }
        #endregion
        #endregion

        #region Filter

        #region Order Filter

        protected override void VisitFilterGroup(FilterGroup item)
        {
            if (item.HasFilters)
            {

                var newFilterExpression = new MetadataFilterGroupExpression(MetadataFilterType.Entity);

                var conjunction = item.Conjunction;
                if (conjunction == Conjunction.Or)
                {
                    newFilterExpression.Filter.FilterOperator = LogicalOperator.Or;
                }
                else
                {
                    newFilterExpression.Filter.FilterOperator = LogicalOperator.And;
                }

                var existingFilter = this.FilterExpression;
                this.FilterExpression = newFilterExpression;

                //TODO: Should only be one where clause?
                foreach (var where in item.Filters)
                {
                    where.Accept(this);
                }

                this.FilterExpression = existingFilter;

                // if there is a filter expression, chain this filter to that one (only if compatible type)

                if (existingFilter != null && existingFilter.FilterType == newFilterExpression.FilterType)
                {
                    existingFilter.Filter.Filters.Add(newFilterExpression.Filter);
                    //  existingFilter.AddFilter(newFilterExpression);
                }
                else
                {
                    // this is top level filter expression, add it directly to query in correct location
                    switch (existingFilter.FilterType)
                    {
                        case MetadataFilterType.Entity:
                            QueryExpression.Criteria.Filters.Add(newFilterExpression.Filter);
                            break;
                        case MetadataFilterType.Attribute:
                            QueryExpression.AttributeQuery.Criteria.Filters.Add(newFilterExpression.Filter);
                            break;
                        case MetadataFilterType.Relationship:
                            QueryExpression.RelationshipQuery.Criteria.Filters.Add(newFilterExpression.Filter);
                            break;
                    }

                    //  QueryExpression.Criteria.Filters.Add(newFilterExpression);

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

                //   LinkEntity.LinkFromEntityName = LeftTable.GetTableLogicalEntityName();
                //   LinkEntity.LinkToEntityName = RightTable.GetTableLogicalEntityName();
                //   LinkEntity.LinkFromAttributeName = LeftColumn.GetColumnLogicalAttributeName();
                //   LinkEntity.LinkToAttributeName = RightColumn.GetColumnLogicalAttributeName();
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

        private void AddJoin(FilteredJoin item, JoinOperator jointype)
        {
            this.IsSingleSource = false;
            // Navigates to the left of the joins to the logical parent. Ensures we deal with parent first.
            NavigateBinaryJoins(item);

            // LinkEntity = CreateLinkEntity(item, jointype);
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

            //if (!QueryExpression.LinkEntities.Any())
            //{
            //if (QueryExpression.EntityName != LinkEntity.LinkFromEntityName)
            //{
            //    throw new InvalidOperationException("The first JOIN in the query must link from the main entity which in your case is " +
            //        QueryExpression.EntityName + " but the first join in your query links from: " +
            //        LinkEntity.LinkFromEntityName + " which is an unknown entity,");
            //}
            //QueryExpression.LinkEntities.Add(LinkEntity);
            //    }
            //  else
            //   {

            //bool isAliased = !string.IsNullOrEmpty(LeftColumn.Source.Alias);
            //var match = string.IsNullOrEmpty(LeftColumn.Source.Alias)
            //                   ? LinkEntity.LinkFromEntityName
            //                   : LeftColumn.Source.Alias;


            //var leftLink = QueryExpression.FindLinkEntity(match, isAliased);
            //if (leftLink == null)
            //{
            //    throw new InvalidOperationException("Could not perform join, as " + match + " is an unknown entity.");
            //}
            //leftLink.LinkEntities.Add(LinkEntity);
            // }


            //  LinkEntity = null;
            EqualToFilter = null;
            LeftColumn = null;
            RightColumn = null;
            LeftTable = null;
            RightTable = null;

        }

        private void NavigateBinaryJoins(BinaryJoin item)
        {
            // visit left side first.            
            using (var ctx = GetSubCommand())
            {
                IJoinItem leftHand = item.LeftHand;
                leftHand.Accept(ctx.Visitor);
            }
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

        private void AddFilterCondition(MetadataConditionExpression condition, Column attColumn)
        {

            var sourceTable = (Table)attColumn.Source.Source;
            var sourceTableName = sourceTable.GetTableLogicalEntityName();

            MetadataFilterType filterType = MetadataFilterType.Entity;

            switch (sourceTableName)
            {
                case "entitymetadata":
                    filterType = MetadataFilterType.Entity;
                    break;

                case "attributemetadata":
                    filterType = MetadataFilterType.Attribute;
                    break;

                case "onetomanyrelationshipmetadata":
                    filterType = MetadataFilterType.Relationship;
                    break;

                case "manytomanyrelationshipmetadata":
                    filterType = MetadataFilterType.Relationship;
                    break;
            }
          
            //  var sourceEntityName = GetEntityNameOrAliasForSource(attColumn.Source, out isAlias, out link);
            //  condition.EntityName = sourceEntityName;

            // if filter expression present, add it to that.
            if (FilterExpression != null)
            {
                if (FilterExpression.FilterType == filterType)
                {
                    FilterExpression.Filter.Conditions.Add(condition);
                    return;
                }
                else
                {
                    // incompatible filter group type..
                }
            }

            switch (filterType)
            {
                case MetadataFilterType.Entity:
                    QueryExpression.Criteria.Conditions.Add(condition);
                    break;

                case MetadataFilterType.Attribute:
                    QueryExpression.AttributeQuery.Criteria.Conditions.Add(condition);
                    break;

                case MetadataFilterType.Relationship:
                    QueryExpression.RelationshipQuery.Criteria.Conditions.Add(condition);
                    break;
            }

        }

        private MetadataConditionExpression GetCondition(Function functionFilter, out Column attColumn)
        {
            throw new NotSupportedException("Unsupported function: '" + functionFilter.Name + "'");
        }

        private MetadataConditionExpression GetCondition(InFilter filter, out Column attColumn)
        {
            var condition = new MetadataConditionExpression();

            var left = filter.LeftHand;
            attColumn = left as Column;

            // defaullt attribute name for the filter condition.
            if (attColumn != null)
            {
                condition.PropertyName = attColumn.Name;
            }
            else
            {
                throw new NotSupportedException("IN operator only works agains a column value.");
            }

            var conditionOperator = MetadataConditionOperator.In;
            if (filter.Not)
            {
                conditionOperator = MetadataConditionOperator.NotIn;
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

        private MetadataConditionExpression GetCondition(NullFilter filter, out Column attColumn)
        {
            throw new NotSupportedException("'Null' filter not supported for entity metadata queries.");
        }

        private MetadataConditionExpression GetCondition(OrderFilter filter, out Column attColumn, out bool isLeft)
        {
            var condition = new MetadataConditionExpression();
            attColumn = GetAttributeColumn(filter, out isLeft);
            if (attColumn != null)
            {
                condition.PropertyName = attColumn.Name;
            }

            MetadataConditionOperator con;
            var equalTo = filter as EqualToFilter;
            if (equalTo != null)
            {
                con = MetadataConditionOperator.Equals;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Not Equals
            var notEqualTo = filter as NotEqualToFilter;
            if (notEqualTo != null)
            {
                con = MetadataConditionOperator.NotEquals;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Greater Than
            var greaterThan = filter as GreaterThanFilter;
            if (greaterThan != null)
            {
                con = MetadataConditionOperator.GreaterThan;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Greater Than Equal
            var greaterEqual = filter as GreaterThanEqualToFilter;
            if (greaterEqual != null)
            {
                throw new NotSupportedException("'Greater Than Equals' filter not supported for entity metadata queries.");
            }

            // Support Less Than
            var lessThan = filter as LessThanFilter;
            if (lessThan != null)
            {
                con = MetadataConditionOperator.LessThan;
                var filterValue = GetFilterValue<object>(filter, isLeft);
                SetConditionExpressionValue(condition, con, filterValue);
                return condition;
            }

            // Support Less Than Equal
            var lessThanEqual = filter as LessThanEqualToFilter;
            if (lessThanEqual != null)
            {
                throw new NotSupportedException("'Less Than Equals' filter not supported for entity metadata queries.");
            }

            // Support Like
            var likeFilter = filter as LikeFilter;
            if (likeFilter != null)
            {
                throw new NotSupportedException("'Like' filter not supported for entity metadata queries.");
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
                throw new InvalidOperationException("The query contains a WHERE clause, however one side of the where condition must refer to a metadata property name.");
            }
            return attributeColumn;
        }

        private void SetConditionExpressionValue(MetadataConditionExpression condition, MetadataConditionOperator conditionOperator, params object[] values)
        {
            condition.ConditionOperator = conditionOperator;
            if (values != null)
            {
                // is the literal a 
                foreach (var value in values)
                {
                    if (value is Array)
                    {
                        var vals = new List<object>();

                        foreach (var o in value as Array)
                        {
                            vals.Add(o);
                        }

                        condition.Value = vals;
                    }
                    else
                    {
                        if (condition.Value != null)
                        {
                            throw new NotSupportedException("You cannot use multiple where condition values in a metadata query. " + condition.Value.ToString());
                        }
                        condition.Value = value;
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
                if (entityName == AttributeMetadadataTableLogicalName)
                {
                    InitialiseAttributeQuery();
                }
            }
            var entMeta = EntityMetadata[entityName];
            return entMeta;
        }
    }
}
