using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.EntityFramework;
using CrmAdo.Tests.Sandbox;
using Rhino.Mocks;

namespace CrmAdo.Tests.Tests.EfTests
{
    [Category("Entity Framework")]
    [TestFixture()]    
    public class ProviderServicesTests
    {
        [Test]
        public void Can_Get_Provider_Services_Instance()
        {
            var instance = CrmEfProviderServices.Instance;
        }

        [Test]
        public void Can_Get_Provider_Manifest_Token()
        {
            var instance = CrmEfProviderServices.Instance;
            using (var sandbox = RequestProviderTestsSandbox.Create())
            {
                var conn = sandbox.FakeCrmDbConnection;
                var crmVersionNumber = "SomeCrmVersionNumber";
                conn.Stub(a => a.ServerVersion).Return(crmVersionNumber);
                var token = instance.GetProviderManifestToken(sandbox.FakeCrmDbConnection);
                Assert.That(token, Is.EqualTo(crmVersionNumber));
            }           

        }

        [Test]
        public void Can_Get_Provider_Manifest()
        {
            var instance = CrmEfProviderServices.Instance;
            var token = "SomeCrmVersionNumber";
            var manifest = instance.GetProviderManifest(token);
            Assert.That(manifest, Is.Not.Null);

            //using (var sandbox = RequestProviderTestsSandbox.Create())
            //{
            //    var conn = sandbox.FakeCrmDbConnection;
               
            //}

        }

    }
}
