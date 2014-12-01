using System;
using System.Data;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;

namespace CrmAdo.Tests
{
    [Category("Schema")]
    [TestFixture()]
    public class SchemaCollectionsProviderTests : BaseTest<SchemaCollectionsProvider>
    {

        [Test]
        public void Should_Be_Able_To_Create_SchemaCollectionsProvider()
        {
            var sut = CreateTestSubject();

        }

        [Test]
        public void Should_Be_Able_To_Get_MetaDataCollections()
        {
            // Arrange
            var sut = CreateTestSubject();
            // Act
            var metadataCollections = sut.GetMetadataCollections();
            // Assert
            Assert.That(metadataCollections, Is.Not.Null);
            Assert.That(metadataCollections.Columns, Is.Not.Null);
            Assert.That(metadataCollections.Columns.Count, Is.EqualTo(3));         

            

        }

        [Test]
        public void Should_Be_Able_To_Get_DataTypeCollection()
        {
            // Arrange
            var sut = CreateTestSubject();
            // Act
            var collection = sut.GetDataTypes();
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Columns, Is.Not.Null);
            Assert.That(collection.Columns.Count, Is.EqualTo(22));



        }    


    }
}