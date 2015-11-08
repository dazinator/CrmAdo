using CrmAdo.Core;
using CrmAdo.Dynamics;
using CrmAdo.Operations;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
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

        public ICrmMetaDataProvider CrmMetadataProvider { get; set; }
        public DbParameterCollection Parameters { get; set; }
        public IDynamicsAttributeTypeProvider TypeProvider { get; set; }

        public bool IsMetadataQuery { get; set; }
        public bool IsBatchOperation { get; set; }

        private bool _DetectingMetadataQuery = false;
        public ICrmOperation CrmOperation { get; private set; }

        public ConnectionSettings Settings { get; set; }

        public OrganizationRequestBuilderVisitor(ICrmMetaDataProvider crmMetadataProvider, DbParameterCollection parameters, IDynamicsAttributeTypeProvider typeProvider, ConnectionSettings settings = null)
        {
            CrmMetadataProvider = crmMetadataProvider;
            Parameters = parameters;
            TypeProvider = typeProvider;
            Settings = settings ?? ConnectionSettings.Default();
            // OrgCommand = new OrganisationRequestCommand();
        }

        protected override void VisitBatch(BatchBuilder item)
        {
            CreateBatchOperation();
            foreach (var command in item.Commands())
            {
                command.Accept(this);
            }
            base.VisitBatch(item);
        }

        protected override void VisitSelect(SelectBuilder item)
        {
            // Could use alternate builders like a fetch xml builder.
            // If the SELECT is for entity metadata then perform a metadata query request.
            bool isMetadataQuery = CheckIsMetadataQuery(item);
            if (!isMetadataQuery)
            {
                var visitor = new RetrieveMultipleRequestBuilderVisitor(Parameters, CrmMetadataProvider, Settings);
                IVisitableBuilder visitable = item;
                visitable.Accept(visitor);
                SetOperation(visitor.GetCommand());
            }
            else
            {
                var visitor = new RetrieveMetadataChangesRequestBuilderVisitor(Parameters, CrmMetadataProvider, Settings);
                IVisitableBuilder visitable = item;
                visitable.Accept(visitor);
                SetOperation(visitor.GetCommand());
                // OrgCommand.OperationType = CrmOperation.RetrieveMetadataChanges;
            }
        }

        protected override void VisitInsert(InsertBuilder item)
        {
            var visitor = new CreateRequestBuilderVisitor(Parameters, CrmMetadataProvider, Settings);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            SetOperation(visitor.GetCommand());
        }

        protected override void VisitUpdate(UpdateBuilder item)
        {
            var visitor = new UpdateRequestBuilderVisitor(Parameters, CrmMetadataProvider, Settings);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            SetOperation(visitor.GetCommand());
        }

        protected override void VisitDelete(DeleteBuilder item)
        {
            var visitor = new DeleteRequestBuilderVisitor(Parameters, CrmMetadataProvider, TypeProvider, Settings);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            SetOperation(visitor.GetCommand());
        }

        protected override void VisitCreate(CreateBuilder item)
        {
            var visitor = new CreateEntityRequestBuilderVisitor(Parameters, CrmMetadataProvider, Settings);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            SetOperation(visitor.GetCommand());
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
            var visitor = new CreateAttributeRequestBuilderVisitor(Parameters, CrmMetadataProvider, Settings);
            IVisitableBuilder visitable = item;
            visitable.Accept(visitor);
            SetOperation(visitor.GetCommand());
        }

        private bool CheckIsMetadataQuery(SelectBuilder item)
        {
            IsMetadataQuery = false;
            _DetectingMetadataQuery = true;
            foreach (var source in item.From)
            {
                IVisitableBuilder visitable = source;
                visitable.Accept(this);
            }
            _DetectingMetadataQuery = false;
            return IsMetadataQuery;
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
                        IsMetadataQuery = true;
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

        protected virtual void SetOperation(ICrmOperation operation)
        {
            if (CrmOperation == null)
            {
                CrmOperation = operation;
                return;
            }
            else if (!IsBatchOperation)
            {
                throw new InvalidOperationException("A BatchOperation is required when translating multiple operations .");
            }
            var batchOperation = (BatchOperation)CrmOperation;
            batchOperation.AddOperation(operation);

        }

        protected void CreateBatchOperation()
        {
            CrmOperation = new BatchOperation();
            IsBatchOperation = true;
        }

    }
}
