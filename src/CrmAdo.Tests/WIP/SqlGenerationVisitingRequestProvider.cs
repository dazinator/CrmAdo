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
                _Visitor.Level = _Visitor.Level - 1;
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

        #region Visit Methods
        protected override void VisitSelect(SelectBuilder item)
        {
            Request.Query = new QueryExpression();
            if (item.From.Any())
            {
                VisitEach(item.From);
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

        #endregion
    }

    public class SqlGenerationVisitingRequestProvider : OrganizationRequestBuilderVisitor
    {
        protected override void VisitSelect(SQLGeneration.Builders.SelectBuilder item)
        {
            var selectVisitorBuilder = new RetrieveMultipleRequestBuilderVisitor();
            ((IVisitableBuilder)item).Accept(selectVisitorBuilder);

            throw new NotImplementedException();

            //AddColumns(selCommand.Projection, query);
            //if (selCommand.WhereFilterGroup != null)
            //{
            //    ProcessFilterGroup(query, selCommand.WhereFilterGroup, null, paramaters);
            //}
            //else
            //{
            //    AddWhere(selCommand.Where, query, paramaters);
            //}
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
