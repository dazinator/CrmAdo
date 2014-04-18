using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.Server;

namespace CrmSync
{
    class Program
    {
        static void Main(string[] args)
        {

            //The SampleStats class handles information from the SyncStatistics
            //object that the Synchronize method returns.
            SampleStats sampleStats = new SampleStats();

            //Request a password for the client database, and delete
            //and re-create the database. The client synchronization
            //provider also enables you to create the client database 
            //if it does not exist.
            Utility.SetPassword_SqlCeClientSync();
            Utility.DeleteAndRecreateCompactDatabase(Utility.ConnStr_SqlCeClientSync, true);

            DoTest();


            //Initial synchronization. Instantiate the SyncAgent
            //and call Synchronize.
            SampleSyncAgent sampleSyncAgent = new SampleSyncAgent();
            SyncStatistics syncStatistics = sampleSyncAgent.Synchronize();
            sampleStats.DisplayStats(syncStatistics, "initial");

            //Make changes on the server and client.
            Utility.MakeDataChangesOnServer("contact");
            Utility.MakeDataChangesOnClient("contact");

            //Subsequent synchronization.
            syncStatistics = sampleSyncAgent.Synchronize();
            sampleStats.DisplayStats(syncStatistics, "subsequent");

            //Return server data back to its original state.
            Utility.CleanUpServer();

            //Exit.
            Console.Write("\nPress Enter to close the window.");
            Console.ReadLine();
        }

