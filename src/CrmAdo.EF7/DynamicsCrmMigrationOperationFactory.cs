using CrmAdo.EntityFramework.Metadata;
using CrmAdo.EntityFramework.Utils;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.MigrationsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework
{
  
        public class DynamicsCrmMigrationOperationFactory : MigrationOperationFactory
        {
            public DynamicsCrmMigrationOperationFactory(
               DynamicsCrmMetadataExtensionProvider extensionProvider)
                : base(extensionProvider)
            {
            }

            public virtual new DynamicsCrmMetadataExtensionProvider ExtensionProvider
            {
                get { return (DynamicsCrmMetadataExtensionProvider)base.ExtensionProvider; }
            }

            public override Column Column(IProperty property)
            {
                var column = base.Column(property);

                // TODO: This is essentially duplicated logic from the selector; combine if possible
                if (property.GenerateValueOnAdd)
                {
                    var strategy = property.DynamicsCrm().ValueGenerationStrategy
                                   ?? property.EntityType.Model.DynamicsCrm().ValueGenerationStrategy;

                    if (strategy == DynamicsCrmValueGenerationStrategy.Identity
                        || (strategy == null
                            && property.PropertyType.IsInteger()
                            && property.PropertyType != typeof(byte)
                            && property.PropertyType != typeof(byte?)))
                    {
                        column.IsIdentity = true;
                    }
                }

                return column;
            }

            public override AddPrimaryKeyOperation AddPrimaryKeyOperation(IKey target)
            {
                var operation = base.AddPrimaryKeyOperation(target);
                var isClustered = ExtensionProvider.Extensions(target).IsClustered;

                if (isClustered.HasValue)
                {
                    operation.IsClustered = isClustered.Value;
                }

                return operation;
            }

            public override CreateIndexOperation CreateIndexOperation(IIndex target)
            {
                var operation = base.CreateIndexOperation(target);
                var isClustered = ExtensionProvider.Extensions(target).IsClustered;

                if (isClustered.HasValue)
                {
                    operation.IsClustered = isClustered.Value;
                }

                return operation;
            }
        }
   
}
