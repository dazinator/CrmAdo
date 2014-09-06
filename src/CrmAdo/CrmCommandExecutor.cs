using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using CrmAdo.Visitor;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmAdo
{
    public class CrmCommandExecutor : ICrmCommandExecutor
    {
        private ICrmRequestProvider _CrmRequestProvider;
        private ICrmMetaDataProvider _MetadataProvider;
        // private ISqlStatementTypeChecker _SqlStatementTypeChecker;

        #region Constructor
        public CrmCommandExecutor(CrmDbConnection connection)
            : this(new VisitingCrmRequestProvider(), connection)
        {
        }

        public CrmCommandExecutor(ICrmRequestProvider requestProvider, CrmDbConnection connection)
        {
            _CrmRequestProvider = requestProvider;
            // _SqlStatementTypeChecker = sqlStatementTypeChecker;
            if (connection != null)
            {
                _MetadataProvider = connection.MetadataProvider;
            }
        }
        #endregion

        public EntityResultSet ExecuteCommand(CrmDbCommand command, CommandBehavior behavior)
        {
            //TODO: Should process the command text, and execute a query to dynamics, returning the Entity Collection results.
            // what would these command types mean in terms of dynamics queries?
            EntityResultSet results = null;
            // if ((behavior & CommandBehavior.KeyInfo) > 0)
            switch (command.CommandType)
            {
                case CommandType.Text:
                    results = ProcessTextCommand(command, behavior);
                    break;
                case CommandType.TableDirect:
                    results = ProcessTableDirectCommand(command, behavior);
                    break;
                case CommandType.StoredProcedure:
                    results = ProcessStoredProcedureCommand(command);
                    break;
            }

            return results;
        }

        private void AssignResponseParameter(CrmDbCommand command, OrganizationResponse response)
        {
            if (command != null && command.Parameters.Contains(SystemCommandParameters.OrgResponse) && command.Parameters[SystemCommandParameters.OrgResponse].Direction == ParameterDirection.Output)
            {
                command.Parameters[SystemCommandParameters.OrgResponse].Value = response;
            }
        }

        private EntityResultSet ProcessTableDirectCommand(CrmDbCommand command, CommandBehavior behavior)
        {
            // The command should be the name of a single entity.
            var entityName = command.CommandText;
            if (entityName.Contains(" "))
            {
                throw new ArgumentException("When CommandType is TableDirect, CommandText should be the name of an entity.");
            }

            var orgService = command.CrmDbConnection.OrganizationService;
            EntityResultSet resultSet;
            // Todo: possibly support paging by returning a PagedEntityCollection implementation? 
            bool schemaOnly = (behavior & CommandBehavior.SchemaOnly) > 0;
            if (!schemaOnly)
            {
                var request = new RetrieveMultipleRequest()
                    {
                        Query = new QueryExpression(entityName) { ColumnSet = new ColumnSet(true), PageInfo = new PagingInfo() { ReturnTotalRecordCount = true } }
                    };
                var response = (RetrieveMultipleResponse)orgService.Execute(request);
                resultSet = new EntityResultSet(command, request);
                AssignResponseParameter(command, response);
                resultSet.Results = response.EntityCollection;
            }
            else
            {
                resultSet = new EntityResultSet(command, null);
            }
            if (_MetadataProvider != null)
            {
                resultSet.ColumnMetadata = new List<ColumnMetadata>();
                resultSet.ColumnMetadata.AddRange(from a in _MetadataProvider.GetEntityMetadata(entityName).Attributes select new ColumnMetadata(a));
                resultSet.ColumnMetadata.Reverse();
            }
            return resultSet;
        }

        private EntityResultSet ProcessTextCommand(CrmDbCommand command, CommandBehavior behavior)
        {
            //  string commandText = "SELECT CustomerId, FirstName, LastName, Created FROM Customer";

            bool schemaOnly = (behavior & CommandBehavior.SchemaOnly) > 0;

            var request = _CrmRequestProvider.GetOrganizationRequest(command);
            var retrieveMultipleRequest = request as RetrieveMultipleRequest;
            if (retrieveMultipleRequest != null)
            {
                return ProcessRetrieveMultiple(command, retrieveMultipleRequest, schemaOnly);
            }

            var createRequest = request as CreateRequest;
            if (createRequest != null)
            {
                return ProcessCreateRequest(command, createRequest);
            }

            var updateRequest = request as UpdateRequest;
            if (updateRequest != null)
            {
                return ProcessUpdateRequest(command, updateRequest);
            }

            var deleteRequest = request as DeleteRequest;
            if (deleteRequest != null)
            {
                return ProcessDeleteRequest(command, deleteRequest);
            }

            throw new NotSupportedException("Sorry, was not able to turn your command into the appropriate Dynamics SDK Organization Request message.");

        }

        private EntityResultSet ProcessDeleteRequest(CrmDbCommand command, DeleteRequest deleteRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(deleteRequest);
            AssignResponseParameter(command, response);
            var resultSet = new EntityResultSet(command, deleteRequest);
            var delResponse = response as DeleteResponse;
            //if (delResponse != null)
            //{
            //    var result = delResponse.Results;
            //    resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            //}
            return resultSet;
        }

        private EntityResultSet ProcessUpdateRequest(CrmDbCommand command, UpdateRequest updateRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(updateRequest);
            var resultSet = new EntityResultSet(command, updateRequest);
            var updateResponse = response as UpdateResponse;
            if (updateResponse != null)
            {
                AssignResponseParameter(command, response);
                var result = updateRequest.Target;
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
            return resultSet;
        }

        private EntityResultSet ProcessCreateRequest(CrmDbCommand command, CreateRequest createRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(createRequest);
            var resultSet = new EntityResultSet(command, createRequest);
            var createResponse = response as CreateResponse;
            if (createResponse != null)
            {
                AssignResponseParameter(command, response);
                // for execute reader and execute scalar purposes, we provide a result that just ahs the newly created id of the entity.
                var result = new Entity(createRequest.Target.LogicalName);
                var idattname = string.Format("{0}id", createRequest.Target.LogicalName);
                result[idattname] = createResponse.id;
                result.Id = createResponse.id;
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));

                // We populate metadata regarding the columns in the results. In this case its just the id attribute column for the inserted record.
                if (_MetadataProvider != null)
                {
                    var columns = new List<ColumnMetadata>();
                    var entityMeta = _MetadataProvider.GetEntityMetadata(createRequest.Target.LogicalName);
                    columns.AddRange((from c in entityMeta.Attributes
                                      join s in result.Attributes.Select(a => a.Key)
                                          on c.LogicalName equals s
                                      select new ColumnMetadata(c)).Reverse());
                    resultSet.ColumnMetadata = columns;
                }

            }
            return resultSet;
        }

        private EntityResultSet ProcessRetrieveMultiple(CrmDbCommand command, RetrieveMultipleRequest retrieveMultipleRequest, bool schemaOnly = false)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var resultSet = new EntityResultSet(command, retrieveMultipleRequest);
            if (!schemaOnly)
            {
                var response = orgService.Execute(retrieveMultipleRequest);
                var retrieveMultipleResponse = response as RetrieveMultipleResponse;
                if (retrieveMultipleResponse != null)
                {
                    AssignResponseParameter(command, response);
                    resultSet.Results = retrieveMultipleResponse.EntityCollection;
                }
            }
            PopulateMetadata(resultSet, retrieveMultipleRequest.Query as QueryExpression);
            return resultSet;
        }

        private EntityResultSet ProcessCreateEntityRequest(CrmDbCommand command, CreateEntityRequest createEntityRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(createEntityRequest);
            var resultSet = new EntityResultSet(command, createEntityRequest);
            var createResponse = response as CreateEntityResponse;
            if (createResponse != null)
            {
                AssignResponseParameter(command, response);
                // for execute reader and execute scalar purposes, we provide a result that just ahs the newly created id of the entity.
                var result = new Entity("entitymetadata");
                var idattname = string.Format("entitymetadataid");

                result[idattname] = createResponse.EntityId;
                result.Id = createResponse.EntityId;
                result["primaryattributeid"] = createResponse.AttributeId;

                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));

                //var columns = new List<ColumnMetadata>();
                //var col = new ColumnMetadata(null);
               

                //col.ColumnName = "primaryattributeid";
                //col.LogicalAttributeName = "primaryattributeid";

                // We populate metadata regarding the columns in the results. In this case its just the id attribute column for the inserted record.
                //if (_MetadataProvider != null)
                //{
                //    var columns = new List<ColumnMetadata>();
                //    var entityMeta = _MetadataProvider.GetEntityMetadata("");
                //    columns.AddRange((from c in entityMeta.Attributes
                //                      join s in result.Attributes.Select(a => a.Key)
                //                          on c.LogicalName equals s
                //                      select new ColumnMetadata(c)).Reverse());
                //    resultSet.ColumnMetadata = columns;
                //}

            }
            return resultSet;
        }

        private object ProcessCreateOnetoManyRequest(CrmDbCommand command, CreateOneToManyRequest createOneToManyRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(createOneToManyRequest);
            var resultSet = new EntityResultSet(command, createOneToManyRequest);
            var createResponse = response as CreateOneToManyResponse;
            if (createResponse != null)
            {
                AssignResponseParameter(command, response);
                // for execute reader and execute scalar purposes, we provide a result that just ahs the newly created id of the entity.
                var result = new Entity("attributemetadata");
                var idattname = string.Format("attributemetadataid");
                result[idattname] = createResponse.AttributeId;
                result.Id = createResponse.AttributeId;
                result["relationshipid"] = createResponse.RelationshipId;              
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
            return resultSet;
        }

        private object ProcessCreateAttributeRequest(CrmDbCommand command, CreateAttributeRequest createAttributeRequest)
        {         
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(createAttributeRequest);
            var resultSet = new EntityResultSet(command, createAttributeRequest);
            var createResponse = response as CreateAttributeResponse;
            if (createResponse != null)
            {
                AssignResponseParameter(command, response);              
                // for execute reader and execute scalar purposes, we provide a result that just ahs the newly created id of the entity.
                var result = new Entity("attributemetadata");
                var idattname = string.Format("attributemetadataid");
                result[idattname] = createResponse.AttributeId;
                result.Id = createResponse.AttributeId;
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
            return resultSet;
        }

        #region Metadata

        private void PopulateMetadata(EntityResultSet resultSet, QueryExpression queryExpression)
        {
            if (_MetadataProvider != null)
            {
                var metaData = new Dictionary<string, CrmEntityMetadata>();
                var columns = new List<ColumnMetadata>();
                PopulateColumnMetadata(queryExpression, metaData, columns);
                resultSet.ColumnMetadata = columns;
            }
        }

        public void PopulateColumnMetadata(QueryExpression query, Dictionary<string, CrmEntityMetadata> entityMetadata, List<ColumnMetadata> columns)
        {
            // get metadata for this entities columns..
            if (!entityMetadata.ContainsKey(query.EntityName))
            {
                entityMetadata[query.EntityName] = _MetadataProvider.GetEntityMetadata(query.EntityName);
            }

            var entMeta = entityMetadata[query.EntityName];
            if (query.ColumnSet.AllColumns)
            {
                columns.AddRange((from c in entMeta.Attributes orderby c.LogicalName select new ColumnMetadata(c)));
            }
            else
            {
                columns.AddRange((from s in query.ColumnSet.Columns
                                  join c in entMeta.Attributes
                                      on s equals c.LogicalName
                                  select new ColumnMetadata(c)));
            }

            if (query.LinkEntities != null && query.LinkEntities.Any())
            {
                foreach (var l in query.LinkEntities)
                {
                    PopulateColumnMetadata(l, entityMetadata, columns);
                }
            }
        }

        public void PopulateColumnMetadata(LinkEntity linkEntity, Dictionary<string, CrmEntityMetadata> entityMetadata, List<ColumnMetadata> columns)
        {
            // get metadata for this entities columns..
            if (!entityMetadata.ContainsKey(linkEntity.LinkToEntityName))
            {
                entityMetadata[linkEntity.LinkToEntityName] = _MetadataProvider.GetEntityMetadata(linkEntity.LinkToEntityName);
            }

            var entMeta = entityMetadata[linkEntity.LinkToEntityName];
            if (linkEntity.Columns.AllColumns)
            {
                columns.AddRange((from c in entMeta.Attributes orderby c.LogicalName select new ColumnMetadata(c, linkEntity.EntityAlias)));
                //columns.AddRange((from c in entMeta.Attributes select new ColumnMetadata(c, linkEntity.EntityAlias)).Reverse());
            }
            else
            {
                columns.AddRange((from s in linkEntity.Columns.Columns
                                  join c in entMeta.Attributes
                                      on s equals c.LogicalName
                                  select new ColumnMetadata(c, linkEntity.EntityAlias)));

                //columns.AddRange((from c in entMeta.Attributes
                //                  join s in linkEntity.Columns.Columns
                //                      on c.LogicalName equals s
                //                  select new ColumnMetadata(c, linkEntity.EntityAlias)).Reverse());

            }

            if (linkEntity.LinkEntities != null && linkEntity.LinkEntities.Any())
            {
                foreach (var l in linkEntity.LinkEntities)
                {
                    PopulateColumnMetadata(l, entityMetadata, columns);
                }
            }
        }
        #endregion

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

            var request = _CrmRequestProvider.GetOrganizationRequest(command);
            var createRequest = request as CreateRequest;
            if (createRequest != null)
            {
                //todo check for output paramater named after id, and set it..
                var results = ProcessCreateRequest(command, createRequest);
                return results.ResultCount();
            }

            var updateRequest = request as UpdateRequest;
            if (updateRequest != null)
            {
                var results = ProcessUpdateRequest(command, updateRequest);
                return results.ResultCount();
            }

            var deleteRequest = request as DeleteRequest;
            if (deleteRequest != null)
            {
                var results = ProcessDeleteRequest(command, deleteRequest);
                return results.ResultCount();
            }

            var createEntityRequest = request as CreateEntityRequest;
            if (createEntityRequest != null)
            {
                var results = ProcessCreateEntityRequest(command, createEntityRequest);
                return -1;
            }

            var createAttributeRequest = request as CreateAttributeRequest;
            if (createAttributeRequest != null)
            {
                var results = ProcessCreateAttributeRequest(command, createAttributeRequest);
                return -1;
            }

            var createOneToManyRequest = request as CreateOneToManyRequest;
            if (createOneToManyRequest != null)
            {
                var results = ProcessCreateOnetoManyRequest(command, createOneToManyRequest);
                return -1;
            }

            // we don't yet support any DDL.
            throw new NotSupportedException();

            // return -1;
        }

       




    }

}