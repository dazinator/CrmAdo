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

namespace CrmAdo.Visitor
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="CreateRequest"/> when it visits an <see cref="InsertBuilder"/> 
    /// </summary>
    public class CreateRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public CreateRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
            : this(null, metadataProvider)
        {

        }

        public CreateRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider)
        {
            Request = new CreateRequest();
            Parameters = parameters;
            MetadataProvider = metadataProvider;
        }

        public CreateRequest Request { get; set; }
        public DbParameterCollection Parameters { get; set; }
        private ICrmMetaDataProvider MetadataProvider { get; set; }
        private EntityBuilder EntityBuilder { get; set; }
        private Column[] Columns { get; set; }
        private Column CurrentColumn { get; set; }

        #region Visit Methods

        protected override void VisitInsert(InsertBuilder item)
        {
            GuardInsertBuilder(item);
            item.Table.Source.Accept(this);
            Columns = item.Columns.ToArray();
            item.Values.Accept(this);
            Columns = null;
            Request.Target = EntityBuilder.Build();
            EntityBuilder = null;
        }

        protected override void VisitTable(Table item)
        {
            var entityName = item.GetTableLogicalEntityName();
            EntityBuilder = EntityBuilder.WithNewEntity(MetadataProvider, entityName);
        }

        protected override void VisitValueList(ValueList item)
        {
            var index = 0;
            foreach (IVisitableBuilder value in item.Values)
            {
                CurrentColumn = this.Columns[index];
                value.Accept(this);
                index++;
            }
            CurrentColumn = null;
        }

        protected override void VisitStringLiteral(StringLiteral item)
        {
            var sqlValue = item.ParseStringLiteralValue();
            var attName = this.CurrentColumn.GetColumnLogicalAttributeName();
            this.EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(sqlValue);
        }

        protected override void VisitNumericLiteral(NumericLiteral item)
        {
            var sqlValue = item.ParseNumericLiteralValue();
            var attName = this.CurrentColumn.GetColumnLogicalAttributeName();
            this.EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(sqlValue);
        }

        protected override void VisitNullLiteral(NullLiteral item)
        {
            var attName = this.CurrentColumn.GetColumnLogicalAttributeName();
            this.EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(null);
        }

        protected override void VisitPlaceholder(Placeholder item)
        {
            var paramVal = GetParamaterValue(item.Value);
            var attName = this.CurrentColumn.GetColumnLogicalAttributeName();
            this.EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(paramVal);
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
        }

        private object GetParamaterValue(string paramName)
        {
            if (!Parameters.Contains(paramName))
            {
                throw new InvalidOperationException("Missing parameter value for parameter named: " + paramName);
            }
            var param = Parameters[paramName];
            return param.Value;
        }

    }
}
