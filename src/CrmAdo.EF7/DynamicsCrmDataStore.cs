using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.Storage;
using Remotion.Linq;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Framework.Logging;
using Microsoft.Data.Entity.Relational.Query;
using Microsoft.Data.Entity.Query;
using Microsoft.Data.Entity.Relational.Query.Methods;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.DynamicsCrm.Update;
using Microsoft.Data.Entity.DynamicsCrm.Query;

namespace Microsoft.Data.Entity.DynamicsCrm
{

    public class DynamicsCrmDataStore : RelationalDataStore
    {
        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected DynamicsCrmDataStore()
        {
        }

        public DynamicsCrmDataStore(
           StateManager stateManager,
           DbContextService<IModel> model,
           EntityKeyFactorySource entityKeyFactorySource,
           EntityMaterializerSource entityMaterializerSource,
           ClrCollectionAccessorSource collectionAccessorSource,
           ClrPropertySetterSource propertySetterSource,
           DynamicsCrmConnection connection,
           DynamicsCrmCommandBatchPreparer batchPreparer,
           DynamicsCrmBatchExecutor batchExecutor,
           DbContextService<IDbContextOptions> options,
           ILoggerFactory loggerFactory)
            : base(
             stateManager,
             model,
             entityKeyFactorySource,
             entityMaterializerSource,
             collectionAccessorSource,
             propertySetterSource,
             connection,
             batchPreparer,
             batchExecutor,
             options,
             loggerFactory)
        {
        }

        protected override RelationalValueReaderFactory ValueReaderFactory
        {
            get
            {
                return new RelationalObjectArrayValueReaderFactory();
            }
        }

        //protected override RelationalValueReaderFactory ValueReaderFactory()
        //{
            
        //}


        protected override RelationalQueryCompilationContext CreateQueryCompilationContext(
            ILinqOperatorProvider linqOperatorProvider,
            IResultOperatorHandler resultOperatorHandler,
            IQueryMethodProvider enumerableMethodProvider,
            IMethodCallTranslator methodCallTranslator)
        {
            // Check.NotNull(linqOperatorProvider, nameof(linqOperatorProvider));
            // Check.NotNull(resultOperatorHandler, nameof(resultOperatorHandler));
            // Check.NotNull(enumerableMethodProvider, nameof(enumerableMethodProvider));
            // Check.NotNull(methodCallTranslator, nameof(methodCallTranslator));

            return new DynamicsCrmQueryCompilationContext(
                Model,
                Logger,
                linqOperatorProvider,
                resultOperatorHandler,
                EntityMaterializerSource,
                EntityKeyFactorySource,
                enumerableMethodProvider,
                methodCallTranslator);
        }
    }


}
