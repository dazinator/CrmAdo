using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Relational.Query;
using Microsoft.Data.Entity.Relational.Query.Methods;
using Microsoft.Data.Entity.Relational.Query.Sql;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Microsoft.Data.Entity.DynamicsCrm.Query
{
    public class DynamicsCrmQueryCompilationContext : RelationalQueryCompilationContext
    {
        public DynamicsCrmQueryCompilationContext(
            IModel model,
            ILogger logger,
            ILinqOperatorProvider linqOperatorProvider,
            IResultOperatorHandler resultOperatorHandler,
            EntityMaterializerSource entityMaterializerSource,
            EntityKeyFactorySource entityKeyFactorySource,
            IQueryMethodProvider queryMethodProvider,
            IMethodCallTranslator methodCallTranslator)
            : base(
                model,
                logger,
                linqOperatorProvider,
                resultOperatorHandler,
                entityMaterializerSource,
                entityKeyFactorySource,
                queryMethodProvider,
                methodCallTranslator)
        {
        }

        public override ISqlQueryGenerator CreateSqlQueryGenerator()
        {
            return new DynamicsCrmQueryGenerator();
        }

        public override string GetTableName(IEntityType entityType)
        {
            //Check.NotNull(entityType, nameof(entityType));

            return entityType.DynamicsCrm().Table;
        }

        public override string GetSchema(IEntityType entityType)
        {
           // Check.NotNull(entityType, nameof(entityType));

            return entityType.DynamicsCrm().Schema;
        }

        public override string GetColumnName(IProperty property)
        {
           // Check.NotNull(property, nameof(property));

            return property.DynamicsCrm().Column;
        }
    }
}
