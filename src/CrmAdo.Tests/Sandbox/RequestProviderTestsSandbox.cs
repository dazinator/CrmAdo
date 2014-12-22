using CrmAdo.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Mocks;

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

            FakeCrmDbConnection = this.RegisterMockInstance<CrmDbConnection>();
            FakeCrmDbConnection.Stub(a => a.MetadataProvider).Return(FakeMetadataProvider);
        }


        public ICrmMetaDataProvider FakeMetadataProvider { get; private set; }

        public IDynamicsAttributeTypeProvider DynamicsAttributeTypeProvider { get; private set; }

        public CrmDbConnection FakeCrmDbConnection { get; private set; }

        public static RequestProviderTestsSandbox Create()
        {
            return new RequestProviderTestsSandbox();
        }


    }
}
