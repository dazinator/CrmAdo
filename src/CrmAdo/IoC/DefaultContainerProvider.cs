using CrmAdo.Core;
using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using CrmAdo.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyIoC;

namespace CrmAdo.IoC
{
    public class DefaultContainerProvider : IContainerProvider
    {
        public IContainer GetContainer()
        {
            var container = new TinyIoCContainer();
            container.Register<ICrmServiceProvider, CrmServiceProvider>().AsMultiInstance();
            container.Register<ICrmMetaDataProvider, InMemoryCachedCrmMetaDataProvider>().AsMultiInstance(); 

            container.Register<ISchemaCollectionsProvider, SchemaCollectionsProvider>(); // singleton instance.

            container.Register<ICrmCommandExecutor, CrmCommandExecutor>().AsMultiInstance(); ;
            container.Register<ICrmRequestProvider, VisitingCrmRequestProvider>().AsMultiInstance(); ;

            container.Register<IDynamicsAttributeTypeProvider, DynamicsAttributeTypeProvider>(); // singleton instance.
            container.Register<ICrmMetadataNamingConventionProvider, CrmAdoCrmMetadataNamingProvider>(); // singleton instance.
            container.Register<ISchemaTableProvider, SchemaTableProvider>(); // singleton instance.
            container.Register<ICrmClientCredentialsProvider, CrmClientCredentialsProvider>();  // singleton instance.
            container.Register<ICrmConnectionProvider, ExplicitConnectionStringProviderWithFallbackToConfig>(); // singleton instance.

            container.Register<ICrmOrganisationManager, CrmOrganisationManager>();
            container.Register<IEntityMetadataRepository, EntityMetadataRepository>();
            return container;
        }
    }
}
