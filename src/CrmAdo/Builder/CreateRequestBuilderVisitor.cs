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
using CrmAdo.Enums;

namespace CrmAdo.Visitor
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="CreateRequest"/> when it visits an <see cref="InsertBuilder"/> 
    /// </summary>
    public class CreateRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor<CreateRequest>
    {

        public CreateRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
            : this(null, metadataProvider)
        {

        }

        public CreateRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider)
            : base(metadataProvider)
        {
            //this.CreateRequest = new CreateRequest();
          //  Request = this.CreateRequest;
            Parameters = parameters;
            //  MetadataProvider = metadataProvider;
        }

       
       // public CreateRequest CreateRequest { get; set; }
      

        public DbParameterCollection Parameters { get; set; }
        //  private ICrmMetaDataProvider MetadataProvider { get; set; }
        private EntityBuilder EntityBuilder { get; set; }
        private Column[] Columns { get; set; }
        private Column CurrentColumn { get; set; }
       
        private bool IsVisitingSingleOutput { get; set; }
        private bool IsOutputSingleId { get; set; }       

        #region Visit Methods

        protected override void VisitInsert(InsertBuilder item)
        {
            GuardInsertBuilder(item);
            item.Table.Source.Accept(this);
            Columns = item.Columns.ToArray();
            item.Values.Accept(this);
            Columns = null;
            CurrentRequest.Target = EntityBuilder.Build();
            EntityBuilder = null;
            OutputColumns = item.Output.ToArray();
            UpgradeToExecuteMultipleIfNecessary();
        }

        /// <summary>
        /// Checks  the output columns. if there are output values that that aren't available from the CreateResponse, then
        /// upgrades the Request to an ExecuteMultiple request, that contains the CreateRequest, and a RetrieveRequest to 
        /// to get the additional output values.
        /// </summary>
        private void UpgradeToExecuteMultipleIfNecessary()
        {
            // If there are output columns for anything that isn't part of the Create Response, then
            // we have to upgrade to an executemultiplerequest, with an additional Retrieve to get the extra values.
            if (OutputColumns.Any())
            {
                // If only a single output column, and if it's the id, then executemultiple not necessary as
                // this is already returned as the result from the createrequest.
                var targetEntity = CurrentRequest.Target;
                if (OutputColumns.Count() == 1)
                {
                    var col = OutputColumns.First();
                    IsVisitingSingleOutput = true;
                    col.ProjectionItem.Accept(this);
                    if (IsOutputSingleId)
                    {
                        return;
                    }
                }

                IsVisitingSingleOutput = false;

                // To get any other output, the id must be specified as part of the insert, so that we have the info
                // we need to do the additional retrieve request.
                if (targetEntity.Id == Guid.Empty)
                {
                    throw new NotSupportedException("An OUTPUT clause can only be used in an Insert statement, if either the INSERT specifies the ID for the new entity, or if the only Output column is the inserted entities ID.");
                }

                UpgradeRequestToExecuteMultipleWithRetrieve(targetEntity.LogicalName, targetEntity.Id);
               
            }
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

        protected override void VisitColumn(Column item)
        {
            var attName = item.GetColumnLogicalAttributeName();

            var entityName = this.CurrentRequest.Target.LogicalName;
            this.AddColumnMetadata(entityName, null, attName);

            if (IsVisitingSingleOutput)
            {
                // var attName = item.GetColumnLogicalAttributeName();
                if (this.IsPrimaryIdColumn(CurrentRequest.Target.LogicalName, attName))
                {
                    IsOutputSingleId = true;
                }
                else
                {
                    IsOutputSingleId = false;
                }
                return;
            }

            RetrieveOutputRequest.ColumnSet.AddColumn(attName);
        }

        protected override void VisitAllColumns(AllColumns item)
        {
            if (!IsVisitingSingleOutput)
            {
                var entityName = this.CurrentRequest.Target.LogicalName;
                base.AddAllColumnMetadata(entityName, null);
                RetrieveOutputRequest.ColumnSet.AllColumns = true;
            }

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
