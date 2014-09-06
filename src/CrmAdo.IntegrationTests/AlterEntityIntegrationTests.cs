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
    [Category("Alter Statement")]
    public class AlterEntityIntegrationTests : BaseTest
    {

        public string TestEntityName { get; set; }

        [TestFixtureSetUp]
        public override void SetUp()
        {
            base.SetUp();
            // Create a new Entity for these tests to be carried out against.
            TestEntityName = base.CreateTestEntity();
        }

        [Test(Description = "Integration test that creates a new type of entity in CRM.")]
        [TestCase("ALTER TABLE {0} ADD {1}{2} BIT", Description = "Can Add Boolean Attribute.")]
        public void Should_Be_Able_To_Add_A_New_Column(string sqlFormatString)
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            string attributeSchemaName = "boolField";
            var sql = string.Format(sqlFormatString, TestEntityName, DefaultPublisherPrefix, attributeSchemaName);
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                base.ExecuteNonQuery(sql, -1);
            }

        }





    }





}