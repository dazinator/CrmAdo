using System.Collections.Generic;
using NUnit.Framework;
using CrmAdo.Tests.Support;

namespace CrmAdo.Tests
{
    [TestFixture(Category = "Select Statement",
        Description = "Tests for ensuring various filter operators can be used in the SQL command text, and are correctly translated to the appropriate Dynamics CRM filter conditions.")]
    public class SelectStatementWhereFilterOperatorVisitorTests : BaseOrganisationRequestBuilderVisitorTest
    {

        [Category("Filtering")]
        [Category("String Literal")]
        [TestCase("=", "", TestName = "Should support Column Equal To a String Literal")]
        [TestCase("<>", "", TestName = "Should support Column Not Equal To a String Literal")]
        [TestCase(">=", "", TestName = "Should support Column Greater Than Or Equals To a String Literal")]
        [TestCase("<=", "", TestName = "Should support Column Less Than Or Equals To a String Literal")]
        [TestCase(">", "", TestName = "Should support Column Greater Than a String Literal")]
        [TestCase("<", "", TestName = "Should support Column Less Than a String Literal")]
        [TestCase("LIKE", "", TestName = "Should support Column Like a String Literal")]
        [TestCase("LIKE", "Albert%", TestName = "Should support Column Like Starts With a String Literal")]
        [TestCase("LIKE", "%Albert", TestName = "Should support Column Like Ends With a String Literal")]
        [TestCase("LIKE", "%Albert%", TestName = "Should support Column Like Contains a String Literal")]
        [TestCase("NOT LIKE","", TestName = "Should support Column Not Like a String Literal")]
        [TestCase("NOT LIKE", "Albert%", TestName = "Should support Column Not Like Starts With a String Literal")]
        [TestCase("NOT LIKE", "%Albert", TestName = "Should support Column Not Like Ends With a String Literal")]
        [TestCase("NOT LIKE", "%Albert%", TestName = "Should support Column Not Like Contains a String Literal")]
        [Test()]
        public void Should_Support_Various_Operators_With_String_Literal_Value(string sqlOperator, string literalValue = "")
        {
            var columnName = "firstname";
            var conditionValue = string.IsNullOrEmpty(literalValue) ? "Albert" : literalValue;
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} {1} '{2}'", columnName, sqlOperator, conditionValue);
            var queryExpression = GetQueryExpression(sql);
            AssertUtils.AssertQueryContainsSingleFilterCondition(queryExpression, columnName, sqlOperator, conditionValue);
        }

        [Category("Filtering")]
        [Category("String Literal")]
        [TestCase("IN", new string[] { "Julius", "Justin" }, TestName = "Should support Column In a String Literal list")]
        [TestCase("IN", new string[] { "097e0140-c269-430c-9caf-d2cffe261d8b", "897e0140-c269-430c-9caf-d2cffe261d8c" }, TestName = "Should support Column In a String Literal list containing Guids")]
        [TestCase("NOT IN", new string[] { "Julius", "Justin" }, TestName = "Should support Column Not In a String Literal list")]
        [TestCase("NOT IN", new string[] { "097e0140-c269-430c-9caf-d2cffe261d8b", "897e0140-c269-430c-9caf-d2cffe261d8c" }, TestName = "Should support Column Not In a String Literal list containing Guids")]
        [Test()]
        public void Should_Support_In_Operator_With_String_Literal_List(string sqlOperator, string[] valuesArray)
        {
            var columnName = "firstname";
            var args = new List<object>();
            args.Add(columnName);
            args.Add(sqlOperator);
            args.AddRange(valuesArray);
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} {1} ('{2}', '{3}')", args.ToArray());
            var queryExpression = GetQueryExpression(sql);
            AssertUtils.AssertQueryContainsSingleFilterCondition(queryExpression, columnName, sqlOperator, valuesArray);
        }

        [Category("Filtering")]
        [Category("String Literal")]
        [TestCase("CONTAINS", TestName = "Should support Column Contains a String Literal")]
        [TestCase("NOT CONTAINS", TestName = "Should support Column Not Contains a String Literal")]
        [Test()]
        public void Should_Support_Contains_Operator_With_String_Literal_Value(string sqlOperator)
        {
            var columnName = "firstname";
            var conditionValue = "Albert"; //string.IsNullOrEmpty(literalValue) ? "Albert" : literalValue;
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {1}({0},  '{2}'))", columnName, sqlOperator, conditionValue);
            var queryExpression = GetQueryExpression(sql);
            AssertUtils.AssertQueryContainsSingleFilterCondition(queryExpression, columnName, sqlOperator, conditionValue);
        }

        [Category("Filtering")]
        [Category("Numeric Literal")]
        [TestCase("=", TestName = "Should support Column Equal To a Numeric Literal")]
        [TestCase("<>", TestName = "Should support Column Not Equal To a Numeric Literal")]
        [TestCase(">=", TestName = "Should support Column Greater Than Or Equals To a Numeric Literal")]
        [TestCase("<=", TestName = "Should support Column Less Than Or Equals To a Numeric Literal")]
        [TestCase(">", TestName = "Should support Column Greater Than a Numeric Literal")]
        [TestCase("<", TestName = "Should support Column Greater Than a Numeric Literal")]
        [Test()]
        public void Should_Support_Various_Operators_With_Numeric_Literal_Value(string sqlOperator)
        {
            var columnName = "firstname";
            var conditionValue = 1;
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} {1} {2}", columnName, sqlOperator, conditionValue);
            var queryExpression = GetQueryExpression(sql);
            AssertUtils.AssertQueryContainsSingleFilterCondition(queryExpression, columnName, sqlOperator, conditionValue);
        }

        [Category("Filtering")]
        [Category("Numeric Literal")]
        [TestCase("IN", new int[] { 1, 2 }, TestName = "Should support Column In a Numeric Literal list")]
        [TestCase("NOT IN", new int[] { 5, 10 }, TestName = "Should support Column Not In a Numeric Literal list")]
        [Test()]
        public void Should_Support_In_Operator_With_Numeric_Literal_List(string sqlOperator, int[] valuesArray)
        {
            var columnName = "firstname";
            //  var stringLiteralValues = new string[] {"Julius", "Justin"};
            var args = new List<object>();
            args.Add(columnName);
            args.Add(sqlOperator);
            foreach (var i in valuesArray)
            {
                args.Add(i.ToString());
            }
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} {1} ({2}, {3})", args.ToArray());
            var queryExpression = GetQueryExpression(sql);
            AssertUtils.AssertQueryContainsSingleFilterCondition(queryExpression, columnName, sqlOperator, valuesArray);
        }

        [Category("Filtering")]
        [Category("Null")]
        [TestCase("IS NULL", TestName = "Should support Column Is Null")]
        [TestCase("IS NOT NULL", TestName = "Should support Column Is Not Null")]
        [Test()]
        public void Should_Support_Null(string sqlOperator)
        {
            var columnName = "firstname";
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} {1}", columnName, sqlOperator);
            var queryExpression = GetQueryExpression(sql);
            AssertUtils.AssertQueryContainsSingleFilterCondition(queryExpression, columnName, sqlOperator, null);
        }


    }
}