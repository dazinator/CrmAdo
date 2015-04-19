using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Data.Common;
using CrmAdo.Core;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmAdo
{
    public class EntityResultSet : ResultSet
    {

        public EntityResultSet(CrmDbConnection connection, OrganizationRequest request, List<ColumnMetadata> columnMetadata)
            : base(connection, request, columnMetadata)
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

        public override void LoadNextPage()
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
                        var response = (RetrieveMultipleResponse)Connection.OrganizationService.Execute(Request);
                        Results = response.EntityCollection;
                    }
                }
            }
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

        public override object GetValue(int columnOrdinal, int position)
        {
            var meta = ColumnMetadata[columnOrdinal];
            var columnName = meta.ColumnName;

            var record = Results[position];
            if (!record.Attributes.ContainsKey(columnName))
            {
                return DBNull.Value;
            }
            var val = record[columnName];

            if (meta.HasAlias)
            {
                var aliasedVal = val as AliasedValue;
                if (aliasedVal != null)
                {
                    //if (!typeof(T).IsAssignableFrom(typeof(AliasedValue)))
                    //{
                    val = aliasedVal.Value;
                    // }
                }
            }

            switch (meta.AttributeMetadata.AttributeType)
            {
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Customer:
                    return ((EntityReference)val).Id;
                case AttributeTypeCode.Money:
                    return ((Money)val).Value;
                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                    return ((OptionSetValue)val).Value;
                default:
                    return val;
            }
        }

        public override bool HasMoreRecords()
        {
            return Results.MoreRecords;
        }
    }
}