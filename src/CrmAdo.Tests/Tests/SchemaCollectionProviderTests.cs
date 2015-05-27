using System;
using System.Data;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Core;

namespace CrmAdo.Tests
{
    [Category("Schema")]
    [TestFixture()]
    public class SchemaCollectionsProviderTests : BaseTest<SchemaCollectionsProvider>
    {

        [Test]
        public void Should_Be_Able_To_Create_SchemaCollectionsProvider()
        {
            var sut = ResolveTestSubjectInstance();

        }

        [Test]
        public void Should_Be_Able_To_Get_MetaDataCollections()
        {
            // Arrange
            var sut = ResolveTestSubjectInstance();
            // Act
            var metadataCollections = sut.GetMetadataCollections();
            // Assert
            Assert.That(metadataCollections, Is.Not.Null);
            Assert.That(metadataCollections.Columns, Is.Not.Null);
            Assert.That(metadataCollections.Columns.Count, Is.EqualTo(3));



        }

        [Test]
        public void Should_Be_Able_To_Get_DataSourceInfo()
        {
            // Arrange
            var sut = ResolveTestSubjectInstance();
            // Act

            var fakeMetadataProvider = new FakeContactMetadataProvider();
            var typeProvider = new DynamicsAttributeTypeProvider();

            var fakeConn = MockRepository.GenerateMock<CrmDbConnection>();
            fakeConn.Stub(a => a.MetadataProvider).Return(fakeMetadataProvider);
            fakeConn.Stub(a => a.ServerVersion).Return("1.0.0.0");

            var collection = sut.GetDataSourceInfo(fakeConn);
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Columns, Is.Not.Null);
            Assert.That(collection.Columns.Count, Is.AtLeast(17));

        }

        [Test]
        public void Should_Be_Able_To_Get_Databases()
        {
            // Arrange
            var sut = ResolveTestSubjectInstance();
            // Act

            var fakeMetadataProvider = new FakeContactMetadataProvider();
            var typeProvider = new DynamicsAttributeTypeProvider();

            var fakeConn = MockRepository.GenerateMock<CrmDbConnection>();
            fakeConn.Stub(a => a.ConnectionInfo).Return(new CrmConnectionInfo() { OrganisationId = Guid.NewGuid(), OrganisationName = "unittest", ServerVersion = "1.0.0.0" });
            fakeConn.Stub(a => a.MetadataProvider).Return(fakeMetadataProvider);
            fakeConn.Stub(a => a.ServerVersion).Return("1.0.0.0");

            var collection = sut.GetDatabases(fakeConn, null);
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Columns, Is.Not.Null);
            Assert.That(collection.Columns.Count, Is.AtLeast(3));
            Assert.That(collection.Rows.Count, Is.AtLeast(1));
            var value = collection.Rows[0][0];
            Assert.That((string)value == "unittest");          
        }

        [Test]
        public void Should_Be_Able_To_Get_DataTypeCollection()
        {
            // Arrange
            var sut = ResolveTestSubjectInstance();
            // Act
            var collection = sut.GetDataTypes();
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Columns, Is.Not.Null);
            Assert.That(collection.Columns.Count, Is.EqualTo(22));
        }

        [Test]
        public void Should_Be_Able_To_Get_ReservedWords()
        {
            // Arrange
            var sut = ResolveTestSubjectInstance();
            // Act
            var collection = sut.GetReservedWords();
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Columns, Is.Not.Null);
            Assert.That(collection.Columns.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_Be_Able_To_Get_Restrictions()
        {
            // Arrange
            var sut = ResolveTestSubjectInstance();

            // Act
            var collection = sut.GetRestrictions();
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Columns, Is.Not.Null);
            Assert.That(collection.Columns.Count, Is.EqualTo(5));

        }

        //[Test]
        //public void Should_Be_Able_To_Get_Users()
        //{
        //    // Arrange
        //    var sut = CreateTestSubject();

        //    var fakeMetadataProvider = new FakeContactMetadataProvider();
        //    var typeProvider = new DynamicsAttributeTypeProvider();

        //    var fakeConn = MockRepository.GenerateMock<CrmDbConnection>();
        //    fakeConn.Stub(a => a.MetadataProvider).Return(fakeMetadataProvider);
        //    fakeConn.Stub(a => a.ServerVersion).Return("1.0.0.0");        

        //    var restrictions = new string[]{""};

        //    // Act
        //    var collection = sut.GetUsers(fakeConn, restrictions);
        //    // Assert
        //    Assert.That(collection, Is.Not.Null);
        //    Assert.That(collection.Columns, Is.Not.Null);
        //    Assert.That(collection.Columns.Count, Is.EqualTo(4));

        //}


    }
}