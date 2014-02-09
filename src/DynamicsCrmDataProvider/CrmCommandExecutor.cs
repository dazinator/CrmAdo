using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace DynamicsCrmDataProvider
{
    public class CrmCommandExecutor : ICrmCommandExecutor
    {
        private ICrmQueryExpressionProvider _CrmQueryExpressionProvider;
        private ICrmMetaDataProvider _MetadataProvider;

        #region Constructor
        public CrmCommandExecutor(CrmDbConnection connection)
            : this(new CrmQueryExpressionProvider(), connection)
        {
        }

        public CrmCommandExecutor(ICrmQueryExpressionProvider queryExpressionProvider, CrmDbConnection connection)
        {
            _CrmQueryExpressionProvider = queryExpressionProvider;
            if (connection != null)
            {
                _MetadataProvider = connection.MetadataProvider;
            }
        }
        #endregion

        public EntityResultSet ExecuteCommand(CrmDbCommand command)
        {
            //TODO: Should process the command text, and execute a query to dynamics, returning the Entity Collection results.
            // what would these command types mean in terms of dynamics queries?
            EntityResultSet results = null;
            switch (command.CommandType)
            {
                case CommandType.Text:
                    results = ProcessTextCommand(command);
                    break;
                case CommandType.TableDirect:
                    results = ProcessTableDirectCommand(command);
                    break;
                case CommandType.StoredProcedure:
                    results = ProcessStoredProcedureCommand(command);
                    break;
            }
            return results;
        }

        private EntityResultSet ProcessTableDirectCommand(CrmDbCommand command)
        {
            // The command should be the name of a single entity.
            var entityName = command.CommandText;
            if (entityName.Contains(" "))
            {
                throw new ArgumentException("When CommandType is TableDirect, CommandText should be the name of an entity.");
            }

            var orgService = command.CrmDbConnection.OrganizationService;
            // Todo: possibly support paging by returning a PagedEntityCollection implementation? 
            var results = orgService.RetrieveMultiple(new QueryExpression(entityName) { ColumnSet = new ColumnSet(true) });
            var resultSet = new EntityResultSet();
            if (_MetadataProvider != null)
            {
                resultSet.ColumnMetadata = _MetadataProvider.GetEntityMetadata(entityName).Attributes;
                resultSet.ColumnMetadata.Reverse();
            }
            resultSet.Results = results;
            return resultSet;
        }

        private EntityResultSet ProcessTextCommand(CrmDbCommand command)
        {
            //  string commandText = "SELECT CustomerId, FirstName, LastName, Created FROM Customer";
            var queryExpression = _CrmQueryExpressionProvider.CreateQueryExpression(command);
            var orgService = command.CrmDbConnection.OrganizationService;
            var results = orgService.RetrieveMultiple(queryExpression);

            var resultSet = new EntityResultSet();
            if (_MetadataProvider != null)
            {
                var entityMetadata = _MetadataProvider.GetEntityMetadata(queryExpression.EntityName);
                var columns = (from c in entityMetadata.Attributes
                               join s in queryExpression.ColumnSet.Columns
                                   on c.LogicalName equals s
                               select c).Reverse().ToList();
                resultSet.ColumnMetadata = columns;

            }

            resultSet.Results = results;
            return resultSet;
        }

        private EntityResultSet ProcessStoredProcedureCommand(CrmDbCommand command)
        {
            // What would a stored procedure be in terms of Dynamics Crm SDK?
            // Perhaps this could be used for exectuign fetch xml commands...?
            throw new System.NotImplementedException();
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