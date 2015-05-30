using System;
using System.Data;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.IntegrationTests;
using System.Configuration;
using CrmAdo.Operations;
using CrmAdo.Core;
using System.Text;
using System.Data.Common;
using System.Data.SqlLocalDb;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace CrmAdo.IntegrationTests
{
    [Category("Experimental")]
    [TestFixture()]
    public class SchemaComparisonWithSqlServerTests : BaseTest
    {

        ISqlLocalDbProvider _SqlLocalDbProvider;
        ISqlLocalDbInstance _SqlLocalDbInstance;

        public string TestEntityName { get; set; }

        public SchemaComparisonWithSqlServerTests()
        {
            _SqlLocalDbProvider = new SqlLocalDbProvider();
        }

        public override void SetUp()
        {
            base.SetUp();

            // Start a Local DB instance.
            _SqlLocalDbInstance = _SqlLocalDbProvider.GetOrCreateInstance("SchemaTesting");
            _SqlLocalDbInstance.Start();

            // Create a test entity in CRM
            string createTableSql = GetCreateTestTableSql();

            //  CreateTestEntity();

            // Create table in LocalDB
            using (SqlConnection connection = _SqlLocalDbInstance.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;

                Console.WriteLine("Executing local db command " + createTableSql);
                command.CommandText = createTableSql;

                var result = command.ExecuteNonQuery();
                Assert.AreEqual(result, -1);
            }

            // Create same table in Crm
            ExecuteCrmNonQuery(createTableSql, -1);
        }

        private string GetCreateTestTableSql()
        {
            // Crm only supports creating an entity with default columsn first (id and name) and then subsequent alter to add in additional columns, so do this.

            string randomEntityName = "createtest" + Guid.NewGuid().ToString().Replace("-", "").Remove(0, 16);

            string createEntitySqlFormatString = "CREATE TABLE {0}{1}({0}{1}id UNIQUEIDENTIFIER PRIMARY KEY, {0}{1}name NVARCHAR(125));";
            var createTableSql = string.Format(createEntitySqlFormatString, DefaultPublisherPrefix, randomEntityName);

            var addBoolSql = "ALTER TABLE {0} ADD {1}MyBool BIT;"; // BOOL ATT
            var addBoolDefaultTrueSql = "ALTER TABLE {0} ADD {1}MyBoolTrue BIT DEFAULT 1;"; // BOOL AT WITH DEFAULT TRUE
            var addBoolDefuaultFalseSql = "ALTER TABLE {0} ADD {1}MyBoolFalse BIT DEFAULT 0;"; // BOOL ATT WITH DEFAULT FALSE
            var addDateTimeSql = "ALTER TABLE {0} ADD {1}MyDateTime DATETIME;"; // DATETIME ATT
            var addDateSql = "ALTER TABLE {0} ADD {1}MyDate DATE;"; // DATE ATT
            var addDecimalSql = "ALTER TABLE {0} ADD {1}MyDecimal DECIMAL;"; // DECIMAL
            var addDecimalWithPrecisionSql = "ALTER TABLE {0} ADD {1}MyDecimalP DECIMAL(12);"; // DECIMAL WITH PRECISION
            var addDecimaplWithPrecisionAndScaleSql = "ALTER TABLE {0} ADD {1}MyDecimalPS DECIMAL(12,4);"; // DECIMAL WITH PRECISION AND SCALE
            var addDoubleSql = "ALTER TABLE {0} ADD {1}MyDouble FLOAT;"; // DOUBLE
            var addDoubleWithPrecisionSql = "ALTER TABLE {0} ADD {1}MyDoubleP FLOAT(12);"; // DOUBLE WITH PRECISION
            //   var addDoubleWithPrecisionAndScaleSql = "ALTER TABLE {0} ADD {1}MyDoublePS FLOAT(12,5);"; // DOUBLE WITH PRECISION AND SCALE
            var addIntSql = "ALTER TABLE {0} ADD {1}MyInt Int;"; // INT
            //  var addLookupSql = "ALTER TABLE {0} ADD {1}MyFirstContactId UNIQUEIDENTIFIER REFERENCES {2};"; // LOOKUP ATT (GUID)
            //  var addLookupWithNamedConstraintSql = "ALTER TABLE {0} ADD {1}MySecondContactId UNIQUEIDENTIFIER CONSTRAINT {0}_{2} REFERENCES {2};"; // LOOKUP ATT (GUID) WITH NAMED CONSTRAINT
            var addMemoSql = "ALTER TABLE {0} ADD {1}MyMemo NVARCHAR(MAX);"; // MEMO ATT
            var addMoneySql = "ALTER TABLE {0} ADD {1}MyMoney MONEY;"; // MONEY ATT
            // var addMoneyWithPrecisionSql = "ALTER TABLE {0} ADD {1}MyMoneyP MONEY(4);"; // MONEY ATT WITH PRECISION
            // var addMoneyWithPrecisionAndScaleSql = "ALTER TABLE {0} ADD {1}MyMoneyPS MONEY(4,2);"; // MONEY ATT WITH PRECISION AND SCALE
            var addStringSql = "ALTER TABLE {0} ADD {1}MyString NVARCHAR;"; // STRING ATTRIBUTE
            var addStringWithMaxLengthSql = "ALTER TABLE {0} ADD {1}MyStringMaxSize NVARCHAR(300);"; // STRING WITH MAX SIZE

            string lookupToEntity = "contact";
            string testEntityName = string.Format("{0}{1}", DefaultPublisherPrefix, randomEntityName);
            TestEntityName = testEntityName;

            var createTableWithColumnsSqlBuilder = new StringBuilder();
            createTableWithColumnsSqlBuilder.Append(createTableSql);
            createTableWithColumnsSqlBuilder.AppendFormat(addBoolSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addBoolDefaultTrueSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addBoolDefuaultFalseSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addDateTimeSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addDateSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addDecimalSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addDecimalWithPrecisionSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addDecimaplWithPrecisionAndScaleSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addDoubleSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addDoubleWithPrecisionSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            // createTableWithColumnsSqlBuilder.AppendFormat(addDoubleWithPrecisionAndScaleSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addIntSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            //    createTableWithColumnsSqlBuilder.AppendFormat(addLookupSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            //    createTableWithColumnsSqlBuilder.AppendFormat(addLookupWithNamedConstraintSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addMemoSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addMoneySql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            // createTableWithColumnsSqlBuilder.AppendFormat(addMoneyWithPrecisionSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            // createTableWithColumnsSqlBuilder.AppendFormat(addMoneyWithPrecisionAndScaleSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addStringSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);
            createTableWithColumnsSqlBuilder.AppendFormat(addStringWithMaxLengthSql, testEntityName, DefaultPublisherPrefix, lookupToEntity);

            return createTableWithColumnsSqlBuilder.ToString();

        }

        private string GetDropTestTableSql()
        {

            string dropTable = "DROP TABLE {0}";
            var dropTableSql = string.Format(dropTable, TestEntityName);
            return dropTableSql;

        }

        public override void TearDown()
        {
            // Drop test entity.
            using (SqlConnection connection = _SqlLocalDbInstance.CreateConnection())
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                var sql = GetDropTestTableSql();
                Console.WriteLine("Executing local db command " + sql);
                command.CommandText = sql;
                var result = command.ExecuteNonQuery();
                Assert.AreEqual(result, -1);
            }

            // Stop localdb instance.
            _SqlLocalDbInstance.Stop();

            base.TearDown();
        }

        [Test]
        public void Write_Schema_Files_For_Comparison()
        {
            var sut = new SchemaCollectionsProvider();

            _SqlLocalDbInstance = _SqlLocalDbProvider.GetOrCreateInstance("SchemaTesting");
            _SqlLocalDbInstance.Start();

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var builder = GetStringBuilder();

            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                // Output common schema collections (https://msdn.microsoft.com/en-us/library/ms254501(v=vs.110).aspx)


                // and also sql local db sql server schema collections (https://msdn.microsoft.com/en-us/library/ms254501(v=vs.110).aspx)
                using (SqlConnection connection = _SqlLocalDbInstance.CreateConnection())
                {
                    connection.Open();

                    // for each connection, we are writing its schema collection to CSV format for easy comparison.
                    // We are writing sql server first, followed by crmado's.

                    WriteDataTableToCsv("Sql MetaDataCollections", builder, connection.GetSchema("MetaDataCollections"));
                    WriteDataTableToCsv("CrmAdo MetaDataCollections", builder, sut.GetMetadataCollections());

                    WriteDataTableToCsv("Sql DataSourceInformation", builder, connection.GetSchema("DataSourceInformation"));
                    WriteDataTableToCsv("CrmAdo DataSourceInformation", builder, sut.GetDataSourceInfo(conn));

                    WriteDataTableToCsv("Sql DataTypes", builder, connection.GetSchema("DataTypes"));
                    WriteDataTableToCsv("CrmAdo DataTypes", builder, sut.GetDataTypes());

                    WriteDataTableToCsv("Sql Restrictions", builder, connection.GetSchema("Restrictions"));
                    WriteDataTableToCsv("CrmAdo Restrictions", builder, sut.GetRestrictions());

                    WriteDataTableToCsv("Sql ReservedWords", builder, connection.GetSchema("ReservedWords"));
                    WriteDataTableToCsv("CrmAdo ReservedWords", builder, sut.GetReservedWords());

                    WriteDataTableToCsv("Sql Tables", builder, connection.GetSchema("Tables"));
                    WriteDataTableToCsv("CrmAdo Tables", builder, sut.GetTables(conn, null));

                    WriteDataTableToCsv("Sql Columns", builder, connection.GetSchema("Columns"));
                    WriteDataTableToCsv("CrmAdo Columns", builder, sut.GetColumns(conn, null));

                    WriteDataTableToCsv("Sql Views", builder, connection.GetSchema("Views"));
                    WriteDataTableToCsv("CrmAdo Views", builder, sut.GetViews(conn, null));

                    WriteDataTableToCsv("Sql ViewColumns", builder, connection.GetSchema("ViewColumns"));
                    WriteDataTableToCsv("CrmAdo View Columns", builder, sut.GetViewColumns(conn, null));

                    WriteDataTableToCsv("Sql Indexes", builder, connection.GetSchema("Indexes"));
                    WriteDataTableToCsv("CrmAdo Indexes", builder, sut.GetIndexes(conn, null));

                    WriteDataTableToCsv("Sql IndexColumns", builder, connection.GetSchema("IndexColumns"));
                    WriteDataTableToCsv("CrmAdo IndexColumns", builder, sut.GetIndexColumns(conn, null));

                    WriteDataTableToCsv("Sql ForeignKeys", builder, connection.GetSchema("ForeignKeys"));
                    WriteDataTableToCsv("CrmAdo ForeignKeys", builder, sut.GetForeignKeys(conn, null));

                    WriteDataTableToCsv("Sql Users", builder, connection.GetSchema("Users"));
                    WriteDataTableToCsv("CrmAdo Users", builder, sut.GetUsers(conn, null));

                    WriteDataTableToCsv("Sql Databases", builder, connection.GetSchema("Databases"));
                    WriteDataTableToCsv("CrmAdo Databases", builder, sut.GetDatabases(conn, null));
                }

                _SqlLocalDbInstance.Stop();


            }

            // save the csv file to disk

            var path = System.IO.Path.Combine(Environment.CurrentDirectory, "schema comparison report" + ".csv");
            System.IO.File.WriteAllText(path, builder.ToString());
            Console.WriteLine("comparison report written to: " + path);


        }

        private void WriteDataTableToCsv(string dataTableName, StringBuilder builder, DataTable dataTable)
        {
            builder.AppendLine(dataTableName);
            var output = DumpDataTableToCsv(builder, dataTable);
            builder.AppendLine();
        }
               
        public static StringBuilder GetStringBuilder()
        {
            StringBuilder sb = new StringBuilder();
            return sb;
        }      

        /// <summary>
        /// Dumps the passed DataSet obj for debugging as list of html tables
        /// </summary>
        /// <param name="msg"> the msg attached </param>
        /// <param name="ds"> the DataSet object passed for Dumping </param>
        /// <returns> the nice looking dump of the DataSet obj in html format</returns>
        public static string DumpDataTableToCsv(StringBuilder sb, System.Data.DataTable dt)
        {
            string[] columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => string.Format("{0} ({1})",column.ColumnName, column.DataType.ToString())).
                                              ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                  string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();

        }






    }
}