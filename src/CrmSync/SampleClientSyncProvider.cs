using System;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServerCe;

namespace CrmSync
{
    public class SampleClientSyncProvider : SqlCeClientSyncProvider
    {

        public SampleClientSyncProvider()
        {
            //Specify a connection string for the sample client database.
            Utility util = new Utility();
            this.ConnectionString = Utility.ConnStr_SqlCeClientSync;

            //We use the CreatingSchema event to change the schema
            //by using the API. We use the SchemaCreated event to 
            //change the schema by using SQL.
            this.CreatingSchema += new EventHandler<CreatingSchemaEventArgs>(SampleClientSyncProvider_CreatingSchema);
            this.SchemaCreated += new EventHandler<SchemaCreatedEventArgs>(SampleClientSyncProvider_SchemaCreated);
        }

        private void SampleClientSyncProvider_CreatingSchema(object sender, CreatingSchemaEventArgs e)
        {
            //Set the RowGuid property because it is not copied
            //to the client by default. This is also a good time
            //to specify literal defaults with .Columns[ColName].DefaultValue;
            //but we will specify defaults like NEWID() by calling
            //ALTER TABLE after the table is created.
            Console.Write("Creating schema for " + e.Table.TableName + " | ");
            e.Schema.Tables["contact"].Columns["contactid"].RowGuid = true;
        }

        private void SampleClientSyncProvider_SchemaCreated(object sender, SchemaCreatedEventArgs e)
        {
            //Call ALTER TABLE on the client. This must be done
            //over the same connection and within the same
            //transaction that Sync Framework uses
            //to create the schema on the client.
            Utility util = new Utility();
            Utility.MakeSchemaChangesOnClient(e.Connection, e.Transaction, e.Table.TableName);
            Console.WriteLine("Schema created for " + e.Table.TableName);
        }
    }
}