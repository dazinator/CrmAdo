using CrmAdo.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Mocks;
using CrmAdo.Core;

namespace CrmAdo.Tests.Sandbox
{
    public class RequestProviderTestsSandbox : UnitTestSandboxContainer
    {

        public RequestProviderTestsSandbox()
            : base()
        {
            FakeMetadataProvider = new FakeContactMetadataProvider();
            this.Container.Register<ICrmMetaDataProvider>(FakeMetadataProvider);
            this.Container.Register<IDynamicsAttributeTypeProvider, DynamicsAttributeTypeProvider>();   // singleton

            FakeSettings = this.RegisterMockInstance<ConnectionSettings>();

            FakeCrmDbConnection = this.RegisterMockInstance<CrmDbConnection>();
            FakeCrmDbConnection.Stub(a => a.MetadataProvider).Return(FakeMetadataProvider);
            FakeCrmDbConnection.Stub(a => a.Settings).Return(FakeSettings);
        }


        public ICrmMetaDataProvider FakeMetadataProvider { get; private set; }

        public IDynamicsAttributeTypeProvider DynamicsAttributeTypeProvider { get; private set; }

        public CrmDbConnection FakeCrmDbConnection { get; private set; }

        public ConnectionSettings FakeSettings { get; private set; }

        public static RequestProviderTestsSandbox Create()
        {
            return new RequestProviderTestsSandbox();
        }


    }
}
