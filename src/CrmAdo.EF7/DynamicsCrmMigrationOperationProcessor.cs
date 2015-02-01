using CrmAdo.EntityFramework.Metadata;
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
    
        public class DynamicsCrmMigrationOperationProcessor : MigrationOperationProcessor
        {
            public DynamicsCrmMigrationOperationProcessor(
                DynamicsCrmMetadataExtensionProvider extensionProvider,
               DynamicsCrmTypeMapper typeMapper,
               DynamicsCrmMigrationOperationFactory operationFactory)
                : base(
                    extensionProvider,
                    typeMapper,
                    operationFactory)
            {
            }

            public virtual new DynamicsCrmMetadataExtensionProvider ExtensionProvider
            {
                get { return (DynamicsCrmMetadataExtensionProvider)base.ExtensionProvider; }
            }

            public virtual new DynamicsCrmTypeMapper TypeMapper
            {
                get { return (DynamicsCrmTypeMapper)base.TypeMapper; }
            }

            public virtual new DynamicsCrmMigrationOperationFactory OperationFactory
            {
                get { return (DynamicsCrmMigrationOperationFactory)base.OperationFactory; }
            }

            public override IReadOnlyList<MigrationOperation> Process(
                MigrationOperationCollection operations, IModel sourceModel, IModel targetModel)
            {
                //Check.NotNull(operations, "operations");
               // Check.NotNull(sourceModel, "sourceModel");
               // Check.NotNull(targetModel, "targetModel");

                var context = new Context(operations, sourceModel, targetModel);

                foreach (var operation in operations.Get<DropTableOperation>())
                {
                    Process(operation, context);
                }

                foreach (var operation in operations.Get<DropColumnOperation>())
                {
                    Process(operation, context);
                }

                foreach (var operation in operations.Get<AlterColumnOperation>())
                {
                    Process(operation, context);
                }

                return context.Operations.GetAll();
            }

            protected virtual void Process(DropTableOperation dropTableOperation, Context context)
            {
               // Check.NotNull(dropTableOperation, "dropTableOperation");
               // Check.NotNull(context, "context");

                var entityType = context.SourceModel.EntityTypes.Single(
                    t => NameBuilder.SchemaQualifiedTableName(t) == dropTableOperation.TableName);

                foreach (var foreignKey in context.SourceModel.EntityTypes
                        .SelectMany(t => t.ForeignKeys)
                        .Where(fk => ReferenceEquals(fk.ReferencedEntityType, entityType)))
                {
                    context.Operations.Add(OperationFactory.DropForeignKeyOperation(foreignKey),
                        (x, y) => x.TableName == y.TableName && x.ForeignKeyName == y.ForeignKeyName);
                }
            }

            protected virtual void Process(DropColumnOperation dropColumnOperation, Context context)
            {
                //Check.NotNull(dropColumnOperation, "dropColumnOperation");
               // Check.NotNull(context, "context");

                var entityType = context.SourceModel.EntityTypes.Single(
                    t => NameBuilder.SchemaQualifiedTableName(t) == dropColumnOperation.TableName);
                var property = entityType.Properties.Single(
                    p => NameBuilder.ColumnName(p) == dropColumnOperation.ColumnName);
                var extensions = property.DynamicsCrm();

                if (extensions.DefaultValue != null || extensions.DefaultExpression != null)
                {
                    context.Operations.Add(OperationFactory.DropDefaultConstraintOperation(property));
                }
            }

            protected virtual void Process(AlterColumnOperation alterColumnOperation, Context context)
            {
               // Check.NotNull(alterColumnOperation, "alterColumnOperation");
               // Check.NotNull(context, "context");

                var entityType = context.SourceModel.EntityTypes.Single(
                    t => NameBuilder.SchemaQualifiedTableName(t) == alterColumnOperation.TableName);
                var property = entityType.Properties.Single(
                    p => NameBuilder.ColumnName(p) == alterColumnOperation.NewColumn.Name);
                var extensions = property.DynamicsCrm();
                var newColumn = alterColumnOperation.NewColumn;

                string dataType, newDataType;
                GetDataTypes(entityType, property, newColumn, context, out dataType, out newDataType);

                var primaryKey = entityType.TryGetPrimaryKey();
                if (primaryKey != null
                    && primaryKey.Properties.Any(p => ReferenceEquals(p, property)))
                {
                    if (context.Operations.Add(OperationFactory.DropPrimaryKeyOperation(primaryKey),
                        (x, y) => x.TableName == y.TableName && x.PrimaryKeyName == y.PrimaryKeyName))
                    {
                        context.Operations.Add(OperationFactory.AddPrimaryKeyOperation(primaryKey));
                    }
                }

                // TODO: Changing the length of a variable-length column used in a UNIQUE constraint is allowed.
                foreach (var uniqueConstraint in entityType.Keys.Where(k => k != primaryKey)
                    .Where(uc => uc.Properties.Any(p => ReferenceEquals(p, property))))
                {
                    if (context.Operations.Add(OperationFactory.DropUniqueConstraintOperation(uniqueConstraint),
                        (x, y) => x.TableName == y.TableName && x.UniqueConstraintName == y.UniqueConstraintName))
                    {
                        context.Operations.Add(OperationFactory.AddUniqueConstraintOperation(uniqueConstraint));
                    }
                }

                foreach (var foreignKey in entityType.ForeignKeys
                    .Where(fk => fk.Properties.Any(p => ReferenceEquals(p, property)))
                    .Concat(context.SourceModel.EntityTypes
                        .SelectMany(t => t.ForeignKeys)
                        .Where(fk => fk.ReferencedProperties.Any(p => ReferenceEquals(p, property)))))
                {
                    if (context.Operations.Add(OperationFactory.DropForeignKeyOperation(foreignKey),
                        (x, y) => x.TableName == y.TableName && x.ForeignKeyName == y.ForeignKeyName))
                    {
                        context.Operations.Add(OperationFactory.AddForeignKeyOperation(foreignKey));
                    }
                }

                if (dataType != newDataType
                    || ((string.Equals(dataType, "varchar", StringComparison.OrdinalIgnoreCase)
                         || string.Equals(dataType, "nvarchar", StringComparison.OrdinalIgnoreCase)
                         || string.Equals(dataType, "varbinary", StringComparison.OrdinalIgnoreCase))
                        && newColumn.MaxLength > property.MaxLength))
                {
                    foreach (var index in entityType.Indexes
                        .Where(ix => ix.Properties.Any(p => ReferenceEquals(p, property))))
                    {
                        if (context.Operations.Add(OperationFactory.DropIndexOperation(index),
                            (x, y) => x.TableName == y.TableName && x.IndexName == y.IndexName))
                        {
                            context.Operations.Add(OperationFactory.CreateIndexOperation(index));
                        }
                    }
                }

                if (!property.IsStoreComputed
                    && (extensions.DefaultValue != null || extensions.DefaultExpression != null))
                {
                    context.Operations.Add(OperationFactory.DropDefaultConstraintOperation(property));
                }

                if (property.IsConcurrencyToken
                    || property.IsStoreComputed != alterColumnOperation.NewColumn.IsComputed)
                {
                    context.Operations.Remove(alterColumnOperation);
                    context.Operations.Add(OperationFactory.DropColumnOperation(property));
                    context.Operations.Add(new AddColumnOperation(
                        alterColumnOperation.TableName, alterColumnOperation.NewColumn));
                }
            }

            protected virtual void GetDataTypes(
                IEntityType entityType, IProperty property, Column newColumn, Context context,
                out string dataType, out string newDataType)
            {
               // Check.NotNull(entityType, "entityType");
               // Check.NotNull(property, "property");
               // Check.NotNull(newColumn, "newColumn");
              //  Check.NotNull(context, "context");

                var isKey = property.IsKey() || property.IsForeignKey();
                var extensions = property.DynamicsCrm();

                dataType
                    = TypeMapper.GetTypeMapping(
                        extensions.ColumnType, NameBuilder.ColumnName(property), property.PropertyType, isKey, property.IsConcurrencyToken)
                        .StoreTypeName;
                newDataType
                    = TypeMapper.GetTypeMapping(
                        newColumn.DataType, newColumn.Name, newColumn.ClrType, isKey, newColumn.IsTimestamp)
                        .StoreTypeName;
            }

            protected class Context
            {
                private readonly MigrationOperationCollection _operations;
                private readonly IModel _sourceModel;
                private readonly IModel _targetModel;

                public Context(
                   MigrationOperationCollection operations,
                   IModel sourceModel,
                    IModel targetModel)
                {
                   // Check.NotNull(operations, "operations");
                  //  Check.NotNull(sourceModel, "sourceModel");
                  //  Check.NotNull(targetModel, "targetModel");

                    _operations = operations;
                    _sourceModel = sourceModel;
                    _targetModel = targetModel;
                }

                public MigrationOperationCollection Operations
                {
                    get { return _operations; }
                }

                public IModel SourceModel
                {
                    get { return _sourceModel; }
                }

                public IModel TargetModel
                {
                    get { return _targetModel; }
                }
            }
        }
   
}
