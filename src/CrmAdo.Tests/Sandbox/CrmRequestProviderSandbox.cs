using CrmAdo.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Sandbox
{
    public class CrmRequestProviderTestsSandbox : UnitTestSandboxContainer
    {

        public CrmRequestProviderTestsSandbox()
            : base()
        {
            FakeMetadataProvider = new FakeContactMetadataProvider();
            this.Container.Register<ICrmMetaDataProvider>(FakeMetadataProvider);

            this.Container.Register<IDynamicsAttributeTypeProvider, DynamicsAttributeTypeProvider>();   // singleton
        }


        public ICrmMetaDataProvider FakeMetadataProvider { get; private set; }

        public IDynamicsAttributeTypeProvider DynamicsAttributeTypeProvider { get; private set; }

        public static CrmRequestProviderTestsSandbox Create()
        {
            return new CrmRequestProviderTestsSandbox();
        }


    }
}
