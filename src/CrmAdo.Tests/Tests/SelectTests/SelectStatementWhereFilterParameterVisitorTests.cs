using System;
using NUnit.Framework;
using CrmAdo.Tests.Support;

namespace CrmAdo.Tests
{
    [TestFixture(Description = "Verifies that paramaters can be used with the filter expressions in the Where clause of the select statement")]
    public class SelectStatementWhereFilterParameterVisitorTests : BaseOrganisationRequestBuilderVisitorTest
    {

       [Category("Filtering")]
        [Test]
        [TestCase("=", "SomeValue", "{0} {1} @param1", TestName = "Should support Equals a String paramater")]
        [TestCase("=", 1, "{0} {1}  @param1", TestName = "Should support Equals a Numeric paramater")]
        [TestCase("<>", "SomeValue", "{0} {1} @param1", TestName = "Should support Not Equals a String paramater")]
        [TestCase("<>", 1, "{0} {1} @param1", TestName = "Should support Not Equals a Numeric paramater")]
        [TestCase(">=", "SomeValue", "{0} {1} @param1", TestName = "Should support Greater Than Or Equals a String paramater")]
        [TestCase(">=", 1, "{0} {1} @param1", TestName = "Should support Greater Than Or Equals a Numeric paramater")]
        [TestCase("<=", "SomeValue", "{0} {1} @param1", TestName = "Should support Less Than Or Equals a String paramater")]
        [TestCase("<=", 1, "{0} {1} @param1", TestName = "Should support Less Than Or Equals a Numeric paramater")]
        [TestCase(">", "SomeValue", "{0} {1} @param1", TestName = "Should support Greater Than a String paramater")]
        [TestCase(">", 1, "{0} {1} @param1", TestName = "Should support Greater Than a Numeric paramater")]
        [TestCase("<", "SomeValue", "{0} {1} @param1", TestName = "Should support Less Than a String paramater")]
        [TestCase("<", 1, "{0} {1} @param1", TestName = "Should support Less Than a Numeric paramater")]
        [TestCase("LIKE", "SomeValue", "{0} {1} @param1", TestName = "Should support Like a paramater")]
        [TestCase("NOT LIKE", "SomeValue", "{0} {1} @param1", TestName = "Should support Not Like a paramater")]
        [TestCase("LIKE", "SomeValue%", "{0} {1} @param1", TestName = "Should support Starts With a paramater")]
        [TestCase("LIKE", "%SomeValue", "{0} {1} @param1", TestName = "Should support Ends With a paramater")]
        [TestCase("LIKE", "%SomeValue%", "{0} {1} @param1", TestName = "Should support Like with wildcards paramater")]
        [TestCase("NOT LIKE", "SomeValue%", "{0} {1} @param1", TestName = "Should support Does Not Start With a paramater")]
        [TestCase("NOT LIKE", "%SomeValue", "{0} {1} @param1", TestName = "Should support Does Not End With a paramater")]
        [TestCase("NOT LIKE", "%SomeValue%", "{0} {1} @param1", TestName = "Should support Not Like with wildcards paramater")]
        public void Should_Support_Parameters(string filterOperator, object value, string filterFormatString)
        {
            // Arrange
            // Formulate DML (SQL) statement from test case data.
            var columnName = "firstname";
            if (value == null || !value.GetType().IsArray)
            {
                filterFormatString = string.Format(filterFormatString, columnName, filterOperator);
            }
            else
            {
                throw new NotImplementedException();
            }
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} ", filterFormatString);
          
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            var param = cmd.CreateParameter();
            param.ParameterName = "@param1";
            param.Value = value;
            cmd.Parameters.Add(param);
            // Create test subject.
            var subject = CreateTestSubject();

            // Act
            // Ask our test subject to Convert the SelectBuilder to a Query Expression.
            var queryExpression = base.GetQueryExpression(cmd);

            // Assert
            // Verify that the Query Expression looks as expected in order to work agaisnt the Dynamics SDK.
            Assert.That(queryExpression.ColumnSet.AllColumns == false);
            Assert.That(queryExpression.ColumnSet.Columns[0] == "contactid");
            Assert.That(queryExpression.ColumnSet.Columns[1] == "firstname");
            Assert.That(queryExpression.EntityName == "contact");

            //var defaultConditons = queryExpression.Criteria.Conditions;
            var defaultConditons = queryExpression.Criteria.Filters[0].Conditions;

            Assert.That(defaultConditons.Count, Is.EqualTo(1));
            //Assert.That(defaultFilterGroup.FilterOperator, Is.EqualTo(LogicalOperator.And));
            Assert.That(defaultConditons[0].AttributeName == "firstname");

            var condition = defaultConditons[0];
            AssertUtils.AssertFilterExpressionContion("firstname", filterOperator, value, condition);
        }


    }
}