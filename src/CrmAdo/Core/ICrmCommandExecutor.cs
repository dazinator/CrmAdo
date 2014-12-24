using CrmAdo.Core;
using System.Collections.Generic;
using System.Data;

namespace CrmAdo.Core
{
    /// <summary>
    /// An executor should implement this interface to handle execution of the CrmDbCommand.
    /// </summary>
    public interface IOrgCommandExecutor
    {
        //ResultSet ExecuteCommand(CrmDbCommand command, CommandBehavior behavior);
        //int ExecuteNonQueryCommand(CrmDbCommand command);

        ResultSet ExecuteCommand(IOrgCommand command, CommandBehavior behavior);
        int ExecuteNonQueryCommand(IOrgCommand command);

        

    }

   
}