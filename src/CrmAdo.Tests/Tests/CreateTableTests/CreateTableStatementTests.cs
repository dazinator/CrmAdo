using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Tests.Support;

namespace CrmAdo.Tests
{
    [Obsolete]
    [Category("Create Table Statement")]
    [TestFixture()]
    public class CreateTableStatementTests : BaseOrganisationRequestBuilderVisitorTest
    {

        [Test(Description = "Should support creating a new entity")]
        public void Can_Create_New_Entity_With_Named_Id_And_Name_Columns()
        {
            // Arrange

            // Due to crm api limitations, new tables (entities) must be created with a GUID ID Primary Key, and a string name column.
            // You cannot include any other columns in the create statement.

            // The table must then be "altered" to add in your additional custom columns.
            string entityName = "testentity";
            string idAttName = string.Format("{0}id", entityName);
            string nameAttName = "name";

            string commandText = string.Format(@"CREATE TABLE {0}({1} UNIQUEIDENTIFIER PRIMARY KEY, {2} VARCHAR)", entityName, idAttName, nameAttName);

            var request = GetCreateEntityRequest(commandText);

            var entMetadata = request.Entity;

            Assert.IsNotNull(entMetadata);
            Assert.That(entMetadata.LogicalName, Is.EqualTo(entityName.ToLower()));

            // var idAtt = entMetadata.PrimaryIdAttribute;
            //  Assert.That(idAtt, Is.EqualTo(idAttName));

            var nameAtt = request.PrimaryAttribute;
            Assert.IsNotNull(nameAtt);
            Assert.That(nameAtt.LogicalName, Is.EqualTo(nameAttName.ToLower()));
        }



        #region Helper Methods

        private CreateEntityRequest GetCreateEntityRequest(string sql)
        {
            // set up fake metadata provider.
            // var fakeMetadataProvider = MockRepository.GenerateMock<ICrmMetaDataProvider>();
            // var fakeMetadata = GetFakeContactMetadata();
            // fakeMetadataProvider.Stub(a => a.GetEntityMetadata("contact")).Return(fakeMetadata);
            var fakeConn = MockRepository.GenerateMock<CrmDbConnection>();
            //  fakeConn.Stub(a => a.MetadataProvider).Return(fakeMetadataProvider);

            var cmd = new CrmDbCommand(fakeConn);
            cmd.CommandText = sql;
            var createRequest = GetOrganizationRequest<CreateEntityRequest>(cmd);
            return createRequest;

        }

        public void CreateEnity(string entityLogicalName)
        {
            // Check Crm metadata to see if entity exists.
            // _LogFactory().WriteInformation("Ensuring Crm has Journal Entity..");         

            //  _LogFactory().WriteInformation("Creating journal entity in Crm..");
            //var journalEntityBuilder = EntityConstruction.ConstructEntity(entityName)
            //                                             .DisplayName("Crm Up Journal Entry")
            //                                             .Description(
            //                                                 "Holds journal entrues regarding upgrades made by CrmUp.")
            //                                             .DisplayCollectionName("CrmUp Journal")
            //                                             .WithAttributes()
            //                                             .StringAttribute("crmup_scriptname", "Script Name",
            //                                                              "The name of the script that was applied by CrmUp.",
            //                                                              AttributeRequiredLevel.ApplicationRequired,
            //                                                              255, StringFormat.Text)
            //                                             .DateTimeAttribute("crmup_appliedon", "Applied On",
            //                                                                "The date the script was applied.",
            //                                                                AttributeRequiredLevel
            //                                                                    .ApplicationRequired,
            //                                                                DateTimeFormat.DateAndTime,
            //                                                                ImeMode.Disabled);




        }

        #endregion
    }
}