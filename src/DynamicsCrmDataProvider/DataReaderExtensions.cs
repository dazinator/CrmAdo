using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsCrmDataProvider
{
    public static class DataReaderExtensions
    {

        public static string SafeGetString(this DbDataReader reader, string colName)
        {
            var ordinal = reader.GetOrdinal(colName);
            return SafeGetString(reader, ordinal);
        }

        public static string SafeGetString(this DbDataReader reader, int colIndex)
        {
            if (!reader.IsDBNull(colIndex))
                return reader.GetString(colIndex);
            else
                return string.Empty;
        }

      

     

    }
}
