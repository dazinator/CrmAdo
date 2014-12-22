using CrmAdo.Dynamics;
using CrmAdo.Visitor;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Support
{
    [Category("Visitor")]
    public class BaseOrganisationRequestBuilderVisitorTest : BaseTest<VisitingCrmRequestProvider>
    {
        protected QueryExpression GetQueryExpression(string sql)
        {            
            var cmd = new CrmDbCommand();
            cmd.CommandText = sql;
            return GetQueryExpression(cmd);
        }

        protected QueryExpression GetQueryExpression(CrmDbCommand command)
        {
            var req = GetOrganizationRequest<RetrieveMultipleRequest>(command);
            return req.Query as QueryExpression;
        }

        protected T GetOrganizationRequest<T>(string sql) where T : OrganizationRequest
        {
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            return GetOrganizationRequest<T>(cmd);
        }

        protected T GetOrganizationRequest<T>(CrmDbCommand command) where T : OrganizationRequest
        {          
            var subject = ResolveTestSubjectInstance();
            List<ColumnMetadata> columnMetadata;
            var request = subject.GetOrganizationRequest(command, out columnMetadata) as T;
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
