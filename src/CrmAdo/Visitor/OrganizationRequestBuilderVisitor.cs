using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CrmAdo.Visitor
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="OrganizationRequest"/> when it visits an <see cref="ICommand"/> 
    /// </summary>
    public class OrganizationRequestBuilderVisitor : BuilderVisitor
    {
        public const string ParameterToken = "@";

        public OrganizationRequest OrganizationRequest { get; set; }
        public ICrmMetaDataProvider CrmMetadataProvider { get; set; }
        public DbParameterCollection Parameters { get; set; }
        public IDynamicsAttributeTypeProvider TypeProvider { get; set; }

        private bool _DetectingMetadataQuery = false;
        private bool _IsMetadataQuery = false;


        public OrganizationRequestBuilderVisitor(ICrmMetaDataProvider crmMetadataProvider, DbParameterCollection parameters, IDynamicsAttributeTypeProvider typeProvider)
        {
            CrmMetadataProvider = crmMetadataProvider;
            Parameters = parameters;
            TypeProvider = typeProvider;
        }

        protected override void VisitSelect(SelectBuilder item)
        {
            // Could use alternate builders like a fetch xml builder.
            // If the SELECT is for entity metadata then perform a metadata query request.

            bool isMetadataQuery = IsMetadataQuery(item);
            if (!isMetadataQuery)
            {
                var visitor = new RetrieveMultipleRequestBuilderVisitor(Parameters);
                IVisitableBuilder visitable = item;
                visitable.Accept(visitor);
                OrganizationRequest = visitor.Request;
            }
            else
            {
                var visitor = new RetrieveMetadataChangesRequestBuilderVisitor(Parameters);
                IVisitableBuilder visitable = item;
                visitable.Accept(visitor);
                OrganizationRequest = visitor.Request;
            }

        }

        protected override void VisitInsert(InsertBuilder item)
        {
            var visitor = new CreateRequestBuilderVisitor(Parameters, CrmMetadataProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrganizationRequest = visitor.Request;
        }

        protected override void VisitUpdate(UpdateBuilder item)
        {
            var visitor = new UpdateRequestBuilderVisitor(Parameters, CrmMetadataProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrganizationRequest = visitor.Request;
        }

        protected override void VisitDelete(DeleteBuilder item)
        {
            var visitor = new DeleteRequestBuilderVisitor(Parameters, CrmMetadataProvider, TypeProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrganizationRequest = visitor.Request;
        }

        protected override void VisitCreate(CreateBuilder item)
        {
            var visitor = new CreateEntityRequestBuilderVisitor(Parameters, CrmMetadataProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrganizationRequest = visitor.Request;
        }

        protected override void VisitAlter(AlterBuilder item)
        {
            if (item.AlterObject != null)
            {
                item.AlterObject.Accept(this);
            }
        }

        protected override void VisitAlterTableDefinition(AlterTableDefinition item)
        {
            var visitor = new CreateAttributeRequestBuilderVisitor(Parameters, CrmMetadataProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrganizationRequest = visitor.Request;
        }

        private bool IsMetadataQuery(SelectBuilder item)
        {
            _DetectingMetadataQuery = true;
            foreach (var source in item.From)
            {
                IVisitableBuilder visitable = source;
                visitable.Accept(this);
            }
            _DetectingMetadataQuery = false;
            return _IsMetadataQuery;
        }

        protected override void VisitTable(Table item)
        {
            if (_DetectingMetadataQuery)
            {
                if (item.Name.ToLower() == "entitymetadata")
                {
                    _IsMetadataQuery = true;
                }
            }
        }

        protected override void VisitAliasedSource(AliasedSource aliasedSource)
        {
            aliasedSource.Source.Accept(this);
            // base.VisitAliasedSource(aliasedSource);
        }

    }
}
