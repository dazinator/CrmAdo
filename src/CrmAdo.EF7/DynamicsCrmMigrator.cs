using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework
{
    
        public class DynamicsCrmMigrator : Migrator
        {
            /// <summary>
            ///     This constructor is intended only for use when creating test doubles that will override members
            ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
            ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
            /// </summary>
            protected DynamicsCrmMigrator()
            {
            }

            public DynamicsCrmMigrator(
                HistoryRepository historyRepository,
                MigrationAssembly migrationAssembly,
                DynamicsCrmModelDiffer modelDiffer,
                DynamicsCrmMigrationOperationSqlGeneratorFactory sqlGeneratorFactory,
                DynamicsCrmSqlGenerator sqlGenerator,
                SqlStatementExecutor sqlStatementExecutor,
                DynamicsCrmDataStoreCreator storeCreator,
                DynamicsCrmConnection connection,
                ILoggerFactory loggerFactory)
                : base(
                    historyRepository,
                    migrationAssembly,
                    modelDiffer,
                    sqlGeneratorFactory,
                    sqlGenerator,
                    sqlStatementExecutor,
                    storeCreator,
                    connection,
                    loggerFactory)
            {
            }
        }
    
}
