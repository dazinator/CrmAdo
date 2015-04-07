using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public class OrganisationRequestCommandResult : ICrmOperationResult
    {

        public bool UseResultCountAsReturnValue { get; set; }

        public OrganisationRequestCommandResult(OrganizationResponse response, ResultSet resultSet, bool useResultCountAsReturnValue)
        {
            this.Response = response;
            this.ResultSet = resultSet;
            UseResultCountAsReturnValue = useResultCountAsReturnValue;
        }

        public OrganizationResponse Response { get; set; }
        public ResultSet ResultSet { get; set; }

        public int ReturnValue
        {
            get
            {
                if (UseResultCountAsReturnValue)
                {
                    if (ResultSet != null)
                    {
                        return ResultSet.ResultCount();
                    }
                }
                return -1;
            }
        }


    }
}
