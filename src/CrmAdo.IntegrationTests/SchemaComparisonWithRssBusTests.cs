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
using System.Data.CData.DynamicsCRM;

namespace CrmAdo.IntegrationTests
{
    [Category("Experimental")]
    [TestFixture()]
    public class SchemaComparisonWithRssBusTests : BaseTest
    {
      

        public string TestEntityName { get; set; }
        public string TestEntityName2 { get; set; }



        //public override void SetUp()
        //{
        //    try
        //    {
        //        base.SetUp();

        //        // Start a Local DB instance.
        //        _SqlLocalDbInstance = _SqlLocalDbProvider.GetOrCreateInstance("SchemaTesting");
        //        _SqlLocalDbInstance.Start();

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //        throw;
        //    }

        //}

        private void PrepareTestData()
        {
            
        }           
          

         

        [Test]
        public void WriteSchemaFilesForComparison_RssBus()
        {

            PrepareTestData();

            var sut = new SchemaCollectionsProvider();          

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            var builder = GetStringBuilder();

            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                // Output common schema collections (https://msdn.microsoft.com/en-us/library/ms254501(v=vs.110).aspx)


                // and also sql local db sql server schema collections (https://msdn.microsoft.com/en-us/library/ms254501(v=vs.110).aspx)
                var rssBus = ConfigurationManager.ConnectionStrings["RssBus"];
                using (DynamicsCRMConnection connection = new DynamicsCRMConnection(rssBus.ConnectionString))
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

               // _SqlLocalDbInstance.Stop();


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
                                              Select(column => string.Format("{0} ({1})", column.ColumnName, column.DataType.ToString())).
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