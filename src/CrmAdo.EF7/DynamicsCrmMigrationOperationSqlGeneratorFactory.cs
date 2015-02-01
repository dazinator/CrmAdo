using CrmAdo.EntityFramework.Metadata;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework
{
   
        public class DynamicsCrmMigrationOperationSqlGeneratorFactory : IMigrationOperationSqlGeneratorFactory
        {
            private readonly DynamicsCrmMetadataExtensionProvider _extensionProvider;

            public DynamicsCrmMigrationOperationSqlGeneratorFactory(
               DynamicsCrmMetadataExtensionProvider extensionProvider)
            {
                //Check.NotNull(extensionProvider, "extensionProvider");

                _extensionProvider = extensionProvider;
            }

            public virtual DynamicsCrmMetadataExtensionProvider ExtensionProvider
            {
                get { return _extensionProvider; }
            }

            public virtual DynamicsCrmMigrationOperationSqlGenerator Create()
            {
                return Create(new Model());
            }

            public virtual DynamicsCrmMigrationOperationSqlGenerator Create(IModel targetModel)
            {
               // Check.NotNull(targetModel, "targetModel");

                return
                    new DynamicsCrmMigrationOperationSqlGenerator(
                        ExtensionProvider,
                        new DynamicsCrmTypeMapper())
                    {
                        TargetModel = targetModel,
                    };
            }

            MigrationOperationSqlGenerator IMigrationOperationSqlGeneratorFactory.Create()
            {
                return Create();
            }

            MigrationOperationSqlGenerator IMigrationOperationSqlGeneratorFactory.Create(IModel targetModel)
            {
                return Create(targetModel);
            }
        }
  
}
