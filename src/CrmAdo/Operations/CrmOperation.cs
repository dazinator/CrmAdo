using CrmAdo.Util;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public abstract class CrmOperation : ICrmOperation
    {

        public List<ColumnMetadata> Columns { get; set; }
        public OrganizationRequest Request { get; set; }
        public ExecuteMultipleRequest BatchRequest
        {
            get
            {
                return Request as ExecuteMultipleRequest;
            }
        }

        public bool IsBatchRequest
        {
            get
            {
                return Request is ExecuteMultipleRequest;
            }
        }

        public int BatchRequestIndex { get; set; }

        public BatchOperation BatchOperation { get; set; }

        private CrmDbCommand _DbCommand { get; set; }
        public virtual CrmDbCommand DbCommand
        {
            get
            {
                if (BatchOperation != null)
                {
                    return BatchOperation.DbCommand;
                }
                else
                {
                    return _DbCommand;
                }
            }
            set
            {
                if (BatchOperation != null)
                {
                    BatchOperation.DbCommand = value;
                }
                else
                {
                    _DbCommand = value;
                }
            }
        }

        private CommandBehavior _CommandBehavior { get; set; }
        public virtual CommandBehavior CommandBehavior
        {
            get
            {
                if (BatchOperation != null)
                {
                    return BatchOperation.CommandBehavior;
                }
                else
                {
                    return _CommandBehavior;
                }
            }
            set
            {
                if (BatchOperation != null)
                {
                    BatchOperation.CommandBehavior = value;
                }
                else
                {
                    _CommandBehavior = value;
                }
            }
        }

        public ICrmOperationResult Execute()
        {
            EnsureDbCommand();
            return this.ExecuteCommand();
        }

        protected abstract ICrmOperationResult ExecuteCommand();

        protected virtual EntityResultSet CreateEntityResultSet()
        {
            var resultSet = new EntityResultSet(DbCommand.CrmDbConnection, Request, Columns);
            return resultSet;
        }

        protected virtual OrganizationResponse ExecuteOrganisationRequest()
        {
            var dbCommand = DbCommand;
            var orgService = dbCommand.CrmDbConnection.OrganizationService;

            // Execute the request and obtain the result.
            OrganizationResponse response = null;

            // If this operation is part of a batch of operations, then the request,
            // is executed allready at the batch level.            
            if (this.BatchOperation == null)
            {
                // otherwise, execute the individual request for this operation.
                response = orgService.Execute(Request);
            }
            else
            {
                // grab the response for this operation from the batch responses.
                var multiPart = this as IMultipartOperation;
                if (multiPart != null && multiPart.HasMultipleRequests)
                {
                    // rebuild an ExecuteMultipleResponse to contain just the responses for this multipart operation.
                    var responses = new ExecuteMultipleResponseItemCollection();
                    ExecuteMultipleResponse multipleResponse = new ExecuteMultipleResponse
                {
                    Results = new ParameterCollection
                    {
                        { "Responses", responses },
                        { "IsFaulted", this.BatchOperation.BatchResponse.IsFaulted}
                    }
                };

                    for (int i = 0; i < multiPart.RequestCount; i++)
                    {
                        int reqIndex = this.BatchRequestIndex + i;
                        var resp = this.BatchOperation.BatchResponse.Responses[reqIndex];
                        responses.Add(resp);
                    }

                    return multipleResponse;
                }

                return this.BatchOperation.BatchResponse.Responses[this.BatchRequestIndex].Response;
            }

            // Allow poeople to get access to the response directly via an output paramater.
            AssignResponseParameter(dbCommand, response);
            return response;
        }

        protected void AssignResponseParameter(CrmDbCommand command, OrganizationResponse response)
        {
            if (command != null && command.Parameters.Contains(SystemCommandParameters.OrgResponse) && command.Parameters[SystemCommandParameters.OrgResponse].Direction == ParameterDirection.Output)
            {
                command.Parameters[SystemCommandParameters.OrgResponse].Value = response;
            }
        }

        public virtual bool IsSchemaOnly()
        {
            bool isSchemaOnly = (CommandBehavior & CommandBehavior.SchemaOnly) > 0;
            return isSchemaOnly;
        }

        protected virtual void EnsureDbCommand()
        {
            if (DbCommand == null)
            {
                throw new InvalidOperationException("DbCommand is null");
            }
        }



    }
}
