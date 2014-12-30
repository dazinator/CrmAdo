using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CrmAdo.Core;
using CrmAdo.Dynamics;
using CrmAdo.Metadata;
using CrmAdo.Results;
using CrmAdo.Util;
using CrmAdo.Visitor;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using SQLGeneration.Generators;
using SQLGeneration.Builders;

using CrmAdo.Enums;

namespace CrmAdo.Core
{

    public class CrmOrgCommandExecutor : IOrgCommandExecutor
    {

        private static readonly CrmOrgCommandExecutor _Instance = new CrmOrgCommandExecutor();

        private CrmOrgCommandExecutor() { }

        public static CrmOrgCommandExecutor Instance
        {
            get
            {
                return _Instance;
            }
        }

        #region ICrmCommandExecutor

        public ResultSet ExecuteCommand(IOrgCommand command, CommandBehavior behaviour)
        {
            GuardOrgCommand(command);

            ResultSet results = null;

            // if ((behavior & CommandBehavior.KeyInfo) > 0)
            switch (command.DbCommand.CommandType)
            {
                case CommandType.Text:
                case CommandType.TableDirect:
                    results = ExecuteOrganisationCommand(command, command.CommandBehavior);
                    break;
                case CommandType.StoredProcedure:
                    results = ExecuteStoredProcedureCommand(command);
                    break;
            }

            return results;
        }

        public int ExecuteNonQueryCommand(IOrgCommand command)
        {
            // You can use ExecuteNonQuery to perform catalog operations (for example, querying the structure of a database or creating database objects such as tables), or to change the data in a database by executing UPDATE, INSERT, or DELETE statements.
            // Although ExecuteNonQuery does not return any rows, any output parameters or return values mapped to parameters are populated with data.
            // For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.
            GuardOrgCommand(command);

            switch (command.OperationType)
            {
                case CrmOperation.Create:
                    var createResult = ExecuteCreateCommand(command);
                    return createResult.ResultCount();

                case CrmOperation.Update:

                    var updateResult = ExecuteUpdateCommand(command);
                    return updateResult.ResultCount();

                case CrmOperation.Delete:

                    var deleteResult = ExecuteDeleteCommand(command);
                    return deleteResult.ResultCount();

                case CrmOperation.CreateAttribute:

                    ExecuteCreateAttributeCommand(command);
                    return -1;

                case CrmOperation.CreateEntity:

                    ExecuteCreateEntityCommand(command);
                    return -1;

                case CrmOperation.CreateOneToMany:

                    ExecuteCreateOnetoManyCommand(command);
                    return -1;
            }

            throw new NotSupportedException();

            // return -1;
        }

        #endregion

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
                    resultSet = ExecuteRetrieveMetadataChangesCommand(orgCommand, schemaOnly);
                    break;
                case CrmOperation.Create:
                    resultSet = ExecuteCreateCommand(orgCommand);
                    break;
                case CrmOperation.CreateWithRetrieve:
                    resultSet = ExecuteCreateWithRetrieveCommand(orgCommand);
                    break;
                case CrmOperation.Update:
                    resultSet = ExecuteUpdateCommand(orgCommand);
                    break;
                case CrmOperation.UpdateWithRetrieve:
                    resultSet = ExecuteUpdateWithRetrieveCommand(orgCommand);
                    break;
                case CrmOperation.Delete:
                    resultSet = ExecuteDeleteCommand(orgCommand);
                    break;
                case CrmOperation.CreateAttribute:
                    resultSet = ExecuteCreateAttributeCommand(orgCommand);
                    break;
                case CrmOperation.CreateEntity:
                    resultSet = ExecuteCreateEntityCommand(orgCommand);
                    break;
                case CrmOperation.CreateOneToMany:
                    resultSet = ExecuteCreateOnetoManyCommand(orgCommand);
                    break;
            }

            if (resultSet == null)
            {
                throw new NotSupportedException("Sorry, was not able to translate the command into the appropriate CRM SDK Organization Request message.");
            }

