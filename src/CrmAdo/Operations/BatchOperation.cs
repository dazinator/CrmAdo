using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public class BatchOperation : CrmOperation
    {
        public BatchOperation()
        {
            var executeMultipleRequest = new ExecuteMultipleRequest();
            executeMultipleRequest.Settings = new ExecuteMultipleSettings() { ContinueOnError = false, ReturnResponses = true };
            executeMultipleRequest.Requests = new OrganizationRequestCollection();
            Request = executeMultipleRequest;
            Operations = new List<ICrmOperation>();
        }

        private List<ICrmOperation> Operations { get; set; }

        public void AddOperation(ICrmOperation operation)
        {
            int count = Operations.Count;
            Operations.Add(operation);
            int startPosition = count;
            if (count > 0)
            {
                var last = Operations[count - 1];               
                var multipPartOp = last as IMultipartOperation;
                if (multipPartOp != null)
                {
                    startPosition = last.BatchRequestIndex + multipPartOp.RequestCount;
                }
                else
                {
                    startPosition = last.BatchRequestIndex + 1;
                }
            }

            if (operation.IsBatchRequest)
            {
                // Combine the ExecuteMultipleRequests by moving the requests out of it into the batch.
                var execMultipleRequest = (ExecuteMultipleRequest)operation.Request;
                if (execMultipleRequest.Settings != null && execMultipleRequest.Settings.ContinueOnError)
                {
                    throw new NotSupportedException("Cannot combine ExecuteMultipleRequest's with different ContinueOnError settings.");
                }
                foreach (var item in execMultipleRequest.Requests)
                {
                    BatchRequest.Requests.Add(item);
                }
            }
            else
            {
                BatchRequest.Requests.Add(operation.Request);
            }


            operation.BatchRequestIndex = startPosition;
            operation.BatchOperation = this;
            operation.DbCommand = this.DbCommand;
            operation.CommandBehavior = this.CommandBehavior;            
        }

        public ExecuteMultipleResponse BatchResponse { get; set; }

        protected override ICrmOperationResult ExecuteCommand()
        {
            var response = ExecuteOrganisationRequest();
            var executeMultipleResponse = response as ExecuteMultipleResponse;
            if (executeMultipleResponse != null)
            {
                BatchResponse = executeMultipleResponse;
                var opResult = new BatchCrmOperationResult(executeMultipleResponse, Operations);
                if (opResult.HasMoreResults)
                {
                    opResult.NextOperationResult();
                }
                return opResult;
            }

            throw new NotSupportedException();
        }

       
    }

    
}
