using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Support
{
    public static class Utils
    {
        public static string GetSqlFilterString(string filterOperator, object filterValue, string filterFormatString, string filterColumnName)
        {
            string sqlFilterString;
            if (filterValue == null || !filterValue.GetType().IsArray)
            {
                sqlFilterString = string.Format(filterFormatString, filterColumnName, filterOperator, filterValue);
                return sqlFilterString;
            }
            var formatArgs = new List<object>();
            formatArgs.Add(filterColumnName);
            formatArgs.Add(filterOperator);
            var args = filterValue as IEnumerable;
            foreach (var arg in args)
            {
                formatArgs.Add(arg);
            }
            sqlFilterString = string.Format(filterFormatString, formatArgs.ToArray());
            return sqlFilterString;
        }

    }
}
