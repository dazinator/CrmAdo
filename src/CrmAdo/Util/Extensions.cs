using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Util
{
    public static class DataTableExtensions   //convert IEnumrable<T> to datatable
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }

    public static class SqlUtils
    {
        /// <summary>
        /// Returns the unquoted identifier from a quoted identifier (removes the quotes) 
        /// </summary>
        /// <param name="quotedIdentifier"></param>
        public static string GetUnquotedIdentifier(string quotedIdentifier)
        {
            if (quotedIdentifier[0] == '[')
            {
                quotedIdentifier = quotedIdentifier.Remove(0, 1);
            }
            var length = quotedIdentifier.Length;
            if (quotedIdentifier[length - 1] == ']')
            {
                quotedIdentifier = quotedIdentifier.Remove(length - 1);
            }
            return quotedIdentifier;
        }

    }
}
