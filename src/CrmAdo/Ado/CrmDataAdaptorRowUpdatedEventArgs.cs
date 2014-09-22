using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Ado
{
    public sealed class CrmDataAdaptorRowUpdatedEventArgs : RowUpdatedEventArgs
    {
        public CrmDataAdaptorRowUpdatedEventArgs(DataRow dataRow, IDbCommand dbCommand, StatementType statementType, DataTableMapping mapping)
            : base(dataRow, dbCommand, statementType, mapping)
        {
        }
    }
}
