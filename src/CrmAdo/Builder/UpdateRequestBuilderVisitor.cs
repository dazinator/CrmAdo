using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using CrmAdo.Core;
using CrmAdo.Operations;

namespace CrmAdo.Visitor
{
    
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="UpdateRequest"/> when it visits a <see cref="UpdateBuilder"/> 
    /// </summary>
    public class UpdateRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor<UpdateRequest>
    {

        private enum UpdateStatementPart
        {
            WhereClause = 0,
            Setter = 1,
            OutputClause = 2
        }

        private enum WhereFilterPart
        {
            LeftHand = 0,
            RightHand = 1,
        }

        public UpdateRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider, ConnectionSettings settings)
            : this(null, metadataProvider, settings)
        {

        }

        public UpdateRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider, ConnectionSettings settings)
           : base(metadataProvider, settings)
        {
            //  Request = new UpdateRequest();
            Parameters = parameters;
            //  IsVisitingRightFilterItem = false;
        }
            

        public DbParameterCollection Parameters { get; set; }
        private EntityBuilder EntityBuilder { get; set; }

        private string EntityName { get; set; }
        private EqualToFilter EqualToFilter { get; set; }

        private UpdateStatementPart CurrentUpdateStatementPart { get; set; }
        private WhereFilterPart CurrentWhereFilterPart { get; set; }      

        private Column IdFilterColumn { get; set; }
        private object IdFilterValue { get; set; }
        private Column CurrentSetterColumn { get; set; }       

        #region Visit Methods

        protected override void VisitUpdate(UpdateBuilder item)
        {
            GuardUpdateBuilder(item);
            item.Table.Source.Accept(this);

            int whereCount = 0;
            CurrentUpdateStatementPart = UpdateStatementPart.WhereClause;
            foreach (IVisitableBuilder where in item.Where)
            {
                where.Accept(this);
                whereCount++;
            }

            // IsVisitingWhereFilter = false;
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
            var idAttName = GetColumnLogicalAttributeName(IdFilterColumn);
            var expectedIdAttributeName = string.Format("{0}id", EntityName.ToLower());
            if (idAttName != expectedIdAttributeName)
            {
                throw new NotSupportedException("The update statement has an unsupported filter in it's where clause. The'equal to' filter should specify the id column of the entity on one side.");
            }
            EntityBuilder.WithAttribute(idAttName).SetValueWithTypeCoersion(IdFilterValue);

            CurrentUpdateStatementPart = UpdateStatementPart.Setter;
            foreach (IVisitableBuilder setter in item.Setters)
            {
                setter.Accept(this);
            }
            CurrentRequest.Target = EntityBuilder.Build();
            EntityBuilder = null;

            CurrentUpdateStatementPart = UpdateStatementPart.OutputClause;
            OutputColumns = item.Output.ToArray();
            UpgradeToExecuteMultipleIfNecessary();
        }

        private void UpgradeToExecuteMultipleIfNecessary()
        {
            // If there are output columns for anything that isn't part of the Create Response, then
            // we have to upgrade to an executemultiplerequest, with an additional Retrieve to get the extra values.
            if (OutputColumns.Any())
            {
                UpgradeRequestToExecuteMultipleWithRetrieve(CurrentRequest.Target.LogicalName, CurrentRequest.Target.Id);
            }
        }

        protected override void VisitEqualToFilter(EqualToFilter item)
        {
            EqualToFilter = item;
            CurrentWhereFilterPart = WhereFilterPart.LeftHand;
            item.LeftHand.Accept(this);
            CurrentWhereFilterPart = WhereFilterPart.RightHand;
            item.RightHand.Accept(this);
        }

        protected override void VisitTable(Table item)
        {
            EntityName = GetTableLogicalEntityName(item);
            EntityBuilder = EntityBuilder.WithNewEntity(MetadataProvider, EntityName);
        }

        protected override void VisitColumn(Column item)
        {
            if (CurrentUpdateStatementPart == UpdateStatementPart.WhereClause)
            {
                IdFilterColumn = item;
            }
            else
            {
                var attName = GetColumnLogicalAttributeName(item);
                var entityName = this.CurrentRequest.Target.LogicalName;
                this.AddColumnMetadata(entityName, null, attName);
                RetrieveOutputRequest.ColumnSet.AddColumn(attName);
            }
        }

        protected override void VisitAllColumns(AllColumns item)
        {
            var entityName = this.CurrentRequest.Target.LogicalName;
            base.AddAllColumnMetadata(entityName, null);
            RetrieveOutputRequest.ColumnSet.AllColumns = true;
        }

        protected override void VisitStringLiteral(StringLiteral item)
        {
            var sqlValue = ParseStringLiteralValue(item);
            if (CurrentUpdateStatementPart == UpdateStatementPart.WhereClause)
            {
                IdFilterValue = sqlValue;
            }
            else
            {
                var attName = GetColumnLogicalAttributeName(this.CurrentSetterColumn);
                EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(sqlValue);
            }
        }

        protected override void VisitNumericLiteral(NumericLiteral item)
        {
            var sqlValue = ParseNumericLiteralValue(item);
            if (CurrentUpdateStatementPart == UpdateStatementPart.WhereClause)
            {
                IdFilterValue = sqlValue;
            }
            else
            {
                var attName = GetColumnLogicalAttributeName(this.CurrentSetterColumn);
                EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(sqlValue);
            }
        }

        protected override void VisitNullLiteral(NullLiteral item)
        {
            if (CurrentUpdateStatementPart == UpdateStatementPart.WhereClause)
            {
                IdFilterValue = null;
            }
            else
            {
                var attName = GetColumnLogicalAttributeName(this.CurrentSetterColumn);
                EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(null);
            }
        }

        protected override void VisitPlaceholder(Placeholder item)
        {
            var paramVal = GetParamaterValue(item.Value);
            if (CurrentUpdateStatementPart == UpdateStatementPart.WhereClause)
            {
                IdFilterValue = paramVal;
            }
            else
            {
                var attName = GetColumnLogicalAttributeName(this.CurrentSetterColumn);
                EntityBuilder.WithAttribute(attName).SetValueWithTypeCoersion(paramVal);
            }

        }

        protected override void VisitSetter(Setter item)
        {
            CurrentSetterColumn = item.Column;
            item.Value.Accept(this);
            CurrentSetterColumn = null;
        }

        protected override void VisitFilterGroup(FilterGroup item)
        {
            foreach (var filter in item.Filters)
            {
                filter.Accept(this);
            }
            // base.VisitFilterGroup(item);
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

        public override ICrmOperation GetCommand()
        {
            var orgCommand = new UpdateEntityOperation(ResultColumnMetadata, Request, IsExecuteMultiple);
            return orgCommand;
        }

    }
}
