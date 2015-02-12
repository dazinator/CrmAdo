using CrmAdo.EntityFramework.Metadata;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.MigrationsModel;
using Microsoft.Data.Entity.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Migrations
{
    //public class DynamicsCrmModelDiffer : ModelDiffer, IEqualityComparer<ISequence>
    //{
    //    public DynamicsCrmModelDiffer(
    //        DynamicsCrmMetadataExtensionProvider extensionProvider,
    //       DynamicsCrmTypeMapper typeMapper,
    //        DynamicsCrmMigrationOperationFactory operationFactory,
    //        DynamicsCrmMigrationOperationProcessor operationProcessor)
    //        : base(
    //            extensionProvider,
    //            typeMapper,
    //            operationFactory,
    //            operationProcessor)
    //    {
    //    }

    //    public virtual new DynamicsCrmMetadataExtensionProvider ExtensionProvider
    //    {
    //        get { return (DynamicsCrmMetadataExtensionProvider)base.ExtensionProvider; }
    //    }

    //    public virtual new DynamicsCrmTypeMapper TypeMapper
    //    {
    //        get { return (DynamicsCrmTypeMapper)base.TypeMapper; }
    //    }

    //    public virtual new DynamicsCrmMigrationOperationFactory OperationFactory
    //    {
    //        get { return (DynamicsCrmMigrationOperationFactory)base.OperationFactory; }
    //    }

    //    public virtual new DynamicsCrmMigrationOperationProcessor OperationProcessor
    //    {
    //        get { return (DynamicsCrmMigrationOperationProcessor)base.OperationProcessor; }
    //    }

    //    protected override ISequence TryGetSequence(IProperty property)
    //    {
    //        return property.DynamicsCrm().TryGetSequence();
    //    }

    //    protected override IReadOnlyList<ISequence> GetSequences(IModel model)
    //    {
    //        // Check.NotNull(model, "model");

    //        return
    //            model.EntityTypes
    //                .SelectMany(t => t.Properties)
    //                .Select(TryGetSequence)
    //                .Where(s => s != null)
    //                .Distinct(this)
    //                .ToList();
    //    }

    //    protected override bool EquivalentPrimaryKeys(IKey source, IKey target, IDictionary<IProperty, IProperty> columnMap)
    //    {
    //        return
    //            base.EquivalentPrimaryKeys(source, target, columnMap)
    //            && ExtensionProvider.Extensions(source).IsClustered == ExtensionProvider.Extensions(target).IsClustered;
    //    }

    //    protected override bool EquivalentIndexes(IIndex source, IIndex target, IDictionary<IProperty, IProperty> columnMap)
    //    {
    //        return
    //            base.EquivalentIndexes(source, target, columnMap)
    //            && ExtensionProvider.Extensions(source).IsClustered == ExtensionProvider.Extensions(target).IsClustered;
    //    }       

    //    public bool Equals(ISequence x, ISequence y)
    //    {
    //        return MatchSequenceNames(x, y) && MatchSequenceSchemas(x, y);
    //    }

    //    public int GetHashCode(ISequence obj)
    //    {
    //        return obj.GetHashCode();
    //    }
    //}

    public class DynamicsCrmModelDiffer : ModelDiffer
    {
        public DynamicsCrmModelDiffer( DynamicsCrmTypeMapper typeMapper)
            : base(typeMapper)
        {
        }

        #region IModel

        private static readonly LazyRef<Sequence> _defaultSequence =
            new LazyRef<Sequence>(() => new Sequence(Sequence.DefaultName));

        protected override IEnumerable<MigrationOperation> Diff(IModel source, IModel target)
        {
            var operations = base.Diff(source, target);

            // TODO: Remove when the default sequence is added to the model (See #1568)
            var sourceUsesDefaultSequence = DefaultSequenceUsed(source);
            var targetUsesDefaultSequence = DefaultSequenceUsed(target);
            if (sourceUsesDefaultSequence == false && targetUsesDefaultSequence == true)
            {
                operations = operations.Concat(Add(_defaultSequence.Value));
            }
            else if (sourceUsesDefaultSequence == true && targetUsesDefaultSequence == false)
            {
                operations = operations.Concat(Remove(_defaultSequence.Value));
            }

            return operations;
        }

        private bool DefaultSequenceUsed(IModel model) {
              model != null
            && model.DynamicsCrm().DefaultSequenceName == null
            && (model.DynamicsCrm().ValueGenerationStrategy == DynamicsCrmValueGenerationStrategy.Sequence
                || model.EntityTypes.SelectMany(t => t.Properties).Any(
                    p => p.DynamicsCrm().ValueGenerationStrategy == DynamicsCrmValueGenerationStrategy.Sequence
                        && p.DynamicsCrm().SequenceName == null));
        }
          

        #endregion

        #region IProperty

        protected override IEnumerable<MigrationOperation> Diff(IProperty source, IProperty target)
        {
            var operations = base.Diff(source, target).ToList();

            var sourceValueGenerationStrategy = GetValueGenerationStrategy(source);
            var targetValueGenerationStrategy = GetValueGenerationStrategy(target);

            var alterColumnOperation = operations.OfType<AlterColumnOperation>().SingleOrDefault();
            if (alterColumnOperation == null
                && (source.DynamicsCrm().ComputedExpression != target.DynamicsCrm().ComputedExpression
                    || sourceValueGenerationStrategy != targetValueGenerationStrategy))
            {
                alterColumnOperation = new AlterColumnOperation(
                    source.EntityType.Relational().Table,
                    source.EntityType.Relational().Schema,
                    new ColumnModel(
                        source.Relational().Column,
                        TypeMapper.GetTypeMapping(target).StoreTypeName,
                        target.IsNullable,
                        target.Relational().DefaultValue,
                        target.Relational().DefaultExpression));
                operations.Add(alterColumnOperation);
            }

            if (alterColumnOperation != null)
            {
                if (targetValueGenerationStrategy == DynamicsCrmValueGenerationStrategy.Identity)
                {
                    alterColumnOperation.Column[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.ValueGeneration] =
                        targetValueGenerationStrategy.ToString();
                }

                if (target.DynamicsCrm().ComputedExpression != null)
                {
                    alterColumnOperation.Column[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.ColumnComputedExpression] =
                        target.DynamicsCrm().ComputedExpression;
                }
            }

            return operations;
        }

        protected override IEnumerable<MigrationOperation> Add(IProperty target)
        {
            var operation = base.Add(target).Cast<AddColumnOperation>().Single();

            var targetValueGenerationStrategy = GetValueGenerationStrategy(target);

            if (targetValueGenerationStrategy == DynamicsCrmValueGenerationStrategy.Identity)
            {
                operation.Column[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.ValueGeneration] =
                    targetValueGenerationStrategy.ToString();
            }

            if (target.DynamicsCrm().ComputedExpression != null)
            {
                operation.Column[DynamicsCrmAnnotationNames.Prefix +DynamicsCrmAnnotationNames.ColumnComputedExpression] =
                    target.DynamicsCrm().ComputedExpression;
            }

            yield return operation;
        }

        // TODO: Move to metadata API?
        // See Issue #1271: Principal keys need to generate values on add, but the database should only have one Identity column.
        private DynamicsCrmValueGenerationStrategy? GetValueGenerationStrategy(IProperty property){
            property.DynamicsCrm().ValueGenerationStrategy
              ?? property.EntityType.Model.DynamicsCrm().ValueGenerationStrategy
              ?? (property.GenerateValueOnAdd && property.PropertyType.IsInteger() && property.IsPrimaryKey()
                  ? DynamicsCrmValueGenerationStrategy.Identity
                  : default(DynamicsCrmValueGenerationStrategy?));
        }
        
          

        #endregion

        #region IKey

        protected override IEnumerable<MigrationOperation> Diff(IKey source, IKey target)
        {
            var operations = base.Diff(source, target).ToList();

            var addOperation = operations.SingleOrDefault(o => o is AddPrimaryKeyOperation || o is AddUniqueConstraintOperation);
            if (addOperation == null)
            {
                if (source.DynamicsCrm().IsClustered != target.DynamicsCrm().IsClustered)
                {
                    if (source != null
                        && !operations.Any(o => o is DropPrimaryKeyOperation || o is DropUniqueConstraintOperation))
                    {
                        operations.AddRange(Remove(source));
                    }

                    operations.AddRange(Add(target));
                }
            }
            else if (target.DynamicsCrm().IsClustered != null)
            {
                addOperation[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.Clustered] =
                    target.DynamicsCrm().IsClustered.ToString();
            }

            return operations;
        }

        protected override IEnumerable<MigrationOperation> Add(IKey target)
        {
            var operation = base.Add(target)
                .Single(o => o is AddPrimaryKeyOperation || o is AddUniqueConstraintOperation);

            if (target.DynamicsCrm().IsClustered != null)
            {
                operation[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.Clustered] =
                    target.DynamicsCrm().IsClustered.ToString();
            }

            yield return operation;
        }

        #endregion

        #region IIndex

        protected override IEnumerable<MigrationOperation> Diff(IIndex source, IIndex target)
        {
            var operations = base.Diff(source, target).ToList();

            var createIndexOperation = operations.OfType<CreateIndexOperation>().SingleOrDefault();
            if (createIndexOperation == null
                && source.DynamicsCrm().IsClustered != target.DynamicsCrm().IsClustered)
            {
                operations.AddRange(Remove(source));

                createIndexOperation = new CreateIndexOperation(
                    target.Relational().Name,
                    target.EntityType.Relational().Table,
                    target.EntityType.Relational().Schema,
                    target.Properties.Select(p => p.Relational().Column).ToArray(),
                    target.IsUnique);
                operations.Add(createIndexOperation);
            }

            if (createIndexOperation != null && target.DynamicsCrm().IsClustered != null)
            {
                createIndexOperation[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.Clustered] =
                    target.DynamicsCrm().IsClustered.ToString();
            }

            return operations;
        }

        protected override IEnumerable<MigrationOperation> Add(IIndex target)
        {
            var operation = base.Add(target).Cast<CreateIndexOperation>().Single();

            if (target.DynamicsCrm().IsClustered != null)
            {
                operation[DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.Clustered] =
                    target.DynamicsCrm().IsClustered.ToString();
            }

            return base.Add(target);
        }

        #endregion
    }
}
