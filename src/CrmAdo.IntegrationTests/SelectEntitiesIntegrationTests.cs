using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using CrmAdo.Operations;
using CrmAdo.Util;
using CrmAdo.Core;

namespace CrmAdo.IntegrationTests
{
    [TestFixture()]
    [Category("Select Statement")]
    public class SelectEntitiesIntegrationTests : BaseTest
    {

        public const int DefaultNumberOfCustomerAddressesPerContact = 3;
        public const int OneContact = DefaultNumberOfCustomerAddressesPerContact * 1;
        public const int TwoContacts = DefaultNumberOfCustomerAddressesPerContact * 2;
        public const int ThreeContacts = DefaultNumberOfCustomerAddressesPerContact * 3;


        [TestFixtureSetUp]
        public void TestDataSetup()
        {

            CleanUp();


            try
            {
                var sqlFormatString = "INSERT INTO CONTACT (contactid, firstname, lastname, address1_line1) VALUES('{0}', '{1}', '{2}', '{3}')";
                var insertAlbertEinstenSql = string.Format(sqlFormatString, Guid.Parse("21476b89-41b1-e311-9351-6c3be5be9f98"), "Albert", "Einstein", "Our hearts");

                ExecuteReader(insertAlbertEinstenSql, 1);

                var insertMaxPlanck = string.Format(sqlFormatString, Guid.Parse("5f90afbb-41b1-e311-9351-6c3be5be9f98"), "Max", "Planck", "Home1");
                ExecuteReader(insertMaxPlanck, 1);

                var insertGalileo = string.Format(sqlFormatString, Guid.Parse("6f90afbb-51b1-e311-9351-6c3ce5be9f93"), "Galileo", "Galilei", "Line1");
                ExecuteReader(insertGalileo, 1);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        // NOTE: THESE TESTS REQUIRE A CONNECTION STRING TO BE SET IN THE CONFIG FILE, WITH A NAME OF 'CrmOrganisation'
        // ============================================================================================================

        [Test(Description = "Integration tests that perform a variety of select queries against CRM.")]
        [TestCase("=", "Some Guy", "{0} {1} '{2}'", TestName = "Should Support Equals a String Constant")]
        [TestCase("<>", "Donald", "{0} {1} '{2}'", TestName = "Should Support Not Equals a String Constant")]
        [TestCase(">=", "Donald", "{0} {1} '{2}'", TestName = "Should Support Greater Than Or Equals a String Constant")]
        [TestCase("<=", "Donald", "{0} {1} '{2}'", TestName = "Should Support Less Than Or Equals Filter a String Constant")]
        [TestCase(">", "Donald", "{0} {1} '{2}'", TestName = "Should Support Greater Than a String Constant")]
        [TestCase("<", "Donald", "{0} {1} '{2}'", TestName = "Should Support Less Than a String Constant")]
        [TestCase("IS NULL", null, "{0} {1} {2}", TestName = "Should Support Is Null")]
        [TestCase("IS NOT NULL", null, "{0} {1}", TestName = "Should Support Is Not Null")]
        [TestCase("LIKE", "Donald", "{0} {1} '{2}'", TestName = "Should Support Like a String Constant")]
        [TestCase("NOT LIKE", "Donald", "{0} {1} '{2}'", TestName = "Should Support Not Like a String Constant")]
        [TestCase("IN", new string[] { "Donald", "Daz" }, "{0} {1} ('{2}', '{3}')", TestName = "Should Support In a string array")]
        [TestCase("NOT IN", new string[] { "Donald", "Daz" }, "{0} {1} ('{2}', '{3}')", TestName = "Should Support Not In a string array")]
        [TestCase("LIKE", "Donald%", "{0} {1} '{2}'", TestName = "Should Support Starts With Like")]
        [TestCase("LIKE", "%nald", "{0} {1} '{2}'", TestName = "Should Support Ends With")]
        [TestCase("LIKE", "%onal%", "{0} {1} '{2}'", TestName = "Should Support Contains")]
        [TestCase("NOT LIKE", "D%", "{0} {1} '{2}'", TestName = "Should Support Does Not Start With")]
        [TestCase("NOT LIKE", "%d", "{0} {1} '{2}'", TestName = "Should Support Does Not End With")]
        [TestCase("NOT LIKE", "%erren%", "{0} {1} '{2}'", TestName = "Should Support Does Not Contain")]
        public void Should_Get_Results_When_Querying_Crm_Using_Filter_Operators(string filterOperator, object value, string filterFormatString)
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


        [Test]
        [TestCase("INNER")]
        [TestCase("LEFT")]
        public void Should_Be_Able_To_Select_Using_Table_Joins(String joinType)
        {
            // var join = JoinOperator.Natural;

            //switch (joinType)
            //{
            //    case "INNER":
            //        //  join = JoinOperator.Inner;
            //        //  Enum.Parse(typeof(JoinOperator), joinType)
            //        break;
            //    case "LEFT":
            //        //   join = JoinOperator.LeftOuter;
            //        break;
            //}


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


        [TestCase("INNER", "((C.firstname = 'Albert' AND C.lastname = 'Einstein') OR (C.lastname = 'Planck' AND C.firstname = 'Max')) AND (C.contactid = '21476b89-41b1-e311-9351-6c3be5be9f98')", OneContact, TestName = "Should be able to chain filter groups in parenthesis using AND as well as OR conjunctions")]
        [TestCase("INNER", "(C.firstname = 'Albert' AND C.lastname = 'Einstein') OR (C.lastname = 'Planck' AND C.firstname = 'Max') OR (C.contactid = '21476b89-41b1-e311-9351-6c3be5be9f98')", TwoContacts, TestName = "Should be able to chain mutiple filter groups in parenthesis using an OR conjunction")]
        [TestCase("INNER", "C.firstname = 'Albert' AND (C.lastname = 'Einstein' AND C.contactid = '21476b89-41b1-e311-9351-6c3be5be9f98')", OneContact, TestName = "Should be able to chain an AND conjunction with a nested filter group containing an AND conjunction")]
        [TestCase("INNER", "C.firstname = 'Albert' OR (C.lastname = 'Planck' AND C.firstname = 'Max')", TwoContacts, TestName = "Should be able to chain an OR conjunction with a nested filter group containing an AND conunction")]
        [TestCase("INNER", "(C.firstname = 'Albert' AND C.lastname = 'Einstein') OR C.contactid = '5f90afbb-41b1-e311-9351-6c3be5be9f98'", TwoContacts, TestName = "Should be able to chain multiple AND conjunctions then a single OR conjunction")]
        [TestCase("INNER", "C.firstname = 'Albert' OR C.lastname = 'Planck' OR C.lastname = 'Galilei'", ThreeContacts, TestName = "Should be able to chain OR conjunctions")]
        [TestCase("INNER", "C.firstname = 'Albert' AND C.lastname = 'Einstein' AND C.contactid = '21476b89-41b1-e311-9351-6c3be5be9f98'", OneContact, TestName = "Should be able to chain AND conjunctions")]
        [TestCase("INNER", "C.firstname = 'Albert' AND C.lastname = 'Einstein'", OneContact, TestName = "Should be able to use a single AND conjunction")]
        [TestCase("INNER", "C.firstname = 'Albert' OR C.firstname = 'Max'", TwoContacts, TestName = "Should be able to use a single OR conjunction")]
        [Test]
        public void Should_Be_Able_To_Query_Using_Filter_Groups(String joinType, string whereClause, int expectedResults)
        {
            // var join = JoinOperator.Natural;

            //switch (joinType)
            //{
            //    case "INNER":
            //        join = JoinOperator.Inner;
            //        //  Enum.Parse(typeof(JoinOperator), joinType)
            //        break;
            //    case "LEFT":
            //        join = JoinOperator.LeftOuter;
            //        break;
            //}


            var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.line1, A.customeraddressid From contact C {0} JOIN customeraddress A on C.contactid = A.parentid WHERE {1}", joinType, whereClause);

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
                        var customerAddressId = (Guid)reader["A.customeraddressid"];
                        Console.WriteLine(string.Format("{0} {1} {2} {3} {4}", contactId, firstName, lastName, line1, customerAddressId));
                    }
                    Console.WriteLine("There were " + resultCount + " results..");
                    Assert.That(resultCount, Is.EqualTo(expectedResults));
                }
            }


        }



