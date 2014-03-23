using System;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    [TestFixture(Category = "Select Statement")]
    public class SelectStatementValidityTests : CrmQueryExpressionProviderTestsBase
    {

        [Category("Validity")]
        [Test(Description = "Should Throw when no From clause present")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_Select_Statement_Does_Not_Have_A_From_Clause()
        {
            // Arrange
            var sql = "Select * From";
            // Act
            var queryExpression = CreateQueryExpression(sql);
        }

        [Category("Validity")]
        [Test(Description = "Should Throw if it's not a Select statement")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_Not_A_Select_Statement()
        {
            // Arrange
            var sql = "Insert into MyTest (mycolumn) values('test')";
            // Act
            var queryExpression = CreateQueryExpression(sql);
        }

        [Category("Validity")]
        [Test(Description = "Should Throw when no Column Name or * present")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_Select_Statement_Does_Not_Have_Any_Column_Names_Or_Star()
        {
            // Arrange
            var sql = "Select From contact";
            // Act
            var queryExpression = CreateQueryExpression(sql);
        }

    }
}