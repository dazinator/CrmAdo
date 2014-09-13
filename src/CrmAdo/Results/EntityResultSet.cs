using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Data.Common;

namespace CrmAdo
{
    public class EntityResultSet : ResultSet
    {

        public EntityResultSet(CrmDbCommand command, OrganizationRequest request, List<ColumnMetadata> columnMetadata)
            : base(command, request, columnMetadata)
        {
        }

        //TODO: Consider return datatable and using datatable reader?
        public EntityCollection Results { get; set; }     

        public override bool HasResults()
        {
            return Results != null && Results.Entities != null && Results.Entities.Any();
        }   

        public override int ResultCount()
        {
            if (HasResults())
            {
                return Results.Entities.Count;
            }
            return -1;
        }

        public void LoadNextPage()
        {
            if (Results != null && Results.MoreRecords)
            {
                var retrieveMultiple = Request as RetrieveMultipleRequest;
                if (retrieveMultiple != null)
                {
                    var query = retrieveMultiple.Query as QueryExpression;
                    if (query != null)
                    {
                        query.PageInfo.PagingCookie = Results.PagingCookie;
                        query.PageInfo.PageNumber++;
                        var response = (RetrieveMultipleResponse)Command.CrmDbConnection.OrganizationService.Execute(Request);
                        Results = response.EntityCollection;
                    }
                }
            }
        }

        public override DbDataReader GetReader(DbConnection connection = null)
        {
            return new CrmDbDataReader(this, connection);
        }

        public override object GetScalar()
        {
            if (HasResults())
            {
                var first = Results.Entities[0];
                if (first.Attributes.Any())
                {
                    var value = first.Attributes.FirstOrDefault().Value;
                    return CrmDbTypeConverter.ToDbType(value);
                }
            }
            return null;
        }
    }
}