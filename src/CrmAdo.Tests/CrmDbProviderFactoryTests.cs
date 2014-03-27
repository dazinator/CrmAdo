using System.Data.Common;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    [Category("ADO")]
    [Category("ProviderFactory")]
    [TestFixture()]
    public class CrmDbProviderFactoryTests : BaseTest<CrmDbProviderFactory>
    {
        [Test]
        public void Should_Be_Able_To_Create_A_New_CrmDbProviderFactory()
        {
            var subject = new CrmDbProviderFactory();
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_CrmDbProviderFactory_Via_ProviderFactory()
        {
            var dbprovider = DbProviderFactories.GetFactory(CrmDbProviderFactory.Invariant);
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Connection()
        {
            var dbprovider = DbProviderFactories.GetFactory(CrmDbProviderFactory.Invariant);
            var conn = dbprovider.CreateConnection();
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Command()
        {
            var dbprovider = DbProviderFactories.GetFactory(CrmDbProviderFactory.Invariant);
            var command = dbprovider.CreateCommand();
        }

    }
}