        private static void DoTest()
        {

            //   var adapter = new Microsoft.Synchronization.Data.Server.SyncAdapter("contact");
            //Create a connection to the sample server database.
            var util = new Utility();
            var serverConn = new CrmDbConnection(Utility.ConnStr_DbServerSync);



            DbCommand selectNewAnchorCommand = new AnchorDbCommandAdapter(serverConn.CreateCommand() as CrmDbCommand);
            var anchorParam = selectNewAnchorCommand.CreateParameter();
            anchorParam.ParameterName = "@" + SyncSession.SyncNewReceivedAnchor;

            var entityName = "contact";
            var sql = "select top 1 versionnumber from " + entityName + " order by versionnumber desc";

            //Create the SyncAdapter.
            var customerSyncAdapter = new TestSyncAdapter("contact");

            //Select inserts from the server.
            // possibly need to extend the entity with a "crmsync_insertedbyclientid" and "crmsync_updatedbyclientid" field to hold sync client ids..

            var customerIncrInserts = serverConn.CreateCommand();
            customerIncrInserts.CommandText =
                "SELECT contactid, firstname, lastname " +
                "FROM contact " +
                "WHERE (versionnumber > @sync_last_received_anchor " +
                "AND versionnumber <= @sync_new_received_anchor " +
                ")";
            //  "AND crmsync_insertedbyclientid <> @sync_client_id)";

            var lastAnchorParam = customerIncrInserts.CreateParameter();
            lastAnchorParam.ParameterName = "@" + SyncSession.SyncLastReceivedAnchor;
            lastAnchorParam.DbType = DbType.Int64;
            customerIncrInserts.Parameters.Add(lastAnchorParam);

            var thisAnchorParam = customerIncrInserts.CreateParameter();
            thisAnchorParam.ParameterName = "@" + SyncSession.SyncNewReceivedAnchor;
            thisAnchorParam.DbType = DbType.Int64;
            customerIncrInserts.Parameters.Add(thisAnchorParam);

            var clientIdParam = customerIncrInserts.CreateParameter();
            clientIdParam.ParameterName = "@" + SyncSession.SyncClientId;
            clientIdParam.DbType = DbType.Guid;
            customerIncrInserts.Parameters.Add(clientIdParam);

            customerIncrInserts.Connection = serverConn;
            customerSyncAdapter.SelectIncrementalInsertsCommand = customerIncrInserts;


            //DT: Might have to adapt command to handle the "SET @sync_row_count = @@rowcount" bit 
            //Apply inserts to the server.
            DbCommand contactInserts = new InsertEntityDbCommandAdapter(serverConn.CreateCommand() as CrmDbCommand);
            //var contactInserts = serverConn.CreateCommand();
            contactInserts.CommandText =
                "INSERT INTO contact (contactid, firstname, lastname) " +
                "VALUES (@contactid, @firstname, @lastname)";

            //"INSERT INTO contact (contactid, firstname, lastname, InsertId, UpdateId) " +
            //  "VALUES (@contactid, @firstname, @lastname, @sync_client_id, @sync_client_id) " +
            //  "SET @sync_row_count = @@rowcount";

            AddParameter(contactInserts, "contactid", DbType.Guid);
            AddParameter(contactInserts, "firstname", DbType.String);
            AddParameter(contactInserts, "lastname", DbType.String);
            AddParameter(contactInserts, SyncSession.SyncClientId, DbType.Guid);
            var param = AddParameter(contactInserts, SyncSession.SyncRowCount, DbType.Int32);
            param.Direction = ParameterDirection.Output;

            contactInserts.Connection = serverConn;
            customerSyncAdapter.InsertCommand = contactInserts;

            //Select updates from the server.
            var customerIncrUpdates = serverConn.CreateCommand();
            customerIncrUpdates.CommandText =
                "SELECT contactid, firstname, lastname " +
                "FROM contact " +
                "WHERE (versionnumber > @sync_last_received_anchor " +
                "AND versionnumber <= @sync_new_received_anchor " +
               ")";

            //"SELECT contactid, firstname, lastname " +
            //   "FROM contact " +
            //   "WHERE (versionnumber > @sync_last_received_anchor " +
            //   "AND versionnumber <= @sync_new_received_anchor " +
            //   "AND crmsync_updatedbyclientid <> @sync_client_id " +
            // These next lines filter out inserts from other clients that have happened since last anchor..
            //   "AND NOT (versionnumber > @sync_last_received_anchor " +
            //   "AND crmsync_insertedbyclientid <> @sync_client_id))";


            AddParameter(customerIncrUpdates, SyncSession.SyncLastReceivedAnchor, DbType.Int64);
            AddParameter(customerIncrUpdates, SyncSession.SyncNewReceivedAnchor, DbType.Int64);
            AddParameter(customerIncrUpdates, SyncSession.SyncClientId, DbType.Guid);
            customerIncrUpdates.Connection = serverConn;
            customerSyncAdapter.SelectIncrementalUpdatesCommand = customerIncrUpdates;

            //DT: Might have to write command adapter to handle the "SET @sync_row_count = @@rowcount" bit.
            var customerUpdates = new UpdateEntityDbCommandAdapter(serverConn.CreateCommand() as CrmDbCommand);
            customerUpdates.CommandText =
                "UPDATE contact SET " +
                "firstname = @firstname, lastname = @lastname " +
                "WHERE (contactid = @contactid)";

            //"UPDATE contact SET " +
            // "firstname = @firstname, lastname = @lastname, " +
            // "crmsync_updatedbyclientid = @sync_client_id " +
            // "WHERE (contactid = @contactid) " +
            // "AND (@sync_force_write = 1 " + // may have to put this all in a plugin..
            // "OR (versionnumber <= @sync_last_received_anchor " +
            // "OR crmsync_updatedbyclientid = @sync_client_id)) " +
            // "SET @sync_row_count = @@rowcount";

            AddParameter(contactInserts, "contactid", DbType.Guid);
            AddParameter(contactInserts, "firstname", DbType.String);
            AddParameter(contactInserts, "lastname", DbType.String);
            AddParameter(customerIncrUpdates, SyncSession.SyncClientId, DbType.Guid);
            AddParameter(customerIncrUpdates, SyncSession.SyncForceWrite, DbType.Boolean);
            AddParameter(customerIncrUpdates, SyncSession.SyncLastReceivedAnchor, DbType.Int64);
            AddParameter(customerIncrUpdates, SyncSession.SyncRowCount, DbType.Int32);

            customerUpdates.Connection = serverConn;
            customerSyncAdapter.UpdateCommand = customerUpdates;

            var dataTable = new DataTable("contact");
            var schema = customerSyncAdapter.FillSchema(dataTable, serverConn);


        }

        private static DbParameter AddParameter(DbCommand command, string syncParamater, DbType dbType)
        {
            var par = command.CreateParameter();
            par.ParameterName = "@" + syncParamater;
            par.DbType = dbType;
            command.Parameters.Add(par);
            return par;
        }

    }
}
