using Microsoft.Data.Entity.DynamicsCrm.Migrations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.History;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using Microsoft.Data.Entity.Relational.Migrations.Sql;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm
{

    public class DynamicsCrmDataStoreServices : RelationalDataStoreServices
    {
        private readonly DynamicsCrmDataStore _store;
        private readonly DynamicsCrmDataStoreCreator _creator;
        private readonly DynamicsCrmConnection _connection;
        private readonly DynamicsCrmValueGeneratorSelector _valueGeneratorSelector;
        private readonly DynamicsCrmDatabase _database;
        private readonly DynamicsCrmModelBuilderFactory _modelBuilderFactory;
        private readonly DynamicsCrmModelSource _modelSource;
        private readonly DynamicsCrmModelDiffer _modelDiffer;
        private readonly DynamicsCrmHistoryRepository _historyRepository;
        private readonly DynamicsCrmMigrationSqlGenerator _migrationSqlGenerator;


        public DynamicsCrmDataStoreServices(
            DynamicsCrmDataStore store,
            DynamicsCrmDataStoreCreator creator,
            DynamicsCrmConnection connection,
            DynamicsCrmValueGeneratorSelector valueGeneratorSelector,
            DynamicsCrmDatabase database,
            DynamicsCrmModelBuilderFactory modelBuilderFactory,
            DynamicsCrmModelDiffer modelDiffer,
            DynamicsCrmHistoryRepository historyRepository,
            DynamicsCrmMigrationSqlGenerator migrationSqlGenerator,
            DynamicsCrmModelSource modelSource)
        {

            _store = store;
            _creator = creator;
            _connection = connection;
            _valueGeneratorSelector = valueGeneratorSelector;
            _database = database;
            _modelBuilderFactory = modelBuilderFactory;
            _modelDiffer = modelDiffer;
            _historyRepository = historyRepository;
            _migrationSqlGenerator = migrationSqlGenerator;
            _modelSource = modelSource;
        }

        public override DataStore Store
        {
            get { return _store; }
        }

        //public override DataStore Store()
        //{
        //    
        //}

        public override DataStoreCreator Creator
        {
            get { return _creator; }
        }

        public override DataStoreConnection Connection
        {
            get { return _connection; }
        }

        public override ValueGeneratorSelectorContract ValueGeneratorSelector
        {
            get { return _valueGeneratorSelector; }
        }

        public override Database Database
        {
            get { return _database; }
        }

        public override ModelBuilderFactory ModelBuilderFactory
        {
            get { return _modelBuilderFactory; }
        }

        public override ModelDiffer ModelDiffer
        {
            get { return _modelDiffer; }
        }

        public override IHistoryRepository HistoryRepository
        {
            get { return _historyRepository; }
        }

        public override MigrationSqlGenerator MigrationSqlGenerator
        {
            get { return _migrationSqlGenerator; }
        }      

        public override ModelSource ModelSource
        {
            get { return _modelSource; }
        }
    }


}
