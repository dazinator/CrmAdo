using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace DynamicsCrmDataProvider.IntegrationTests
{
    [TestFixture()]
    public class IntegrationTests
    {
        // NOTE: THESE TESTS REQUIRE A CONNECTION STRING TO BE SET IN THE CONFIG FILE, WITH A NAME OF 'CrmOrganisation'
        // ============================================================================================================
        [Category("Integration")]
        [Test(Description = "Integration tests that perform a variety of select queries against CRM.")]
        [TestCase("=", "Donald", "{0} {1} '{2}'", TestName = "Equals Filter with a String Constant")]
        [TestCase("<>", "Donald", "{0} {1} '{2}'", TestName = "Not Equals Filter with a String Constant")]
        [TestCase(">=", "Donald", "{0} {1} '{2}'", TestName = "Greater Than Or Equals Filter with a String Constant")]
        [TestCase("<=", "Donald", "{0} {1} '{2}'", TestName = "Less Than Or Equals Filter with a String Constant")]
        [TestCase(">", "Donald", "{0} {1} '{2}'", TestName = "Greater Than Filter with a String Constant")]
        [TestCase("<", "Donald", "{0} {1} '{2}'", TestName = "Less Than Filter with a String Constant")]
        [TestCase("IS NULL", null, "{0} {1} {2}", TestName = "Is Null Filter")]
        [TestCase("IS NOT NULL", null, "{0} {1}", TestName = "Is Not Null Filter")]
        [TestCase("LIKE", "Donald", "{0} {1} '{2}'", TestName = "Like Filter with a String Constant")]
        [TestCase("NOT LIKE", "Donald", "{0} {1} '{2}'", TestName = "Not Like Filter with a String Constant")]
        [TestCase("IN", new string[] { "Donald", "Daz" }, "{0} {1} ('{2}', '{3}')", TestName = "In Filter with string array")]
        [TestCase("NOT IN", new string[] { "Donald", "Daz" }, "{0} {1} ('{2}', '{3}')", TestName = "Not In Filter with string array")]
        [TestCase("LIKE", "Donald%", "{0} {1} '{2}'", TestName = "Starts With Filter")]
        [TestCase("LIKE", "%nald", "{0} {1} '{2}'", TestName = "Ends With Filter")]
        [TestCase("LIKE", "%onal%", "{0} {1} '{2}'", TestName = "Contains Filter")]
        [TestCase("NOT LIKE", "D%", "{0} {1} '{2}'", TestName = "Does Not Start With Filter")]
        [TestCase("NOT LIKE", "%d", "{0} {1} '{2}'", TestName = "Does Not End With Filter")]
        [TestCase("NOT LIKE", "%onal%", "{0} {1} '{2}'", TestName = "Does Not Contain Filter")]
        public void Should_Get_Results_From_Crm_When_Filtering_On_String_Attribute(string filterOperator, object value, string filterFormatString)
        {
            // Arrange
            // Formulate DML (SQL) statement from test case data.
            var columnName = "firstname";
            if (value == null || !value.GetType().IsArray)
            {
                filterFormatString = string.Format(filterFormatString, columnName, filterOperator, value);
            }
            else
            {
                var formatArgs = new List<object>();
                formatArgs.Add(columnName);
                formatArgs.Add(filterOperator);
                var args = value as IEnumerable;
                foreach (var arg in args)
                {
                    formatArgs.Add(arg);
                }
                filterFormatString = string.Format(filterFormatString, formatArgs.ToArray());
            }
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} ", filterFormatString);

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
                        var firstName = (string)reader.SafeGetString(1);
                        var lastName = (string)reader.SafeGetString(2);
                        Console.WriteLine(string.Format("{0} {1} {2}", contactId, firstName, lastName));
                    }
                    //while (reader.Read())
                    //{

                    //}
                    Console.WriteLine("There were " + resultCount + " results..");
                }
            }

        }

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

            // assert
            Assert.That(contactMetadata, Is.Not.Null);
            Assert.That(contactMetadata, Is.Not.Null);
        
            Assert.That(contactMetadata.Attributes, Is.Not.Null);
            Assert.That(contactMetadata.Attributes.FirstOrDefault(a => a.LogicalName == "firstname"), Is.Not.Null);
            Assert.That(contactMetadata.Attributes.FirstOrDefault(a => a.LogicalName == "lastname"), Is.Not.Null);
        }


        [Test(Description = "Queries using a join")]
        public void Should_Get_Results_From_Crm_When_Using_A_Join()
        {
            // arrange
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                       new CrmClientCredentialsProvider());

            var sut = new EntityMetadataRepository(serviceProvider);
            // act
            var contactMetadata = sut.GetEntityMetadata("contact");

            // assert
            Assert.That(contactMetadata, Is.Not.Null);
            Assert.That(contactMetadata, Is.Not.Null);

            Assert.That(contactMetadata.Attributes, Is.Not.Null);
            Assert.That(contactMetadata.Attributes.FirstOrDefault(a => a.LogicalName == "firstname"), Is.Not.Null);
            Assert.That(contactMetadata.Attributes.FirstOrDefault(a => a.LogicalName == "lastname"), Is.Not.Null);
        }

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


    }
}
