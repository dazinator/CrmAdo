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
    [Category("Batch Statement")]
    public class CommandBatchIntegrationTests : BaseTest
    {
        [Test(Description = "Integration test that creates a new entity, then alters the entity to add columns in one command.")]
        public void Should_Be_Able_To_Create_A_New_Entity_And_Then_Alter_It_In_Single_Batch()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            string createEntitySqlFormatString = "CREATE TABLE {0}{1} ({0}{1}id UNIQUEIDENTIFIER PRIMARY KEY, {0}{1}name NVARCHAR(125));";
            string randomEntityName = "createtest" + Guid.NewGuid().ToString().Replace("-", "").Remove(0, 16);

            var sql = string.Format(createEntitySqlFormatString, DefaultPublisherPrefix, randomEntityName);

            string alterEntitySql = string.Format("ALTER TABLE {0}{1} ADD {0}MyBool BIT", DefaultPublisherPrefix, randomEntityName);
            sql = sql + alterEntitySql;

            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;

                var reader = command.ExecuteReader();
                Assert.That(reader.NextResult());

                while (reader.Read())
                {

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Console.Write(reader[i]);
                    }
                }

            }

        }

        [Test(Description = "Should be able to execute a Batch of sql statements")]
        public void Should_Be_Able_To_Execute_Bath_Of_Statements()
        {
            // Arrange

            Guid id = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");

            string insertSql = string.Format("INSERT INTO contact (contactid, firstname, lastname) VALUES ('{0}','billy','bob');", id);
            string insertSqlWithOutputClause = "INSERT INTO contact (firstname, lastname) OUTPUT INSERTED.contactid VALUES ('bat','man');";
            string updateSql = string.Format("UPDATE contact SET firstname = 'john', lastname = 'doe' WHERE contactid = '{0}';", id);
            string updateSqlWithOutputClause = string.Format("UPDATE contact SET firstname = 'johny', lastname = 'doe' OUTPUT INSERTED.modifiedon WHERE contactid = '{0}';", id);
            string deleteSql = string.Format("DELETE FROM contact WHERE contactid = '{0}';", id);
            string selectSql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname ASC;";

            var sql = insertSql + insertSqlWithOutputClause + updateSql + updateSqlWithOutputClause + deleteSql + selectSql;

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                conn.Open();

                // Act
                var reader = cmd.ExecuteReader();

                // Should have one row containing inserted record id.             
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 1);

                while (reader.Read())
                {
                    // Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(createdOnDate));
                    Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                }

                // Move to second result for insert plus output.
                reader.NextResult();

                // Should have one row containing inserted record id.               
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 1);

                while (reader.Read())
                {
                    // Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(createdOnDate));
                    // we didn't specify the id, so one should be generated for us.
                    Assert.That(reader.GetGuid(0), NUnit.Framework.Is.Not.Null);
                }

                // Move to third result for update.
                reader.NextResult();

                // Move to fourth result for update plus output.
                reader.NextResult();

                // Should have one row containing output clause values.               
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 1);

                DateTime expectedOutputModifiedOnDate = DateTime.UtcNow;

                while (reader.Read())
                {
                    Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.Not.Null);
                    //Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                }

                // Move to fifth result for delete.
                reader.NextResult();

                // no result set..

                // Move to sixth result for retrieve multiple.
                reader.NextResult();

                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 2);
                int recordCount = 0;
                while (reader.Read())
                {
                    recordCount++;
                    Console.WriteLine("{0} - {1}", reader.GetString(0), reader.GetString(1));
                    // Assert.That(reader.GetString(0), NUnit.Framework.Is.Not.Null);
                    //  Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                }

                Assert.That(recordCount == 10);


            }


        }

    }
}