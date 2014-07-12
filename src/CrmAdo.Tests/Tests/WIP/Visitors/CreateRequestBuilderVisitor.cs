using CrmAdo.Tests.WIP;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.Dynamics.Metadata;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;

namespace CrmAdo.Tests.Tests.WIP.Visitors
{
    public class CreateRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public int SourceTableLevel = 0;
        public AliasedSource MainSource = null;
        public Table MainSourceTable = null;

        public CreateRequestBuilderVisitor()
            : this(null)
        {

        }

        public CreateRequestBuilderVisitor(DbParameterCollection parameters)
        {
            Request = new CreateRequest();
            Parameters = parameters;
        }

        public CreateRequest Request { get; set; }
        public DbParameterCollection Parameters { get; set; }

        #region Visit Methods

        protected override void VisitInsert(InsertBuilder item)
        {
            GuardInsertBuilder(item);
        }       

        #endregion

        private void GuardInsertBuilder(InsertBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            if (builder.Table == null)
            {
                throw new ArgumentException("The insert statement must specify a table name to insert to (this is the logical name of the entity).");
            }
            // if columns specified, then number of values should equate.
            if (builder.Columns.Any())
            {
                if (builder.Values == null || !builder.Values.IsValueList || ((ValueList)builder.Values).Values.Count() != builder.Columns.Count())
                {
                    throw new ArgumentException("There is a mismatch between the number of columns and the number of values specified in the insert statement.");
                }
            }
            //if (!builder.Values > 1)
            //{
            //    throw new NotSupportedException("The select statement must select from a single entity.");
            //}
            //if (!builder.Columns.Any())
            //{
            //    throw new InvalidOperationException("The select statement must select atleast 1 attribute.");
            //}
        }

        private Entity BuildEntityFromInsert(InsertBuilder insertCommandBuilder, CrmDbCommand command)
        {
            var source = insertCommandBuilder.Table.Source;
            //if (!source.)
            //{
            //    throw new ArgumentException("Can only insert into a table");
            //}

            var table = source as Table;

            var metadataProvider = command.CrmDbConnection.MetadataProvider;
            var entityName = GetTableLogicalEntityName(table);

            var entityBuilder = EntityBuilder.WithNewEntity(metadataProvider, entityName);

            ValueList valuesList = insertCommandBuilder.Values.IsValueList ? insertCommandBuilder.Values as ValueList : null;
            if (valuesList != null)
            {
                var values = valuesList.Values.ToArray();
                int columnOrdinal = 0;
                foreach (var column in insertCommandBuilder.Columns)
                {
                    var columnValue = values[columnOrdinal];
                    bool isParameter;
                    var sqlValue = GetSqlValue(columnValue, out isParameter);
                    var attName = GetColumnLogicalAttributeName(column);
                    entityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(sqlValue);
                    columnOrdinal++;
                }
            }

            return entityBuilder.Build();
        }

        private object GetSqlValue(IProjectionItem projectionItem, out bool isParameter)
        {
            isParameter = false;
            var literal = projectionItem as Literal;
            if (literal != null)
            {
                return GitLiteralValue(literal);
            }
            var placeholder = projectionItem as Placeholder;
            if (placeholder != null)
            {
                isParameter = true;
                var paramVal = GetParamaterValue<object>(placeholder.Value);
                return paramVal;
            }
            throw new NotSupportedException();
        }

        private T GetParamaterValue<T>(string paramName)
        {
            if (!Parameters.Contains(paramName))
            {
                throw new InvalidOperationException("Missing parameter value for parameter named: " + paramName);
            }
            var param = Parameters[paramName];
            return (T)param.Value;
        }

    }
}
