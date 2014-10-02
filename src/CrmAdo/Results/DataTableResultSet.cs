using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Results
{
    public class DataTableResultSet : ResultSet
    {

        private DataTable _Results = null;
        private int _ResultCount = 0;

        public DataTableResultSet(CrmDbCommand command, OrganizationRequest request)
            : base(command, request, null)
        {

        }

        public DataTable Results { get { return _Results; } set { _Results = value; _ResultCount = _Results.Rows.Count; } }

        public override bool HasResults()
        {
            return Results != null && Results.Rows.Count > 0;
        }

        public override int ResultCount()
        {
            return _ResultCount;
        }

        public override DbDataReader GetReader(DbConnection connection = null)
        {
            // datatables are disconnected so if active connection then dispose.
            if (connection != null)
            {
                connection.Dispose();
            }
            return new DataTableReader(_Results);
        }

        public override object GetScalar()
        {
            throw new NotImplementedException();
        }
    }
}