            return resultSet;

        }

        private ResultSet ExecuteUpdateWithRetrieveCommand(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            // Execute the request and obtain the result.
            var response = ExecuteOrganisationRequest(orgCommand);
            var resultSet = CreateEntityResultSet(orgCommand);

            var execMultipleResponse = response as ExecuteMultipleResponse;
            if (execMultipleResponse != null)
            {
                // for execute reader and execute scalar purposes, we provide a result that has the newly created id of the entity.
                var execMultipleRequest = (ExecuteMultipleRequest)orgCommand.Request;
                var request = (UpdateRequest)execMultipleRequest.Requests[0];

                var retrieveResponse = (RetrieveResponse)execMultipleResponse.Responses[1].Response;
                var result = retrieveResponse.Entity;

                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
            return resultSet;
        }

        private ResultSet ExecuteCreateWithRetrieveCommand(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            // Execute the request and obtain the result.
            var response = ExecuteOrganisationRequest(orgCommand);
            var resultSet = CreateEntityResultSet(orgCommand);

            var execMultipleResponse = response as ExecuteMultipleResponse;
            if (execMultipleResponse != null)
            {
                // for execute reader and execute scalar purposes, we provide a result that has the newly created id of the entity.
                var execMultipleRequest = (ExecuteMultipleRequest)orgCommand.Request;
                var request = (CreateRequest)execMultipleRequest.Requests[0];

                var retrieveResponse = (RetrieveResponse)execMultipleResponse.Responses[1].Response;
                var result = retrieveResponse.Entity;

                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
            return resultSet;
        }

        private ResultSet ExecuteRetrieveMultipleCommand(IOrgCommand orgCommand, bool schemaOnly = false)
        {
            var orgService = orgCommand.DbCommand.CrmDbConnection.OrganizationService;
            var entityResultSet = CreateEntityResultSet(orgCommand);
            if (!schemaOnly)
            {
                var response = ExecuteOrganisationRequest(orgCommand);
                var retrieveMultipleResponse = response as RetrieveMultipleResponse;
                if (retrieveMultipleResponse != null)
                {
                    entityResultSet.Results = retrieveMultipleResponse.EntityCollection;
                }
            }
            return entityResultSet;
        }

        private ResultSet ExecuteDeleteCommand(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            var entityResultSet = CreateEntityResultSet(orgCommand);

            //  var deleteRequest = (DeleteRequest)orgCommand.Request;
            var response = ExecuteOrganisationRequest(orgCommand);
            return entityResultSet;

            // var resultSet = new EntityResultSet(command, deleteRequest, null);
            //  var delResponse = response as DeleteResponse;

            //if (delResponse != null)
            //{
            //    var result = delResponse.Results;
            //    resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            //}

        }

        private ResultSet ExecuteUpdateCommand(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            var response = ExecuteOrganisationRequest(orgCommand);
            var entityResultSet = CreateEntityResultSet(orgCommand);

            // var resultSet = new EntityResultSet(command, updateRequest, null);
            var updateResponse = response as UpdateResponse;
            if (updateResponse != null)
            {
                var updateRequest = (UpdateRequest)orgCommand.Request;
                var result = updateRequest.Target;
                entityResultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
            return entityResultSet;
        }

        private ResultSet ExecuteCreateCommand(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            // Execute the request and obtain the result.
            var response = ExecuteOrganisationRequest(orgCommand);
            var resultSet = CreateEntityResultSet(orgCommand);

            var createResponse = response as CreateResponse;
            if (createResponse != null)
            {

               // if(orgCommand.Columns.Any)
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
                //  AddResultColumnMetadata(orgCommand, resultSet, request.Target.LogicalName, resultattributes);
            }
            return resultSet;
        }

        private ResultSet ExecuteCreateEntityCommand(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            var createEntityRequest = (CreateEntityRequest)orgCommand.Request;

            var response = ExecuteOrganisationRequest(orgCommand);
            var resultSet = CreateEntityResultSet(orgCommand);

            var createResponse = response as CreateEntityResponse;
            if (createResponse != null)
            {
                // AssignResponseParameter(command, response);
                // for execute reader and execute scalar purposes, we provide a result that just ahs the newly created id of the entity.
                var result = new Entity("entitymetadata");
                var idattname = string.Format("entitymetadataid");

                result[idattname] = createResponse.EntityId;
                result.Id = createResponse.EntityId;
                result["primaryattributeid"] = createResponse.AttributeId;

                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));

            }
            return resultSet;
        }

        private ResultSet ExecuteCreateOnetoManyCommand(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            var createOneToManyRequest = (CreateOneToManyRequest)orgCommand.Request;

            var response = ExecuteOrganisationRequest(orgCommand);
            var resultSet = CreateEntityResultSet(orgCommand);

            var createResponse = response as CreateOneToManyResponse;
            if (createResponse != null)
            {
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

        private ResultSet ExecuteCreateAttributeCommand(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;
            // var createAttributeRequest = (CreateAttributeRequest)orgCommand.Request;

            var response = ExecuteOrganisationRequest(orgCommand);
            var resultSet = CreateEntityResultSet(orgCommand);

            var createResponse = response as CreateAttributeResponse;
            if (createResponse != null)
            {
                // for execute reader and execute scalar purposes, we provide a result that just ahs the newly created id of the entity.
                var result = new Entity("attributemetadata");
                var idattname = string.Format("attributemetadataid");
                result[idattname] = createResponse.AttributeId;
                result.Id = createResponse.AttributeId;
                resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { result }));
            }
            return resultSet;
        }

        private ResultSet ExecuteRetrieveMetadataChangesCommand(IOrgCommand orgCommand, bool schemaOnly = false)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            CrmAdo.EntityMetadataResultSet.DenormalisedMetadataResult[] results = null;
            // EntityMetadataDataSet ds = new EntityMetadataDataSet();
            var metadataResultSet = new EntityMetadataResultSet(command, orgCommand.Request, orgCommand.Columns);

            if (!schemaOnly)
            {
                //  var retrieveMetadataChangesRequest = (RetrieveMetadataChangesRequest)orgCommand.Request;

                var response = ExecuteOrganisationRequest(orgCommand);
                //var response = orgService.Execute(retrieveMetadataChangesRequest);
                var retrieveMultipleResponse = response as RetrieveMetadataChangesResponse;
                if (retrieveMultipleResponse != null)
                {
                    // AssignResponseParameter(command, response);
                    // denormalise object graph to results.
                    if (retrieveMultipleResponse.EntityMetadata != null)
                    {
                        var attFactory = new AttributeInfoFactory();

                        results = (from r in retrieveMultipleResponse.EntityMetadata
                                   from a in (r.Attributes ?? Enumerable.Empty<AttributeMetadata>()).DefaultIfEmpty()
                                   from o in (r.OneToManyRelationships ?? Enumerable.Empty<OneToManyRelationshipMetadata>()).Union(r.ManyToOneRelationships ?? Enumerable.Empty<OneToManyRelationshipMetadata>()).DefaultIfEmpty()
                                   from m in (r.ManyToManyRelationships ?? Enumerable.Empty<ManyToManyRelationshipMetadata>()).DefaultIfEmpty()
                                   select new CrmAdo.EntityMetadataResultSet.DenormalisedMetadataResult { EntityMetadata = r, AttributeMetadata = attFactory.Create(a), OneToManyRelationship = o, ManyToManyRelationship = m }).ToArray();

                        metadataResultSet.Results = results;

                    }

                }
            }

            return metadataResultSet;

        }

        private EntityResultSet ExecuteStoredProcedureCommand(IOrgCommand command)
        {
            // What would a stored procedure be in terms of Dynamics Crm SDK?
            // Perhaps this could be used for exectuign fetch xml commands...?
            throw new System.NotImplementedException();
        }


        #region Helper Methods

        protected virtual OrganizationResponse ExecuteOrganisationRequest(IOrgCommand orgCommand)
        {
            var command = orgCommand.DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            // Execute the request and obtain the result.
            var response = orgService.Execute(orgCommand.Request);

            // Allow poeople to get access to the response directly via an output paramater.
            AssignResponseParameter(command, response);

            return response;

        }

        protected void GuardOrgCommand(IOrgCommand orgCommand)
        {
            if (orgCommand == null)
            {
                throw new ArgumentNullException("orgCommand");
            }
            if (orgCommand.DbCommand == null)
            {
                throw new InvalidOperationException("orgCommand CrmDbCommand is null");
            }
        }

        protected void AssignResponseParameter(CrmDbCommand command, OrganizationResponse response)
        {
            if (command != null && command.Parameters.Contains(SystemCommandParameters.OrgResponse) && command.Parameters[SystemCommandParameters.OrgResponse].Direction == ParameterDirection.Output)
            {
                command.Parameters[SystemCommandParameters.OrgResponse].Value = response;
            }
        }

        public EntityResultSet CreateEntityResultSet(IOrgCommand command)
        {
            var resultSet = new EntityResultSet(command.DbCommand, command.Request, command.Columns);
            return resultSet;
        }

        //[Obsolete]
        //private void AddResultColumnMetadata(IOrgCommand orgCommand, EntityResultSet results, string entityName, string[] attributeNames)
        //{
        //    var metadataProvider = orgCommand.DbCommand.CrmDbConnection.MetadataProvider;
        //    if (metadataProvider != null)
        //    {
        //        if (results.HasResults())
        //        {
        //            if (results.ColumnMetadata == null)
        //            {
        //                results.ColumnMetadata = new List<ColumnMetadata>(); ;
        //            }

        //            var entityMeta = metadataProvider.GetEntityMetadata(entityName);
        //            results.ColumnMetadata.AddRange(
        //                (from c in attributeNames
        //                 join s in entityMeta.Attributes
        //                 on c equals s.LogicalName
        //                 select new ColumnMetadata(s)));
        //        }
        //    }
        //}

        #endregion


    }

}