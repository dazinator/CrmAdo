using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.SqlServer;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.SqlServer.Extensions;
using CrmAdo.EntityFramework.Extensions;
using CrmAdo.EntityFramework;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class DynamicsCrmDbContextOptionsExtensions
    {
        public static DynamicsCrmDbContextOptions UseDynamicsCrm(this DbContextOptions options)
        {
           // Check.NotNull(options, "options");

            ((IDbContextOptions)options)
                .AddOrUpdateExtension<DynamicsCrmOptionsExtension>(x => { });

            return new DynamicsCrmDbContextOptions(options);
        }

        public static DynamicsCrmDbContextOptions UseDynamicsCrm(this DbContextOptions options, string connectionString)
        {
           // Check.NotNull(options, "options");
           // Check.NotEmpty(connectionString, "connectionString");

            ((IDbContextOptions)options)
                .AddOrUpdateExtension<DynamicsCrmOptionsExtension>(x => x.ConnectionString = connectionString);

            return new DynamicsCrmDbContextOptions(options);
        }

        public static DynamicsCrmDbContextOptions UseDynamicsCrm<T>(this DbContextOptions<T> options, string connectionString)
        {
            return UseDynamicsCrm((DbContextOptions)options, connectionString);
        }

        // Note: Decision made to use DbConnection not SqlConnection: Issue #772
        public static DynamicsCrmDbContextOptions UseDynamicsCrm(this DbContextOptions options, DbConnection connection)
        {
           // Check.NotNull(options, "options");
            //Check.NotNull(connection, "connection");

            ((IDbContextOptions)options)
                .AddOrUpdateExtension<DynamicsCrmOptionsExtension>(x => x.Connection = connection);

            return new DynamicsCrmDbContextOptions(options);
        }

        // Note: Decision made to use DbConnection not SqlConnection: Issue #772
        public static DynamicsCrmDbContextOptions UseDynamicsCrm<T>(this DbContextOptions<T> options, DbConnection connection)
        {
            return UseDynamicsCrm((DbContextOptions)options, connection);
        }
    }
}

