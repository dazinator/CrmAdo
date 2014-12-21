using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Mocks;

namespace CrmAdo.Tests
{
    /// <summary>
    /// Provides a sandbox, with fake / mock implementation of services registered in the current container.
    /// </summary>
    public class ConnectionTestsSandbox : UnitTestSandboxContainer
    {
        public ConnectionTestsSandbox()
            : base()
        {

            // Arrange by registering our fake services into the test container.
            FakeOrgService = this.RegisterMockInstance<IOrganizationService, IDisposable>();
            FakeServiceProvider = this.RegisterMockInstance<ICrmServiceProvider>();
            FakeServiceProvider.Stub(c => c.GetOrganisationService()).Return(FakeOrgService);

            FakeConnectionProvider = this.RegisterMockInstance<ICrmConnectionProvider>();
            FakeConnectionProvider.Stub(c => c.OrganisationServiceConnectionString).Return("fakeconn");
            FakeServiceProvider.Stub(c => c.ConnectionProvider).Return(FakeConnectionProvider);

            FakeMetadataProvider = new FakeContactMetadataProvider();
            this.Container.Register<ICrmMetaDataProvider>(FakeMetadataProvider);
        }


        public IOrganizationService FakeOrgService { get; private set; }
        public ICrmServiceProvider FakeServiceProvider { get; private set; }
        public ICrmConnectionProvider FakeConnectionProvider { get; private set; }
        public ICrmMetaDataProvider FakeMetadataProvider { get; private set; }

        public static ConnectionTestsSandbox Create()
        {
            return new ConnectionTestsSandbox();
        }

    }
}
