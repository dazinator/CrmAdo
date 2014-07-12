using System;
using System.Collections.Generic;
using NUnit.Framework;
using CrmAdo.Tests.Support;

namespace CrmAdo.Tests.Visitor
{
    [TestFixture(Category = "Select Statement", 
        Description = "Tests for ensuring suitable errors are thrown if unsupported filter operators are encountered in the sql.")]
    public class SelectStatementWhereFilterOperatorValidityTests : BaseOrganisationRequestBuilderVisitorTest
    {

        [Category("Filtering")]
        [Test]
        [TestCase("=", TestName = "Should Throw when Equals filter does not have a column on one side of the expression.")]
        [TestCase("<>", TestName = "Should Throw if Not Equals filter does not have a column on one side of the expression.")]
        [TestCase(">=", TestName = "Should Throw if Greater Than Equals filter does not have a column on one side of the expression.")]
        [TestCase("<=", TestName = "Should Throw if Less Than Equals filter does not have a column on one side of the expression.")]
        [TestCase(">", TestName = "Should Throw if Greater Than filter does not have a column on one side of the expression.")]
        [TestCase("<", TestName = "Should Throw if Less Than filter does not have a column on one side of the expression.")]
        [TestCase("LIKE", TestName = "Should Throw if Like filter does not have a column on one side of the expression.")]
        [TestCase("NOT LIKE", TestName = "Should Throw if Not Like filter does not have a column on one side of the expression.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_The_Where_Equals_Clause_Does_Not_Refer_To_A_Column_Name(string sqlOperator)
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact Where 'Julius' {0} 'Julius' and lastname {0} 'Caeser'";
            sql = string.Format(sql, sqlOperator);
          
            // Act
            var queryExpression = base.GetQueryExpression(sql);

        }

    }
}