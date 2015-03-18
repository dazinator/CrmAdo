using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public class DeleteOperation : CrmOperation
    {
        public DeleteOperation(List<ColumnMetadata> columnMetadata, OrganizationRequest request)
        {
            this.Columns = columnMetadata;
            this.Request = request;
        }

        protected override OrganisationRequestCommandResult ExecuteCommand()
        {
            OrganisationRequestCommandResult commandResponse = null;
            var resultSet = CreateEntityResultSet();
            var response = ExecuteOrganisationRequest();
            commandResponse = new OrganisationRequestCommandResult(response, resultSet, true);
            DeleteRequest deleteRequest = Request as DeleteRequest;
            // Expose the deleted entities id / name in the result set.
            var deletedEntity = new Entity(deleteRequest.Target.LogicalName);
            deletedEntity.Id = deletedEntity.Id;
            resultSet.Results = new EntityCollection(new List<Entity>(new Entity[] { deletedEntity }));
            return commandResponse;
        }
  
    }
}
