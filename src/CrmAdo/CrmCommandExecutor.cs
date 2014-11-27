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
using Microsoft.Xrm.Sdk.Metadata.Query;
using CrmAdo.Results;
using CrmAdo.Util.DynamicLinq;
using System.ComponentModel;
using CrmAdo.Metadata;

namespace CrmAdo
{

    public class CrmCommandExecutor : ICrmCommandExecutor
    {
        private ICrmRequestProvider _CrmRequestProvider;
        private ICrmMetaDataProvider _MetadataProvider;

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

        #region ICrmCommandExecutor

        public ResultSet ExecuteCommand(CrmDbCommand command, CommandBehavior behavior)
        {
            //TODO: Should process the command text, and execute a query to dynamics, returning the Entity Collection results.
            // what would these command types mean in terms of dynamics queries?
            ResultSet results = null;
            
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

        public int ExecuteNonQueryCommand(CrmDbCommand command)
        {
            // You can use ExecuteNonQuery to perform catalog operations (for example, querying the structure of a database or creating database objects such as tables), or to change the data in a database by executing UPDATE, INSERT, or DELETE statements.
            // Although ExecuteNonQuery does not return any rows, any output parameters or return values mapped to parameters are populated with data.
            // For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.
            List<ColumnMetadata> columnMetadata = null;
            var request = _CrmRequestProvider.GetOrganizationRequest(command, out columnMetadata);
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

            // we don't yet support other crm messages.
            throw new NotSupportedException();

            // return -1;
        }

        #endregion

        private void AssignResponseParameter(CrmDbCommand command, OrganizationResponse response)
        {
            if (command != null && command.Parameters.Contains(SystemCommandParameters.OrgResponse) && command.Parameters[SystemCommandParameters.OrgResponse].Direction == ParameterDirection.Output)
            {
                command.Parameters[SystemCommandParameters.OrgResponse].Value = response;
            }
        }

        private ResultSet ProcessTableDirectCommand(CrmDbCommand command, CommandBehavior behavior)
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
                resultSet = new EntityResultSet(command, request, null);
                AssignResponseParameter(command, response);
                resultSet.Results = response.EntityCollection;
            }
            else
            {
                resultSet = new EntityResultSet(command, null, null);
            }
            if (_MetadataProvider != null)
            {
                resultSet.ColumnMetadata = new List<ColumnMetadata>();
                resultSet.ColumnMetadata.AddRange(from a in _MetadataProvider.GetEntityMetadata(entityName).Attributes select new ColumnMetadata(a));
                resultSet.ColumnMetadata.Reverse();
            }
            return resultSet;
        }

