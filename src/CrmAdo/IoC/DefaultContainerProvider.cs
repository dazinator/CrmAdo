using CrmAdo.Core;
using CrmAdo.Dynamics;
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

            container.Register<IDynamicsAttributeTypeProvider, DynamicsAttributeTypeProvider>(); // singleton instance.
            container.Register<ICrmMetadataNamingConventionProvider, CrmAdoCrmMetadataNamingProvider>(); // singleton instance.
            container.Register<ISchemaTableProvider, SchemaTableProvider>(); // singleton instance.
            container.Register<ICrmClientCredentialsProvider, CrmClientCredentialsProvider>();  // singleton instance.
            container.Register<ICrmConnectionProvider, ExplicitConnectionStringProviderWithFallbackToConfig>(); // singleton instance.

            IOrgCommandExecutor commandExecutor = CrmOrgCommandExecutor.Instance;
            container.Register<IOrgCommandExecutor>(commandExecutor); // singleton instance.

            container.Register<IOrganisationCommandProvider, SqlGenerationOrganizationCommandProvider>().AsMultiInstance(); ;

            container.Register<ICrmOrganisationManager, CrmOrganisationManager>();
            container.Register<IEntityMetadataRepository, EntityMetadataRepository>();

            // When CrmDbCommands are resolved, use the contructor that specifies a null connection, but provide rest of dependencies from container.
            // Issue raised: https://github.com/grumpydev/TinyIoC/issues/70
            //container.Register<CrmDbCommand>().UsingConstructor(() => new CrmDbCommand(null as CrmDbConnection, container.Resolve<IOrgCommandExecutor>(), container.Resolve<IOrganisationCommandProvider>()));
            //var resolve = container.Resolve<CrmDbCommand>();

            return container;
        }
    }
}
