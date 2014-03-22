using System;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;

namespace CrmAdo.Tests
{
    [TestFixture()]
    public class CrmDbCommandTests : BaseTest<CrmDbCommand>
    {
        [Test]
        public void Should_Be_Able_To_Create_A_New_Command()
        {
            var subject = CreateTestSubject();
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Command_With_Connection()
        {
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            var subject = CreateTestSubject(conn);
            Assert.That(subject.Connection, Is.SameAs(conn));
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Command_With_Connection_And_Command_Text()
        {
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            var commandText = "TESTCOMMAND";
            var subject = CreateTestSubject(conn, commandText);
            Assert.That(subject.Connection, Is.SameAs(conn));
            Assert.That(subject.CommandText, Is.EqualTo(commandText));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_An_Unopen_Connection_And_ExecuteDbDataReader_Is_Called_()
        {
            // Arrange
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            conn.Stub(c => c.State).Return(ConnectionState.Closed);
            var subject = CreateTestSubject(conn);
            // Act
            subject.ExecuteReader();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_An_Unopen_Connection_And_ExecuteNonQuery_Is_Called_()
        {
            // Arrange
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            conn.Stub(c => c.State).Return(ConnectionState.Closed);
            var subject = CreateTestSubject(conn);
            // Act
            subject.ExecuteNonQuery();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_It_Has_An_Unopen_Connection_And_ExecuteScalar_Is_Called_()
        {
            // Arrange
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            conn.Stub(c => c.State).Return(ConnectionState.Closed);
            var subject = CreateTestSubject(conn);
            // Act
            subject.ExecuteScalar();
        }
        
    }
}
