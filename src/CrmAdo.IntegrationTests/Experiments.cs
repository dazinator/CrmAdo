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
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Metadata;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace CrmAdo.IntegrationTests
{
    [TestFixture()]
    [Category("Experimental")]
    public class Experiments : BaseTest
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
        [TestCase(TestName = "Experiment for selecting highest version number")]
        public void Experiment_For_Selecting_Highest_Version_Number()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");
            var sql = string.Format("SELECT TOP 10 contactid, firstname, lastname, versionnumber FROM contact ORDER BY versionnumber DESC");

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
                        var versionNumber = (long)reader[3];
                        Console.WriteLine(string.Format("{0} {1} {2} {3}", contactId, firstName, lastName, versionNumber.ToString()));
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
        [TestCase(TestName = "Experiment for selecting version number greater than")]
        public void Experiment_For_Selecting_Version_Number_Greater_Than()
        {
            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");
            var sql = string.Format("SELECT TOP 10 contactid, firstname, lastname, versionnumber FROM contact WHERE versionnumber NOT NULL ORDER BY versionnumber DESC");

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
                        var versionNumber = (long)reader[3];
                        Console.WriteLine(string.Format("{0} {1} {2} {3}", contactId, firstName, lastName, versionNumber.ToString()));
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
        [TestCase(TestName = "Experiment for creating publisher")]
        public void Experiment_For_Creating_Publisher()
        {

            ////Define a new publisher
            //Publisher _crmSdkPublisher = new Publisher
            //{
            //    UniqueName = "sdksamples",
            //    FriendlyName = "Microsoft CRM SDK Samples",
            //    SupportingWebsiteUrl = "http://msdn.microsoft.com/en-us/dynamics/crm/default.aspx",
            //    CustomizationPrefix = "sample",
            //    EMailAddress = "someone@microsoft.com",
            //    Description = "This publisher was created with samples from the Microsoft Dynamics CRM SDK"

            //};


            // var sql = string.Format("Select C.firstname, C.lastname From contact Where firstname Like '%ax%' ");
            var sqlFormatString = @"INSERT INTO Publisher (UniqueName,FriendlyName,SupportingWebsiteUrl,CustomizationPrefix,EMailAddress,Description) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}');";
            var sql = string.Format(sqlFormatString, "CrmAdo", "Crm Ado", @"http://dazinator.github.io/CrmAdo/", "crmado", "darrell.tunnell@googlemail.com", "crm ado publisher");


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
                        var publisherid = (Guid)reader["publisherid"];
                        // var versionNumber = (long)reader[3];
                        Console.WriteLine(string.Format("{0}", publisherid));
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
        [TestCase(TestName = "Experiment for selecting entity metadata")]
        public void Experiment_For_Selecting_Entity_Metadata()
        {


            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                         new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {
                MetadataFilterExpression entityFilter = new MetadataFilterExpression(LogicalOperator.And);
                entityFilter.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, "contact"));


                var relationShipQuery = new RelationshipQueryExpression();
                MetadataFilterExpression relationShipFilter = new MetadataFilterExpression(LogicalOperator.And);
                relationShipFilter.Conditions.Add(new MetadataConditionExpression("RelationshipType", MetadataConditionOperator.Equals, RelationshipType.OneToManyRelationship));
                relationShipQuery.Criteria = relationShipFilter;

                EntityQueryExpression entityQueryExpression = new EntityQueryExpression()
                {
                    Criteria = entityFilter,
                    Properties = new MetadataPropertiesExpression() { AllProperties = true }
                    ,
                    RelationshipQuery = relationShipQuery
                };
                RetrieveMetadataChangesRequest retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
                {
                    Query = entityQueryExpression,
                    ClientVersionStamp = null
                };
                RetrieveMetadataChangesResponse response = (RetrieveMetadataChangesResponse)orgService.Execute(retrieveMetadataChangesRequest);
            }



        }


        [Category("Experimentation")]
        [Test]
        [TestCase("customeraddress", TestName = "Experiment for saving address entity metadata to a local file.")]
        [TestCase("account", TestName = "Experiment for saving account metadata to a local file.")]
        [TestCase("pluginassembly", TestName = "Experiment for saving pluginassembly metadata to a local file.")]
        [TestCase("plugintype", TestName = "Experiment for saving plugintype metadata to a local file.")]
        [TestCase("sdkmessageprocessingstep", TestName = "Experiment for saving sdkmessageprocessingstep metadata to a local file.")]
        [TestCase("sdkmessageprocessingstepimage", TestName = "Experiment for saving sdkmessageprocessingstepimage metadata to a local file.")]
        [TestCase("sdkmessageprocessingstepsecureconfig", TestName = "Experiment for saving sdkmessageprocessingstepsecureconfig metadata to a local file.")]
        public void Experiment_For_Saving_Entity_Metadata_To_File(string entityName)
        {


            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                         new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {
                MetadataFilterExpression entityFilter = new MetadataFilterExpression(LogicalOperator.And);
                entityFilter.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, entityName));




                EntityQueryExpression entityQueryExpression = new EntityQueryExpression()
                {
                    Criteria = entityFilter,
                    Properties = new MetadataPropertiesExpression() { AllProperties = true }
                };
                RetrieveMetadataChangesRequest retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
                {
                    Query = entityQueryExpression,
                    ClientVersionStamp = null
                };
                RetrieveMetadataChangesResponse response = (RetrieveMetadataChangesResponse)orgService.Execute(retrieveMetadataChangesRequest);
                var entityMetadata = response.EntityMetadata[0];



                var path = Environment.CurrentDirectory;
                var shortFileName = entityName + "Metadata.xml";


                var fileName = System.IO.Path.Combine(path, shortFileName);
                var serialised = EntityMetadataUtils.SerializeMetaData(entityMetadata, System.Xml.Formatting.Indented);
                using (var writer = new System.IO.StreamWriter(fileName))
                {
                    writer.Write(serialised);
                    writer.Flush();
                    writer.Close();
                }

                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException("Could not save metadata file for entity " + entityName);
                }
            }



        }


        [Category("Experimentation")]
        [Test]
        [TestCase(TestName = "Experiment for selecting plugins")]
        public void Experiment_For_Selecting_Plugins()
        {
            var sql = string.Format("Select * from pluginassembly");

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

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write(reader.GetName(i));
                            Console.Write("=");
                            Console.Write(reader[i]);
                            Console.WriteLine();
                        }

                        Console.WriteLine("*****");

                    }

                    Console.WriteLine("There were " + resultCount + " results..");
                }
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


        [Category("Experimentation")]
        [Test]
        [TestCase(TestName = "Experiment for retrieving crm version")]
        public void Experiment_For_Crm_Version_Request()
        {

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var serviceProvider = new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString.ConnectionString },
                                                         new CrmClientCredentialsProvider());

            var orgService = serviceProvider.GetOrganisationService();
            using (orgService as IDisposable)
            {
                var req = new RetrieveVersionRequest();
                var resp = (RetrieveVersionResponse)orgService.Execute(req);
                //assigns the version to a string
                string versionNumber = resp.Version;
                Console.WriteLine(versionNumber);
            }

        }

        [Category("Experimentation")]
        [Test]
        [TestCase(TestName = "Experiment for sql connection get datatyoes schema collection")]
        public void Experiment_For_Sql_Connection_Schema()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["SqlConnection"];

            using (var conn = new SqlConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var dataTypes = conn.GetSchema("DataTypes");
                dataTypes.WriteXml("DataTypes.xml", XmlWriteMode.WriteSchema);

                var resrictions = conn.GetSchema("Restrictions");
                resrictions.WriteXml("Restrictions.xml", XmlWriteMode.WriteSchema);

                var collections = conn.GetSchema("MetaDataCollections");
                collections.WriteXml("MetaDataCollections.xml", XmlWriteMode.WriteSchema);

                var columns = conn.GetSchema("Columns", new string[] { null, null, "Table", null });
                columns.WriteXml("Columns.xml", XmlWriteMode.WriteSchema);

                conn.Close();
            }

        }





    }
}