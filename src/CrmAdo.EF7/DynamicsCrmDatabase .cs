using JetBrains.Annotations;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Framework.Logging;

namespace CrmAdo.EntityFramework
{

    public class DynamicsCrmDatabase : RelationalDatabase
    {
        public DynamicsCrmDatabase(
            DbContextService<DbContext> model, 
            DynamicsCrmDataStoreCreator dataStoreCreator,
            DynamicsCrmConnection connection,
            Migrator migrator,
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
