using System.Data;

namespace CrmAdo
{
    /// <summary>
    /// An executor should implement this interface to handle execution of the CrmDbCommand.
    /// </summary>
    public interface ICrmCommandExecutor
    {
        EntityResultSet ExecuteCommand(CrmDbCommand command, CommandBehavior behavior);
        int ExecuteNonQueryCommand(CrmDbCommand command);
    }

   
}