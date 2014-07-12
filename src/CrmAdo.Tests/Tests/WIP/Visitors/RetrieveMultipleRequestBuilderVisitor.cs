using CrmAdo.Tests.WIP;
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

namespace CrmAdo.Tests.Tests.WIP.Visitors
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="RetrieveMultipleRequest"/> when it visits a <see cref="SelectBuilder"/> 
    /// </summary>
    public class RetrieveMultipleRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public int SourceTableLevel = 0;
        public AliasedSource MainSource = null;
        public Table MainSourceTable = null;

        public RetrieveMultipleRequestBuilderVisitor():this(null)
        {
           
        }

        public RetrieveMultipleRequestBuilderVisitor(DbParameterCollection parameters)
        {
            Parameters = parameters;
            Request = new RetrieveMultipleRequest();
            QueryExpression = new QueryExpression();
            Request.Query = QueryExpression;
        }

        public RetrieveMultipleRequest Request { get; set; }
        public QueryExpression QueryExpression { get; set; }
        public DbParameterCollection Parameters { get; set; }

      

        #region Visit Methods

        protected override void VisitSelect(SelectBuilder item)
        {
            this.QueryExpression = new QueryExpression();
            Request.Query = this.QueryExpression;
            if (!item.From.Any())
            {
                throw new InvalidOperationException("The query does not have a valid FROM clause");
            }
            VisitEach(item.From);
            if (item.Projection.Any())
            {
                NavigateProjections(item.Projection);
            }
            var filterBuilder = new FilterExpressionBuilderVisitor(QueryExpression, Parameters);
            if (item.WhereFilterGroup != null)
            {
                IFilter where = item.WhereFilterGroup;
                where.Accept(filterBuilder);
            }
            else
            {
                //TODO: Should only be one where clause?
                foreach (var where in item.Where)
                {
                    where.Accept(this);
                }
            }
            if (item.Top != null)
            {
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
                NavigateOrderBy(item.OrderBy);
            }

        }

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
            var topCountInt = (int)GitLiteralValue(topCount);
            if (topCountInt > 5000)
            {
                //TODO: To get around the fact that dynamics wont support TOP that is greater than 5000,
                // we could actually set up paging, and then in the result set, limit the max page number / record number to
                // coincide with the TOP value.
                throw new NotSupportedException("Dynamics will not allow a TOP value that is greater than 5000. A work around will be implemented in this provider at a later date.");
            }
            QueryExpression.TopCount = topCountInt;
        }

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
            var attName = GetColumnLogicalAttributeName(column);
            QueryExpression.AddOrder(attName, orderType);
        }

        #endregion

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

        #region Helper Methods

        #endregion

        #endregion

        #region Projections

        private void NavigateProjections(IEnumerable<AliasedProjection> projections)
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

        #region Order By

        private void NavigateOrderBy(IEnumerable<OrderBy> orderBy)
        {
            foreach (var order in orderBy)
            {
                IVisitableBuilder o = order;
                o.Accept(this);
            }
        }

        #endregion

    }
}
