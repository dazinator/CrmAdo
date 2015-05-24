using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CrmAdo.IntegrationTests
{
    [TestFixture()]
    [Category("Create Statement")]
    public class CreateEntityIntegrationTests : BaseTest
    {
        [Test(Description = "Integration test that creates a new type of entity in CRM.")]
        [TestCase("CREATE TABLE {0}{1} ({0}{1}id UNIQUEIDENTIFIER PRIMARY KEY, {0}{1}name NVARCHAR(125))")]
        public void Should_Be_Able_To_Create_A_New_Entity(string sqlFormatString)
        {
            // create a random name for the entity. We use half a guid because names cant be too long.

            string randomEntityName = "createtest" + Guid.NewGuid().ToString().Replace("-", "").Remove(0, 16);
            var sql = string.Format(sqlFormatString, DefaultPublisherPrefix, randomEntityName);
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                base.ExecuteCrmNonQuery(sql, -1);
            }

        }

    }
}