using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.DdexProvider;
using System.Configuration;
using Rhino.Mocks;
using Microsoft.VisualStudio.Data.Services;

namespace CrmAdo.DdexProvider.IntegrationTests
{
    [TestFixture()]
    [Category("DDEX")]
    [Category("Metadata")]
    public class CrmObjectSelectorTests : BaseTest
    {


        [Test(Description = "Integration test for selecting root for ddex.")]
        public void Should_Be_Able_To_Select_Root()
        {

            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();

                IVsDataConnection mockVsDataConnection = MockRepository.GenerateMock<IVsDataConnection>();
                mockVsDataConnection.Stub(c => c.State).Return(DataConnectionState.Open);
                mockVsDataConnection.Stub(c => c.GetLockedProviderObject()).Return(conn);
                mockVsDataConnection.Stub(c => c.UnlockProviderObject());

                var sut = new CrmObjectSelector(mockVsDataConnection);

                var reader = sut.SelectObjects(CrmObjectTypes.Root, null, null);
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("ROOT: " + reader.GetItem("name"));
                    }

                }
            }

        }

        [Test(Description = "Integration test for selecting entities for ddex.")]
        public void Should_Be_Able_To_Select_Entities()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();

                IVsDataConnection mockVsDataConnection = MockRepository.GenerateMock<IVsDataConnection>();
                mockVsDataConnection.Stub(c => c.State).Return(DataConnectionState.Open);
                mockVsDataConnection.Stub(c => c.GetLockedProviderObject()).Return(conn);
                mockVsDataConnection.Stub(c => c.UnlockProviderObject());

                var sut = new CrmObjectSelector(mockVsDataConnection);
                Console.Write(conn.ServerVersion);
               

                var reader = sut.SelectObjects(CrmObjectTypes.EntityMetadata, null, null);
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("TABLE: " + reader.GetItem("LogicalName"));
                    }

                }
            }

        }

        [Test(Description = "Integration test for selecting attributes for ddex.")]
        [TestCase("contact", Description = "Selects attributes for contact entity")]
        public void Should_Be_Able_To_Select_Attributes_Restricted_By_Entity(string restriction)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();

                IVsDataConnection mockVsDataConnection = MockRepository.GenerateMock<IVsDataConnection>();
                mockVsDataConnection.Stub(c => c.State).Return(DataConnectionState.Open);
                mockVsDataConnection.Stub(c => c.GetLockedProviderObject()).Return(conn);
                mockVsDataConnection.Stub(c => c.UnlockProviderObject());

                var sut = new CrmObjectSelector(mockVsDataConnection);
                var restrictions = new object[] { restriction };

                var reader = sut.SelectObjects(CrmObjectTypes.AttributeMetadata, restrictions, null);
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("Attribute: " + reader.GetItem("LogicalName"));
                    }

                }
            }

        }

        [Test(Description = "Integration test for selecting plugins for ddex.")]
        [TestCase(null, Description = "Selects plugins no restrictions")]
        public void Should_Be_Able_To_Select_Plugins(string restriction)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["CrmOrganisation"];
            using (var conn = new CrmDbConnection(connectionString.ConnectionString))
            {
                conn.Open();


                IVsDataConnection mockVsDataConnection = MockRepository.GenerateMock<IVsDataConnection>();
                mockVsDataConnection.Stub(c => c.State).Return(DataConnectionState.Open);
                mockVsDataConnection.Stub(c => c.GetLockedProviderObject()).Return(conn);
                mockVsDataConnection.Stub(c => c.UnlockProviderObject());

                var sut = new CrmObjectSelector(mockVsDataConnection);
                object[] restrictions;
                if (restriction == null)
                {
                    restrictions = null;
                }
                else
                {
                    restrictions = new object[] { restriction };
                }

                var reader = sut.SelectObjects(CrmObjectTypes.PluginAssembly, restrictions, null);
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("Plugin: " + reader.GetItem("name"));
                        Console.Write(" version: " + reader.GetItem("version"));
                        Console.Write(" major: " + reader.GetItem("major"));
                        Console.Write(" minor: " + reader.GetItem("minor"));
                        // Console.Write(" patch: " + reader.GetItem("patch"));

                    }

                }
            }

        }



    }
}
