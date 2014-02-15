using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
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
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            //  var cmd = commandBuilder.GetCommand(sql);

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);
            // Assert
            Assert.That(queryExpression.ColumnSet.AllColumns == true);
            Assert.That(queryExpression.EntityName == "contact");

        }

        [Test]
        public void Select_Clause_Contains_Attribute_Names()
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact";

            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);
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
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Throws_When_No_From_Clause()
        {
            // Arrange
            var sql = "Select * From";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Throws_When_Not_A_Select_Statement()
        {
            // Arrange
            var sql = "Insert into MyTest (mycolumn) values('test')";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

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
        [TestCase("IS NULL", null, "{0} {1} {2}", TestName = "Is Null Filter")]
        [TestCase("IS NOT NULL", null, "{0} {1}", TestName = "Is Not Null Filter")]
        [TestCase("LIKE", "SomeValue", "{0} {1} '{2}'", TestName = "Like Filter with a String Constant")]
        [TestCase("NOT LIKE", "SomeValue", "{0} {1} '{2}'", TestName = "Not Like Filter with a String Constant")]
        [TestCase("IN", new string[] { "Julius", "Justin" }, "{0} {1} ('{2}', '{3}')", TestName = "In Filter with string array")]
        [TestCase("IN", new int[] { 1, 2 }, "{0} {1} ({2}, {3})", TestName = "In Filter with Numeric array")]
        [TestCase("IN", new string[] { "097e0140-c269-430c-9caf-d2cffe261d8b", "897e0140-c269-430c-9caf-d2cffe261d8c" }, "{0} {1} ('{2}', '{3}')", TestName = "In Filter with Guid array")]
        [TestCase("NOT IN", new string[] { "Julius", "Justin" }, "{0} {1} ('{2}', '{3}')", TestName = "Not In Filter with string array")]
        [TestCase("NOT IN", new int[] { 5, 10 }, "{0} {1} ({2}, {3})", TestName = "Not In Filter with Numeric array")]
        [TestCase("NOT IN", new string[] { "097e0140-c269-430c-9caf-d2cffe261d8b", "897e0140-c269-430c-9caf-d2cffe261d8c" }, "{0} {1} ('{2}', '{3}')", TestName = "Not In Filter with Guid array")]
        [TestCase("LIKE", "SomeValue%", "{0} {1} '{2}'", TestName = "Starts With Filter")]
        [TestCase("LIKE", "%SomeValue", "{0} {1} '{2}'", TestName = "Ends With Filter")]
        [TestCase("LIKE", "%SomeValue%", "{0} {1} '{2}'", TestName = "Contains Filter")]
        [TestCase("NOT LIKE", "SomeValue%", "{0} {1} '{2}'", TestName = "Does Not Start With Filter")]
        [TestCase("NOT LIKE", "%SomeValue", "{0} {1} '{2}'", TestName = "Does Not End With Filter")]
        [TestCase("NOT LIKE", "%SomeValue%", "{0} {1} '{2}'", TestName = "Does Not Contain Filter")]
        public void Should_Convert_Filter_Condition_To_Correct_Query_Expression_Condition(string filterOperator, object value, string filterFormatString)
        {
            // Arrange
            // Formulate DML (SQL) statement from test case data.
            var columnName = "firstname";
            if (value == null || !value.GetType().IsArray)
            {
                filterFormatString = string.Format(filterFormatString, columnName, filterOperator, value);
            }
            else
            {
                var formatArgs = new List<object>();
                formatArgs.Add(columnName);
                formatArgs.Add(filterOperator);
                var args = value as IEnumerable;
                foreach (var arg in args)
                {
                    formatArgs.Add(arg);
                }
                filterFormatString = string.Format(filterFormatString, formatArgs.ToArray());
            }
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0} ", filterFormatString);
            // Convery the DML (SQL) statement to a SelectBuilder object which an object representation of the DML.
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            // Create test subject.
            var subject = CreateTestSubject();

            // Act
            // Ask our test subject to Convert the SelectBuilder to a Query Expression.
            var queryExpression = subject.CreateQueryExpression(cmd);

            // Assert
            // Verify that the Query Expression looks as expected in order to work agaisnt the Dynamics SDK.
            Assert.That(queryExpression.ColumnSet.AllColumns == false);
            Assert.That(queryExpression.ColumnSet.Columns[0] == "contactid");
            Assert.That(queryExpression.ColumnSet.Columns[1] == "firstname");
            Assert.That(queryExpression.EntityName == "contact");
            Assert.That(queryExpression.Criteria.Conditions.Count, Is.EqualTo(1));
            Assert.That(queryExpression.Criteria.FilterOperator, Is.EqualTo(LogicalOperator.And));
            Assert.That(queryExpression.Criteria.Conditions[0].AttributeName == "firstname");

            switch (filterOperator)
            {
                case "=":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Equal));
                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));
                    break;
                case "<>":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.NotEqual));
                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));
                    break;
                case ">":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.GreaterThan));
                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));
                    break;
                case "<":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.LessThan));
                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));
                    break;
                case ">=":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.GreaterEqual));
                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));
                    break;
                case "<=":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.LessEqual));
                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));
                    break;
                case "IS NULL":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Null));
                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Is.Empty);
                    break;
                case "IS NOT NULL":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.NotNull));
                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Is.Empty);
                    break;
                case "LIKE":

                    if (value != null)
                    {
                        bool startsWith = false;
                        bool endsWith = false;
                        var stringVal = value.ToString();
                        startsWith = stringVal.EndsWith("%");
                        endsWith = stringVal.StartsWith("%");
                        if (startsWith && endsWith)
                        {
                            // contains..
                            var actualValue = stringVal.Remove(0, 1);
                            if (actualValue.Length > 0 && actualValue.EndsWith("%"))
                            {
                                actualValue = actualValue.Remove(actualValue.Length - 1, 1);
                            }
                            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Contains));
                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(actualValue));
                        }
                        else if (startsWith)
                        {
                            // starts with..
                            var actualValue = stringVal.Remove(stringVal.Length - 1, 1);
                            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.BeginsWith));
                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(actualValue));

                        }
                        else if (endsWith)
                        {
                            // ends with
                            var actualValue = stringVal.Remove(0, 1);
                            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.EndsWith));
                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(actualValue));
                        }
                        else
                        {
                            // like
                            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Like));
                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));
                        }



                    }
                    else
                    {
                        Assert.Fail("Unhandled test case data");
                    }

                    break;
                case "NOT LIKE":
                    if (value != null)
                    {
                        bool startsWith = false;
                        bool endsWith = false;
                        var stringVal = value.ToString();
                        startsWith = stringVal.EndsWith("%");
                        endsWith = stringVal.StartsWith("%");
                        if (startsWith && endsWith)
                        {
                            // contains..
                            var actualValue = stringVal.Remove(0, 1);
                            if (actualValue.Length > 0 && actualValue.EndsWith("%"))
                            {
                                actualValue = actualValue.Remove(actualValue.Length - 1, 1);
                            }
                            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.DoesNotContain));
                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(actualValue));
                        }
                        else if (startsWith)
                        {
                            // starts with..
                            var actualValue = stringVal.Remove(stringVal.Length - 1, 1);
                            Assert.That(queryExpression.Criteria.Conditions[0].Operator,
                                        Is.EqualTo(ConditionOperator.DoesNotBeginWith));
                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(actualValue));

                        }
                        else if (endsWith)
                        {
                            // ends with
                            var actualValue = stringVal.Remove(0, 1);
                            Assert.That(queryExpression.Criteria.Conditions[0].Operator,
                                        Is.EqualTo(ConditionOperator.DoesNotEndWith));
                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(actualValue));
                        }
                        else
                        {
                            // like
                            Assert.That(queryExpression.Criteria.Conditions[0].Operator,
                                        Is.EqualTo(ConditionOperator.NotLike));
                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(value));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case data");
                    }
                    break;
                case "IN":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.In));
                    if (value != null && value.GetType().IsArray)
                    {
                        foreach (var val in value as Array)
                        {
                            Guid guidVal;
                            if (val is string)
                            {
                                if (Guid.TryParse(val as string, out guidVal))
                                {
                                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(guidVal));
                                    continue;
                                }
                            }

                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(val));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case for IN expression.");
                    }
                    break;
                case "NOT IN":
                    Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.NotIn));
                    if (value != null && value.GetType().IsArray)
                    {
                        foreach (var val in value as Array)
                        {
                            Guid guidVal;
                            if (val is string)
                            {
                                if (Guid.TryParse(val as string, out guidVal))
                                {
                                    Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(guidVal));
                                    continue;
                                }
                            }

                            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item(val));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case for IN expression.");
                    }

                    break;
                default:
                    Assert.Fail("Unhandled test case.");
                    break;
            }

            //TODO: Haven't yet implemented support for the following dynamics crm condition operators..

            //ConditionOperator.Between // >= x and <= y
            //ConditionOperator.NotBetween // <= x and >= y
            //ConditionOperator.NotOn //  -The value is not on the specified date.
            //ConditionOperator.On // = The value is on the specified date.

        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Throws_When_Where_Clause_Equals_Does_Not_Refer_To_Attribute_Name()
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact Where 'Julius' = 'Julius' and lastname = 'Caeser'";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

        }


        [Test]
        [TestCase("INNER")]
        [TestCase("LEFT")]
        public void Should_Support_Joins(String joinType)
        {
            var join = JoinOperator.Natural;

            switch (joinType)
            {
                case "INNER":
                    join = JoinOperator.Inner;
                    //  Enum.Parse(typeof(JoinOperator), joinType)
                    break;
                case "LEFT":
                    join = JoinOperator.LeftOuter;
                    break;
            }


            var sql = string.Format("Select contactid, firstname, lastname From contact {0} JOIN address on contact.id = address.contactid", joinType);

            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            // Create test subject.
            var subject = CreateTestSubject();

            // Act
            // Ask our test subject to Convert the SelectBuilder to a Query Expression.
            var queryExpression = subject.CreateQueryExpression(cmd);

            //Assert
            Assert.That(queryExpression.LinkEntities, Is.Not.Null);
            Assert.That(queryExpression.LinkEntities[0], Is.Not.Null);
            Assert.That(queryExpression.LinkEntities[0].JoinOperator, Is.EqualTo(join));
        


        }

        [Category("Experimentation")]
        [Test]
        public void Experiment_With_Joins()
        {

            var logBuilder = new StringBuilder();
            
            string joinType = "INNER";
            var sql = string.Format("Select contactid, firstname, lastname From contact AS CONTACT {0} JOIN address AS ADDRESS on contact.contactid = address.contactid", joinType);

            var commandText = sql;
            var commandBuilder = new CommandBuilder();
            var builder = commandBuilder.GetCommand(commandText) as SelectBuilder;
            LogUtils.LogCommand(builder, logBuilder);
            logBuilder.AppendLine();
          
            var nestedJoinsSql =
                string.Format(
                    "Select contactid, firstname, lastname From contact INNER JOIN address on contact.id = address.contactid INNER JOIN occupant on address.addressid = occupant.addressid ",
                    joinType);

            builder = commandBuilder.GetCommand(nestedJoinsSql) as SelectBuilder;
            LogUtils.LogCommand(builder, logBuilder);
            logBuilder.AppendLine();

            var anotherJoinSql =
             string.Format(
                 "Select contactid, firstname, lastname From contact INNER JOIN address on contact.id = address.contactid INNER JOIN occupant on contact.contactid = occupant.contactid ",
                 joinType);

            builder = commandBuilder.GetCommand(anotherJoinSql) as SelectBuilder;
            LogUtils.LogCommand(builder, logBuilder);
            logBuilder.AppendLine();


            var moreSql =
            string.Format(
                "Select C.contactid, C.firstname, C.lastname, O.fullname From contact C INNER JOIN address A on c.id = A.contactid LEFT JOIN occupant O on C.contactid = O.contactid ",
                joinType);

            builder = commandBuilder.GetCommand(moreSql) as SelectBuilder;
            LogUtils.LogCommand(builder, logBuilder);
            logBuilder.AppendLine();


            Console.Write(logBuilder.ToString());

        }

       


    }
}
