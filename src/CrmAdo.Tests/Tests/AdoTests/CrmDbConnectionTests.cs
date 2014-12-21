using System;
using System.Data;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Tests.Support;

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
            using (ConnectionTestsSandbox.Create())
            {
                var dbConnection = CreateTestSubject();
            }
        }

        [Test]
        public void Should_Be_Able_To_Open_And_Close_Connection()
        {
            // Arrange
            using (ConnectionTestsSandbox.Create())
            {
                var dbConnection = new CrmDbConnection();
                // Act
                dbConnection.Open();
                // Assert
                Assert.That(dbConnection.State == ConnectionState.Open);
                dbConnection.Close();
                Assert.That(dbConnection.State == ConnectionState.Closed);
            }

        }

        [Test]
        public void Should_Obtain_Organisation_Service_Instance_On_Open()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                var dbConnection = new CrmDbConnection();

                // Act
                dbConnection.Open();

                // Assert
                sandbox.FakeServiceProvider.AssertWasCalled(o => o.GetOrganisationService(), options => options.Repeat.Once());
            }

        }

        [Test]
        public void Should_Close_Connection_On_Dispose()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                var dbConnection = new CrmDbConnection();

                // Act
                using (dbConnection)
                {
                    dbConnection.Open();
                }

                // Assert
                Assert.That(dbConnection.State == ConnectionState.Closed);

            }


        }

        [Test]
        public void Should_Dispose_Organisation_Service_Instance_On_Close()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                var dbConnection = new CrmDbConnection();

                // Act
                dbConnection.Open();
                dbConnection.Close();

                // Assert
                var orgs = sandbox.FakeOrgService as IDisposable;
                orgs.AssertWasCalled(o => o.Dispose(), options => options.Repeat.Once());
            }

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_On_Attempt_To_Open_A_Non_Closed_Connection()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                var dbConnection = new CrmDbConnection();

                // Act
                dbConnection.Open();
                dbConnection.Open();
            }

        }

        [Test]
        public void Should_Be_Able_To_Spawn_A_Command()
        {
            // Arrange           
            using (var sandbox = ConnectionTestsSandbox.Create())
            {
                var dbConnection = new CrmDbConnection();

                // Act
                var command = dbConnection.CreateCommand();

                // Assert
                Assert.That(command != null);

            }         

        }


    }
}