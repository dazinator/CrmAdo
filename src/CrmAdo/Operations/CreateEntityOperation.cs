using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    /// <summary>
    /// An operation that creates a new entity in CRM.
    /// </summary>
    public class CreateEntityOperation : CrmOperation
    {
        public CreateEntityOperation(List<ColumnMetadata> columnMetadata, OrganizationRequest request)
        {
            this.Columns = columnMetadata;
            this.Request = request;
        }

        protected override ICrmOperationResult ExecuteCommand()
        {                              

            CrmOperationResult commandResponse = null;
            var resultSet = CreateEntityResultSet();
            var response = ExecuteOrganisationRequest();
            commandResponse = new CrmOperationResult(response, resultSet, false);

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
           
            return commandResponse;
        }
  
    }
}