        [Test]
        [TestCase("dbo", TestName = "Should be able to use a schema prefix in the from clause, without effecting the query")]
        [TestCase("abc", TestName = "Should be able to use a schema prefix in the from clause, without effecting the query")]
        public void Should_Be_Able_To_Select_From_Table_Prefixed_With_Dbo_Schema(String schemaPrefix)
        {

            var sql = string.Format("Select C.contactid, C.firstname, C.lastname From {0}.contact C", schemaPrefix);

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
                        Console.WriteLine(string.Format("{0} {1} {2}", contactId, firstName, lastName));
                    }
                    Console.WriteLine("There were " + resultCount + " results..");
                }
            }


        }

        [TestFixtureTearDown]
        public void CleanUp()
        {
            var sqlFormatString = "DELETE FROM CONTACT WHERE contactid = '{0}'";
            var deleteAlbertEinstenSql = string.Format(sqlFormatString, Guid.Parse("21476b89-41b1-e311-9351-6c3be5be9f98"));

            try
            {
                ExecuteNonQuery(deleteAlbertEinstenSql, 1);
            }
            catch (Exception e)
            {
                // throw;
                Console.Write(e.Message);
            }

            var deleteMaxPlanck = string.Format(sqlFormatString, Guid.Parse("5f90afbb-41b1-e311-9351-6c3be5be9f98"));
            try
            {
                ExecuteNonQuery(deleteMaxPlanck, 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var deleteGalileo = string.Format(sqlFormatString, Guid.Parse("6f90afbb-51b1-e311-9351-6c3ce5be9f93"));
            try
            {
                ExecuteNonQuery(deleteGalileo, 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


    }
}