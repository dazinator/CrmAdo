using CrmAdo.Dynamics.Metadata;
using CrmAdo.Tests.WIP;
using Microsoft.Xrm.Sdk;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Tests.WIP.Visitors
{
    public class OrganizationRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {
        public const string ParameterToken = "@";

        public OrganizationRequest OrganizationRequest { get; set; }
        public ICrmMetaDataProvider CrmMetadataProvider { get; set; }
        public DbParameterCollection Parameters { get; set; }

        public OrganizationRequestBuilderVisitor(ICrmMetaDataProvider crmMetadataProvider, DbParameterCollection parameters)
        {
            CrmMetadataProvider = crmMetadataProvider;
            Parameters = parameters;
        }

        protected override void VisitSelect(SQLGeneration.Builders.SelectBuilder item)
        {
            // Could use alternate builders like a fetch xml builder.
            var selectVisitorBuilder = new RetrieveMultipleRequestBuilderVisitor(Parameters);
            IVisitableBuilder builder = item;
            builder.Accept(selectVisitorBuilder);
            OrganizationRequest = selectVisitorBuilder.Request;
        }

        protected override void VisitInsert(InsertBuilder item)
        {
            var createBuilder = new CreateRequestBuilderVisitor(Parameters, CrmMetadataProvider);
            IVisitableBuilder builder = item;
            builder.Accept(createBuilder);
            OrganizationRequest = createBuilder.Request;
        }

    }
}
