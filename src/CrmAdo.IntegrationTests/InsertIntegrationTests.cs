using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
    public class InsertIntegrationTests
    {
        private Guid _NewContactId;

        [SetUp]
        public void Setup()
        {
            _NewContactId = Guid.NewGuid();
        }

        [TearDown]
        public void TearDown()
        {
            _NewContactId = Guid.NewGuid();
        }


        // NOTE: THESE TESTS REQUIRE A CONNECTION STRING TO BE SET IN THE CONFIG FILE, WITH A NAME OF 'CrmOrganisation'
        // ============================================================================================================
        [Category("Integration")]
        [Test(Description = "Integration tests that perform a variety of select queries against CRM.")]
        [TestCase("INSERT INTO contact (contactid, firstname, lastname) Values('{0}','{1}','{2}')")]
        public void Should_Get_Results_When_Querying_Crm_Using_Filter_Operators(string insertSql)
        {

            var sql = string.Format(insertSql, _NewContactId, "Derren", "Brown");

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader())
                {
                    int resultCount = 0;
                    foreach (var result in reader)
                    {
                        resultCount++;
                        var contactId = (Guid)reader["contactid"];
                        Console.WriteLine(string.Format("{0}", contactId));
                    }
                    //while (reader.Read())
                    //{

                    //}
                    Console.WriteLine("There were " + resultCount + " results..");
                }
            }

        }

        [Category("Integration")]
        [Test(Description = "Integration tests that gets metadata from crm.")]
        public void Should_Get_Changed_Metadata()
        {
            // arrange
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                       new CrmClientCredentialsProvider());

            var sut = new EntityMetadataRepository(serviceProvider);
            // act
            var contactMetadata = sut.GetEntityMetadata("contact");

            var serialised = EntityMetadataUtils.SerializeMetaData(contactMetadata, Formatting.Indented);
            var path = Environment.CurrentDirectory;
            var fileName = System.IO.Path.Combine(path, "contactMedadata.xml");
            Console.Write("writing to: " + fileName);
            using (var writer = new StreamWriter(fileName))
            {
                writer.Write(serialised);
                writer.Flush();
            }

            // assert
            Assert.That(contactMetadata, Is.Not.Null);
            Assert.That(contactMetadata, Is.Not.Null);

            Assert.That(contactMetadata.Attributes, Is.Not.Null);
            Assert.That(contactMetadata.Attributes.FirstOrDefault(a => a.LogicalName == "firstname"), Is.Not.Null);
            Assert.That(contactMetadata.Attributes.FirstOrDefault(a => a.LogicalName == "lastname"), Is.Not.Null);
        }

        [Category("Integration")]
        [Test]
        [TestCase("INNER")]
        [TestCase("LEFT")]
        public void Should_Be_Able_To_Select_Using_Table_Joins(String joinType)
        {
            var join = JoinOperator.Natural;

            switch (joinType)
            {
                case "INNER":
                    join = JoinOperator.Inner;
                    //  Enum.Parse(typeof(JoinOperator), joinType)
                    break;
                case "LEFT":
                    join = JoinOperator.LeftOuter;
                    break;
            }


            var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.line1 From contact C {0} JOIN customeraddress A on C.contactid = A.parentid", joinType);

            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader())
                {
                    int resultCount = 0;
                    foreach (var result in reader)
                    {
                        resultCount++;
                        var contactId = (Guid)reader["C.contactid"];
                        var firstName = (string)reader.SafeGetString(1);
                        var lastName = (string)reader.SafeGetString(2);
                        var line1 = (string)reader.SafeGetString(3);
                        var alsoLine1 = (string)reader.SafeGetString("A.line1");
                        Console.WriteLine(string.Format("{0} {1} {2} {3}", contactId, firstName, lastName, line1));
                    }
                    Console.WriteLine("There were " + resultCount + " results..");
                }
            }


        }

        [Category("Integration")]
        [TestCase("INNER", "((C.firstname = 'Albert' AND C.lastname = 'Einstein') OR (C.lastname = 'Planck' AND C.firstname = 'Max')) AND (C.contactid = '21476b89-41b1-e311-9351-6c3be5be9f98')", 2, TestName = "Should be able to chain filter groups in parenthesis using AND as well as OR conjunctions")]
        [TestCase("INNER", "(C.firstname = 'Albert' AND C.lastname = 'Einstein') OR (C.lastname = 'Planck' AND C.firstname = 'Max') OR (C.contactid = '21476b89-41b1-e311-9351-6c3be5be9f98')", 4, TestName = "Should be able to chain mutiple filter groups in parenthesis using an OR conjunction")]
        [TestCase("INNER", "C.firstname = 'Albert' AND (C.lastname = 'Einstein' AND C.contactid = '21476b89-41b1-e311-9351-6c3be5be9f98')", 2, TestName = "Should be able to chain an AND conjunction with a nested filter group containing an AND conjunction")]
        [TestCase("INNER", "C.firstname = 'Albert' OR (C.lastname = 'Planck' AND C.firstname = 'Max')", 4, TestName = "Should be able to chain an OR conjunction with a nested filter group containing an AND conunction")]
        [TestCase("INNER", "(C.firstname = 'Albert' AND C.lastname = 'Einstein') OR C.contactid = '5f90afbb-41b1-e311-9351-6c3be5be9f98'", 4, TestName = "Should be able to chain multiple AND conjunctions then a single OR conjunction")]
        [TestCase("INNER", "C.firstname = 'Albert' OR C.lastname = 'Planck' OR C.lastname = 'Galilei'", 6, TestName = "Should be able to chain OR conjunctions")]
        [TestCase("INNER", "C.firstname = 'Albert' AND C.lastname = 'Einstein' AND C.contactid = '21476b89-41b1-e311-9351-6c3be5be9f98'", 2, TestName = "Should be able to chain AND conjunctions")]
        [TestCase("INNER", "C.firstname = 'Albert' AND C.lastname = 'Einstein'", 2, TestName = "Should be able to use a single AND conjunction")]
        [TestCase("INNER", "C.firstname = 'Albert' OR C.firstname = 'Max'", 4, TestName = "Should be able to use a single OR conjunction")]
        [Test]
        public void Should_Be_Able_To_Query_Using_Filter_Groups(String joinType, string whereClause, int expectedResults)
        {
            var join = JoinOperator.Natural;

            switch (joinType)
            {
                case "INNER":
                    join = JoinOperator.Inner;
                    //  Enum.Parse(typeof(JoinOperator), joinType)
                    break;
                case "LEFT":
                    join = JoinOperator.LeftOuter;
                    break;
            }


            var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.line1 From contact C {0} JOIN customeraddress A on C.contactid = A.parentid WHERE {1}", joinType, whereClause);

            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader())
                {
                    int resultCount = 0;
                    foreach (var result in reader)
                    {
                        resultCount++;
                        var contactId = (Guid)reader["C.contactid"];
                        var firstName = (string)reader.SafeGetString(1);
                        var lastName = (string)reader.SafeGetString(2);
                        var line1 = (string)reader.SafeGetString(3);
                        var alsoLine1 = (string)reader.SafeGetString("A.line1");
                        Console.WriteLine(string.Format("{0} {1} {2} {3}", contactId, firstName, lastName, line1));
                    }
                    Console.WriteLine("There were " + resultCount + " results..");
                    Assert.That(resultCount, Is.EqualTo(expectedResults));
                }
            }


        }





    }
}