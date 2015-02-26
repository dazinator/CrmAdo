using CrmAdo.Dynamics;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations.History;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm.Migrations
{

    // TODO: Log
    public class DynamicsCrmHistoryRepository : IHistoryRepository
    {
        private readonly DynamicsCrmConnection _connection;
        private readonly DynamicsCrmDataStoreCreator _creator;
        private readonly Type _contextType;

        public const string PublisherUniqueName = "CrmAdo";
        public const string PublisherPrefix = "CrmAdo";

        public DynamicsCrmHistoryRepository(
           DynamicsCrmConnection connection,
            DynamicsCrmDataStoreCreator creator,
            DbContextService<DbContext> context)
        {
            // Check.NotNull(connection, nameof(connection));
            // Check.NotNull(creator, nameof(creator));
            // Check.NotNull(context, nameof(context));

            _connection = connection;
            _creator = creator;
            _contextType = context.Service.GetType();
        }

        /// <summary>
        /// Ensures a publisher is created in CRM.
        /// </summary>
        public virtual void EnsurePublisherExists()
        {
            CrmPublisherRepository publisherRepo = GetPublisherRepo();
            using (var conn = _connection.DbConnection)
            {
                var result = publisherRepo.Find(_connection.DbConnection, PublisherUniqueName);
                if (!result.Exists())
                {
                    var createResult = publisherRepo.Create(_connection.DbConnection, PublisherUniqueName, PublisherPrefix);
                    if (!createResult.Exists())
                    {
                        throw new Exception("Could not create publisher in crm: " + PublisherUniqueName);
                    }
                }
            }

        }

        protected virtual CrmPublisherRepository GetPublisherRepo()
        {
            return new CrmPublisherRepository();
        }


        public string GetMigrationsTableName()
        {
            return string.Format("{0}_{1}", PublisherPrefix, "migrationhistory");
        }

        public virtual bool Exists()
        {
            var exists = false;

            if (!_creator.Exists())
            {
                return exists;
            }

            //TODO: use GetSchema() method to see if table exists.
            var command = _connection.DbConnection.CreateCommand();
            string commandText = string.Format("SELECT 1 FROM entitymetadata WHERE LogicalName = '{0}'", GetMigrationsTableName());
            command.CommandText = commandText;
            _connection.Open();
            try
            {
                exists = command.ExecuteScalar() != null;
            }
            finally
            {
                _connection.Close();
            }

            return exists;
        }

        public virtual IReadOnlyList<IHistoryRow> GetAppliedMigrations()
        {
            var rows = new List<HistoryRow>();

            if (!Exists())
            {
                return rows;
            }

            _connection.Open();
            try
            {
                var command = _connection.DbConnection.CreateCommand();
                string commandText = string.Format(@"SELECT [MigrationId], [ProductVersion]
FROM {0}
WHERE [ContextKey] = @ContextKey ORDER BY [MigrationId]", GetMigrationsTableName());

                command.CommandText = commandText;

                var param = command.CreateParameter();
                param.ParameterName = "@ContextKey";
                param.Value = _contextType.FullName;
                param.DbType = System.Data.DbType.String;
                command.Parameters.Add(param);

                // command.Parameters.AddWithValue("@ContextKey", _contextType.FullName);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rows.Add(new HistoryRow(reader.GetString(0), reader.GetString(1)));
                    }
                }
            }
            finally
            {
                _connection.Close();
            }

            return rows;
        }

        public virtual MigrationOperation GetCreateOperation()
        {

            //TODO: We have to be able to do a create, and then an alter, as CRM does not support creating entities with specified columns in one operation,
            // you have to create an entity with the default columns, and then ALTER to add in the rest..

            string createCommandText = string.Format(@"CREATE TABLE {0} (
    {0}id UniqueIdentifier PRIMARY KEY,    
    [MigrationId] nvarchar(150) NOT NULL,
    [ContextKey] nvarchar(300) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL    
)", GetMigrationsTableName());
          
            return new SqlOperation(createCommandText, suppressTransaction: false);
        }

        public virtual MigrationOperation GetDeleteOperation(string migrationId)
        {
            // Check.NotEmpty(migrationId, nameof(migrationId));
            // TODO: Escape. Can we parameterize?
            string deleteCommandText = string.Format("DELETE FROM {0} WHERE [MigrationId] = '{1}' AND [ContextKey] = '{2}'", GetMigrationsTableName(), migrationId, _contextType.FullName);

            return new SqlOperation(
                deleteCommandText,
                suppressTransaction: false);
        }

        public virtual MigrationOperation GetInsertOperation(IHistoryRow row)
        {
            //  Check.NotNull(row, nameof(row));
            string insertCommandText = string.Format("INSERT INTO {0} ([MigrationId], [ContextKey], [ProductVersion]) VALUES ('{1}','{2}','{3}')", GetMigrationsTableName(), row.MigrationId, row.ProductVersion);
            
            // TODO: Escape. Can we parameterize?
            return new SqlOperation(
                insertCommandText,
                suppressTransaction: false);
        }
    }
}
