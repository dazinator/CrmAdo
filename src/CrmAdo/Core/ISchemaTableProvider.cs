using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace CrmAdo.Core
{
    /// <summary>
    /// The interface for a provider that generates the schema table, based on Column Metadata.
    /// </summary>
    public interface ISchemaTableProvider
    {
        DataTable GetSchemaTable(DbConnection connection, IEnumerable<ColumnMetadata> columns);
    }
}
