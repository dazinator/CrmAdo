using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsCrmDataProvider
{
    public interface ICrmCommandExecutor
    {
        EntityResultSet ExecuteCommand(CrmDbCommand command);
        int ExecuteNonQueryCommand(CrmDbCommand command);
    }

   
}