using CrmAdo.Dynamics.Metadata;
using CrmAdo.Results;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{

    public class EntityMetadataResultSet : ResultSet
    {

        private DenormalisedMetadataResult[] _Results = null;
        private int _ResultCount = 0;

        public EntityMetadataResultSet(CrmDbCommand command, OrganizationRequest request, DenormalisedMetadataResult[] results)
            : base(command, request, null)
        {
            _Results = results;
            if (_Results != null)
            {
                _ResultCount = _Results.Count();
            }
        }


        public DenormalisedMetadataResult[] Results { get { return _Results; } }

        public override bool HasResults()
        {
            return Results != null && Results.Any();
        }

        public override int ResultCount()
        {
            return _ResultCount;
        }

        public override DbDataReader GetReader(DbConnection connection = null)
        {
            return new CrmDbMetadataReader(this, connection);
        }

        public override object GetScalar()
        {
            throw new NotImplementedException();
        }
    }
}
