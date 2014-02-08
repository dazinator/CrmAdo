using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Core;
using NUnit.Framework;
using Rhino.Mocks;
using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace DynamicsCrmDataProvider.Tests
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
                    while (reader.Read())
                    {
                        resultCount++;
                        var contactId = (Guid)reader["contactid"];
                        var firstName = (string)reader["firstname"];
                        var lastName = (string)reader["lastname"];
                        Console.WriteLine(string.Format("{0} {1} {2}", contactId, firstName, lastName));
                    }
                    Console.WriteLine("There were " + resultCount + " results..");
                }
            }

        }

    }
}
