using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.DynamicsCrm.Metadata;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{    
    public static class DynamicsCrmBuilderExtensions
    {
        public static DynamicsCrmPropertyBuilder ForSqlServer<TPropertyBuilder>(
            this IPropertyBuilder<TPropertyBuilder> propertyBuilder)
            where TPropertyBuilder : IPropertyBuilder<TPropertyBuilder>
        {
           // Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            return new DynamicsCrmPropertyBuilder(propertyBuilder.Metadata);
        }

        public static TPropertyBuilder ForSqlServer<TPropertyBuilder>(
            this IPropertyBuilder<TPropertyBuilder> propertyBuilder,
            Action<DynamicsCrmPropertyBuilder> sqlServerPropertyBuilder)
            where TPropertyBuilder : IPropertyBuilder<TPropertyBuilder>
        {
           // Check.NotNull(propertyBuilder, nameof(propertyBuilder));
           // Check.NotNull(sqlServerPropertyBuilder, nameof(sqlServerPropertyBuilder));

            sqlServerPropertyBuilder(ForSqlServer(propertyBuilder));

            return (TPropertyBuilder)propertyBuilder;
        }

        public static DynamicsCrmEntityBuilder ForSqlServer<TEntityBuilder>(
             this IEntityBuilder<TEntityBuilder> entityBuilder)
            where TEntityBuilder : IEntityBuilder<TEntityBuilder>
        {
            //Check.NotNull(entityBuilder, nameof(entityBuilder));

            return new DynamicsCrmEntityBuilder(entityBuilder.Metadata);
        }

        public static TEntityBuilder ForSqlServer<TEntityBuilder>(
            this IEntityBuilder<TEntityBuilder> entityBuilder,
            Action<DynamicsCrmEntityBuilder> relationalEntityBuilder)
            where TEntityBuilder : IEntityBuilder<TEntityBuilder>
        {
            //Check.NotNull(entityBuilder, nameof(entityBuilder));

            relationalEntityBuilder(ForSqlServer(entityBuilder));

            return (TEntityBuilder)entityBuilder;
        }

        public static DynamicsCrmEntityBuilder ForSqlServer<TEntity, TEntityBuilder>(
            this IEntityBuilder<TEntity, TEntityBuilder> entityBuilder)
            where TEntity : class
            where TEntityBuilder : IEntityBuilder<TEntity, TEntityBuilder>
        {
           // Check.NotNull(entityBuilder, nameof(entityBuilder));

            return new DynamicsCrmEntityBuilder(entityBuilder.Metadata);
        }

        public static TEntityBuilder ForSqlServer<TEntity, TEntityBuilder>(
             this IEntityBuilder<TEntity, TEntityBuilder> entityBuilder,
             Action<DynamicsCrmEntityBuilder> relationalEntityBuilder)
            where TEntity : class
            where TEntityBuilder : IEntityBuilder<TEntity, TEntityBuilder>
        {
          //  Check.NotNull(entityBuilder, nameof(entityBuilder));

            relationalEntityBuilder(ForSqlServer(entityBuilder));

            return (TEntityBuilder)entityBuilder;
        }

        public static DynamicsCrmKeyBuilder ForSqlServer<TKeyBuilder>(
            this IKeyBuilder<TKeyBuilder> keyBuilder)
            where TKeyBuilder : IKeyBuilder<TKeyBuilder>
        {
           // Check.NotNull(keyBuilder, nameof(keyBuilder));

            return new DynamicsCrmKeyBuilder(keyBuilder.Metadata);
        }

        public static TKeyBuilder ForSqlServer<TKeyBuilder>(
           this IKeyBuilder<TKeyBuilder> keyBuilder,
            Action<DynamicsCrmKeyBuilder> relationalKeyBuilder)
            where TKeyBuilder : IKeyBuilder<TKeyBuilder>
        {
           // Check.NotNull(keyBuilder, nameof(keyBuilder));
           // Check.NotNull(relationalKeyBuilder, nameof(relationalKeyBuilder));

            relationalKeyBuilder(ForSqlServer(keyBuilder));

            return (TKeyBuilder)keyBuilder;
        }

        public static DynamicsCrmForeignKeyBuilder ForSqlServer<TForeignKeyBuilder>(
           this IForeignKeyBuilder<TForeignKeyBuilder> foreignKeyBuilder)
            where TForeignKeyBuilder : IForeignKeyBuilder<TForeignKeyBuilder>
        {
          //  Check.NotNull(foreignKeyBuilder, nameof(foreignKeyBuilder));

            return new DynamicsCrmForeignKeyBuilder(foreignKeyBuilder.Metadata);
        }

        public static TForeignKeyBuilder ForSqlServer<TForeignKeyBuilder>(
           this IForeignKeyBuilder<TForeignKeyBuilder> foreignKeyBuilder,
            Action<DynamicsCrmForeignKeyBuilder> relationalForeignKeyBuilder)
            where TForeignKeyBuilder : IForeignKeyBuilder<TForeignKeyBuilder>
        {
          //  Check.NotNull(foreignKeyBuilder, nameof(foreignKeyBuilder));
          //  Check.NotNull(relationalForeignKeyBuilder, nameof(relationalForeignKeyBuilder));

            relationalForeignKeyBuilder(ForSqlServer(foreignKeyBuilder));

            return (TForeignKeyBuilder)foreignKeyBuilder;
        }

        public static DynamicsCrmIndexBuilder ForSqlServer<TIndexBuilder>(
            this IIndexBuilder<TIndexBuilder> indexBuilder)
            where TIndexBuilder : IIndexBuilder<TIndexBuilder>
        {
           // Check.NotNull(indexBuilder, nameof(indexBuilder));

            return new DynamicsCrmIndexBuilder(indexBuilder.Metadata);
        }

        public static TIndexBuilder ForSqlServer<TIndexBuilder>(
           this IIndexBuilder<TIndexBuilder> indexBuilder,
           Action<DynamicsCrmIndexBuilder> relationalIndexBuilder)
            where TIndexBuilder : IIndexBuilder<TIndexBuilder>
        {
          //  Check.NotNull(indexBuilder, nameof(indexBuilder));
          //  Check.NotNull(relationalIndexBuilder, nameof(relationalIndexBuilder));

            relationalIndexBuilder(ForSqlServer(indexBuilder));

            return (TIndexBuilder)indexBuilder;
        }

        public static DynamicsCrmForeignKeyBuilder ForSqlServer<TOneToManyBuilder>(
             this IOneToManyBuilder<TOneToManyBuilder> oneToManyBuilder)
            where TOneToManyBuilder : IOneToManyBuilder<TOneToManyBuilder>
        {
           // Check.NotNull(oneToManyBuilder, nameof(oneToManyBuilder));

            return new DynamicsCrmForeignKeyBuilder(oneToManyBuilder.Metadata);
        }

        public static TOneToManyBuilder ForSqlServer<TOneToManyBuilder>(
             this IOneToManyBuilder<TOneToManyBuilder> oneToManyBuilder,
            Action<DynamicsCrmForeignKeyBuilder> relationalOneToManyBuilder)
            where TOneToManyBuilder : IOneToManyBuilder<TOneToManyBuilder>
        {
           // Check.NotNull(oneToManyBuilder, nameof(oneToManyBuilder));
           // Check.NotNull(relationalOneToManyBuilder, nameof(relationalOneToManyBuilder));

            relationalOneToManyBuilder(ForSqlServer(oneToManyBuilder));

            return (TOneToManyBuilder)oneToManyBuilder;
        }

        public static DynamicsCrmForeignKeyBuilder ForSqlServer<TManyToOneBuilder>(
            this IManyToOneBuilder<TManyToOneBuilder> manyToOneBuilder)
            where TManyToOneBuilder : IManyToOneBuilder<TManyToOneBuilder>
        {
           // Check.NotNull(manyToOneBuilder, nameof(manyToOneBuilder));

            return new DynamicsCrmForeignKeyBuilder(manyToOneBuilder.Metadata);
        }

        public static TManyToOneBuilder ForSqlServer<TManyToOneBuilder>(
            this IManyToOneBuilder<TManyToOneBuilder> manyToOneBuilder,
            Action<DynamicsCrmForeignKeyBuilder> relationalManyToOneBuilder)
            where TManyToOneBuilder : IManyToOneBuilder<TManyToOneBuilder>
        {
           // Check.NotNull(manyToOneBuilder, nameof(manyToOneBuilder));
           // Check.NotNull(relationalManyToOneBuilder, nameof(relationalManyToOneBuilder));

            relationalManyToOneBuilder(ForSqlServer(manyToOneBuilder));

            return (TManyToOneBuilder)manyToOneBuilder;
        }

        public static DynamicsCrmForeignKeyBuilder ForSqlServer<TOneToOneBuilder>(
            this IOneToOneBuilder<TOneToOneBuilder> oneToOneBuilder)
            where TOneToOneBuilder : IOneToOneBuilder<TOneToOneBuilder>
        {
           // Check.NotNull(oneToOneBuilder, nameof(oneToOneBuilder));

            return new DynamicsCrmForeignKeyBuilder(oneToOneBuilder.Metadata);
        }

        public static TOneToOneBuilder ForSqlServer<TOneToOneBuilder>(
            this IOneToOneBuilder<TOneToOneBuilder> oneToOneBuilder,
           Action<DynamicsCrmForeignKeyBuilder> relationalOneToOneBuilder)
            where TOneToOneBuilder : IOneToOneBuilder<TOneToOneBuilder>
        {
            //Check.NotNull(oneToOneBuilder, nameof(oneToOneBuilder));
            //Check.NotNull(relationalOneToOneBuilder, nameof(relationalOneToOneBuilder));

            relationalOneToOneBuilder(ForSqlServer(oneToOneBuilder));

            return (TOneToOneBuilder)oneToOneBuilder;
        }

        public static DynamicsCrmModelBuilder ForSqlServer<TModelBuilder>(
            this IModelBuilder<TModelBuilder> modelBuilder)
            where TModelBuilder : IModelBuilder<TModelBuilder>
        {
           // Check.NotNull(modelBuilder, nameof(modelBuilder));

            return new DynamicsCrmModelBuilder(modelBuilder.Metadata);
        }

        public static TModelBuilder ForSqlServer<TModelBuilder>(
            this IModelBuilder<TModelBuilder> modelBuilder,
             Action<DynamicsCrmModelBuilder> sqlServerModelBuilder)
            where TModelBuilder : IModelBuilder<TModelBuilder>
        {
           // Check.NotNull(modelBuilder, nameof(modelBuilder));
           // Check.NotNull(sqlServerModelBuilder, nameof(sqlServerModelBuilder));

            sqlServerModelBuilder(ForSqlServer(modelBuilder));

            return (TModelBuilder)modelBuilder;
        }
    }

}
