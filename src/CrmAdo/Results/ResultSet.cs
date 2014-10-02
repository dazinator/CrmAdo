using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{
    public abstract class ResultSet
    {
        private CrmDbCommand _Command;
        private OrganizationRequest _OrgRequest;

        public CrmDbCommand Command { get { return _Command; } set { _Command = value; } }
        public OrganizationRequest Request { get { return _OrgRequest; } set { _OrgRequest = value; } }

        protected ResultSet(CrmDbCommand command, OrganizationRequest request, List<ColumnMetadata> columnMetadata)
        {
            _Command = command;
            _OrgRequest = request;
            ColumnMetadata = columnMetadata;
        }

        public abstract int ResultCount();

        public abstract bool HasResults();

        public abstract DbDataReader GetReader(DbConnection connection = null);

        public abstract object GetScalar();

        public List<ColumnMetadata> ColumnMetadata { get; set; }

        public virtual bool HasColumnMetadata()
        {
            return ColumnMetadata != null && ColumnMetadata.Any();
        }

    }
}
