using Microsoft.Xrm.Sdk;

namespace DynamicsCrmDataProvider
{
    public interface ICrmCommandExecutor
    {
        EntityCollection ExecuteCommand(CrmDbCommand command);
        int ExecuteNonQueryCommand(CrmDbCommand command);
    }
}