using CrmAdo.Operations;
using System.Collections.Generic;
using System.Data;

namespace CrmAdo.Core
{
    /// <summary>
    /// An executor should implement this interface to handle execution of the CrmDbCommand.
    /// </summary>
    public interface IOperationExecutor
    {
        //ResultSet ExecuteCommand(CrmDbCommand command, CommandBehavior behavior);
        //int ExecuteNonQueryCommand(CrmDbCommand command);

        ICrmOperationResult ExecuteOperation(ICrmOperation command);
        int ExecuteNonQueryOperation(ICrmOperation command);

        

    }

   
}