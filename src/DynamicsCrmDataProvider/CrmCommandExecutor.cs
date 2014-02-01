using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace DynamicsCrmDataProvider
{
    public class CrmCommandExecutor : ICrmCommandExecutor
    {
        public EntityCollection ExecuteCommand(CrmDbCommand command)
        {
            //TODO: Should process the command text, and execute a query to dynamics, returning the Entity Collection results.
            return new EntityCollection(new List<Entity>());
        }
        public int ExecuteNonQueryCommand(CrmDbCommand command)
        {
            // You can use ExecuteNonQuery to perform catalog operations (for example, querying the structure of a database or creating database objects such as tables), or to change the data in a database by executing UPDATE, INSERT, or DELETE statements.
            // Although ExecuteNonQuery does not return any rows, any output parameters or return values mapped to parameters are populated with data.
            // For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.
            return -1;
        }
    }
}