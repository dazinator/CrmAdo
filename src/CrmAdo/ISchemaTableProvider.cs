using System.Collections.Generic;
using System.Data;

namespace CrmAdo
{
    /// <summary>
    /// The interface for a provider that generates the schema table, based on Column Metadata.
    /// </summary>
    public interface ISchemaTableProvider
    {
        DataTable GetSchemaTable(IEnumerable<ColumnMetadata> columns);
    }
}
