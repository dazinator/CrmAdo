using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{
    public class EntityMetadataResultSet : ResultSet
    {
        public EntityMetadataResultSet(CrmDbCommand command, OrganizationRequest request, EntityMetadataCollection results)
            : base(command, request)
        {
            Results = results;
        }

        public EntityMetadataCollection Results { get; set; }

        public override bool HasResults()
        {
            return Results != null && Results.Any();
        }

        public override int ResultCount()
        {
            if (HasResults())
            {
                return Results.Count;
            }
            return -1;
        }

        public override DbDataReader GetReader(DbConnection connection = null)
        {
            throw new NotImplementedException();
        }

        public override object GetScalar()
        {
            throw new NotImplementedException();
        }
    }
}
