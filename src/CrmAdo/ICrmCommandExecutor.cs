using System.Data;

namespace CrmAdo
{
    public interface ICrmCommandExecutor
    {
        EntityResultSet ExecuteCommand(CrmDbCommand command, CommandBehavior behavior);
        int ExecuteNonQueryCommand(CrmDbCommand command);
    }

   
}