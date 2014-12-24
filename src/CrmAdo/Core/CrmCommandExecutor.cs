using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using CrmAdo.Visitor;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using CrmAdo.Results;
using CrmAdo.Util;
using System.ComponentModel;
using CrmAdo.Metadata;
using CrmAdo.Dynamics;
using SQLGeneration.Generators;
using SQLGeneration.Builders;
using CrmAdo.Core;
using CrmAdo.Enums;

namespace CrmAdo.Core
{

    public class CrmOrgCommandExecutor : IOrgCommandExecutor
    {
        //   private ICrmMetaDataProvider _MetadataProvider;

        private static readonly CrmOrgCommandExecutor _Instance = new CrmOrgCommandExecutor();

        private CrmOrgCommandExecutor() { }

        public static CrmOrgCommandExecutor Instance
        {
            get
            {
                return _Instance;
            }
        }

        #region Constructor

        //public CrmOrgCommandExecutor(ICrmMetaDataProvider metadataProvider)
        //{
        //    if (metadataProvider == null)
        //    {
        //        throw new ArgumentNullException("metadataProvider");
        //        // 
        //    }
        //    _MetadataProvider = metadataProvider;
        //}

        #endregion

        #region ICrmCommandExecutor

        public ResultSet ExecuteCommand(IOrgCommand command, CommandBehavior behaviour)
        {
            //TODO: Should process the command text, and execute a query to dynamics, returning the Entity Collection results.
            // what would these command types mean in terms of dynamics queries?
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (command.DbCommand == null)
            {
                throw new InvalidOperationException("CrmDbCommand is null");
            }
            var dbCommand = command.DbCommand;
            ResultSet results = null;

            // if ((behavior & CommandBehavior.KeyInfo) > 0)
            switch (command.DbCommand.CommandType)
            {
                case CommandType.Text:
                case CommandType.TableDirect:
                    results = ExecuteOrganisationCommand(command, command.CommandBehavior);
                    break;
                case CommandType.StoredProcedure:
                    results = ProcessStoredProcedureCommand(command);
                    break;
            }

            return results;
        }

        public EntityResultSet CreateEntityResultSet(IOrgCommand command)
        {
            var resultSet = new EntityResultSet(command.DbCommand, command.Request, command.Columns);
            return resultSet;
        }

