using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;
using CrmAdo.EntityFramework;
using Microsoft.Data.Entity.DynamicsCrm.Metadata;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class DynamicsCrmMetadataExtensions
    {
        public static DynamicsCrmPropertyExtensions DynamicsCrm(this Property property)
        {
          //  Check.NotNull(property, "property");
            return new DynamicsCrmPropertyExtensions(property);
        }

        public static IDynamicsCrmPropertyExtensions DynamicsCrm(this IProperty property)
        {
            //Check.NotNull(property, "property");
            return new ReadOnlyDynamicsCrmPropertyExtensions(property);
        }

        public static DynamicsCrmEntityTypeExtensions DynamicsCrm(this EntityType entityType)
        {
           // Check.NotNull(entityType, "entityType");
            return new DynamicsCrmEntityTypeExtensions(entityType);
        }

        public static IDynamicsCrmEntityTypeExtensions DynamicsCrm(this IEntityType entityType)
        {
           // Check.NotNull(entityType, "entityType");
            return new ReadOnlyDynamicsCrmEntityTypeExtensions(entityType);
        }

        public static DynamicsCrmKeyExtensions DynamicsCrm(this Key key)
        {
           // Check.NotNull(key, "key");
            return new DynamicsCrmKeyExtensions(key);
        }

        public static IDynamicsCrmKeyExtensions DynamicsCrm(this IKey key)
        {
          //  Check.NotNull(key, "key");
            return new ReadOnlyDynamicsCrmKeyExtensions(key);
        }

        public static DynamicsCrmIndexExtensions DynamicsCrm(this Index index)
        {
           // Check.NotNull(index, "index");
            return new DynamicsCrmIndexExtensions(index);
        }

        public static IDynamicsCrmIndexExtensions DynamicsCrm(this IIndex index)
        {
           // Check.NotNull(index, "index");
            return new ReadOnlyDynamicsCrmIndexExtensions(index);
        }

        public static DynamicsCrmForeignKeyExtensions DynamicsCrm(this ForeignKey foreignKey)
        {
           // Check.NotNull(foreignKey, "foreignKey");
            return new DynamicsCrmForeignKeyExtensions(foreignKey);
        }

        public static IDynamicsCrmForeignKeyExtensions DynamicsCrm(this IForeignKey foreignKey)
        {
           // Check.NotNull(foreignKey, "foreignKey");
            return new ReadOnlyDynamicsCrmForeignKeyExtensions(foreignKey);
        }

        public static DynamicsCrmModelExtensions DynamicsCrm(this Model model)
        {
           // Check.NotNull(model, "model");
            return new DynamicsCrmModelExtensions(model);
        }

        public static IDynamicsCrmModelExtensions DynamicsCrm(this IModel model)
        {
            // Check.NotNull(model, "model");
            return new ReadOnlyDynamicsCrmModelExtensions(model);
        }
    }
}