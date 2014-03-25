using System.Collections;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace CrmAdo.Tests
{
    public abstract class CrmQueryExpressionProviderTestsBase : BaseTest<SqlGenerationRequestProvider>
    {
        protected QueryExpression GetQueryExpression(string sql)
        {
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            return GetQueryExpression(cmd);
        }

        protected QueryExpression GetQueryExpression(CrmDbCommand command)
        {
            var req = GetOrganizationRequest<RetrieveMultipleRequest>(command);
            return req.Query as QueryExpression;
        }

        protected T GetOrganizationRequest<T>(string sql) where T: OrganizationRequest
        {
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            return GetOrganizationRequest<T>(cmd);
        }

        protected T GetOrganizationRequest<T>(CrmDbCommand command) where T : OrganizationRequest
        {
            var subject = CreateTestSubject();
            var request = subject.GetOrganizationRequest(command) as RetrieveMultipleRequest;
            return request as T;
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