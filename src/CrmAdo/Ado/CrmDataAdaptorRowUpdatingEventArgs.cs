using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{
    public sealed class CrmDataAdaptorRowUpdatingEventArgs : RowUpdatingEventArgs
    {
        public CrmDataAdaptorRowUpdatingEventArgs(DataRow dataRow, IDbCommand dbCommand, StatementType statementType, DataTableMapping mapping)
            : base(dataRow, dbCommand, statementType, mapping)
        {
        }
    }
}
