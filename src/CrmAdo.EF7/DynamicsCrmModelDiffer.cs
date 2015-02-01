using CrmAdo.EntityFramework.Metadata;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework
{
    public class DynamicsCrmModelDiffer : ModelDiffer, IEqualityComparer<ISequence>
    {
        public DynamicsCrmModelDiffer(
            DynamicsCrmMetadataExtensionProvider extensionProvider,
           DynamicsCrmTypeMapper typeMapper,
            DynamicsCrmMigrationOperationFactory operationFactory,
            DynamicsCrmMigrationOperationProcessor operationProcessor)
            : base(
                extensionProvider,
                typeMapper,
                operationFactory,
                operationProcessor)
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

        public virtual new DynamicsCrmMigrationOperationProcessor OperationProcessor
        {
            get { return (DynamicsCrmMigrationOperationProcessor)base.OperationProcessor; }
        }

        protected override ISequence TryGetSequence(IProperty property)
        {
            return property.DynamicsCrm().TryGetSequence();
        }

        protected override IReadOnlyList<ISequence> GetSequences(IModel model)
        {
            // Check.NotNull(model, "model");

            return
                model.EntityTypes
                    .SelectMany(t => t.Properties)
                    .Select(TryGetSequence)
                    .Where(s => s != null)
                    .Distinct(this)
                    .ToList();
        }

        protected override bool EquivalentPrimaryKeys(IKey source, IKey target, IDictionary<IProperty, IProperty> columnMap)
        {
            return
                base.EquivalentPrimaryKeys(source, target, columnMap)
                && ExtensionProvider.Extensions(source).IsClustered == ExtensionProvider.Extensions(target).IsClustered;
        }

        protected override bool EquivalentIndexes(IIndex source, IIndex target, IDictionary<IProperty, IProperty> columnMap)
        {
            return
                base.EquivalentIndexes(source, target, columnMap)
                && ExtensionProvider.Extensions(source).IsClustered == ExtensionProvider.Extensions(target).IsClustered;
        }       

        public bool Equals(ISequence x, ISequence y)
        {
            return MatchSequenceNames(x, y) && MatchSequenceSchemas(x, y);
        }

        public int GetHashCode(ISequence obj)
        {
            return obj.GetHashCode();
        }
    }
}
