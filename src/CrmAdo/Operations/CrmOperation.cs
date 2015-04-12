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
         

        public CrmDbCommand DbCommand { get; set; }
        public CommandBehavior CommandBehavior { get; set; }
     
        public ICrmOperationResult Execute()
        {
            EnsureDbCommand();
            return this.ExecuteCommand();
        }

        protected abstract ICrmOperationResult ExecuteCommand();

        protected virtual EntityResultSet CreateEntityResultSet()
        {
            var resultSet = new EntityResultSet(DbCommand, Request, Columns);
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
                response = this.BatchOperation.BatchResponse.Responses[this.BatchRequestIndex].Response;
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
