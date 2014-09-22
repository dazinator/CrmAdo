using System.Data.Common;
using NUnit.Framework;
using System;

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
        public void Should_Be_Able_To_Resolve_CrmDbProviderFactory_Type()
        {
            var x = Type.GetType("CrmAdo.CrmDbProviderFactory, CrmAdo");
            Assert.That(x, Is.Not.Null);
            // var dbprovider = DbProviderFactories.GetFactory(CrmDbProviderFactory.Invariant);
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
        public void Should_Be_Able_To_Create_A_New_Command_Via_Provider_Factory()
        {
            var dbprovider = DbProviderFactories.GetFactory(CrmDbProviderFactory.Invariant);
            var command = dbprovider.CreateCommand();
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_DataAdaptor_Via_Provider_Factory()
        {
            var dbprovider = DbProviderFactories.GetFactory(CrmDbProviderFactory.Invariant);
            var command = dbprovider.CreateDataAdapter();
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_CommandBuilder_Via_Provider_Factory()
        {
            var dbprovider = DbProviderFactories.GetFactory(CrmDbProviderFactory.Invariant);
            var command = dbprovider.CreateCommandBuilder();
        }

    }
}
