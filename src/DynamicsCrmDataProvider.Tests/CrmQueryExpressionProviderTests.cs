using System;
using System.Data;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Core;
using NUnit.Framework;
using Rhino.Mocks;
using SQLGeneration.Builders;
using SQLGeneration.Generators;

namespace DynamicsCrmDataProvider.Tests
{
    [TestFixture()]
    public class CrmQueryExpressionProviderTests : BaseTest<CrmQueryExpressionProvider>
    {
        [Test]
        public void Select_Clause_Contains_Star()
        {
            // Arrange
            var sql = "Select * From contact";
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);
            // Assert
            Assert.That(queryExpression.ColumnSet.AllColumns == true);
            Assert.That(queryExpression.EntityName == "contact");

        }

        [Test]
        public void Select_Clause_Contains_Attribute_Names()
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact";
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);
            // Assert
            Assert.That(queryExpression.ColumnSet.AllColumns == false);
            Assert.That(queryExpression.ColumnSet.Columns[0] == "contactid");
            Assert.That(queryExpression.ColumnSet.Columns[1] == "firstname");
            Assert.That(queryExpression.ColumnSet.Columns[2] == "lastname");
            Assert.That(queryExpression.EntityName == "contact");

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Throws_When_Select_Clause_Contains_No_Columns()
        {
            // Arrange
            var sql = "Select From contact";
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Throws_When_No_From_Clause()
        {
            // Arrange
            var sql = "Select * From";
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Throws_When_Not_A_Select_Statement()
        {
            // Arrange
            var sql = "Insert into MyTest (mycolumn) values('test')";
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);

        }

        [Test]
        public void Where_Clause_Equals_Numeric_Literal_Value()
        {
            // Arrange
            var sql = "Select contactid, title, lastname From contact Where title = 1000000 and lastname = 'Caeser'";
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);
            // Assert
            Assert.That(queryExpression.ColumnSet.AllColumns == false);
            Assert.That(queryExpression.ColumnSet.Columns[0] == "contactid");
            Assert.That(queryExpression.ColumnSet.Columns[1] == "title");
            Assert.That(queryExpression.ColumnSet.Columns[2] == "lastname");
            Assert.That(queryExpression.EntityName == "contact");
            Assert.That(queryExpression.Criteria.Conditions.Count, Is.EqualTo(2));
            Assert.That(queryExpression.Criteria.FilterOperator, Is.EqualTo(LogicalOperator.And));
            Assert.That(queryExpression.Criteria.Conditions[0].AttributeName == "title");
            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Equal));
            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(1000000));

            Assert.That(queryExpression.Criteria.Conditions[1].AttributeName == "lastname");
            Assert.That(queryExpression.Criteria.Conditions[1].Operator, Is.EqualTo(ConditionOperator.Equal));
            Assert.That(queryExpression.Criteria.Conditions[1].Values, Contains.Item("Caeser"));

        }

        [Test]
        public void Where_Clause_Not_Equals_Numeric_Literal_Value()
        {
            // Arrange
            var sql = "Select contactid, firstname, iscustomer From contact Where firstname <> 'Julius' and iscustomer <> 1";
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);
            // Assert
            Assert.That(queryExpression.ColumnSet.AllColumns == false);
            Assert.That(queryExpression.ColumnSet.Columns[0] == "contactid");
            Assert.That(queryExpression.ColumnSet.Columns[1] == "firstname");
            Assert.That(queryExpression.ColumnSet.Columns[2] == "iscustomer");
            Assert.That(queryExpression.EntityName == "contact");
            Assert.That(queryExpression.Criteria.Conditions.Count, Is.EqualTo(2));
            Assert.That(queryExpression.Criteria.FilterOperator, Is.EqualTo(LogicalOperator.And));
            Assert.That(queryExpression.Criteria.Conditions[0].AttributeName == "firstname");
            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.NotEqual));
            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item("Julius"));
            Assert.That(queryExpression.Criteria.Conditions[1].AttributeName == "iscustomer");
            Assert.That(queryExpression.Criteria.Conditions[1].Operator, Is.EqualTo(ConditionOperator.NotEqual));
            Assert.That(queryExpression.Criteria.Conditions[1].Values, Contains.Item(1));

        }

        [Test]
        [TestCase("=", "SomeValue", "{0} {1} '{2}'", TestName = "Equals Filter with a String Constant")]
        [TestCase("=", 1, "{0} {1} {2}", TestName = "Equals Filter with a Numeric Constant")]
        [TestCase("<>", "SomeValue", "{0} {1} '{2}'", TestName = "Not Equals Filter with a String Constant")]
        [TestCase("<>", 1, "{0} {1} {2}", TestName = "Not Equals Filter with a Numeric Constant")]
        [TestCase(">=", "SomeValue", "{0} {1} '{2}'", TestName = "Greater Than Or Equals Filter with a String Constant")]
        [TestCase(">=", 1, "{0} {1} {2}", TestName = "Greater Than Or Equals Filter with a Numeric Constant")]
        [TestCase("<=", "SomeValue", "{0} {1} '{2}'", TestName = "Less Than Or Equals Filter with a String Constant")]
        [TestCase("<=", 1, "{0} {1} {2}", TestName = "Less Than Or Equals Filter with a Numeric Constant")]
        [TestCase(">", "SomeValue", "{0} {1} '{2}'", TestName = "Greater Than Filter with a String Constant")]
        [TestCase(">", 1, "{0} {1} {2}", TestName = "Greater Than Filter with a Numeric Constant")]
        [TestCase("<", "SomeValue", "{0} {1} '{2}'", TestName = "Less Than Filter with a String Constant")]
        [TestCase("<", 1, "{0} {1} {2}", TestName = "Less Than Filter with a Numeric Constant")]
        public void Should_Convert_Filter_Condition_To_Correct_Query_Expression_Condition(string filterOperator, object value, string filterFormatString)
        {
            // Arrange
            var columnName = "firstname";
            filterFormatString = string.Format(filterFormatString, columnName, filterOperator, value);

            //   var filterCondition = string.Format("{0} {1}", filterOperator, filterValue);

            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} ", filterFormatString);
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);
            // Assert
            Assert.That(queryExpression.ColumnSet.AllColumns == false);
            Assert.That(queryExpression.ColumnSet.Columns[0] == "contactid");
            Assert.That(queryExpression.ColumnSet.Columns[1] == "firstname");
            Assert.That(queryExpression.EntityName == "contact");
            Assert.That(queryExpression.Criteria.Conditions.Count, Is.EqualTo(1));
            Assert.That(queryExpression.Criteria.FilterOperator, Is.EqualTo(LogicalOperator.And));
            Assert.That(queryExpression.Criteria.Conditions[0].AttributeName == "firstname");
            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));

            switch (filterOperator)
            {
                case "=":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Equal));
                    break;
                case "<>":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.NotEqual));
                    break;
                case ">":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.GreaterThan));
                    break;
                case "<":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.LessThan));
                    break;
                case ">=":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.GreaterEqual));
                    break;
                case "<=":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.LessEqual));
                    break;
                default:
                    Assert.Fail("Unhandled test case.");
                    break;
            }

            // ConditionOperator.Null // IS NULL
            // ConditionOperator.NotNull // IS NOT NULL

            // ConditionOperator.Like // LIKE ''
            // ConditionOperator.NotLike // NOT LIKE ''

            // ConditionOperator.In // IN (value1, value2, value 3)
            // ConditionOperator.NotIn // NOT IN (value1, value2, value 3)

            //ConditionOperator.BeginsWith // LIKE 'X%' 
            //ConditionOperator.EndsWith // LIKE '%text';
            //ConditionOperator.Contains // LIKE '%X%'
            //ConditionOperator.DoesNotBeginWith // NOT LIKE 'text%';
            //ConditionOperator.DoesNotEndWith // NOT LIKE '%text';
            //ConditionOperator.DoesNotContain // NOT LIKE '%X%' 

            //ConditionOperator.Between // >= x and <= y
            //ConditionOperator.NotBetween // <= x and >= y
          
            // ConditionOperator.NotOn // <> value  -The value is not on the specified date.
            // ConditionOperator.On // = value The value is on the specified date.

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Throws_When_Where_Clause_Equals_Does_Not_Refer_To_Attribute_Name()
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact Where 'Julius' = 'Julius' and lastname = 'Caeser'";
            var commandBuilder = new CommandBuilder();
            var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd as SelectBuilder);

        }

    }
}