        public int ExecuteNonQueryCommand(IOrgCommand command)
        {
            // You can use ExecuteNonQuery to perform catalog operations (for example, querying the structure of a database or creating database objects such as tables), or to change the data in a database by executing UPDATE, INSERT, or DELETE statements.
            // Although ExecuteNonQuery does not return any rows, any output parameters or return values mapped to parameters are populated with data.
            // For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            if (command.DbCommand == null)
            {
                throw new InvalidOperationException("CrmDbCommand is null");
            }

            switch (command.OperationType)
            {
                case CrmOperation.Create:
                    var createResult = ProcessCreateRequest(command);
                    return createResult.ResultCount();

                case CrmOperation.Update:

                    var updateResult = ProcessUpdateRequest(command);
                    return updateResult.ResultCount();

                case CrmOperation.Delete:

                    var deleteResult = ProcessDeleteRequest(command);
                    return deleteResult.ResultCount();

                case CrmOperation.CreateAttribute:

                    ProcessCreateAttributeRequest(command);
                    return -1;

                case CrmOperation.CreateEntity:

                    ProcessCreateEntityRequest(command);
                    return -1;

                case CrmOperation.CreateOneToMany:

                    ProcessCreateOnetoManyRequest(command);
                    return -1;
            }

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

        private ResultSet ExecuteOrganisationCommand(IOrgCommand orgCommand, CommandBehavior behavior)
        {
            //  string commandText = "SELECT CustomerId, FirstName, LastName, Created FROM Customer";
            //   List<ColumnMetadata> columnMetadata = null;

            var command = orgCommand.DbCommand;
            bool schemaOnly = (behavior & CommandBehavior.SchemaOnly) > 0;

            var request = orgCommand.Request;
            ResultSet resultSet = null; // = CreateEntityResultSet(orgCommand);

            switch (orgCommand.OperationType)
            {
                case CrmOperation.RetrieveMultiple:
                    resultSet = ExecuteRetrieveMultipleCommand(orgCommand, schemaOnly);
                    break;
                case CrmOperation.RetrieveMetadataChanges:
                    resultSet = ProcessRetrieveMetadataChangesRequest(orgCommand, schemaOnly);
                    break;
                case CrmOperation.Create:
                    resultSet = ProcessCreateRequest(orgCommand);
                    break;
                case CrmOperation.Update:
                    resultSet = ProcessUpdateRequest(orgCommand);
                    break;
                case CrmOperation.Delete:
                    resultSet = ProcessDeleteRequest(orgCommand);
                    break;
                case CrmOperation.CreateAttribute:
                    resultSet = ProcessCreateAttributeRequest(orgCommand);
                    break;
                case CrmOperation.CreateEntity:
                    resultSet = ProcessCreateEntityRequest(orgCommand);
                    break;
                case CrmOperation.CreateOneToMany:
                    resultSet = ProcessCreateOnetoManyRequest(orgCommand);
                    break;
            }

            if (resultSet == null)
            {
                throw new NotSupportedException("Sorry, was not able to translate the command into the appropriate CRM SDK Organization Request message.");
            }

            return resultSet;

        }

        private ResultSet ExecuteRetrieveMultipleCommand(IOrgCommand orgCommand, bool schemaOnly = false)
        {
            var orgService = orgCommand.DbCommand.CrmDbConnection.OrganizationService;
            var entityResultSet = CreateEntityResultSet(orgCommand);
            if (!schemaOnly)
            {
                var response = orgService.Execute(orgCommand.Request);
                var retrieveMultipleResponse = response as RetrieveMultipleResponse;
                if (retrieveMultipleResponse != null)
                {
                    AssignResponseParameter(orgCommand.DbCommand, response);
                    entityResultSet.Results = retrieveMultipleResponse.EntityCollection;
                }
            }
            return entityResultSet;
            //PopulateMetadata(resultSet, retrieveMultipleRequest.Query as QueryExpression);
            // return resultSet;
        }

        private ResultSet ProcessDeleteRequest(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            var deleteRequest = (DeleteRequest)orgCommand.Request;

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

        private ResultSet ProcessUpdateRequest(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            var updateRequest = (UpdateRequest)orgCommand.Request;

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

        private ResultSet ProcessCreateRequest(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            // Execute the request and obtain the result.
            var response = orgService.Execute(orgCommand.Request);

            var resultSet = CreateEntityResultSet(orgCommand);

            var createResponse = response as CreateResponse;
            if (createResponse != null)
            {
                AssignResponseParameter(command, response);

                // for execute reader and execute scalar purposes, we provide a result that has the newly created id of the entity.
                var request = (CreateRequest)orgCommand.Request;
                var result = new Entity(request.Target.LogicalName);

                // Populate results for execute reader and execute scalar purposes.
                var idattname = string.Format("{0}id", request.Target.LogicalName);
                result[idattname] = createResponse.id;
                result.Id = createResponse.id;
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));

                // Add column metadata for the results we are returning, in this case it's just the id.
                string[] resultattributes = new string[] { idattname };
                AddResultColumnMetadata(orgCommand, resultSet, request.Target.LogicalName, resultattributes);


            }
            return resultSet;
        }

        private void AddResultColumnMetadata(IOrgCommand orgCommand, EntityResultSet results, string entityName, string[] attributeNames)
        {
            var metadataProvider = orgCommand.DbCommand.CrmDbConnection.MetadataProvider;
            if (metadataProvider != null)
            {
                if (results.HasResults())
                {
                    if (results.ColumnMetadata == null)
                    {
                        results.ColumnMetadata = new List<ColumnMetadata>(); ;
                    }

                    var entityMeta = metadataProvider.GetEntityMetadata(entityName);
                    results.ColumnMetadata.AddRange(
                        (from c in attributeNames
                         join s in entityMeta.Attributes
                         on c equals s.LogicalName
                         select new ColumnMetadata(s)));
                }
            }
        }

        private ResultSet ProcessCreateEntityRequest(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            var createEntityRequest = (CreateEntityRequest)orgCommand.Request;

            var response = orgService.Execute(createEntityRequest);
            var resultSet = CreateEntityResultSet(orgCommand);

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

                //  We populate metadata regarding the columns in the results. In this case its just the id attribute column for the inserted record.
                //var metaDataProvider = orgCommand.DbCommand.CrmDbConnection.MetadataProvider;
                //if (metaDataProvider != null)
                //{
                //    var columns = new List<ColumnMetadata>();
                //    var entityMeta = metaDataProvider.GetEntityMetadata(createEntityRequest.Entity.LogicalName);
                //    columns.AddRange((from c in entityMeta.Attributes
                //                      join s in result.Attributes.Select(a => a.Key)
                //                          on c.LogicalName equals s
                //                      select new ColumnMetadata(c)).Reverse());
                //    resultSet.ColumnMetadata = columns;
                //}

            }
            return resultSet;
        }

        private ResultSet ProcessCreateOnetoManyRequest(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            var createOneToManyRequest = (CreateOneToManyRequest)orgCommand.Request;

            var response = orgService.Execute(createOneToManyRequest);
            var resultSet = CreateEntityResultSet(orgCommand);

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

        private ResultSet ProcessCreateAttributeRequest(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            var createAttributeRequest = (CreateAttributeRequest)orgCommand.Request;

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

        private ResultSet ProcessRetrieveMetadataChangesRequest(IOrgCommand orgCommand, bool schemaOnly = false)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            CrmAdo.EntityMetadataResultSet.DenormalisedMetadataResult[] results = null;
            // EntityMetadataDataSet ds = new EntityMetadataDataSet();
            var metadataResultSet = new EntityMetadataResultSet(command, orgCommand.Request, orgCommand.Columns);

            if (!schemaOnly)
            {
                var retrieveMetadataChangesRequest = (RetrieveMetadataChangesRequest)orgCommand.Request;
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



                        metadataResultSet.Results = results;

                    }

                }
            }

            return metadataResultSet;

            //var resultSet = new EntityMetadataResultSet(command, retrieveMetadataChangesRequest, results);
            // PopulateMetadata(resultSet, retrieveMetadataChangesRequest.Query);
            // return resultSet;
        }

        private EntityResultSet ProcessStoredProcedureCommand(IOrgCommand command)
        {
            // What would a stored procedure be in terms of Dynamics Crm SDK?
            // Perhaps this could be used for exectuign fetch xml commands...?
            throw new System.NotImplementedException();
        }



    }

}