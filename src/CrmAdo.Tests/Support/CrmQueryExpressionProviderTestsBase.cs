using System.Collections;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;

namespace CrmAdo.Tests
{
    public abstract class CrmQueryExpressionProviderTestsBase : BaseTest<CrmQueryExpressionProvider>
    {
        protected QueryExpression CreateQueryExpression(string sql)
        {
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            var subject = CreateTestSubject();
            return subject.CreateQueryExpression(cmd);
        }

        protected static string GetSqlFilterString(string filterOperator, object filterValue, string filterFormatString, string filterColumnName)
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