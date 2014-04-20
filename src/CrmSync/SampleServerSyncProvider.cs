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
            
            // new fields required against entities to be synchronised.
            // crmsync_updatedbyclientid
            // crmsync_insertedbyclientid

            // Those fields will allow a client to filter out change detections for records that it has inserted or updated itself during a last sync run.
            // In addition:-

            // plugin to be fire on delete of entities in crm that will:-
            //  create a "sync_tombstone" record:-
            //      entity name, 
            //      entity id, 
            //      sync_deletedbyclientid

            //  sync_deletedbyclientid is only set by a sync client that syncs a delete to CRM, so it can later filter out its own delete from change detection on the next sync.


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
            var customerIncrInserts = new SelectIncrementalChangesDbCommandAdapter(serverConn.CreateCommand() as CrmDbCommand);
            customerIncrInserts.CommandText =
                "SELECT contactid, firstname, lastname, createdon, modifiedon " +
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
            
           
            //Apply inserts to the server.
            DbCommand contactInserts = new InsertEntityDbCommandAdapter(serverConn.CreateCommand() as CrmDbCommand);
            contactInserts.CommandText =
                "INSERT INTO contact (contactid, firstname, lastname, createdon, modifiedon) " +
                "VALUES (@contactid, @firstname, @lastname)";
            //"INSERT INTO contact (contactid, firstname, lastname, InsertId, UpdateId) " +
            //  "VALUES (@contactid, @firstname, @lastname, @sync_client_id, @sync_client_id) " +
            //  "SET @sync_row_count = @@rowcount";

            AddParameter(contactInserts, "contactid", DbType.Guid);
            AddParameter(contactInserts, "firstname", DbType.String);
            AddParameter(contactInserts, "lastname", DbType.String);
            AddParameter(contactInserts, "createdon", DbType.DateTime);
            AddParameter(contactInserts, "modifiedon", DbType.DateTime);
            AddParameter(contactInserts, SyncSession.SyncClientId, DbType.Guid);
            var param = AddParameter(contactInserts, SyncSession.SyncRowCount, DbType.Int32);
            param.Direction = ParameterDirection.Output;

            contactInserts.Connection = serverConn;
            customerSyncAdapter.InsertCommand = contactInserts;

            //Select updates from the server.
            var customerIncrUpdates = new SelectIncrementalChangesDbCommandAdapter(serverConn.CreateCommand() as CrmDbCommand);
            customerIncrUpdates.CommandText =
                "SELECT contactid, firstname, lastname, createdon, modifiedon " +
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