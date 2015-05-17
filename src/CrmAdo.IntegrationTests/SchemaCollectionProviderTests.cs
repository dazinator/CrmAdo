using System;
using System.Data;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.IntegrationTests;
using System.Configuration;
using CrmAdo.Operations;
using CrmAdo.Core;

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
                Assert.That(firstRow[col], Is.EqualTo(conn.ConnectionInfo.OrganisationName));

                col = collection.Columns["table_schema"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo("dbo"));

                col = collection.Columns["table_name"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.Not.EqualTo(""));

                col = collection.Columns["table_type"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo("BASE TABLE"));


            }





        }

        [Test]
        [TestCase("account")]
        public void Should_Be_Able_To_Get_A_Table(string tableName)
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { tableName };
                // Act
                var collection = sut.GetTables(conn, restrictions);
                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Rows.Count, Is.EqualTo(1));

                var firstRow = collection.Rows[0];

                var col = collection.Columns["table_catalog"];

                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo(conn.ConnectionInfo.OrganisationName));

                col = collection.Columns["table_schema"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo("dbo"));

                col = collection.Columns["table_name"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo(tableName));

                col = collection.Columns["table_type"];
                Assert.That(col, Is.Not.Null);
                Assert.That(firstRow[col], Is.EqualTo("BASE TABLE"));


            }





        }

        [Test]
        [TestCase("account")]
        public void Should_Be_Able_To_Get_Columns(string tableName)
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { tableName };
                // Act
                var collection = sut.GetColumns(conn, restrictions);

                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Rows.Count, Is.GreaterThan(0));


                foreach (DataRow row in collection.Rows)
                {

                    var val = AssertColVal(collection, row, "table_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_schema");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_name");
                    Assert.That(val, Is.EqualTo(tableName));

                    val = AssertColVal(collection, row, "column_default");
                    val = AssertColVal(collection, row, "is_nullable");
                    string dataType = (string)AssertColVal(collection, row, "data_type");

                    var charMaxLength = AssertColVal(collection, row, "character_maximum_length");
                    var octetVal = AssertColVal(collection, row, "character_octet_length");
                    if (dataType == "nvarchar")
                    {
                        var charMax = (int)charMaxLength;
                        var octetMax = (int)octetVal;
                        Assert.That(octetMax, Is.EqualTo(charMax * 2));
                    }
                    else
                    {
                        Assert.That(charMaxLength, Is.EqualTo(DBNull.Value));
                        Assert.That(octetVal, Is.EqualTo(DBNull.Value));
                    }

                    val = AssertColVal(collection, row, "numeric_precision");
                    val = AssertColVal(collection, row, "numeric_precision_radix");
                    val = AssertColVal(collection, row, "numeric_scale");

                    val = AssertColVal(collection, row, "datetime_precision");
                    if (dataType == "datetime")
                    {
                        Assert.That(val, Is.EqualTo(3));
                    }
                    else
                    {
                        Assert.That(val, Is.EqualTo(DBNull.Value));
                    }

                    val = AssertColVal(collection, row, "character_set_catalog");
                    val = AssertColVal(collection, row, "character_set_schema");
                    var charSetName = AssertColVal(collection, row, "character_set_name");

                    if (dataType == "nvarchar")
                    {
                        Assert.That(charSetName, Is.EqualTo("UNICODE"));
                    }
                    else
                    {
                        Assert.That(charSetName, Is.EqualTo(DBNull.Value));
                    }

                    val = AssertColVal(collection, row, "collation_catalog");


                    val = AssertColVal(collection, row, "is_sparse");
                    Assert.That(val, Is.EqualTo(false));

                    val = AssertColVal(collection, row, "is_column_set");
                    Assert.That(val, Is.EqualTo(false));

                    val = AssertColVal(collection, row, "is_filestream");
                    Assert.That(val, Is.EqualTo(false));
                }

            }

        }

        [Test]
        [TestCase("account", "accountid")]
        public void Should_Be_Able_To_Get_A_Column(string tableName, string columnName)
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { tableName, columnName };
                // Act
                var collection = sut.GetColumns(conn, restrictions);

                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Rows.Count, Is.EqualTo(1));


                foreach (DataRow row in collection.Rows)
                {

                    var val = AssertColVal(collection, row, "table_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_schema");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_name");
                    Assert.That(val, Is.EqualTo(tableName));

                    val = AssertColVal(collection, row, "column_name");
                    Assert.That(val, Is.EqualTo(columnName));

                    val = AssertColVal(collection, row, "column_default");
                    val = AssertColVal(collection, row, "is_nullable");
                    string dataType = (string)AssertColVal(collection, row, "data_type");

                    var charMaxLength = AssertColVal(collection, row, "character_maximum_length");
                    var octetVal = AssertColVal(collection, row, "character_octet_length");
                    if (dataType == "nvarchar")
                    {
                        var charMax = (int)charMaxLength;
                        var octetMax = (int)octetVal;
                        Assert.That(octetMax, Is.EqualTo(charMax * 2));
                    }
                    else
                    {
                        Assert.That(charMaxLength, Is.EqualTo(DBNull.Value));
                        Assert.That(octetVal, Is.EqualTo(DBNull.Value));
                    }

                    val = AssertColVal(collection, row, "numeric_precision");
                    val = AssertColVal(collection, row, "numeric_precision_radix");
                    val = AssertColVal(collection, row, "numeric_scale");

                    val = AssertColVal(collection, row, "datetime_precision");
                    if (dataType == "datetime")
                    {
                        Assert.That(val, Is.EqualTo(3));
                    }
                    else
                    {
                        Assert.That(val, Is.EqualTo(DBNull.Value));
                    }

                    val = AssertColVal(collection, row, "character_set_catalog");
                    val = AssertColVal(collection, row, "character_set_schema");
                    var charSetName = AssertColVal(collection, row, "character_set_name");

                    if (dataType == "nvarchar")
                    {
                        Assert.That(charSetName, Is.EqualTo("UNICODE"));
                    }
                    else
                    {
                        Assert.That(charSetName, Is.EqualTo(DBNull.Value));
                    }

                    val = AssertColVal(collection, row, "collation_catalog");


                    val = AssertColVal(collection, row, "is_sparse");
                    Assert.That(val, Is.EqualTo(false));

                    val = AssertColVal(collection, row, "is_column_set");
                    Assert.That(val, Is.EqualTo(false));

                    val = AssertColVal(collection, row, "is_filestream");
                    Assert.That(val, Is.EqualTo(false));
                }

            }

        }

        [Test]
        [TestCase("account")]
        public void Should_Be_Able_To_Get_ForeignKeys_For_A_Table(string tableName)
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { tableName };
                // Act
                var collection = sut.GetForeignKeys(conn, restrictions);

                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Rows.Count, Is.GreaterThan(0));


                foreach (DataRow row in collection.Rows)
                {

                    var val = AssertColVal(collection, row, "constraint_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "constraint_schema");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "constraint_name");
                    Assert.IsFalse(string.IsNullOrEmpty((string)val));
                    Console.WriteLine(val);

                    val = AssertColVal(collection, row, "table_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_schema");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_name");
                    Assert.That(val, Is.EqualTo(tableName));

                    val = AssertColVal(collection, row, "constraint_type");
                    Assert.That(val, Is.EqualTo("FOREIGN KEY"));

                    val = AssertColVal(collection, row, "is_deferrable");
                    Assert.That(val, Is.EqualTo("NO"));

                    val = AssertColVal(collection, row, "initially_deferred");
                    Assert.That(val, Is.EqualTo("NO"));

                }

            }

        }

        [Test]
        public void Should_Be_Able_To_Get_Indexes()
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { };
                // Act
                var collection = sut.GetIndexes(conn, restrictions);

                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Rows.Count, Is.GreaterThan(0));




                foreach (DataRow row in collection.Rows)
                {

                    //<constraint_catalog>PortalDarrellDev</constraint_catalog>
                    //<constraint_schema>dbo</constraint_schema>
                    //<constraint_name>PK__Table__3214EC07326C5B6A</constraint_name>
                    //<table_catalog>PortalDarrellDev</table_catalog>
                    //<table_schema>dbo</table_schema>
                    //<table_name>Table</table_name>
                    //<index_name>PK__Table__3214EC07326C5B6A</index_name>
                    //<type_desc>CLUSTERED</type_desc>

                    var val = AssertColVal(collection, row, "constraint_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "constraint_schema");
                    Assert.That(val, Is.EqualTo(""));

                    var constraintName = AssertColVal(collection, row, "constraint_name");
                    Assert.IsFalse(string.IsNullOrEmpty((string)constraintName));
                    Assert.That((string)constraintName, Is.StringStarting("PK__"));

                    Console.WriteLine(constraintName);

                    val = AssertColVal(collection, row, "table_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_schema");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_name");
                    Assert.That(val, Is.Not.EqualTo(""));
                    Console.Write(" - ");
                    Console.Write(val);

                    val = AssertColVal(collection, row, "index_name");
                    Assert.That(val, Is.EqualTo(constraintName));

                    val = AssertColVal(collection, row, "type_desc");
                    Assert.That(val, Is.EqualTo("CLUSTERED"));

                }

            }

        }

        [Test]
        [TestCase("account")]
        public void Should_Be_Able_To_Get_Indexes_For_A_Table(string tableName)
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { tableName };
                // Act
                var collection = sut.GetIndexes(conn, restrictions);

                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Rows.Count, Is.EqualTo(1));

                foreach (DataRow row in collection.Rows)
                {

                    //<constraint_catalog>PortalDarrellDev</constraint_catalog>
                    //<constraint_schema>dbo</constraint_schema>
                    //<constraint_name>PK__Table__3214EC07326C5B6A</constraint_name>
                    //<table_catalog>PortalDarrellDev</table_catalog>
                    //<table_schema>dbo</table_schema>
                    //<table_name>Table</table_name>
                    //<index_name>PK__Table__3214EC07326C5B6A</index_name>
                    //<type_desc>CLUSTERED</type_desc>

                    var val = AssertColVal(collection, row, "constraint_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "constraint_schema");
                    Assert.That(val, Is.EqualTo(""));

                    var constraintName = AssertColVal(collection, row, "constraint_name");
                    Assert.IsFalse(string.IsNullOrEmpty((string)constraintName));
                    Assert.That((string)constraintName, Is.StringStarting("PK__"));

                    Console.WriteLine(constraintName);

                    val = AssertColVal(collection, row, "table_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_schema");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_name");
                    Assert.That(val, Is.Not.EqualTo(""));
                    Console.Write(" - ");
                    Console.Write(val);

                    val = AssertColVal(collection, row, "index_name");
                    Assert.That(val, Is.EqualTo(constraintName));

                    val = AssertColVal(collection, row, "type_desc");
                    Assert.That(val, Is.EqualTo("CLUSTERED"));

                }

            }
        }


        [Test]
        public void Should_Be_Able_To_Get_IndexColumns()
        {
            // Arrange
            var sut = new SchemaCollectionsProvider();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var restrictions = new string[] { };
                // Act
                var collection = sut.GetIndexColumns(conn, restrictions);

                // Assert
                Assert.That(collection, Is.Not.Null);
                Assert.That(collection.Columns, Is.Not.Null);
                Assert.That(collection.Rows.Count, Is.GreaterThan(0));

                foreach (DataRow row in collection.Rows)
                {

                    //         <IndexColumns>
                    //  <constraint_catalog>PortalDarrellDev</constraint_catalog>
                    //  <constraint_schema>dbo</constraint_schema>
                    //  <constraint_name>PK__tmp_ms_x__3214EC0737311087</constraint_name>
                    //  <table_catalog>PortalDarrellDev</table_catalog>
                    //  <table_schema>dbo</table_schema>
                    //  <table_name>Table</table_name>
                    //  <column_name>Id</column_name>
                    //  <ordinal_position>1</ordinal_position>
                    //  <KeyType>36</KeyType>
                    //  <index_name>PK__tmp_ms_x__3214EC0737311087</index_name>
                    //</IndexColumns>

                    var val = AssertColVal(collection, row, "constraint_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "constraint_schema");
                    Assert.That(val, Is.EqualTo(""));

                    var constraintName = AssertColVal(collection, row, "constraint_name");
                    Assert.IsFalse(string.IsNullOrEmpty((string)constraintName));
                    Assert.That((string)constraintName, Is.StringStarting("PK__"));

                    Console.WriteLine(constraintName);

                    val = AssertColVal(collection, row, "table_catalog");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_schema");
                    Assert.That(val, Is.EqualTo(""));

                    val = AssertColVal(collection, row, "table_name");
                    Assert.That(val, Is.Not.EqualTo(""));
                    Console.Write(" - ");
                    Console.Write(val);

                    val = AssertColVal(collection, row, "column_name");
                    Assert.That(val, Is.Not.EqualTo(""));

                    val = AssertColVal(collection, row, "ordinal_position");
                    Assert.That(val, Is.Not.EqualTo(default(int)));

                    val = AssertColVal(collection, row, "KeyType");
                    Assert.That(val, Is.EqualTo(36)); // unique identifier.

                    val = AssertColVal(collection, row, "index_name");
                    Assert.That(val, Is.EqualTo(constraintName));



                }

            }

        }

        //[Test]
        //public void Should_Be_Able_To_Get_All_ForeignKeys()
        //{
        //    // Arrange
        //    var sut = new SchemaCollectionsProvider();

        //    var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
        //    using (var conn = new CrmDbConnection(connectionString.ConnectionString))
        //    {
        //        var restrictions = new string[] { };
        //        // Act
        //        var collection = sut.GetForeignKeys(conn, restrictions);

        //        // Assert
        //        Assert.That(collection, Is.Not.Null);
        //        Assert.That(collection.Columns, Is.Not.Null);
        //        Assert.That(collection.Rows.Count, Is.GreaterThan(0));


        //        foreach (DataRow row in collection.Rows)
        //        {

        //            var val = AssertColVal(collection, row, "constraint_catalog");
        //            Assert.That(val, Is.EqualTo(""));

        //            val = AssertColVal(collection, row, "constraint_schema");
        //            Assert.That(val, Is.EqualTo(""));

        //            val = AssertColVal(collection, row, "constraint_name");
        //            Assert.IsFalse(string.IsNullOrEmpty((string)val));
        //            Console.WriteLine(val);

        //            val = AssertColVal(collection, row, "table_catalog");
        //            Assert.That(val, Is.EqualTo(""));

        //            val = AssertColVal(collection, row, "table_schema");
        //            Assert.That(val, Is.EqualTo(""));

        //            val = AssertColVal(collection, row, "table_name");
        //            Assert.IsFalse(string.IsNullOrEmpty((string)val));
        //            Console.Write(" : " + val);

        //            val = AssertColVal(collection, row, "constraint_type");
        //            Assert.That(val, Is.EqualTo("FOREIGN KEY"));

        //            val = AssertColVal(collection, row, "is_deferrable");
        //            Assert.That(val, Is.EqualTo("NO"));

        //            val = AssertColVal(collection, row, "initially_deferred");
        //            Assert.That(val, Is.EqualTo("NO"));

        //        }

        //    }

        //}

        private object AssertColVal(DataTable table, DataRow row, string columnName)
        {
            var col = table.Columns[columnName];
            Assert.That(col, Is.Not.Null);
            var val = row[col];
            return val;
        }


    }
}