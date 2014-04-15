using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CrmAdo.IntegrationTests
{
    [TestFixture()]
    public class Experiments
    {
        [Category("Experimentation")]
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

        [Category("Experimentation")]
        [Test]
        public void Experiment_For_Filters_1()
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

        [Category("Experimentation")]
        [Test]
        public void Experiment_For_Filters_2_With_Linq_Conversion()
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

        [Category("Experimentation")]
        [Test]
        public void Experiment_For_Filters_3()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");


            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                         new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {

                var query = new QueryExpression("contact");
                query.Distinct = true;
                query.ColumnSet.AddColumns("firstname", "lastname");
                query.Criteria.FilterOperator = LogicalOperator.Or;
                var f = query.Criteria.AddFilter(LogicalOperator.And);
                f.AddCondition("firstname", ConditionOperator.Equal, "Max");
                f.AddCondition("lastname", ConditionOperator.Equal, "Planck");
                f = query.Criteria.AddFilter(LogicalOperator.And);
                f.AddCondition("firstname", ConditionOperator.Equal, "Albert");
                f.AddCondition("lastname", ConditionOperator.Equal, "Einstein");
                var a = query.AddLink("customeraddress", "contactid", "parentid");
                a.LinkCriteria.AddCondition("line1", ConditionOperator.Equal, "The secret moonbase");

                // var response = orgService.RetrieveMultiple(query);



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

        [Category("Experimentation")]
        [Test]
        public void Experiment_For_Filters_4()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");


            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                         new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {

                var query = new QueryExpression("contact");
                query.Distinct = true;
                query.ColumnSet.AddColumns("firstname", "lastname");
                query.Criteria.FilterOperator = LogicalOperator.And;

                var filterGroup = new FilterExpression(LogicalOperator.Or);
                var f = filterGroup.AddFilter(LogicalOperator.And);
                f.AddCondition("firstname", ConditionOperator.Equal, "Max");
                f.AddCondition("lastname", ConditionOperator.Equal, "Planck");
                f = filterGroup.AddFilter(LogicalOperator.And);
                f.AddCondition("firstname", ConditionOperator.Equal, "Albert");
                f.AddCondition("lastname", ConditionOperator.Equal, "Einstein");

                query.Criteria.AddFilter(filterGroup);

                // var response = orgService.RetrieveMultiple(query);



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

        [Category("Experimentation")]
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


        [Category("Experimentation")]
        [Test]
        [TestCase(TestName = "Experiment for min active row version")]
        public void Experiment_For_Min_Active_Row_Version()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");

            var threadCount = 50;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread((a) => DoSomeWork(a));
            }

            // now keep querying for min active row version..
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                         new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {
                // start some accounts being inserted on background thread..
                foreach (Thread thread in threads)
                {
                    thread.Start();
                }

                // Whilst that is happening keep querying min active row version..
                for (int i = 0; i < 100; i++)
                {
                    var accounts = orgService.RetrieveMultiple(new QueryExpression("account") { ColumnSet = new ColumnSet("accountid") });
                    Console.WriteLine("min active is: " + accounts.MinActiveRowVersion);
                }
            }
            // ensure threads all finished.
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

        }

        [Category("Experimentation")]
        [Test]
        [TestCase(TestName = "Experiment for min active row version")]
        public void Experiment_For_Version_Number()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");


            // now keep querying for min active row version..
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                         new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {
                var accounts = orgService.RetrieveMultiple(new QueryExpression("account") { ColumnSet = new ColumnSet("versionnumber","name") });
                foreach (var a in accounts.Entities)
                {
                    // 621ae9e6-3fb1-e311-9caa-d89d6764506c is 369649
                    Console.WriteLine(a.Id + " version number is: " + a["versionnumber"]);
                }

                var id = Guid.Parse("621ae9e6-3fb1-e311-9caa-d89d6764506c");
                var acc = orgService.Retrieve("account", id, new ColumnSet("versionnumber", "name"));

                Console.WriteLine("version number is: " + acc["versionnumber"]);

                acc["name"] = acc["name"] + "a";
                orgService.Update(acc);

                Console.WriteLine("version number immediately following update: " + acc["versionnumber"]);

                acc = orgService.Retrieve("account", id, new ColumnSet("versionnumber", "name"));
                Console.WriteLine("version number requeried after update: " + acc["versionnumber"]);

            }




        }

        private void DoSomeWork(object o)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                         new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {

                var account = new Entity("account");
                account["name"] = "test";
                orgService.Create(account);
            }
        }




    }
}