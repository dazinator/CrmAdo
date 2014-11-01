using System;
using NUnit.Framework;
using System.Configuration;

namespace CrmAdo.DdexProvider.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public abstract class BaseTest
    {

        public const string DefaultPublisherPrefix = "crmado_";
        private Guid PublisherId = Guid.Empty;

        [TestFixtureSetUp]
        public virtual void SetUp()
        {
          //  CreatePublisher("crmado", "crmado");
        }

        /// <summary>
        /// Executes the Sql using ExecuteNonQuery, and makes an assertion against the reported number of rows effected.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="assertResult"></param>
        protected virtual void ExecuteNonQuery(string sql, int assertResult)
        {

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing sql command: " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;

                var result = command.ExecuteNonQuery();

                Assert.That(result, Is.EqualTo(assertResult));

            }
        }

        /// <summary>
        /// Executes the SQL using ExecuteReader and makes an assertion against the number of rows returned.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="assertResultCount"></param>
        protected virtual void ExecuteReader(string sql, int assertResultCount)
        {

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing sql command: " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;

                using (var reader = command.ExecuteReader())
                {
                    int resultCount = 0;
                    foreach (var result in reader)
                    {
                        resultCount++;
                        //   var contactId = (Guid)reader["contactid"];
                        // Console.WriteLine(string.Format("{0}", contactId));
                    }
                    Assert.That(resultCount, Is.EqualTo(assertResultCount));
                }
            }
        }

        /// <summary>
        /// Creates a publisher in Crm if it doesn't allready exist.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prefix"></param>
        protected virtual void CreatePublisher(string name, string prefix)
        {
            var sqlFormatString = @"INSERT INTO Publisher (UniqueName,FriendlyName,SupportingWebsiteUrl,CustomizationPrefix,EMailAddress,Description) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}');";
            var sql = string.Format(sqlFormatString, name, name, @"http://dazinator.github.io/CrmAdo/", prefix, "darrell.tunnell@googlemail.com", "crmado int test publisher");
            var existsSqlFormatString = "SELECT PublisherId FROM Publisher WHERE UniqueName = '{0}'";
            var existsSql = string.Format(existsSqlFormatString, name);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();

                var existingPublisherCommand = conn.CreateCommand();
                existingPublisherCommand.CommandText = existsSql;

                using (var publisherReader = existingPublisherCommand.ExecuteReader())
                {
                    if (publisherReader.HasRows)
                    {
                        Console.WriteLine("Publisher with unique name: " + name + " exists.");
                        publisherReader.Read();
                        PublisherId = (Guid)publisherReader["publisherid"];
                        return;
                    }
                }

                // Create publiseher.
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
                        PublisherId = (Guid)reader["publisherid"];
                        Console.WriteLine(string.Format("Publisher created, id: {0}", PublisherId));
                    }
                }
            }

        }

        /// <summary>
        /// Deetes a publisher from Crm.
        /// </summary>
        /// <param name="publisherId"></param>
        protected virtual void DeletedPublisher(Guid publisherId)
        {
            var deleteSqlFormatString = "DELETE FROM Publisher WHERE PublisherId = '{0}'";
            var deleteSql = string.Format(deleteSqlFormatString, publisherId);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();

                // Create publiseher.
                var command = conn.CreateCommand();
                Console.WriteLine("Executing command " + deleteSql);
                command.CommandText = deleteSql;

                var result = command.ExecuteNonQuery();
                if (result != -1)
                {
                    Console.WriteLine("Deleted publisher id: " + publisherId);
                }
            }

        }

        /// <summary>
        /// Creates a new entity for testing with in Crm and returns its name.
        /// </summary>
        /// <param name="sqlFormatString"></param>
        protected string CreateTestEntity()
        {
            // create a random name for the entity. We use half a guid because names cant be too long.
            string randomEntityName = "createtest" + Guid.NewGuid().ToString().Replace("-", "").Remove(0, 16);
            string createEntitySqlFormatString = "CREATE TABLE {0}{1}({0}{1}id UNIQUEIDENTIFIER PRIMARY KEY, {0}{1}name NVARCHAR(125))";
            var sql = string.Format(createEntitySqlFormatString, DefaultPublisherPrefix, randomEntityName);
            Console.WriteLine(sql);

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();
                var command = conn.CreateCommand();

                Console.WriteLine("Executing command " + sql);
                command.CommandText = sql;
                //   command.CommandType = CommandType.Text;
                ExecuteNonQuery(sql, -1);
            }

            string entityName = string.Format("{0}{1}", DefaultPublisherPrefix, randomEntityName);
            return entityName;
        }

        [TestFixtureSetUp]
        public virtual void TearDown()
        {
            ////if (PublisherId != Guid.Empty)
            ////{
            ////    DeletedPublisher(PublisherId);
            ////}
        }
    }
}