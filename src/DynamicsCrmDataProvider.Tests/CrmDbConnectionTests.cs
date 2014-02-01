using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicsCrmDataProvider.Dynamics;
using DynamicsCrmDataProvider.Tests.Fakes;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;

namespace DynamicsCrmDataProvider.Tests
{
    [TestFixture]
    public abstract class BaseTest<TTestSubject>
    {
        protected virtual TTestSubject CreateTestSubject()
        {
            return Activator.CreateInstance<TTestSubject>();
        }

        protected virtual TTestSubject CreateTestSubject(params object[] args)
        {
            return (TTestSubject)Activator.CreateInstance(typeof(TTestSubject), args);
        }

    }





    [TestFixture()]
    public class CrmDbConnectionTests : BaseTest<CrmDbConnection>
    {

        [Test]
        public void Should_Be_Able_To_Create_A_New_Connection()
        {
            var dbConnection = CreateTestSubject();
        }

        [Test]
        public void Should_Be_Able_To_Open_And_Close_Connection()
        {
            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = new FakeCrmServiceProvider(fakeOrgService, null, null);
            var dbConnection = CreateTestSubject(mockServiceProvider);
            // Act
            dbConnection.Open();
            // Assert
            Assert.That(dbConnection.State == ConnectionState.Open);
            dbConnection.Close();
            Assert.That(dbConnection.State == ConnectionState.Closed);
        }

        [Test]
        public void Should_Obtain_Organisation_Service_Instance_On_Open()
        {
            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider
                .Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            //  new FakeCrmServiceProvider(fakeOrgService, null, null);
            var dbConnection = CreateTestSubject(mockServiceProvider);

            // Act
            dbConnection.Open();

            // Assert
            mockServiceProvider.AssertWasCalled(o => o.GetOrganisationService(), options => options.Repeat.Once());
        }

        [Test]
        public void Should_Close_Connection_On_Dispose()
        {
            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = new FakeCrmServiceProvider(fakeOrgService, null, null);
            var dbConnection = CreateTestSubject(mockServiceProvider);

            using (dbConnection)
            {
                dbConnection.Open();
            }
            Assert.That(dbConnection.State == ConnectionState.Closed);
        }

        [Test]
        public void Should_Dispose_Organisation_Service_Instance_On_Close()
        {
            // Arrange
            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider
                .Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var dbConnection = CreateTestSubject(mockServiceProvider);

            // Act
            dbConnection.Open();
            dbConnection.Close();

            // Assert
            var orgs = fakeOrgService as IDisposable;
            orgs.AssertWasCalled(o => o.Dispose(), options => options.Repeat.Once());
        }
    }
}
