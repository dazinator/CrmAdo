using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.Operations;

namespace CrmAdo
{
    public abstract class ResultSet
    {
        private CrmDbConnection _Connection;
        private OrganizationRequest _OrgRequest;

        public CrmDbConnection Connection { get { return _Connection; } set { _Connection = value; } }
        public OrganizationRequest Request { get { return _OrgRequest; } set { _OrgRequest = value; } }

        protected ResultSet(CrmDbConnection connection, OrganizationRequest request, List<ColumnMetadata> columnMetadata)
        {
            _Connection = connection;
            _OrgRequest = request;
            ColumnMetadata = columnMetadata;
        }

        public abstract int ResultCount();

        public abstract bool HasResults();

        public abstract object GetScalar();

        public List<ColumnMetadata> ColumnMetadata { get; set; }

        public virtual bool HasColumnMetadata()
        {
            return ColumnMetadata != null && ColumnMetadata.Any();
        }

        public abstract object GetValue(int columnOrdinal, int position);

        public virtual void LoadNextPage()
        {
            throw new NotSupportedException();
        }

        public virtual bool HasMoreRecords()
        {
            return false;
        }



    }
}