        private ResultSet ProcessTextCommand(CrmDbCommand command, CommandBehavior behavior)
        {
            //  string commandText = "SELECT CustomerId, FirstName, LastName, Created FROM Customer";

            bool schemaOnly = (behavior & CommandBehavior.SchemaOnly) > 0;
            List<ColumnMetadata> columnMetadata = null;
            var request = _CrmRequestProvider.GetOrganizationRequest(command, out columnMetadata);

            var retrieveMultipleRequest = request as RetrieveMultipleRequest;
            if (retrieveMultipleRequest != null)
            {
                var resultSet = new EntityResultSet(command, retrieveMultipleRequest, columnMetadata);
                ProcessRetrieveMultiple(command, retrieveMultipleRequest, resultSet, schemaOnly);
                return resultSet;
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

            var retrieveMetadataChangesRequest = request as RetrieveMetadataChangesRequest;
            if (retrieveMetadataChangesRequest != null)
            {
                var resultSet = new EntityMetadataResultSet(command, retrieveMetadataChangesRequest, columnMetadata);
                ProcessRetrieveMetadataChangesRequest(command, retrieveMetadataChangesRequest, resultSet, schemaOnly);
                return resultSet;
            }

            throw new NotSupportedException("Sorry, was not able to turn your command into the appropriate Dynamics SDK Organization Request message.");

        }

        private ResultSet ProcessDeleteRequest(CrmDbCommand command, DeleteRequest deleteRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(deleteRequest);
            AssignResponseParameter(command, response);
            var resultSet = new EntityResultSet(command, deleteRequest, null);
            var delResponse = response as DeleteResponse;
            //if (delResponse != null)
            //{
            //    var result = delResponse.Results;
            //    resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            //}
            return resultSet;
        }

        private ResultSet ProcessUpdateRequest(CrmDbCommand command, UpdateRequest updateRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(updateRequest);
            var resultSet = new EntityResultSet(command, updateRequest, null);
            var updateResponse = response as UpdateResponse;
            if (updateResponse != null)
            {
                AssignResponseParameter(command, response);
                var result = updateRequest.Target;
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
            return resultSet;
        }

        private ResultSet ProcessCreateRequest(CrmDbCommand command, CreateRequest createRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(createRequest);
            var resultSet = new EntityResultSet(command, createRequest, null);
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

        private void ProcessRetrieveMultiple(CrmDbCommand command, RetrieveMultipleRequest retrieveMultipleRequest, EntityResultSet resultSet, bool schemaOnly = false)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
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
            //PopulateMetadata(resultSet, retrieveMultipleRequest.Query as QueryExpression);
            // return resultSet;
        }

        private ResultSet ProcessCreateEntityRequest(CrmDbCommand command, CreateEntityRequest createEntityRequest)
        {
            var orgService = command.CrmDbConnection.OrganizationService;
            var response = orgService.Execute(createEntityRequest);
            var resultSet = new EntityResultSet(command, createEntityRequest, null);
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
            var resultSet = new EntityResultSet(command, createOneToManyRequest, null);
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
            var resultSet = new EntityResultSet(command, createAttributeRequest, null);
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

        private void ProcessRetrieveMetadataChangesRequest(CrmDbCommand command, RetrieveMetadataChangesRequest retrieveMetadataChangesRequest, EntityMetadataResultSet resultSet, bool schemaOnly = false)
        {
            var orgService = command.CrmDbConnection.OrganizationService;

            CrmAdo.EntityMetadataResultSet.DenormalisedMetadataResult[] results = null;
            EntityMetadataDataSet ds = new EntityMetadataDataSet();
            if (!schemaOnly)
            {
                var response = orgService.Execute(retrieveMetadataChangesRequest);
                var retrieveMultipleResponse = response as RetrieveMetadataChangesResponse;
                if (retrieveMultipleResponse != null)
                {
                    AssignResponseParameter(command, response);
                    // denormalise object graph to results.
                    if (retrieveMultipleResponse.EntityMetadata != null)
                    {
                        //int index = 0;                    
                        //foreach (var item in retrieveMultipleResponse.EntityMetadata)
                        //{
                        //    ds.AddSdkMetadata(item, retrieveMultipleResponse.ServerVersionStamp);
                        //}

                        //var t = (from r in retrieveMultipleResponse.EntityMetadata
                        //         from a in (r.Attributes ?? Enumerable.Empty<AttributeMetadata>()).DefaultIfEmpty()
                        //         from o in (r.OneToManyRelationships ?? Enumerable.Empty<OneToManyRelationshipMetadata>()).Union(r.ManyToOneRelationships ?? Enumerable.Empty<OneToManyRelationshipMetadata>()).DefaultIfEmpty()
                        //         from m in (r.ManyToManyRelationships ?? Enumerable.Empty<ManyToManyRelationshipMetadata>()).DefaultIfEmpty()
                        //         select new DenormalisedMetadataResult { EntityMetadata = r, AttributeMetadata = a, OneToManyRelationship = o, ManyToManyRelationship = m }).AsEnumerable();

                        //DataTable resultTable = BuildMetadataResultDataTable(resultSet.ColumnMetadata, t);

                        var attFactory = new AttributeInfoFactory();

                        results = (from r in retrieveMultipleResponse.EntityMetadata
                                   from a in (r.Attributes ?? Enumerable.Empty<AttributeMetadata>()).DefaultIfEmpty()
                                   from o in (r.OneToManyRelationships ?? Enumerable.Empty<OneToManyRelationshipMetadata>()).Union(r.ManyToOneRelationships ?? Enumerable.Empty<OneToManyRelationshipMetadata>()).DefaultIfEmpty()
                                   from m in (r.ManyToManyRelationships ?? Enumerable.Empty<ManyToManyRelationshipMetadata>()).DefaultIfEmpty()
                                   select new CrmAdo.EntityMetadataResultSet.DenormalisedMetadataResult { EntityMetadata = r, AttributeMetadata = attFactory.Create(a), OneToManyRelationship = o, ManyToManyRelationship = m }).ToArray();

                        // var dataResults = resul


                        //var x = (from e in ds.EntityMetadata.AsEnumerable()
                        //         from a in ds.AttributeMetadata.as
                        //         from a in e.GetAttributeMetadataRows().AsEnumerable()    
                        //        ).AsQueryable().Select("");


                        //results = 

                        //    (from e in ds.EntityMetadata.AsQueryable()
                        //     from a in (e.GetAttributeMetadataRows() ?? Enumerable.Empty<CrmAdo.Metadata.EntityMetadataDataSet.AttributeMetadataRow>()).DefaultIfEmpty()).Select()


                        //   .Select()

                        //  select e[]
                        //          




                        //from a in (r.Attributes ?? Enumerable.Empty<AttributeMetadata>()).DefaultIfEmpty()
                        //from o in (r.OneToManyRelationships ?? Enumerable.Empty<OneToManyRelationshipMetadata>()).Union(r.ManyToOneRelationships ?? Enumerable.Empty<OneToManyRelationshipMetadata>()).DefaultIfEmpty()
                        //from m in (r.ManyToManyRelationships ?? Enumerable.Empty<ManyToManyRelationshipMetadata>()).DefaultIfEmpty()
                        //select new DenormalisedMetadataResult { EntityMetadata = r, AttributeMetadata = a, OneToManyRelationship = o, ManyToManyRelationship = m }).ToArray();


                        //              var q = from tA in tblA.AsEnumerable()
                        //join tB in tblB.AsEnumerable()
                        //on (int)tA["ID"] equals (int)tB["ID"]
                        //select new
                        //{
                        //  ID = (int)tA["ID"],
                        //  Name = (string)tA["NAME"],
                        //  Address = (string)tA["ADDRESS"],
                        //  Phone = (string)tB["PHONE"],
                        //  Mail = (string)tB["MAIL"]
                        //};

                        //   ds.DefaultViewManager.CreateDataView()



                        resultSet.Results = results;
                    }

                }
            }

            //var resultSet = new EntityMetadataResultSet(command, retrieveMetadataChangesRequest, results);
            // PopulateMetadata(resultSet, retrieveMetadataChangesRequest.Query);
            // return resultSet;
        }
                
        #region Metadata

        //private void PopulateMetadata(EntityResultSet resultSet, QueryExpression queryExpression)
        //{
        //    if (_MetadataProvider != null)
        //    {
        //        var metaData = new Dictionary<string, CrmEntityMetadata>();
        //        var columns = new List<ColumnMetadata>();
        //        PopulateColumnMetadata(queryExpression, metaData, columns);
        //        resultSet.ColumnMetadata = columns;
        //    }
        //}

        //public void PopulateColumnMetadata(QueryExpression query, Dictionary<string, CrmEntityMetadata> entityMetadata, List<ColumnMetadata> columns)
        //{
        //    // get metadata for this entities columns..
        //    if (!entityMetadata.ContainsKey(query.EntityName))
        //    {
        //        entityMetadata[query.EntityName] = _MetadataProvider.GetEntityMetadata(query.EntityName);
        //    }

        //    var entMeta = entityMetadata[query.EntityName];
        //    if (query.ColumnSet.AllColumns)
        //    {
        //        columns.AddRange((from c in entMeta.Attributes orderby c.LogicalName select new ColumnMetadata(c)));
        //    }
        //    else
        //    {
        //        columns.AddRange((from s in query.ColumnSet.Columns
        //                          join c in entMeta.Attributes
        //                              on s equals c.LogicalName
        //                          select new ColumnMetadata(c)));
        //    }

        //    if (query.LinkEntities != null && query.LinkEntities.Any())
        //    {
        //        foreach (var l in query.LinkEntities)
        //        {
        //            PopulateColumnMetadata(l, entityMetadata, columns);
        //        }
        //    }
        //}

        //public void PopulateColumnMetadata(LinkEntity linkEntity, Dictionary<string, CrmEntityMetadata> entityMetadata, List<ColumnMetadata> columns)
        //{
        //    // get metadata for this entities columns..
        //    if (!entityMetadata.ContainsKey(linkEntity.LinkToEntityName))
        //    {
        //        entityMetadata[linkEntity.LinkToEntityName] = _MetadataProvider.GetEntityMetadata(linkEntity.LinkToEntityName);
        //    }

        //    var entMeta = entityMetadata[linkEntity.LinkToEntityName];
        //    if (linkEntity.Columns.AllColumns)
        //    {
        //        columns.AddRange((from c in entMeta.Attributes orderby c.LogicalName select new ColumnMetadata(c, linkEntity.EntityAlias)));
        //        //columns.AddRange((from c in entMeta.Attributes select new ColumnMetadata(c, linkEntity.EntityAlias)).Reverse());
        //    }
        //    else
        //    {
        //        columns.AddRange((from s in linkEntity.Columns.Columns
        //                          join c in entMeta.Attributes
        //                              on s equals c.LogicalName
        //                          select new ColumnMetadata(c, linkEntity.EntityAlias)));

        //        //columns.AddRange((from c in entMeta.Attributes
        //        //                  join s in linkEntity.Columns.Columns
        //        //                      on c.LogicalName equals s
        //        //                  select new ColumnMetadata(c, linkEntity.EntityAlias)).Reverse());

        //    }

        //    if (linkEntity.LinkEntities != null && linkEntity.LinkEntities.Any())
        //    {
        //        foreach (var l in linkEntity.LinkEntities)
        //        {
        //            PopulateColumnMetadata(l, entityMetadata, columns);
        //        }
        //    }
        //}

        //private void PopulateMetadata(EntityMetadataResultSet resultSet, MetadataQueryExpression queryExpression)
        //{
        //    if (_MetadataProvider != null)
        //    {
        //        var metaData = new Dictionary<string, CrmEntityMetadata>();
        //        var columns = new List<ColumnMetadata>();
        //        PopulateColumnMetadata(queryExpression, metaData, columns);
        //        resultSet.ColumnMetadata = columns;
        //    }
        //}

        //public void PopulateColumnMetadata(MetadataQueryExpression query, Dictionary<string, CrmEntityMetadata> entityMetadata, List<ColumnMetadata> columns)
        //{
        //    // get metadata for this entities columns..
        //    if (!entityMetadata.ContainsKey("entitymetadata"))
        //    {
        //        entityMetadata["entitymetadata"] = _MetadataProvider.GetEntityMetadata("entitymetadata");
        //    }

        //    var entMeta = entityMetadata["entitymetadata"];
        //    if (query.Properties.AllProperties)
        //    {
        //        columns.AddRange((from c in entMeta.Attributes orderby c.LogicalName select new ColumnMetadata(c)));
        //    }
        //    else
        //    {
        //        columns.AddRange((from s in query.Properties.PropertyNames
        //                          join c in entMeta.Attributes
        //                              on s.ToLower() equals c.LogicalName
        //                          select new ColumnMetadata(c)));
        //    }




        //    // populate rest of metadata.
        //    //  throw new NotImplementedException();

        //}

        #endregion

        private EntityResultSet ProcessStoredProcedureCommand(CrmDbCommand command)
        {
            // What would a stored procedure be in terms of Dynamics Crm SDK?
            // Perhaps this could be used for exectuign fetch xml commands...?
            throw new System.NotImplementedException();
        }
    }

}