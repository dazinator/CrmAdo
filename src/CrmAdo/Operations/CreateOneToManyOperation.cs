using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public class CreateOneToManyOperation : CrmOperation
    {
        public CreateOneToManyOperation(List<ColumnMetadata> columnMetadata, OrganizationRequest request)
        {
            this.Columns = columnMetadata;
            this.Request = request;
        }

        protected override OrganisationRequestCommandResult ExecuteCommand()
        {                              

            OrganisationRequestCommandResult commandResponse = null;
            var resultSet = CreateEntityResultSet();
            var response = ExecuteOrganisationRequest();
            commandResponse = new OrganisationRequestCommandResult(response, resultSet, false);

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
           
            return commandResponse;
        }
  
    }
}
