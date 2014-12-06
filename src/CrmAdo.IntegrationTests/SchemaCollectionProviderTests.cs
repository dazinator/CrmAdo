using System;
using System.Data;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.IntegrationTests;
using System.Configuration;

namespace CrmAdo.IntegrationTests
{
    [Category("Schema")]
    [TestFixture()]
    public class SchemaCollectionsProviderTests : BaseTest
    {

        [Test]
        public void Should_Be_Able_To_Create_SchemaCollectionsProvider()
        {
            var sut = new SchemaCollectionsProvider();

        }

        [Test]
        public void Should_Be_Able_To_Get_MetaDataCollections()
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();
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
            var sut = new SchemaCollectionsProvider();
            // Act

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {

                var collection = sut.GetDataSourceInfo(conn);

                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Columns.Count, Is.EqualTo(17));
            }





        }

        [Test]
        public void Should_Be_Able_To_Get_DataTypeCollection()
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();
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
            var sut = new SchemaCollectionsProvider();
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
            var sut = new SchemaCollectionsProvider();

            // Act
            var collection = sut.GetRestrictions();
            // Assert
            Assert.That(collection, Is.Not.Null);
            Assert.That(collection.Columns, Is.Not.Null);
            Assert.That(collection.Columns.Count, Is.EqualTo(5));

        }

        [Test]
        public void Should_Be_Able_To_Get_Users()
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { "" };
                // Act
                var collection = sut.GetUsers(conn, restrictions);
                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Columns.Count, Is.EqualTo(5));

            }
        }

        [Test]
        public void Should_Be_Able_To_Get_Tables()
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { "" };
                // Act
                var collection = sut.GetTables(conn, restrictions);
                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Rows.Count, Is.GreaterThan(0));

                var firstRow = collection.Rows[0];

                var col = collection.Columns["table_catalog"];

                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo(""));

                col = collection.Columns["table_schema"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo(""));

                col = collection.Columns["table_name"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.Not.EqualTo(""));

                col = collection.Columns["table_type"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo("BASE TABLE"));         


            }





        }


    }
}