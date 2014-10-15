using System;
using System.Data;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;

namespace CrmAdo.Tests
{
    [Category("ADO")]
    [Category("Connection")]
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
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider
                .Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeConnProvider = MockRepository.GenerateMock<ICrmConnectionProvider>();
            fakeConnProvider.Stub(c => c.OrganisationServiceConnectionString).Return("fakeconn");
            mockServiceProvider.Stub(c => c.ConnectionProvider).Return(fakeConnProvider);

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

            var fakeConnProvider = MockRepository.GenerateMock<ICrmConnectionProvider>();
            fakeConnProvider.Stub(c => c.OrganisationServiceConnectionString).Return("fakeconn");
            mockServiceProvider.Stub(c => c.ConnectionProvider).Return(fakeConnProvider);

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

            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider.Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeConnProvider = MockRepository.GenerateMock<ICrmConnectionProvider>();
            fakeConnProvider.Stub(c => c.OrganisationServiceConnectionString).Return("fakeconn");
            mockServiceProvider.Stub(c => c.ConnectionProvider).Return(fakeConnProvider);

            var dbConnection = CreateTestSubject(mockServiceProvider);

            // Act
            using (dbConnection)
            {
                dbConnection.Open();
            }

            // Assert
            Assert.That(dbConnection.State == ConnectionState.Closed);
        }

        [Test]
        public void Should_Dispose_Organisation_Service_Instance_On_Close()
        {
            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider
                .Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeConnProvider = MockRepository.GenerateMock<ICrmConnectionProvider>();
            fakeConnProvider.Stub(c => c.OrganisationServiceConnectionString).Return("fakeconn");
            mockServiceProvider.Stub(c => c.ConnectionProvider).Return(fakeConnProvider);

            var dbConnection = CreateTestSubject(mockServiceProvider);

            // Act
            dbConnection.Open();
            dbConnection.Close();

            // Assert
            var orgs = fakeOrgService as IDisposable;
            orgs.AssertWasCalled(o => o.Dispose(), options => options.Repeat.Once());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_On_Attempt_To_Open_A_Non_Closed_Connection()
        {
            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider
                .Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeConnProvider = MockRepository.GenerateMock<ICrmConnectionProvider>();
            fakeConnProvider.Stub(c => c.OrganisationServiceConnectionString).Return("fakeconn");
            mockServiceProvider.Stub(c => c.ConnectionProvider).Return(fakeConnProvider);

            var dbConnection = CreateTestSubject(mockServiceProvider);

            // Act
            dbConnection.Open();
            dbConnection.Open();
        }

        [Test]
        public void Should_Be_Able_To_Spawn_A_Command()
        {
            // Arrange
            var dbConnection = CreateTestSubject();

            // Act
            var command = dbConnection.CreateCommand();

            // Assert
            Assert.That(command != null);

        }
    }
}