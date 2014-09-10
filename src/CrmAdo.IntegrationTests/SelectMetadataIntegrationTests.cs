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
                    Console.WriteLine("MetadataId: " + results[0]);
                }
            }

        }

    }





}