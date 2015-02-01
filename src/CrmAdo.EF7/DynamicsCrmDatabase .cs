using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Framework.Logging;

namespace CrmAdo.EntityFramework
{

    public class DynamicsCrmDatabase : MigrationsEnabledDatabase
    {
        public DynamicsCrmDatabase(DbContextService<IModel> model, DynamicsCrmDataStoreCreator dataStoreCreator,
            DynamicsCrmConnection connection,
            DynamicsCrmMigrator migrator,
             ILoggerFactory loggerFactory)
            : base(model, dataStoreCreator, connection, migrator, loggerFactory)
        {
        }

        public new virtual DynamicsCrmConnection Connection
        {
            get { return (DynamicsCrmConnection)base.Connection; }
        }
    }

 
}
