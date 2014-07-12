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
    public class DeleteRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public DeleteRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
            : this(null, metadataProvider, new DynamicsAttributeTypeProvider())
        {

        }

        public DeleteRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider, IDynamicsAttributeTypeProvider typeProvider)
        {
            Request = new DeleteRequest();
            Parameters = parameters;
            MetadataProvider = metadataProvider;
            IsVisitingRightFilterItem = false;
            DynamicsTypeProvider = typeProvider;
        }

        private IDynamicsAttributeTypeProvider DynamicsTypeProvider { get; set; }
        public DeleteRequest Request { get; set; }

        public DbParameterCollection Parameters { get; set; }
        private ICrmMetaDataProvider MetadataProvider { get; set; }

        private string EntityName { get; set; }
        private EqualToFilter EqualToFilter { get; set; }
        private bool IsVisitingRightFilterItem { get; set; }
        private bool IsVisitingFilterItem { get; set; }

        private Column IdFilterColumn { get; set; }
        private object IdFilterValue { get; set; }

        #region Visit Methods

        protected override void VisitDelete(DeleteBuilder item)
        {
            GuardDeleteBuilder(item);
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
            var idAttName = GetColumnLogicalAttributeName(IdFilterColumn);
            var expectedIdAttributeName = string.Format("{0}id", EntityName.ToLower());
            if (idAttName != expectedIdAttributeName)
            {
                throw new NotSupportedException("The update statement has an unsupported filter in it's where clause. The'equal to' filter should specify the id column of the entity on one side.");
            }

            EntityReference entRef = new EntityReference();
            entRef.LogicalName = EntityName;
            entRef.Id = DynamicsTypeProvider.GetUniqueIdentifier(IdFilterValue);
            Request.Target = entRef;

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
            EntityName = GetTableLogicalEntityName(item);
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
            var sqlValue = ParseStringLiteralValue(item);
            if (IsVisitingFilterItem)
            {
                IdFilterValue = sqlValue;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected override void VisitNumericLiteral(NumericLiteral item)
        {
            var sqlValue = ParseNumericLiteralValue(item);
            if (IsVisitingFilterItem)
            {
                IdFilterValue = sqlValue;
            }
            else
            {
                throw new NotSupportedException();
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
                throw new NotSupportedException();
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
                throw new NotSupportedException();
            }

        }

        #endregion

        private void GuardDeleteBuilder(DeleteBuilder builder)
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
