using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using CrmAdo;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.Server;

namespace CrmSync
{
    public class SampleServerSyncProvider : DbServerSyncProvider
    {
        public SampleServerSyncProvider()
        {

            //Create a connection to the sample server database.
            var util = new Utility();
            var serverConn = new CrmDbConnection(Utility.ConnStr_DbServerSync);
            this.Connection = serverConn;

            DbCommand selectNewAnchorCommand = new AnchorDbCommandAdapter(serverConn.CreateCommand() as CrmDbCommand);
            var anchorParam = selectNewAnchorCommand.CreateParameter();
            anchorParam.ParameterName = "@" + SyncSession.SyncNewReceivedAnchor;

            var entityName = "contact";
            var sql = "select top 1 versionnumber from " + entityName + " order by versionnumber desc";

            selectNewAnchorCommand.CommandText = sql;
            selectNewAnchorCommand.Parameters.Add(anchorParam);
            anchorParam.Direction = ParameterDirection.Output;
            selectNewAnchorCommand.Connection = serverConn;
            this.SelectNewAnchorCommand = selectNewAnchorCommand;

            //Create the SyncAdapter.
            var customerSyncAdapter = new SyncAdapter("contact");

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




            ////Select deletes from the server.
            //DbCommand customerIncrDeletes = serverConn.CreateCommand();
            //customerIncrDeletes.CommandText =
            //    "SELECT contactid, firstname, lastname " +
            //    "FROM contactSyncTombstone " +
            //    "WHERE (@sync_initialized = 1 " +
            //    "AND DeleteTimestamp > @sync_last_received_anchor " +
            //    "AND DeleteTimestamp <= @sync_new_received_anchor " +
            //    "AND DeleteId <> @sync_client_id)";


            ////"SELECT CustomerId, CustomerName, SalesPerson, CustomerType " +
            ////  "FROM Sales.Customer_Tombstone " +
            ////  "WHERE (@sync_initialized = 1 " +
            ////  "AND DeleteTimestamp > @sync_last_received_anchor " +
            ////  "AND DeleteTimestamp <= @sync_new_received_anchor " +
            ////  "AND DeleteId <> @sync_client_id)";


            //AddParameter(customerIncrDeletes, SyncSession.SyncInitialized, DbType.Boolean);
            //AddParameter(customerIncrDeletes, SyncSession.SyncLastReceivedAnchor, DbType.Int64);
            //AddParameter(customerIncrDeletes, SyncSession.SyncNewReceivedAnchor, DbType.Int64);
            //AddParameter(customerIncrDeletes, SyncSession.SyncClientId, DbType.Guid);

            //customerIncrDeletes.Connection = serverConn;
            //customerSyncAdapter.SelectIncrementalDeletesCommand = customerIncrDeletes;

            ////DT: Might have to put this into a plugin..
            ////Apply deletes to the server.            
            //DbCommand customerDeletes = serverConn.CreateCommand();
            //customerDeletes.CommandText =
            //    "DELETE FROM Sales.Customer " +
            //    "WHERE (CustomerId = @CustomerId) " +
            //    "AND (@sync_force_write = 1 " +
            //    "OR (UpdateTimestamp <= @sync_last_received_anchor " + // hasnt been updated since
            //    "OR UpdateId = @sync_client_id)) " + // or could have been updated since but if it was us then thats fine we can still delete
            //    "SET @sync_row_count = @@rowcount " + // informs sync fx of the number of rows effected = should be 1
            //    "IF (@sync_row_count > 0)  BEGIN " + // as long as one record was deleted then we have to store the tombstone record - this could be done by a plugin..
            //    "UPDATE Sales.Customer_Tombstone " +
            //    "SET DeleteId = @sync_client_id " +
            //    "WHERE (CustomerId = @CustomerId) " +
            //    "END";

            //AddParameter(customerDeletes, "CustomerId", DbType.Guid);
            //AddParameter(customerDeletes, SyncSession.SyncForceWrite, DbType.Boolean);
            //AddParameter(customerDeletes, SyncSession.SyncLastReceivedAnchor, DbType.Int64);
            //AddParameter(customerDeletes, SyncSession.SyncClientId, DbType.Guid);
            //AddParameter(customerDeletes, SyncSession.SyncRowCount, DbType.Int32);

            //customerDeletes.Connection = serverConn;
            //customerSyncAdapter.DeleteCommand = customerDeletes;

            //Add the SyncAdapter to the server synchronization provider.
            this.SyncAdapters.Add(customerSyncAdapter);

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