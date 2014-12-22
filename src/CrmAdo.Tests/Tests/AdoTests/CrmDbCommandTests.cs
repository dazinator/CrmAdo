using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Tests.Sandbox;

namespace CrmAdo.Tests
{
    [Category("ADO")]
    [Category("Command")]
    [TestFixture()]
    public class CrmDbCommandTests : BaseTest<CrmDbCommand>
    {
        [Test]
        public void Should_Be_Able_To_Create_A_New_Command()
        {
            var subject = new CrmDbCommand();
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Command_With_Connection()
        {
            using (var sandbox = CommandTestsSandbox.Create())
            {
                var subject = ResolveTestSubjectInstance();
                Assert.That(subject.Connection, Is.SameAs(sandbox.FakeCrmDbConnection));
            }
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Command_With_Connection_And_Command_Text()
        {
            using (var sandbox = CommandTestsSandbox.Create())
            {
                var commandText = "TESTCOMMAND";

                var subject = new CrmDbCommand(sandbox.FakeCrmDbConnection, commandText);

                Assert.That(subject.Connection, Is.SameAs(sandbox.FakeCrmDbConnection));
                Assert.That(subject.CommandText, Is.EqualTo(commandText));

            }

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_An_Unopen_Connection_And_ExecuteDbDataReader_Is_Called_()
        {
            // Arrange
            using (var sandbox = CommandTestsSandbox.Create())
            {
                sandbox.FakeCrmDbConnection.Stub(c => c.State).Return(ConnectionState.Closed);
                var subject = ResolveTestSubjectInstance();

                // Act
                subject.ExecuteReader();

            }

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_An_Unopen_Connection_And_ExecuteNonQuery_Is_Called_()
        {
            // Arrange
            using (var sandbox = CommandTestsSandbox.Create())
            {
                sandbox.FakeCrmDbConnection.Stub(c => c.State).Return(ConnectionState.Closed);
                var subject = ResolveTestSubjectInstance();

                // Act
                subject.ExecuteNonQuery();

            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_An_Unopen_Connection_And_ExecuteScalar_Is_Called_()
        {
            // Arrange
            using (var sandbox = CommandTestsSandbox.Create())
            {
                sandbox.FakeCrmDbConnection.Stub(c => c.State).Return(ConnectionState.Closed);
                var subject = ResolveTestSubjectInstance();

                // Act
                subject.ExecuteScalar();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_Empty_CommandText_And_ExecuteDbDataReader_Is_Called_()
        {
            // Arrange
            using (var sandbox = CommandTestsSandbox.Create())
            {
                sandbox.FakeCrmDbConnection.Stub(c => c.State).Return(ConnectionState.Open);
                var subject = ResolveTestSubjectInstance();

                // Act
                subject.ExecuteReader();
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_Empty_CommandText_And_ExecuteNonQuery_Is_Called_()
        {
            // Arrange
            using (var sandbox = CommandTestsSandbox.Create())
            {
                sandbox.FakeCrmDbConnection.Stub(c => c.State).Return(ConnectionState.Open);
                var subject = ResolveTestSubjectInstance();

                // Act
                subject.ExecuteNonQuery();
            }          
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_Empty_CommandText_And_ExecuteScalar_Is_Called_()
        {
            // Arrange
            using (var sandbox = CommandTestsSandbox.Create())
            {
                sandbox.FakeCrmDbConnection.Stub(c => c.State).Return(ConnectionState.Open);
                var subject = ResolveTestSubjectInstance();

                // Act
                subject.ExecuteScalar();
            }                
        }

    }
}
