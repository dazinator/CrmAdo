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

        public virtual bool Exists()
        {
            var exists = false;

            if (!_creator.Exists())
            {
                return exists;
            }

            var command = _connection.DbConnection.CreateCommand();
            command.CommandText =
                @"SELECT 1 FROM [INFORMATION_SCHEMA].[TABLES]
WHERE [TABLE_SCHEMA] = N'dbo' AND [TABLE_NAME] = '__MigrationHistory' AND [TABLE_TYPE] = 'BASE TABLE'";

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
                command.CommandText =
                    @"SELECT [MigrationId], [ProductVersion]
FROM [dbo].[__MigrationHistory]
WHERE [ContextKey] = @ContextKey ORDER BY [MigrationId]";

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
            return new SqlOperation(
                @"CREATE TABLE [dbo].[__MigrationHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ContextKey] nvarchar(300) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK_MigrationHistory] PRIMARY KEY ([MigrationId], [ContextKey])
)",
                suppressTransaction: false);
        }

        public virtual MigrationOperation GetDeleteOperation(string migrationId)
        {
           // Check.NotEmpty(migrationId, nameof(migrationId));

            // TODO: Escape. Can we parameterize?
            return new SqlOperation(
                @"DELETE FROM [dbo].[__MigrationHistory]
WHERE [MigrationId] = '" + migrationId + "' AND [ContextKey] = '" + _contextType.FullName + "'",
                suppressTransaction: false);
        }

        public virtual MigrationOperation GetInsertOperation(IHistoryRow row)
        {
          //  Check.NotNull(row, nameof(row));

            // TODO: Escape. Can we parameterize?
            return new SqlOperation(
                @"INSERT INTO [dbo].[__MigrationHistory] ([MigrationId], [ContextKey], [ProductVersion])
VALUES ('" + row.MigrationId + "', '" + _contextType.FullName + "', '" + row.ProductVersion + "')",
                suppressTransaction: false);
        }
    }
}
