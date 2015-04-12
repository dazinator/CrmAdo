using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
   

    public class InsertOperation : CrmOperation, IMultipartOperation
    {
        public InsertOperation(List<ColumnMetadata> columnMetadata, OrganizationRequest request, bool hasOutput)
        {
            this.Columns = columnMetadata;
            this.Request = request;
            this.HasOutput = hasOutput;
        }

        public bool HasOutput { get; set; }

        protected override ICrmOperationResult ExecuteCommand()
        {
            CrmOperationResult commandResponse = null;
            var resultSet = CreateEntityResultSet();
            var response = ExecuteOrganisationRequest();
            commandResponse = new CrmOperationResult(response, resultSet, true);
            if (HasOutput)
            {
                HandleCreateWithRetreiveResponse(response as ExecuteMultipleResponse, resultSet);
            }
            else
            {
                HandleCreateResponse(response as CreateResponse, resultSet);
            }
            return commandResponse;
        }

        private void HandleCreateWithRetreiveResponse(ExecuteMultipleResponse executeMultipleResponse, EntityResultSet resultSet)
        {
            if (executeMultipleResponse != null)
            {
                // for execute reader and execute scalar purposes, we provide a result that has the newly created id of the entity.
                //  var execMultipleRequest = (ExecuteMultipleRequest)executeMultipleRequest;
                int createOperationBatchPosition = BatchRequestIndex;

                var batchRequest = BatchRequest;
                if (batchRequest == null)
                {
                    throw new InvalidOperationException("BatchRequest should not be null when executing a request with an output clause.");
                }

                var request = (CreateRequest)batchRequest.Requests[createOperationBatchPosition];
                int retrieveOperationBatchPosition = createOperationBatchPosition + 1;
                var retrieveResponse = (RetrieveResponse)executeMultipleResponse.Responses[retrieveOperationBatchPosition].Response;
                var result = retrieveResponse.Entity;

                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
        }

        private void HandleCreateResponse(CreateResponse createResponse, EntityResultSet resultSet)
        {
            if (createResponse != null)
            {
                // if(orgCommand.Columns.Any)
                // for execute reader and execute scalar purposes, we provide a result that has the newly created id of the entity.
                var request = (CreateRequest)Request;
                var entResult = new Entity(request.Target.LogicalName);

                // Populate results for execute reader and execute scalar purposes.
                var idattname = string.Format("{0}id", request.Target.LogicalName);
                entResult[idattname] = createResponse.id;
                entResult.Id = createResponse.id;
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { entResult }));

                // Add column metadata for the results we are returning, in this case it's just the id.
                string[] resultattributes = new string[] { idattname };
                //  AddResultColumnMetadata(orgCommand, resultSet, request.Target.LogicalName, resultattributes);
            }
        }

        public bool HasMultipleRequests
        {
            get
            {
                return HasOutput;
            }
        }

        public int RequestCount
        {
            get
            {
                return HasOutput ? 2 : 1;
            }
        }
    }
}
