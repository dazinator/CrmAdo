using CrmAdo.EntityFramework.Migrations;
using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Data.Entity.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework
{

    public class DynamicsCrmDataStoreServices : MigrationsDataStoreServices
    {
        private readonly DynamicsCrmDataStore _store;
        private readonly DynamicsCrmDataStoreCreator _creator;
        private readonly DynamicsCrmConnection _connection;
        private readonly DynamicsCrmValueGeneratorCache _valueGeneratorCache;
        private readonly DynamicsCrmDatabase _database;
        private readonly ModelBuilderFactory _modelBuilderFactory;
        private readonly DynamicsCrmModelSource _modelSource;
        // private readonly DynamicsCrmMigrator _migrator;

        public DynamicsCrmDataStoreServices(
            DynamicsCrmDataStore store,
            DynamicsCrmDataStoreCreator creator,
            DynamicsCrmConnection connection,
            DynamicsCrmValueGeneratorCache valueGeneratorCache,
            DynamicsCrmDatabase database,
            ModelBuilderFactory modelBuilderFactory,
            DynamicsCrmModelDiffer modelDiffer,
            DynamicsCrmHistoryRepository historyRepository,
            DynamicsCrmSqlGenerator migrationSqlGenerator,
            DynamicsCrmModelSource modelSource
            )
        {
            //  Check.NotNull(store, "store");
            // Check.NotNull(creator, "creator");
            // Check.NotNull(connection, "connection");
            // Check.NotNull(valueGeneratorCache, "valueGeneratorCache");
            // Check.NotNull(database, "database");
            // Check.NotNull(modelBuilderFactory, "modelBuilderFactory");
            //  Check.NotNull(migrator, "migrator");

            _store = store;
            _creator = creator;
            _connection = connection;
            _valueGeneratorCache = valueGeneratorCache;
            _database = database;
            _modelBuilderFactory = modelBuilderFactory;
            ModelDiffer = modelDiffer;
            HistoryRepository = historyRepository;
            MigrationSqlGenerator = migrationSqlGenerator;
            _modelSource = modelSource;
        }

        public override DataStore Store
        {
            get { return _store; }
        }

        public override DataStoreCreator Creator
        {
            get { return _creator; }
        }

        public override DataStoreConnection Connection
        {
            get { return _connection; }
        }

        public override ValueGeneratorCache ValueGeneratorCache
        {
            get { return _valueGeneratorCache; }
        }

        public override Database Database
        {
            get { return _database; }
        }

        public override ModelBuilderFactory ModelBuilderFactory
        {
            get { return _modelBuilderFactory; }
        }

        public override ModelDiffer ModelDiffer { get; private set; }
        public override IHistoryRepository HistoryRepository { get; private set; }
        public override MigrationSqlGenerator MigrationSqlGenerator { get; private set; }

        public override ModelSource ModelSource
        {
            get { return _modelSource; }
        }


    }


}
