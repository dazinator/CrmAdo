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

namespace CrmAdo.Tests
{
    [Category("Delete Statement")]
    [TestFixture()]
    public class DeleteStatementTests : CrmQueryExpressionProviderTestsBase
    {

        [Category("String Literal")]
        [Test(Description = "Should support Deletion of a single entity by id")]
        public void Should_Support_Deletion_Of_A_Single_Entity_By_Id()
        {
            // Arrange
            Guid id = Guid.NewGuid();

            var sql = "DELETE FROM contact WHERE contactid = '" + id + "'";

            // set up fake metadata provider.
            var fakeMetadataProvider = MockRepository.GenerateMock<ICrmMetaDataProvider>();
            var fakeMetadata = GetFakeContactMetadata();
            fakeMetadataProvider.Stub(a => a.GetEntityMetadata("contact")).Return(fakeMetadata);
            var fakeConn = MockRepository.GenerateMock<CrmDbConnection>();
            fakeConn.Stub(a => a.MetadataProvider).Return(fakeMetadataProvider);

            var cmd = new CrmDbCommand(fakeConn);
            cmd.CommandText = sql;

            // Act
            var deleteRequest = GetOrganizationRequest<DeleteRequest>(cmd);

            // Assert
            EntityReference targetEntityRef = deleteRequest.Target;
            Assert.That(targetEntityRef, Is.Not.Null);
            Assert.That(targetEntityRef.Id, Is.EqualTo(id));
        }
        
        #region Helper Methods

        private CrmEntityMetadata GetFakeContactMetadata()
        {
            var path = Environment.CurrentDirectory;
            var fileName = System.IO.Path.Combine(path, "MetadataFiles\\contactMetadata.xml");

            using (var reader = new XmlTextReader(fileName))
            {
                var deserialised = EntityMetadataUtils.DeserializeMetaData(reader);
                var crmMeta = new CrmEntityMetadata();
                var atts = new List<AttributeMetadata>();
                atts.AddRange(deserialised.Attributes);
                crmMeta.Attributes = atts;
                crmMeta.EntityName = "contact";
                return crmMeta;
            }
        }

        #endregion
    }
}