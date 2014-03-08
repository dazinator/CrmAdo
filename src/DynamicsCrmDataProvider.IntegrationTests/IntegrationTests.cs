using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client;
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
        [TestCase("=", "Donald", "{0} {1} '{2}'", TestName = "Should Support Equals a String Constant")]
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
        [TestCase("NOT LIKE", "%onal%", "{0} {1} '{2}'", TestName = "Should Support Does Not Contain")]
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


        [Test]
        [TestCase(TestName = "Experiment for contains and not contains")]
        public void Experiment_For_Contains_And_Not_Contains()
        {
            var sql = string.Format("Select contactid, firstname, lastname From contact Where firstname Like '%ax%' ");

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
        public void Experiment_For_Filter_Groups_All_At_Top_Level()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");


            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                       new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {
              
                var query = new QueryExpression("contact");
                query.ColumnSet.AddColumn("firstname");
                query.ColumnSet.AddColumn("lastname");

                // so link in customer address.
                query.AddLink("customeraddress", "contactid", "parentid", JoinOperator.Inner);
                var addressLink = query.LinkEntities[0];
                addressLink.EntityAlias = "A";
                addressLink.IncludeAllColumns();
                
                // conditions for max planck
                var firstName1Condition = new ConditionExpression("firstname", ConditionOperator.Equal, "Max");
                var lastname1Condition = new ConditionExpression("lastname", ConditionOperator.Equal, "Planck");

                // Groups those conditions using an "AND" conjunction.
                var maxPlankFilter = new FilterExpression(LogicalOperator.And);
                maxPlankFilter.AddCondition(firstName1Condition);
                maxPlankFilter.AddCondition(lastname1Condition);
                
                // conditions for albert einstein
                var firstname2Condition = new ConditionExpression("firstname", ConditionOperator.Equal, "Albert");
                var lastname2Condition = new ConditionExpression("lastname", ConditionOperator.Equal, "Einstein");

                // Groups those conditions using an "AND" conjunction.
                var albertEinsteinFilter = new FilterExpression(LogicalOperator.And);
                albertEinsteinFilter.AddCondition(firstname2Condition);
                albertEinsteinFilter.AddCondition(lastname2Condition);

                // could optionally chain the 2 filters so we get Albert's contitions chained (using AND) to max's conditions 
                //  albertEinsteinFilter.AddFilter(maxPlankFilter);

                // conditions for address line 1 moonbase
                var addressLine1Filter = new FilterExpression(LogicalOperator.And); // dictates that this filter is chained to 
                var line1Condition = new ConditionExpression("A", "line1", ConditionOperator.Equal, "The secret moonbase");
                addressLine1Filter.AddCondition(line1Condition);

              
                // add filters to query 
                // ensures each filter that we add to our queries criteria is chained together using an OR.
                query.Criteria.FilterOperator = LogicalOperator.Or;
                query.Criteria.AddFilter(albertEinsteinFilter);
                query.Criteria.AddFilter(maxPlankFilter);
                query.Criteria.AddFilter(addressLine1Filter);

                var results = orgService.RetrieveMultiple(query);
                int resultCount = 0;
                foreach (var r in results.Entities)
                {
                    resultCount++;
                    Console.WriteLine(string.Format("{0} {1}", (string)r["firstname"], (string)r["lastname"]));
                }
                Console.WriteLine("There were " + resultCount + " results..");


            }


        }

        [Test]
        public void Experiment_For_Filter_Groups_With_Linq_Conversion()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");


            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                       new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService() as CrmOrganizationServiceContext;
            using (orgService as IDisposable)
            {

                var contactsQuery = from c in orgService.CreateQuery("contact")
                                    join a in orgService.CreateQuery("customeraddress") on (Guid)c["contactid"] equals
                                        (Guid)a["parentid"]
                                    where (((string)c["firstname"] == "Max" && (string)c["lastname"] == "Planck")
                                    || ((string)c["firstname"] == "Albert" && (string)c["lastname"] == "Einstein"))
                                    || (string)a["line1"] == "Line2"

                               select c;


                IQueryProvider queryProvider = contactsQuery.Provider;
                
                MethodInfo translateMethodInfo = queryProvider.GetType().GetMethod("Translate");
                QueryExpression query = (QueryExpression)translateMethodInfo.Invoke(queryProvider, new object[] { contactsQuery.Expression });

                QueryExpressionToFetchXmlRequest reqConvertToFetchXml = new QueryExpressionToFetchXmlRequest { Query = query };
                QueryExpressionToFetchXmlResponse respConvertToFetchXml = (QueryExpressionToFetchXmlResponse)orgService.Execute(reqConvertToFetchXml);

                System.Diagnostics.Debug.Print(respConvertToFetchXml.FetchXml);
                

                var results = contactsQuery.ToList();
                int resultCount = 0;
                foreach (var r in results)
                {
                    resultCount++;
                   // Console.WriteLine(string.Format("{0} {1} {2}", (string)r["firstname"], (string)r["lastname"], (string)r["line1"]));
                    Console.WriteLine(string.Format("{0} {1}", (string)r["firstname"], (string)r["lastname"]));
                }
                Console.WriteLine("There were " + resultCount + " results..");


            }


        }


        [Test]
        [TestCase(TestName = "Experiment for filter groups")]
        public void Experiment_For_Filter_Groups()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");


            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                       new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {

                // var request = new RetrieveMultipleRequest();
                var query = new QueryExpression("contact");
                // request.Query = query;
                query.ColumnSet.AddColumn("firstname");
                query.ColumnSet.AddColumn("lastname");
                var condition1 = new ConditionExpression("firstname", ConditionOperator.Equal, "Max");
                var condition2 = new ConditionExpression("lastname", ConditionOperator.Equal, "Planck");
                var filter1 = new FilterExpression(LogicalOperator.And);
                filter1.AddCondition(condition1);
                filter1.AddCondition(condition2);

                var condition3 = new ConditionExpression("firstname", ConditionOperator.Equal, "Albert");
                var filter2 = new FilterExpression(LogicalOperator.Or);
                filter2.AddCondition(condition3);
                filter2.AddFilter(filter1);

                query.Criteria.Filters.Clear();
                query.Criteria.AddFilter(filter2);

                var results = orgService.RetrieveMultiple(query);
                int resultCount = 0;
                foreach (var r in results.Entities)
                {
                    resultCount++;
                    Console.WriteLine(string.Format("{0} {1}", (string)r["firstname"], (string)r["lastname"]));
                }
                Console.WriteLine("There were " + resultCount + " results..");


            }


        }


    }
}