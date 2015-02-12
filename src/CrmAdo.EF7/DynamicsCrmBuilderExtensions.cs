using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata;
using CrmAdo.EntityFramework.Metadata;
using Microsoft.Data.Entity.Utilities;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class DynamicsCrmBuilderExtensions
    {
        public static DynamicsCrmPropertyBuilder ForDynamicsCrm<TPropertyBuilder>(
           this IPropertyBuilder<TPropertyBuilder> propertyBuilder)
           where TPropertyBuilder : IPropertyBuilder<TPropertyBuilder>
        {
            //Check.NotNull(propertyBuilder, "propertyBuilder");
            return new DynamicsCrmPropertyBuilder(propertyBuilder.Metadata);
        }

        public static TPropertyBuilder ForDynamicsCrm<TPropertyBuilder>(
             this IPropertyBuilder<TPropertyBuilder> propertyBuilder,
             Action<DynamicsCrmPropertyBuilder> propertyBuilderAction)
            where TPropertyBuilder : IPropertyBuilder<TPropertyBuilder>
        {
            //Check.NotNull(propertyBuilder, "propertyBuilder");
            //Check.NotNull(sqlServerPropertyBuilder, "sqlServerPropertyBuilder");

            propertyBuilderAction(ForDynamicsCrm(propertyBuilder));
            return (TPropertyBuilder)propertyBuilder;
        }

        public static DynamicsCrmEntityBuilder ForDynamicsCrm<TEntityBuilder>(
             this IEntityBuilder<TEntityBuilder> entityBuilder)
            where TEntityBuilder : IEntityBuilder<TEntityBuilder>
        {
           // Check.NotNull(entityBuilder, "entityBuilder");

            return new DynamicsCrmEntityBuilder(entityBuilder.Metadata);
        }

        public static TEntityBuilder ForDynamicsCrm<TEntityBuilder>(
             this IEntityBuilder<TEntityBuilder> entityBuilder,
             Action<DynamicsCrmEntityBuilder> relationalEntityBuilder)
            where TEntityBuilder : IEntityBuilder<TEntityBuilder>
        {
            //Check.NotNull(entityBuilder, "entityBuilder");

            relationalEntityBuilder(ForDynamicsCrm(entityBuilder));

            return (TEntityBuilder)entityBuilder;
        }

        public static DynamicsCrmEntityBuilder ForDynamicsCrm<TEntity, TEntityBuilder>(
            this IEntityBuilder<TEntity, TEntityBuilder> entityBuilder)
            where TEntity : class
            where TEntityBuilder : IEntityBuilder<TEntity, TEntityBuilder>
        {
           // Check.NotNull(entityBuilder, "entityBuilder");

            return new DynamicsCrmEntityBuilder(entityBuilder.Metadata);
        }

        public static TEntityBuilder ForDynamicsCrm<TEntity, TEntityBuilder>(
            this IEntityBuilder<TEntity, TEntityBuilder> entityBuilder,
            Action<DynamicsCrmEntityBuilder> relationalEntityBuilder)
            where TEntity : class
            where TEntityBuilder : IEntityBuilder<TEntity, TEntityBuilder>
        {
           // Check.NotNull(entityBuilder, "entityBuilder");

            relationalEntityBuilder(ForDynamicsCrm(entityBuilder));

            return (TEntityBuilder)entityBuilder;
        }

        public static DynamicsCrmKeyBuilder ForDynamicsCrm<TKeyBuilder>(
             this IKeyBuilder<TKeyBuilder> keyBuilder)
            where TKeyBuilder : IKeyBuilder<TKeyBuilder>
        {
            //Check.NotNull(keyBuilder, "keyBuilder");

            return new DynamicsCrmKeyBuilder(keyBuilder.Metadata);
        }

        public static TKeyBuilder ForDynamicsCrm<TKeyBuilder>(
            this IKeyBuilder<TKeyBuilder> keyBuilder,
            Action<DynamicsCrmKeyBuilder> relationalKeyBuilder)
            where TKeyBuilder : IKeyBuilder<TKeyBuilder>
        {
           // Check.NotNull(keyBuilder, "keyBuilder");
          //  Check.NotNull(relationalKeyBuilder, "relationalKeyBuilder");

            relationalKeyBuilder(ForDynamicsCrm(keyBuilder));
            return (TKeyBuilder)keyBuilder;
        }

        public static DynamicsCrmForeignKeyBuilder ForDynamicsCrm<TForeignKeyBuilder>(
            this IForeignKeyBuilder<TForeignKeyBuilder> foreignKeyBuilder)
            where TForeignKeyBuilder : IForeignKeyBuilder<TForeignKeyBuilder>
        {
           // Check.NotNull(foreignKeyBuilder, "foreignKeyBuilder");
            return new DynamicsCrmForeignKeyBuilder(foreignKeyBuilder.Metadata);
        }

        public static TForeignKeyBuilder ForDynamicsCrm<TForeignKeyBuilder>(
            this IForeignKeyBuilder<TForeignKeyBuilder> foreignKeyBuilder,
           Action<DynamicsCrmForeignKeyBuilder> relationalForeignKeyBuilder)
            where TForeignKeyBuilder : IForeignKeyBuilder<TForeignKeyBuilder>
        {
           // Check.NotNull(foreignKeyBuilder, "foreignKeyBuilder");
          //  Check.NotNull(relationalForeignKeyBuilder, "relationalForeignKeyBuilder");

            relationalForeignKeyBuilder(ForDynamicsCrm(foreignKeyBuilder));
            return (TForeignKeyBuilder)foreignKeyBuilder;
        }

        public static DynamicsCrmIndexBuilder ForDynamicsCrm<TIndexBuilder>(
            this IIndexBuilder<TIndexBuilder> indexBuilder)
            where TIndexBuilder : IIndexBuilder<TIndexBuilder>
        {
           // Check.NotNull(indexBuilder, "indexBuilder");
            return new DynamicsCrmIndexBuilder(indexBuilder.Metadata);
        }

        public static TIndexBuilder ForDynamicsCrm<TIndexBuilder>(
          this IIndexBuilder<TIndexBuilder> indexBuilder,
            Action<DynamicsCrmIndexBuilder> relationalIndexBuilder)
            where TIndexBuilder : IIndexBuilder<TIndexBuilder>
        {
            //Check.NotNull(indexBuilder, "indexBuilder");
           // Check.NotNull(relationalIndexBuilder, "relationalIndexBuilder");

            relationalIndexBuilder(ForDynamicsCrm(indexBuilder));
            return (TIndexBuilder)indexBuilder;
        }

        public static DynamicsCrmForeignKeyBuilder ForDynamicsCrm<TOneToManyBuilder>(
             this IOneToManyBuilder<TOneToManyBuilder> oneToManyBuilder)
            where TOneToManyBuilder : IOneToManyBuilder<TOneToManyBuilder>
        {
            //Check.NotNull(oneToManyBuilder, "oneToManyBuilder");
            return new DynamicsCrmForeignKeyBuilder(oneToManyBuilder.Metadata);
        }

        public static TOneToManyBuilder ForDynamicsCrm<TOneToManyBuilder>(
           this IOneToManyBuilder<TOneToManyBuilder> oneToManyBuilder,
            Action<DynamicsCrmForeignKeyBuilder> relationalOneToManyBuilder)
            where TOneToManyBuilder : IOneToManyBuilder<TOneToManyBuilder>
        {
           // Check.NotNull(oneToManyBuilder, "oneToManyBuilder");
           // Check.NotNull(relationalOneToManyBuilder, "relationalOneToManyBuilder");

            relationalOneToManyBuilder(ForDynamicsCrm(oneToManyBuilder));

            return (TOneToManyBuilder)oneToManyBuilder;
        }

        public static DynamicsCrmForeignKeyBuilder ForDynamicsCrm<TManyToOneBuilder>(
            this IManyToOneBuilder<TManyToOneBuilder> manyToOneBuilder)
            where TManyToOneBuilder : IManyToOneBuilder<TManyToOneBuilder>
        {
           // Check.NotNull(manyToOneBuilder, "manyToOneBuilder");

            return new DynamicsCrmForeignKeyBuilder(manyToOneBuilder.Metadata);
        }

        public static TManyToOneBuilder ForDynamicsCrm<TManyToOneBuilder>(
           this IManyToOneBuilder<TManyToOneBuilder> manyToOneBuilder,
            Action<DynamicsCrmForeignKeyBuilder> relationalManyToOneBuilder)
            where TManyToOneBuilder : IManyToOneBuilder<TManyToOneBuilder>
        {
          //  Check.NotNull(manyToOneBuilder, "manyToOneBuilder");
          //  Check.NotNull(relationalManyToOneBuilder, "relationalManyToOneBuilder");

            relationalManyToOneBuilder(ForDynamicsCrm(manyToOneBuilder));

            return (TManyToOneBuilder)manyToOneBuilder;
        }

        public static DynamicsCrmForeignKeyBuilder ForDynamicsCrm<TOneToOneBuilder>(
            this IOneToOneBuilder<TOneToOneBuilder> oneToOneBuilder)
            where TOneToOneBuilder : IOneToOneBuilder<TOneToOneBuilder>
        {
           // Check.NotNull(oneToOneBuilder, "oneToOneBuilder");

            return new DynamicsCrmForeignKeyBuilder(oneToOneBuilder.Metadata);
        }

        public static TOneToOneBuilder ForDynamicsCrm<TOneToOneBuilder>(
           this IOneToOneBuilder<TOneToOneBuilder> oneToOneBuilder,
           Action<DynamicsCrmForeignKeyBuilder> relationalOneToOneBuilder)
            where TOneToOneBuilder : IOneToOneBuilder<TOneToOneBuilder>
        {
           // Check.NotNull(oneToOneBuilder, "oneToOneBuilder");
           // Check.NotNull(relationalOneToOneBuilder, "relationalOneToOneBuilder");

            relationalOneToOneBuilder(ForDynamicsCrm(oneToOneBuilder));

            return (TOneToOneBuilder)oneToOneBuilder;
        }

        public static DynamicsCrmModelBuilder ForDynamicsCrm<TModelBuilder>(
            this IModelBuilder<TModelBuilder> modelBuilder)
            where TModelBuilder : IModelBuilder<TModelBuilder>
        {
            //Check.NotNull(modelBuilder, "modelBuilder");

            return new DynamicsCrmModelBuilder(modelBuilder.Metadata);
        }

        public static TModelBuilder ForDynamicsCrm<TModelBuilder>(
            this IModelBuilder<TModelBuilder> modelBuilder,
            Action<DynamicsCrmModelBuilder> dynamicsCrmModelBuilder)
            where TModelBuilder : IModelBuilder<TModelBuilder>
        {
           // Check.NotNull(modelBuilder, "modelBuilder");
           // Check.NotNull(DynamicsCrmModelBuilder, "DynamicsCrmModelBuilder");

            dynamicsCrmModelBuilder(ForDynamicsCrm(modelBuilder));

            return (TModelBuilder)modelBuilder;
        }
    }
}
