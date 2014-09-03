using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace CrmAdo
{
    public class EntityResultSet
    {
        private CrmDbCommand _Command;
        private OrganizationRequest _OrgRequest;

        public EntityResultSet(CrmDbCommand command, OrganizationRequest request)
        {
            _Command = command;
            _OrgRequest = request;
        }

        //TODO: Consider return datatable and using datatable reader?
        public EntityCollection Results { get; set; }
        public List<ColumnMetadata> ColumnMetadata { get; set; }

        public bool HasResults()
        {
            return Results != null && Results.Entities != null && Results.Entities.Any();
        }

        public int ResultCount()
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
                var retrieveMultiple = _OrgRequest as RetrieveMultipleRequest;
                if (retrieveMultiple != null)
                {
                    var query = retrieveMultiple.Query as QueryExpression;
                    if (query != null)
                    {
                        query.PageInfo.PagingCookie = Results.PagingCookie;
                        query.PageInfo.PageNumber++;
                        var response = (RetrieveMultipleResponse)_Command.CrmDbConnection.OrganizationService.Execute(_OrgRequest);
                        Results = response.EntityCollection;
                    }
                }
            }
        }
    }
}