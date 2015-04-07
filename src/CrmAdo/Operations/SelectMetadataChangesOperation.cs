using CrmAdo.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Operations
{
    public class SelectMetadataChangesOperation : CrmOperation
    {
        public SelectMetadataChangesOperation(List<ColumnMetadata> columnMetadata, OrganizationRequest request)
        {
            this.Columns = columnMetadata;
            this.Request = request;
        }

        protected override OrganisationRequestCommandResult ExecuteCommand()
        {

            var command = DbCommand;
            var orgService = command.CrmDbConnection.OrganizationService;

            CrmAdo.EntityMetadataResultSet.DenormalisedMetadataResult[] results = null;
            var metadataResultSet = new EntityMetadataResultSet(command, Request, Columns);
            OrganisationRequestCommandResult commandResponse = null;

            if (IsSchemaOnly())
            {
                commandResponse = new OrganisationRequestCommandResult(null, metadataResultSet, false);
            }
            else
            {
                var response = ExecuteOrganisationRequest();
                commandResponse = new OrganisationRequestCommandResult(response, metadataResultSet, false);

                var retrieveMultipleResponse = response as RetrieveMetadataChangesResponse;
                if (retrieveMultipleResponse != null)
                {
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

            return commandResponse;

        }
    }
}
