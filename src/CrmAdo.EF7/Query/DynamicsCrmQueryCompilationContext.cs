using Microsoft.Data.Entity;
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


namespace CrmAdo.EntityFramework.Query
{   

    public class DynamicsCrmQueryCompilationContext : RelationalQueryCompilationContext
    {
        public DynamicsCrmQueryCompilationContext(
            IModel model,
            ILogger logger,
            ILinqOperatorProvider linqOperatorProvider,
            IResultOperatorHandler resultOperatorHandler,
            EntityMaterializerSource entityMaterializerSource,
            IQueryMethodProvider queryMethodProvider,
            IMethodCallTranslator methodCallTranslator)
            : base(model, logger, linqOperatorProvider, resultOperatorHandler, entityMaterializerSource, queryMethodProvider, methodCallTranslator)
                //Check.NotNull(model, "model"),
                //Check.NotNull(logger, "logger"),
                //Check.NotNull(linqOperatorProvider, "linqOperatorProvider"),
                //Check.NotNull(resultOperatorHandler, "resultOperatorHandler"),
                //Check.NotNull(entityMaterializerSource, "entityMaterializerSource"),
                //Check.NotNull(queryMethodProvider, "queryMethodProvider"),
                //Check.NotNull(methodCallTranslator, "methodCallTranslator"))
        {
        }

        public override ISqlQueryGenerator CreateSqlQueryGenerator()
        {
            return new DynamicsCrmQueryGenerator();
        }

        public override string GetTableName(IEntityType entityType)
        {
           // Check.NotNull(entityType, "entityType");

            return entityType.DynamicsCrm().Table;
        }

        public override string GetSchema(IEntityType entityType)
        {
           // Check.NotNull(entityType, "entityType");

            return entityType.DynamicsCrm().Schema;
        }

        public override string GetColumnName(IProperty property)
        {
          //  Check.NotNull(property, "property");

            return property.DynamicsCrm().Column;
        }
    }
}
