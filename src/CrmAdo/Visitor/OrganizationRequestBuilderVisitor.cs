using CrmAdo.Core;
using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using CrmAdo.Enums;
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

        // public OrganizationRequest OrganizationRequest { get; set; }
        // public List<ColumnMetadata> ColumnMetadata { get; set; }

        public ICrmMetaDataProvider CrmMetadataProvider { get; set; }
        public DbParameterCollection Parameters { get; set; }
        public IDynamicsAttributeTypeProvider TypeProvider { get; set; }

        private bool _DetectingMetadataQuery = false;
        // private bool _IsMetadataQuery = false;
        public IOrgCommand OrgCommand { get; private set; }


        public OrganizationRequestBuilderVisitor(ICrmMetaDataProvider crmMetadataProvider, DbParameterCollection parameters, IDynamicsAttributeTypeProvider typeProvider)
        {
            CrmMetadataProvider = crmMetadataProvider;
            Parameters = parameters;
            TypeProvider = typeProvider;
            OrgCommand = new OrgCommand();
        }

        protected override void VisitSelect(SelectBuilder item)
        {
            // Could use alternate builders like a fetch xml builder.
            // If the SELECT is for entity metadata then perform a metadata query request.
            bool isMetadataQuery = IsMetadataQuery(item);
            if (!isMetadataQuery)
            {
                var visitor = new RetrieveMultipleRequestBuilderVisitor(Parameters, CrmMetadataProvider);
                IVisitableBuilder visitable = item;
                visitable.Accept(visitor);
                OrgCommand.Request = visitor.Request;
                OrgCommand.Columns = visitor.ColumnMetadata;
                OrgCommand.OperationType = CrmOperation.RetrieveMultiple;
            }
            else
            {
                var visitor = new RetrieveMetadataChangesRequestBuilderVisitor(Parameters, CrmMetadataProvider);
                IVisitableBuilder visitable = item;
                visitable.Accept(visitor);
                OrgCommand.Request = visitor.Request;
                OrgCommand.Columns = visitor.ColumnMetadata;
                OrgCommand.OperationType = CrmOperation.RetrieveMetadataChanges;
            }

        }

        protected override void VisitInsert(InsertBuilder item)
        {
            var visitor = new CreateRequestBuilderVisitor(Parameters, CrmMetadataProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrgCommand.Request = visitor.Request;
            OrgCommand.OperationType = CrmOperation.Create;

        }

        protected override void VisitUpdate(UpdateBuilder item)
        {
            var visitor = new UpdateRequestBuilderVisitor(Parameters, CrmMetadataProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrgCommand.Request = visitor.Request;
            OrgCommand.OperationType = CrmOperation.Update;
        }

        protected override void VisitDelete(DeleteBuilder item)
        {
            var visitor = new DeleteRequestBuilderVisitor(Parameters, CrmMetadataProvider, TypeProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrgCommand.Request = visitor.Request;
            OrgCommand.OperationType = CrmOperation.Delete;
        }

        protected override void VisitCreate(CreateBuilder item)
        {
            var visitor = new CreateEntityRequestBuilderVisitor(Parameters, CrmMetadataProvider);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            OrgCommand.Request = visitor.Request;
            OrgCommand.OperationType = CrmOperation.CreateEntity;
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
            OrgCommand.Request = visitor.Request;
            OrgCommand.OperationType = CrmOperation.CreateAttribute;
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
            return OrgCommand.OperationType == CrmOperation.RetrieveMetadataChanges;
        }

        protected override void VisitTable(Table item)
        {
            if (_DetectingMetadataQuery)
            {
                switch (item.Name.ToLower())
                {
                    case "entitymetadata":
                    case "attributemetadata":
                    case "onetomanyrelationshipmetadata":
                    case "manytomanyrelationshipmetadata":
                        OrgCommand.OperationType = CrmOperation.RetrieveMetadataChanges;
                        // _IsMetadataQuery = true;
                        break;
                }
            }
        }

        protected override void VisitAliasedSource(AliasedSource aliasedSource)
        {
            if (_DetectingMetadataQuery)
            {
                aliasedSource.Source.Accept(this);
            }
            // base.VisitAliasedSource(aliasedSource);
        }

        protected override void VisitInnerJoin(InnerJoin item)
        {
            if (_DetectingMetadataQuery)
            {
                item.RightHand.Source.Accept(this);
            }
        }

        protected override void VisitLeftOuterJoin(LeftOuterJoin item)
        {
            if (_DetectingMetadataQuery)
            {
                item.RightHand.Source.Accept(this);
            }
        }

    }
}
