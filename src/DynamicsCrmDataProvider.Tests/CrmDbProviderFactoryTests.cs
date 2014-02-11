using System;
using System.Data;
using System.Data.Common;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;

namespace DynamicsCrmDataProvider.Tests
{
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
            var dbprovider = DbProviderFactories.GetFactory("System.Data.DynamicsCrm.2013");
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Connection()
        {
            var dbprovider = DbProviderFactories.GetFactory("System.Data.DynamicsCrm.2013");
            var conn = dbprovider.CreateConnection();
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Command()
        {
            var dbprovider = DbProviderFactories.GetFactory("System.Data.DynamicsCrm.2013");
            var command = dbprovider.CreateCommand();
        }

    }
}
