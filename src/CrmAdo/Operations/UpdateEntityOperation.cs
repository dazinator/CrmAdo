using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public class UpdateEntityOperation : CrmOperation, IMultipartOperation
    {
        public UpdateEntityOperation(List<ColumnMetadata> columnMetadata, OrganizationRequest request, bool hasOutput)
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
                HandleUpdateWithRetreiveResponse(response as ExecuteMultipleResponse, resultSet);
            }
            else
            {
                HandleUpdateResponse(response as UpdateResponse, resultSet);
            }
            return commandResponse;
        }

        private void HandleUpdateWithRetreiveResponse(ExecuteMultipleResponse executeMultipleResponse, EntityResultSet resultSet)
        {
            if (executeMultipleResponse != null)
            {
                // for execute reader and execute scalar purposes, we provide a result that has the newly created id of the entity.
                //  var execMultipleRequest = (ExecuteMultipleRequest)executeMultipleRequest;
                int updateOperationBatchPosition = BatchRequestIndex;

                var batchRequest = BatchRequest;
                if (batchRequest == null)
                {
                    throw new InvalidOperationException("BatchRequest should not be null when executing a request with an output clause.");
                }

                var request = (UpdateRequest)batchRequest.Requests[updateOperationBatchPosition];
                int retrieveOperationBatchPosition = updateOperationBatchPosition + 1;
                var retrieveResponse = (RetrieveResponse)executeMultipleResponse.Responses[retrieveOperationBatchPosition].Response;
                var result = retrieveResponse.Entity;

                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
        }

        private void HandleUpdateResponse(UpdateResponse updateResponse, EntityResultSet resultSet)
        {
            if (updateResponse != null)
            {
                var updateRequest = (UpdateRequest)Request;
                var result = updateRequest.Target;
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));

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
