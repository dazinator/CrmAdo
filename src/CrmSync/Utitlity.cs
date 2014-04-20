using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using CrmAdo;

namespace CrmSync
{
    public class Utility
    {


        // Set the password and connection string for samples with clients 
        // that use SqlCeClientSyncProvider.    
        private static string _clientPassword;

        public static string Password_SqlCeClientSync
        {
            get { return _clientPassword; }
            set { _clientPassword = value; }
        }

        public static void SetPassword_SqlCeClientSync()
        {
            Console.WriteLine("Type a strong password for the client");
            Console.WriteLine("database, and then press Enter.");
            Utility.Password_SqlCeClientSync = Console.ReadLine();
        }

        public static string ConnStr_SqlCeClientSync
        {
            get { return @"Data Source='SyncSampleClient.sdf'; Password=" + Utility.Password_SqlCeClientSync; }
        }

        public static string ConnStr_SqlExpressClientSync
        {
            get { return @"Data Source=PHOTON\SQLEXPRESS;Initial Catalog=TestCrmSync;Integrated Security=True"; }
        }


        public static string ConnStr_DbServerSync
        {
            get { return System.Configuration.ConfigurationManager.ConnectionStrings["CrmOrganisation"].ConnectionString; }
        }



        // ----------  BEGIN CODE RELATED TO SQL SERVER COMPACT --------- //

        public static void DeleteAndRecreateCompactDatabase(string sqlCeConnString, bool recreateDatabase)
        {

            using (SqlCeConnection clientConn = new SqlCeConnection(sqlCeConnString))
            {
                if (File.Exists(clientConn.Database))
                {
                    File.Delete(clientConn.Database);
                }
            }

            if (recreateDatabase == true)
            {
                SqlCeEngine sqlCeEngine = new SqlCeEngine(sqlCeConnString);
                sqlCeEngine.CreateDatabase();
            }

        }

        // ----------  END CODE RELATED TO SQL SERVER COMPACT --------- //




        /* ----------  BEGIN CODE FOR DBSERVERSYNCPROVIDER AND --------- //
           ----------      SQLCECLIENTSYNCPROVIDER SAMPLES     --------- */

        public static void MakeDataChangesOnServer(string tableName)
        {
            int rowCount = 0;

            using (CrmDbConnection serverConn = new CrmDbConnection(Utility.ConnStr_DbServerSync))
            {

                serverConn.Open();
                if (tableName == "contact")
                {
                    // An insert..
                    using (var command = serverConn.CreateCommand())
                    {
                        command.CommandText =
                         "INSERT INTO contact (firstname, lastname) " +
                         "VALUES ('Bill" + Guid.NewGuid().ToString() + "', 'Gates')";
                        rowCount = command.ExecuteNonQuery();
                    }

                    // An update..
                    //using (var command = serverConn.CreateCommand())
                    //{
                    //    command.CommandText =
                    //     "UPDATE contact " +
                    //     "SET  firstname = 'James' " +
                    //     "WHERE firstname = 'Tandem Bicycle Store' "
                    //       var result = command.ExecuteNonQuery();
                    //}
                    // A delete..
                    //using (var command = serverConn.CreateCommand())
                    //{
                    //    command.CommandText =//   
                    //       var result = command.ExecuteNonQuery();
                    //}

                }

                serverConn.Close();
            }

            Console.WriteLine("Rows inserted, updated, or deleted at the server: " + rowCount);
        }


        //Revert changes that were made during synchronization.
        public static void CleanUpServer()
        {
            //using (SqlConnection serverConn = new SqlConnection(Utility.ConnStr_DbServerSync))
            //{
            //    SqlCommand sqlCommand = serverConn.CreateCommand();
            //    sqlCommand.CommandType = CommandType.StoredProcedure;
            //    sqlCommand.CommandText = "usp_InsertSampleData";

            //    serverConn.Open();
            //    sqlCommand.ExecuteNonQuery();
            //    serverConn.Close();
            //}
        }

        //Add DEFAULT constraints on the client.
        public static void MakeSchemaChangesOnClient(IDbConnection clientConn, IDbTransaction clientTran, string tableName)
        {

            //Execute the command over the same connection and within
            //the same transaction that Sync Framework uses
            //to create the schema on the client.
            var alterTable = new SqlCeCommand();
            alterTable.Connection = (SqlCeConnection)clientConn;
            alterTable.Transaction = (SqlCeTransaction)clientTran;
            alterTable.CommandText = String.Empty;

            //Execute the command, then leave the transaction and 
            //connection open. The client provider will commit and close.
            switch (tableName)
            {
                case "contact":
                    alterTable.CommandText =
                        "ALTER TABLE contact " +
                        "ADD CONSTRAINT DF_contactid " +
                        "DEFAULT NEWID() FOR contactid";
                    alterTable.ExecuteNonQuery();
                    break;
                
            }

        }

        public static void MakeDataChangesOnClient(string tableName)
        {
            int rowCount = 0;

            using (SqlCeConnection clientConn = new SqlCeConnection(Utility.ConnStr_SqlCeClientSync))
            {
                
                clientConn.Open();

                if (tableName == "contact")
                {
                    using (var sqlCeCommand = clientConn.CreateCommand())
                    {
                        sqlCeCommand.CommandText =
                      "INSERT INTO contact (firstname, lastname) " +
                      "VALUES ('Carl', 'Sagan') ";
                        rowCount = sqlCeCommand.ExecuteNonQuery();
                    }
                }

                clientConn.Close();
            }

            Console.WriteLine("Rows inserted, updated, or deleted at the client: " + rowCount);
        }

        /* ----------  END CODE FOR DBSERVERSYNCPROVIDER AND   --------- //
           ----------      SQLCECLIENTSYNCPROVIDER SAMPLES     --------- */
    }
}
