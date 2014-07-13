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
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="UpdateRequest"/> when it visits a <see cref="UpdateBuilder"/> 
    /// </summary>
    public class UpdateRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public UpdateRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
            : this(null, metadataProvider)
        {

        }

        public UpdateRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider)
        {
            Request = new UpdateRequest();
            Parameters = parameters;
            MetadataProvider = metadataProvider;
            IsVisitingRightFilterItem = false;
        }

        public UpdateRequest Request { get; set; }
        public DbParameterCollection Parameters { get; set; }
        private ICrmMetaDataProvider MetadataProvider { get; set; }
        private EntityBuilder EntityBuilder { get; set; }

        private string EntityName { get; set; }
        private EqualToFilter EqualToFilter { get; set; }
        private bool IsVisitingRightFilterItem { get; set; }
        private bool IsVisitingFilterItem { get; set; }

        private Column IdFilterColumn { get; set; }
        private object IdFilterValue { get; set; }
        private Column CurrentSetterColumn { get; set; }

        #region Visit Methods

        protected override void VisitUpdate(UpdateBuilder item)
        {
            GuardUpdateBuilder(item);
            item.Table.Source.Accept(this);

            int whereCount = 0;
            foreach (IVisitableBuilder where in item.Where)
            {
                where.Accept(this);
                whereCount++;
            }
            if (whereCount != 1)
            {
                throw new ArgumentException("The update statement should have a single filter in the where clause, which should specify the entity id of the record to be updated.");
            }
            if (EqualToFilter == null)
            {
                throw new NotSupportedException("The update statement has an unsupported filter in it's where clause. The where clause should contain a single 'equal to' filter that specifies the entity id of the particular record to update.");
            }
            if (IdFilterColumn == null)
            {
                throw new NotSupportedException("The update statement has an unsupported filter in it's where clause. The'equal to' filter should specify the entity id column on one side.");
            }
            var idAttName = IdFilterColumn.GetColumnLogicalAttributeName();
            var expectedIdAttributeName = string.Format("{0}id", EntityName.ToLower());
            if (idAttName != expectedIdAttributeName)
            {
                throw new NotSupportedException("The update statement has an unsupported filter in it's where clause. The'equal to' filter should specify the id column of the entity on one side.");
            }
            EntityBuilder.WithAttribute(idAttName).SetValueWithTypeCoersion(IdFilterValue);
            foreach (IVisitableBuilder setter in item.Setters)
            {
                setter.Accept(this);
            }
            Request.Target = EntityBuilder.Build();
            EntityBuilder = null;
        }

        protected override void VisitEqualToFilter(EqualToFilter item)
        {

            EqualToFilter = item;
            IsVisitingFilterItem = true;
            item.LeftHand.Accept(this);
            IsVisitingRightFilterItem = true;
            item.RightHand.Accept(this);
            IsVisitingRightFilterItem = false;
            IsVisitingFilterItem = false;
        }

        protected override void VisitTable(Table item)
        {
            EntityName = item.GetTableLogicalEntityName();
            EntityBuilder = EntityBuilder.WithNewEntity(MetadataProvider, EntityName);
        }

        protected override void VisitColumn(Column item)
        {
            if (IsVisitingFilterItem)
            {
                IdFilterColumn = item;
            }
        }

        protected override void VisitStringLiteral(StringLiteral item)
        {
            var sqlValue = item.ParseStringLiteralValue();
            if (IsVisitingFilterItem)
            {
                IdFilterValue = sqlValue;
            }
            else
            {
                var attName = this.CurrentSetterColumn.GetColumnLogicalAttributeName();
                EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(sqlValue);
            }
        }

        protected override void VisitNumericLiteral(NumericLiteral item)
        {
            var sqlValue = item.ParseNumericLiteralValue();
            if (IsVisitingFilterItem)
            {
                IdFilterValue = sqlValue;
            }
            else
            {
                var attName = this.CurrentSetterColumn.GetColumnLogicalAttributeName();
                EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(sqlValue);
            }
        }

        protected override void VisitNullLiteral(NullLiteral item)
        {
            if (IsVisitingFilterItem)
            {
                IdFilterValue = null;
            }
            else
            {
                var attName = this.CurrentSetterColumn.GetColumnLogicalAttributeName();
                EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(null);
            }
        }

        protected override void VisitPlaceholder(Placeholder item)
        {
            var paramVal = GetParamaterValue(item.Value);
            if (IsVisitingFilterItem)
            {
                IdFilterValue = paramVal;
            }
            else
            {
                var attName = this.CurrentSetterColumn.GetColumnLogicalAttributeName();
                EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(paramVal);
            }

        }

        protected override void VisitSetter(Setter item)
        {
            CurrentSetterColumn = item.Column;
            item.Value.Accept(this);
            CurrentSetterColumn = null;
        }

        #endregion

        private void GuardUpdateBuilder(UpdateBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            if (builder.Table == null)
            {
                throw new ArgumentException("The update statement must specify a single table name to update (this is the logical name of the entity).");
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
