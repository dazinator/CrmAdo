using System;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    [TestFixture(Category = "Select Statement")]
    public class SelectStatementTests : CrmQueryExpressionProviderTestsBase
    {
        [Category("TOP")]
        [Test(Description = "Should support TOP X")]
        public void Should_Support_Select_TOP_X()
        {
            // Arrange
            var sql = "Select TOP 15 contactid, firstname, lastname From contact";
            // Act
            var queryExpression = GetQueryExpression(sql);
            // Assert
            Assert.That(queryExpression.TopCount == 15);
        }

        [Category("TOP")]
        [ExpectedException(typeof(NotSupportedException))]
        [Test(Description = "Should not support TOP X PERCENT")]
        public void Should_Not_Support_Select_TOP_X_PERCENT()
        {
            // Arrange
            var sql = "Select TOP 10 PERCENT contactid, firstname, lastname From contact";
            // Act
            var queryExpression = GetQueryExpression(sql);
        }

    }
}