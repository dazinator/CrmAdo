using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public class BatchCrmOperationResult : ICrmOperationResult
    {

        private ExecuteMultipleResponse _ExecuteMultipleResponse;
        private ICrmOperationResult _CurrentOperationResult;

        public int CurrentResultIndex { get; private set; }

        public BatchCrmOperationResult(ExecuteMultipleResponse response, IList<ICrmOperation> operationResults)
        {
            _ExecuteMultipleResponse = response;
            CurrentResultIndex = -1;
            OperationResults = operationResults;
        }

        public ExecuteMultipleResponse ExecuteMultipleResponse
        {
            get { return _ExecuteMultipleResponse; }
        }

        public OrganizationResponse Response
        {
            get { return CurrentOperationResult.Response; }
            set { CurrentOperationResult.Response = value; }
        }

        public ResultSet ResultSet
        {
            get { return CurrentOperationResult.ResultSet; }
            set { CurrentOperationResult.ResultSet = value; }
        }

        public int ReturnValue
        {
            get
            {
                if (CurrentOperationResult.UseResultCountAsReturnValue)
                {
                    if (CurrentOperationResult.ResultSet != null)
                    {
                        return CurrentOperationResult.ResultSet.ResultCount();
                    }
                }
                return -1;
            }
        }

        public ICrmOperationResult CurrentOperationResult
        {
            get { return _CurrentOperationResult; }
        }

        public bool HasMoreResults
        {
            get { return CurrentResultIndex < OperationResults.Count - 1; }
        }

        public void NextOperationResult()
        {
            if (!HasMoreResults)
            {
                throw new InvalidOperationException("There are no more result.");
            }
            else
            {
                CurrentResultIndex = CurrentResultIndex + 1;
                var crmOperation = OperationResults[CurrentResultIndex];
                _CurrentOperationResult = crmOperation.Execute();
            }
        }

        public IList<ICrmOperation> OperationResults { get; set; }

        public bool UseResultCountAsReturnValue
        {
            get
            {
                return _CurrentOperationResult.UseResultCountAsReturnValue;
            }
            set
            {
                _CurrentOperationResult.UseResultCountAsReturnValue = value;
            }
        }
    }
}
