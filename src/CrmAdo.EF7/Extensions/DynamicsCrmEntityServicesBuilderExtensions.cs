using CrmAdo.EntityFramework.Metadata;
using CrmAdo.EntityFramework.Update;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.EntityFramework;
// ReSharper disable once CheckNamespace

namespace Microsoft.Framework.DependencyInjection
{
    public static class DynamicsCrmEntityServicesBuilderExtensions
    {
        public static EntityServicesBuilder AddDynamicsCrm(this EntityServicesBuilder builder)
        {
            //Check.NotNull(builder, "builder");

            builder.AddMigrations().ServiceCollection
                .AddScoped<DataStoreSource, DynamicsCrmDataStoreSource>()
                .TryAdd(new ServiceCollection()
                    .AddSingleton<DynamicsCrmValueGeneratorCache>()
                    .AddSingleton<DynamicsCrmValueGeneratorSelector>()
                    .AddSingleton<SimpleValueGeneratorFactory<SequentialGuidValueGenerator>>()
                    .AddSingleton<DynamicsCrmSequenceValueGeneratorFactory>()
                    .AddSingleton<DynamicsCrmSqlGenerator>()
                    .AddSingleton<SqlStatementExecutor>()
                    .AddSingleton<DynamicsCrmTypeMapper>()
                    .AddSingleton<DynamicsCrmModificationCommandBatchFactory>()
                    .AddSingleton<DynamicsCrmCommandBatchPreparer>()
                    .AddSingleton<DynamicsCrmMetadataExtensionProvider>()
                    .AddSingleton<DynamicsCrmMigrationOperationFactory>()
                    .AddSingleton<DynamicsCrmModelSource>()
                    .AddScoped<DynamicsCrmBatchExecutor>()
                    .AddScoped<DynamicsCrmDataStoreServices>()
                    .AddScoped<DynamicsCrmDataStore>()
                    .AddScoped<DynamicsCrmConnection>()
                    .AddScoped<DynamicsCrmMigrationOperationProcessor>()
                    .AddScoped<DynamicsCrmModelDiffer>()
                    .AddScoped<DynamicsCrmDatabase>()
                    .AddScoped<DynamicsCrmMigrationOperationSqlGeneratorFactory>()
                    .AddScoped<DynamicsCrmDataStoreCreator>()
                    .AddScoped<DynamicsCrmMigrator>());

            return builder;
        }

    }
}
