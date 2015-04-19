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
    public class InsertIntegrationTests : BaseTest
    {
        private List<Guid> _NewContactIds;

        [SetUp]
        public void Setup()
        {
            _NewContactIds = new List<Guid>();
        }

        //[TearDown]
        //public void TearDown()
        //{
        //    // remove the created contacts from dynamics..

        //}

        // NOTE: THESE TESTS REQUIRE A CONNECTION STRING TO BE SET IN THE CONFIG FILE, WITH A NAME OF 'CrmOrganisation'
        // ============================================================================================================

        [Category("Insert Statement")]
        [Test(Description = "Integration tests that inserts a new contact into Dynamics CRM contacts.")]
        [TestCase("INSERT INTO contact (firstname, lastname) Values('{0}','{1}')")]        
        public void Should_Be_Able_To_Insert_A_Contact(string insertSql)
        {

            var sql = string.Format(insertSql, "Derren", "Brown");
            Console.WriteLine(sql);
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
                    Console.WriteLine("There were " + resultCount + " results..");
                }
            }

        }


        [Category("Insert Statement")]
        [Test(Description = "Integration tests that inserts a new contact into Dynamics CRM contacts.")]
        [TestCase("INSERT INTO [contact] ([contactid],[fullname]) VALUES (@0,@1)", "Gödel")]
        public void Should_Be_Able_To_Insert_A_Contact_With_IndexedParameters(string insertSql, string name)
        {           

         //   var sql = string.Format(insertSql, "Derren", "Brown");
            Console.WriteLine(insertSql);
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + insertSql);
                command.CommandText = insertSql;
               
                Guid id = Guid.NewGuid();
                var param =  command.CreateParameter();
                param.ParameterName = "@0";
                param.DbType = System.Data.DbType.String;
                param.Direction = System.Data.ParameterDirection.Input;
                param.Size = 40;
                param.Value = id;
                command.Parameters.Add(param);

                var param2 = command.CreateParameter();
                param2.ParameterName = "@1";
                param2.DbType = System.Data.DbType.String;
                param2.Direction = System.Data.ParameterDirection.Input;
                param2.Size = 4000;
                param2.Value = id;
                command.Parameters.Add(param2);

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
                    Console.WriteLine("There were " + resultCount + " results..");
                }
            }

        }
        

        [Category("Insert Statement")]
        [Test(Description = "Integration tests that inserts a new contact into Dynamics CRM contacts with an OUTPUT clause to get the created on date of the inserted record.")]
        [TestCase("INSERT INTO [contact] ([contactid],[fullname]) OUTPUT INSERTED.createdon VALUES (@0,@1)", "Curie")]
        public void Should_Be_Able_To_Insert_A_Contact_With_OutputClause(string insertSql, string name)
        {

            //   var sql = string.Format(insertSql, "Derren", "Brown");
            Guid id = Guid.NewGuid();
            Console.WriteLine(insertSql);
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + insertSql);
                command.CommandText = insertSql;

                var param = command.CreateParameter();
                param.ParameterName = "@0";
                param.DbType = System.Data.DbType.String;
                param.Direction = System.Data.ParameterDirection.Input;
                param.Size = 40;
                param.Value = id;
                command.Parameters.Add(param);

                var param2 = command.CreateParameter();
                param2.ParameterName = "@1";
                param2.DbType = System.Data.DbType.String;
                param2.Direction = System.Data.ParameterDirection.Input;
                param2.Size = 4000;
                param2.Value = id;
                command.Parameters.Add(param2);

                //   command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader())
                {
                    int resultCount = 0;
                    foreach (var result in reader)
                    {
                        resultCount++;
                        var createdOn = (DateTime)reader["createdon"];
                        Console.WriteLine(string.Format("{0}", createdOn));
                    }
                    Console.WriteLine("There were " + resultCount + " results..");
                }
            }

        }
        

        [Category("Insert Statement")]
        [Category("Performance")]
        [Test(Description = "Integration tests that inserts 1000 contacts into Dynamics CRM.")]
        public void Should_Be_Able_To_Insert_Many_Contacts()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                // create 10000 contacts in dynamics..
                Console.WriteLine("starting insert..");
                var watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 1000; i++)
                {
                    Guid testdata = Guid.NewGuid();
                    var sql = string.Format("INSERT INTO contact (contactid, firstname, lastname) Values('{0}','{1}','{2}')", testdata, "Derren" + testdata.ToString(), "Brown");

                    var command = conn.CreateCommand();
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                    Assert.That(result, Is.EqualTo(1));
                }
                watch.Stop();
                Console.WriteLine("Inserting 1000 contacts took " + watch.Elapsed.ToString());

            }

        }


        [Category("Select Statement")]
        [Category("Performance")]
        [TestCase(-1, Description = "Can select all contacts")]
        [TestCase(20000, Description = "Can select TOP 20000 contacts")]
        [TestCase(5000, Description = "Can select TOP 5000 contacts")]
        public void Can_Select_Large_Number_Of_Contacts(int total)
        {

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                Console.WriteLine("selecting " + total + " contacts..");
                var watch = new Stopwatch();
                watch.Start();

                string sql;
                if (total != -1)
                {
                    sql = string.Format("select top " + total + " contactid, firstname, lastname from contact");
                }
                else
                {
                    sql = string.Format("select contactid, firstname, lastname from contact");
                }

                var command = conn.CreateCommand();
                command.CommandText = sql;
                using (var reader = command.ExecuteReader())
                {
                    int resultCount = 0;
                    foreach (var result in reader)
                    {
                        resultCount++;
                        var contactId = (Guid)reader["contactid"];
                        Console.WriteLine(contactId);
                    }
                    Console.WriteLine("There were " + resultCount + " results..");

                    if (total != -1)
                    {
                        Assert.That(resultCount, Is.Not.GreaterThan(total));
                    }

                    watch.Stop();
                    Console.WriteLine("selecting " + resultCount + " contacts took " + watch.Elapsed.ToString());
                }

            }

        }

    }
}