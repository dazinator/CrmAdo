using CrmAdo.Core;
using CrmAdo.Dynamics;
using CrmAdo.Operations;
using CrmAdo.Visitor;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Support
{
    [Category("Visitor")]
    public class BaseOrganisationRequestBuilderVisitorTest : BaseTest<SqlGenerationCrmOperationProvider>
    {
      //  private CrmDbConnection _MockConnection = null;

        protected QueryExpression GetQueryExpression(CrmDbConnection connection, string sql)
        {
            var cmd = new CrmDbCommand(connection);
            cmd.CommandText = sql;
            return GetQueryExpression(cmd);
        }

        protected QueryExpression GetQueryExpression(CrmDbCommand command)
        {
            var req = GetOrganizationRequest<RetrieveMultipleRequest>(command);
            return req.Query as QueryExpression;
        }

        protected T GetOrganizationRequest<T>(CrmDbConnection connection, string sql) where T : OrganizationRequest
        {
            var cmd = new CrmDbCommand(connection);
            cmd.CommandText = sql;
            return GetOrganizationRequest<T>(cmd);

        }

        protected T GetOrganizationRequest<T>(CrmDbCommand command, CommandBehavior behaviour = CommandBehavior.Default) where T : OrganizationRequest
        {
           // List<ColumnMetadata> columnMetadata;
            var subject = this.ResolveTestSubjectInstance();
            // IoC.ContainerServices.GetOperationolve<VisitingCrmRequestProvider>();
            var orgCommand = subject.GetOperation(command, behaviour);
            return orgCommand.Request as T;
        }

        protected ICrmOperation GetOperation(CrmDbConnection connection, string sql) 
        {
            var cmd = new CrmDbCommand(connection);
            cmd.CommandText = sql;
            return GetOperation(cmd);

        }

        protected ICrmOperation GetOperation(CrmDbCommand command, CommandBehavior behaviour = CommandBehavior.Default)
        {
            // List<ColumnMetadata> columnMetadata;
            var subject = this.ResolveTestSubjectInstance();
            // IoC.ContainerServices.GetOperationolve<VisitingCrmRequestProvider>();
            var orgCommand = subject.GetOperation(command, behaviour);
            return orgCommand;
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
