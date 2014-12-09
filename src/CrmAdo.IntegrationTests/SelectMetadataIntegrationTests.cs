using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CrmAdo.IntegrationTests
{
    [TestFixture()]
    [Category("Select Statement")]
    [Category("Metadata")]
    public class SelectMetadataIntegrationTests : BaseTest
    {

        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();
            // Create a new Entity for these tests to be carried out against.
            //  TestEntityName = base.CreateTestEntity();
        }

        [Test(Description = "Integration test that selects entity metadata from crm.")]
        public void Should_Be_Able_To_Select_Entity_Metadata()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            //  string attributeSchemaName = "boolField";
            //string lookupToEntity = "contact";
            var sql = "SELECT e.MetadataId FROM EntityMetadata AS e";
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                //   Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                var results = command.ExecuteReader();
                while (results.Read())
                {

                    Console.WriteLine("MetadataId: " + results["MetadataId"]);
                }
            }

        }

        [Test(Description = "Integration test that selects entity metadata from crm.")]
        public void Should_Be_Able_To_Select_Entity_Metadata_All_Columns()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            //  string attributeSchemaName = "boolField";
            //string lookupToEntity = "contact";
            var sql = "SELECT * FROM EntityMetadata";
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                //   Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                var results = command.ExecuteReader();
                while (results.Read())
                {
                    Console.WriteLine("MetadataId: " + results[0]);
                }
            }

        }

        [Test(Description = "Integration test that selects entity logical names from crm.")]
        public void Should_Be_Able_To_Select_Entity_Logical_Names()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            //  string attributeSchemaName = "boolField";
            //string lookupToEntity = "contact";
            var sql = "SELECT LogicalName FROM EntityMetadata";
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                //   Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                var results = command.ExecuteReader();
                while (results.Read())
                {
                    Console.WriteLine("LogicalName: " + results[0]);
                }
            }

        }

        [Test(Description = "Integration test that selects entity metadata joined to attribute metadata.")]
        public void Should_Be_Able_To_Select_EntityMetadata_Joined_To_Attribute_Metadata()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            //  string attributeSchemaName = "boolField";
            //string lookupToEntity = "contact";
            var sql = "SELECT e.LogicalName, a.LogicalName FROM EntityMetadata AS e INNER JOIN AttributeMetadata a ON e.MetadataId = a.MetadataId";
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                //   Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                var results = command.ExecuteReader();
                while (results.Read())
                {
                    Console.WriteLine("Entity: " + results[0] + ", Atttribute: " + results[1]);
                }
            }

        }

        [Test(Description = "Integration test that selects attribute metadata for a particular entity from crm.")]
        public void Should_Be_Able_To_Select_Attribute_Metadata_For_Entity()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            //  string attributeSchemaName = "boolField";
            //string lookupToEntity = "contact";
            var sql = "SELECT a.LogicalName FROM EntityMetadata AS e INNER JOIN AttributeMetadata a ON e.MetadataId = a.MetadataId WHERE e.LogicalName = 'contact'";
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                //   Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                var results = command.ExecuteReader();
                while (results.Read())
                {
                    for (int i = 0; i < results.FieldCount; i++)
                    {
                        Console.Write(results[i]);
                        Console.Write(',');
                    }
                    Console.WriteLine("");
                }
            }

        }

        [Test(Description = "Integration test that selects specific attribute metadata for a particular entity from crm.")]
        public void Should_Be_Able_To_Select_Specific_Attribute_Metadata_Columns_For_Entity()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            //  string attributeSchemaName = "boolField";
            //string lookupToEntity = "contact";
            var sql = "SELECT attributemetadata.MetadataId, attributemetadata.EntityLogicalName, attributemetadata.LogicalName, attributemetadata.ColumnNumber FROM entitymetadata INNER JOIN attributemetadata ON entitymetadata.MetadataId = attributemetadata.MetadataId WHERE entitymetadata.LogicalName = '{0}'";
            sql = string.Format(sql, "contact");
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                //   Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                var results = command.ExecuteReader();
                while (results.Read())
                {
                    for (int i = 0; i < results.FieldCount; i++)
                    {
                        Console.Write("field " + i.ToString() + ": ");
                        var value = results[i];
                        if (value == null)
                        {
                            Console.Write("NULL");
                        }
                        else
                        {
                            Console.Write(value);
                        }

                        Console.Write(',');
                    }
                    Console.WriteLine("");
                }
            }

        }


        [Test(Description = "Integration test that selects one to many relationship metadata for a particular entity from crm.")]
        public void Should_Be_Able_To_Select_One_To_Many_Relationship_Metadata_For_Entity()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            //  string attributeSchemaName = "boolField";
            //string lookupToEntity = "contact";
            var sql = "SELECT o.* FROM entitymetadata e INNER JOIN onetomanyrelationshipmetadata o ON e.MetadataId = o.MetadataId WHERE e.LogicalName = 'account'";
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                //   Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                var results = command.ExecuteReader();
                while (results.Read())
                {
                    for (int i = 0; i < results.FieldCount; i++)
                    {
                        Console.Write(results[i]);
                        Console.Write(',');
                    }
                    Console.WriteLine("");
                }
            }

        }

        //[Test(Description = "Integration test that selects entity metadata joined to attribute metadata all columns.")]
        //public void Should_Be_Able_To_Select_EntityMetadata_Joined_To_Attribute_Metadata_All_Columns()
        //{
        //    // create a random name for the entity. We use half a guid because names cant be too long.
        //    //  string attributeSchemaName = "boolField";
        //    //string lookupToEntity = "contact";
        //    var sql = "SELECT e.*, a.* FROM EntityMetadata AS e INNER JOIN AttributeMetadata a ON e.MetadataId = a.MetadataId";
        //    Console.WriteLine(sql);

        //    var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
        //    using (var conn = new CrmDbConnection(connectionString.ConnectionString))
        //    {
        //        conn.Open();
        //        var command = conn.CreateCommand();

        //        //   Console.WriteLine("Executing command " + sql);
        //        command.CommandText = sql;
        //        //   command.CommandType = CommandType.Text;
        //        var results = command.ExecuteReader();               

        //        while (results.Read())
        //        {
        //            for (int i = 0; i < results.FieldCount; i++)
        //            {
        //                Console.Write(results[i]);
        //                Console.Write(',');
        //            }
        //            Console.WriteLine("");
        //        }
        //    }

        //}

    }





}