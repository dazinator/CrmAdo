using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace DynamicsCrmDataProvider.Tests
{
    [TestFixture()]
    public class CrmQueryExpressionProviderTests : BaseTest<CrmQueryExpressionProvider>
    {
        [Category("Projections")]
        [Test]
        [TestCase(TestName = "Should support *")]
        public void Should_Support_Select_Statement_Containing_Star()
        {
            // Arrange
            var sql = "Select * From contact";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);
            // Assert
            Assert.That(queryExpression.ColumnSet.AllColumns == true);
            Assert.That(queryExpression.EntityName == "contact");
        }

        [Category("Projections")]
        [Test]
        [TestCase(TestName = "Should support Column Names")]
        public void Should_Support_Select_Statement_Containing_Column_Names()
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

        [Category("Projections")]
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestCase(TestName = "Should Throw when no Column Name or * present")]
        public void Should_Throw_When_Select_Statement_Does_Not_Have_Any_Column_Names_Or_Star()
        {
            // Arrange
            var sql = "Select From contact";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

        }

        [Category("Projections")]
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        [TestCase(TestName = "Should Throw when no From clause present")]
        public void Should_Throw_When_Select_Statement_Does_Not_Have_A_From_Clause()
        {
            // Arrange
            var sql = "Select * From";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

        }

        [Category("Projections")]
        [Test]
        [TestCase(TestName = "Should Throw if it's not a Select statement")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_Not_A_Select_Statement()
        {
            // Arrange
            var sql = "Insert into MyTest (mycolumn) values('test')";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

        }


        [Category("Filter Operators")]
        [Test]
        [TestCase("=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Equals a String Constant")]
        [TestCase("=", 1, "{0} {1} {2}", TestName = "Should support Equals a Numeric Constant")]
        [TestCase("<>", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Not Equals a String Constant")]
        [TestCase("<>", 1, "{0} {1} {2}", TestName = "Should support Not Equals Filter a Numeric Constant")]
        [TestCase(">=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Greater Than Or Equals a String Constant")]
        [TestCase(">=", 1, "{0} {1} {2}", TestName = "Should support Greater Than Or Equals a Numeric Constant")]
        [TestCase("<=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Less Than Or Equals a String Constant")]
        [TestCase("<=", 1, "{0} {1} {2}", TestName = "Should support Less Than Or Equals a Numeric Constant")]
        [TestCase(">", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Greater Than a String Constant")]
        [TestCase(">", 1, "{0} {1} {2}", TestName = "Should support Greater Than a Numeric Constant")]
        [TestCase("<", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Less Than a String Constant")]
        [TestCase("<", 1, "{0} {1} {2}", TestName = "Should support Less Than a Numeric Constant")]
        [TestCase("IS NULL", null, "{0} {1} {2}", TestName = "Should support Is Null")]
        [TestCase("IS NOT NULL", null, "{0} {1}", TestName = "Should support Is Not Null")]
        [TestCase("LIKE", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Like")]
        [TestCase("NOT LIKE", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Not Like")]
        [TestCase("IN", new string[] { "Julius", "Justin" }, "{0} {1} ('{2}', '{3}')", TestName = "Should support In a string array")]
        [TestCase("IN", new int[] { 1, 2 }, "{0} {1} ({2}, {3})", TestName = "Should support In Filter a Numeric array")]
        [TestCase("IN", new string[] { "097e0140-c269-430c-9caf-d2cffe261d8b", "897e0140-c269-430c-9caf-d2cffe261d8c" }, "{0} {1} ('{2}', '{3}')", TestName = "Should support In a Guid array")]
        [TestCase("NOT IN", new string[] { "Julius", "Justin" }, "{0} {1} ('{2}', '{3}')", TestName = "Should support Not In a string array")]
        [TestCase("NOT IN", new int[] { 5, 10 }, "{0} {1} ({2}, {3})", TestName = "Should support Not In a Numeric array")]
        [TestCase("NOT IN", new string[] { "097e0140-c269-430c-9caf-d2cffe261d8b", "897e0140-c269-430c-9caf-d2cffe261d8c" }, "{0} {1} ('{2}', '{3}')", TestName = "Should support Not In a Guid array")]
        [TestCase("LIKE", "SomeValue%", "{0} {1} '{2}'", TestName = "Should support Starts With")]
        [TestCase("LIKE", "%SomeValue", "{0} {1} '{2}'", TestName = "Should support Ends With")]
        [TestCase("LIKE", "%SomeValue%", "{0} {1} '{2}'", TestName = "Should support Like with wildcards")]
        [TestCase("NOT LIKE", "SomeValue%", "{0} {1} '{2}'", TestName = "Should support Does Not Start With")]
        [TestCase("NOT LIKE", "%SomeValue", "{0} {1} '{2}'", TestName = "Should support Does Not End With")]
        [TestCase("NOT LIKE", "%SomeValue%", "{0} {1} '{2}'", TestName = "Should support Not Like with wildcards")]
        [TestCase("CONTAINS", "albert", "{1}({0},  '{2}')", TestName = "Should support Contains.")]
        [TestCase("NOT CONTAINS", "albert", "{1}({0},  '{2}')", TestName = "Should support Not Contains")]
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
            var sql = string.Format("Select contactid, firstname, lastname From contact Where {0}", filterFormatString);
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;
            
            // Create test subject.
            var subject = CreateTestSubject();

            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

            // Assert
            Assert.That(queryExpression.ColumnSet.AllColumns == false);
            Assert.That(queryExpression.ColumnSet.Columns[0] == "contactid");
            Assert.That(queryExpression.ColumnSet.Columns[1] == "firstname");
            Assert.That(queryExpression.EntityName == "contact");

            var defaultConditons = queryExpression.Criteria.Filters[0].Conditions;
            //var defaultConditons = queryExpression.Criteria.Conditions;
                //queryExpression.Criteria.Filters[0];

            Assert.That(defaultConditons.Count, Is.EqualTo(1));
          //  Assert.That(defaultFilterGroup.FilterOperator, Is.EqualTo(LogicalOperator.And));
            Assert.That(defaultConditons[0].AttributeName == "firstname");
            var condition = defaultConditons[0];
            AssertFilterExpressionContion(filterOperator, value, condition);

        }

        [Category("Filter Operators")]
        [Test]
        [TestCase(TestName = "Should Throw if equals filter does not have a column name on one side of the expression.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_The_Where_Equals_Clause_Does_Not_Refer_To_A_Column_Name()
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact Where 'Julius' = 'Julius' and lastname = 'Caeser'";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

        }

        [Category("Joins")]
        [Test]
        [TestCase("INNER", TestName = "Should support Inner Join")]
        [TestCase("LEFT", TestName = "Should support Left Join")]
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


            var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.addressline1 From contact C {0} JOIN address A on C.contactid = A.contactid", joinType);

            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            // Create test subject.
            var subject = CreateTestSubject();

            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

            //Assert
            Assert.That(queryExpression.ColumnSet.Columns.Count, Is.EqualTo(3));
            Assert.That(queryExpression.LinkEntities, Is.Not.Null);
            Assert.That(queryExpression.LinkEntities[0], Is.Not.Null);
            Assert.That(queryExpression.LinkEntities[0].LinkFromEntityName, Is.EqualTo("contact"));
            Assert.That(queryExpression.LinkEntities[0].LinkToEntityName, Is.EqualTo("address"));
            Assert.That(queryExpression.LinkEntities[0].LinkFromAttributeName, Is.EqualTo("contactid"));
            Assert.That(queryExpression.LinkEntities[0].LinkToAttributeName, Is.EqualTo("contactid"));
            Assert.That(queryExpression.LinkEntities[0].EntityAlias, Is.EqualTo("A"));
            Assert.That(queryExpression.LinkEntities[0].JoinOperator, Is.EqualTo(join));
            Assert.That(queryExpression.LinkEntities[0].Columns, Is.Not.Null);
            Assert.That(queryExpression.LinkEntities[0].Columns.Columns, Contains.Item("addressline1"));
        }

        [Category("Joins")]
        [Test]
        [TestCase("INNER", "=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Inner Joins with filter conditions on the Joined table")]
        [TestCase("LEFT", "=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Left Joins with filter condition on the Joined table")]
        public void Should_Support_Joins_With_Where_Filter(string joinType, string filterOperator, object filterValue, string filterFormatString)
        {

            var sql = GenerateTestSqlWithJoinAndFilters(joinType, filterOperator, filterValue, filterFormatString);
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            // Create test subject.
            var subject = CreateTestSubject();
            // Act
            // Ask our test subject to Convert the SelectBuilder to a Query Expression.
            var queryExpression = subject.CreateQueryExpression(cmd);

            // There should be filter criteria on the main entity and also on the link entity.
            //var defaultConditons = queryExpression.Criteria.Conditions;
           var defaultConditons = queryExpression.Criteria.Filters[0].Conditions;
            Assert.That(defaultConditons.Count, Is.EqualTo(2), "There should be two conditions.");

            var condition = defaultConditons[0];
            AssertFilterExpressionContion(filterOperator, filterValue, condition);

            var joinCondition = defaultConditons[1];
            AssertFilterExpressionContion(filterOperator, filterValue, joinCondition);

        }

        [Category("Joins")]
        [Test]
        [TestCase("INNER", "=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Nested Inner Joins with filter conditions on the Joined tables")]
        [TestCase("LEFT", "=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Nested Left Joins with filter conditions on the Joined tables")]
        public void Should_Support_Nested_Joins_With_Where_Filter(string joinType, string filterOperator, object filterValue, string filterFormatString)
        {

            var sql = GenerateTestSqlWithNestedJoinAndFilters(joinType, filterOperator, filterValue, filterFormatString);
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            // Create test subject.
            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);

            // Assert
            // the filter should have 3 conditions, one for main entity attribue, and others for the linked entities..
           // var defaultFilter = queryExpression.Criteria; //.Filters[0];
            var defaultFilter = queryExpression.Criteria.Filters[0];
            var defaultFilterConditions = defaultFilter.Conditions;

            Assert.That(defaultFilterConditions.Count, Is.EqualTo(3), "Wrong number of conditions.");

            var condition = defaultFilterConditions[0];
            Assert.That(condition.EntityName, Is.EqualTo("contact"));
            AssertFilterExpressionContion(filterOperator, filterValue, condition);

            var joinCondition = defaultFilterConditions[1];
            Assert.That(joinCondition.EntityName, Is.EqualTo("A"));
            AssertFilterExpressionContion(filterOperator, filterValue, joinCondition);

            var nestedjoinCondition = defaultFilterConditions[2];
            Assert.That(nestedjoinCondition.EntityName, Is.EqualTo("AC"));
            AssertFilterExpressionContion(filterOperator, filterValue, nestedjoinCondition);

        }

        [Category("Filter Operators")]
        [Test]
        [TestCase(TestName = "Should support filter groups.")]
        public void Should_Support_Filter_Groups()
        {
            // Arrange
            var sql = "SELECT c.firstname, c.lastname FROM contact c WHERE (c.firstname = 'Albert' AND c.lastname = 'Einstein') OR (c.firstname = 'Max' AND c.lastname = 'Planck')";
            var cmd = new CrmDbCommand(null);
            cmd.CommandText = sql;

            var subject = CreateTestSubject();
            // Act
            var queryExpression = subject.CreateQueryExpression(cmd);


            //Assert
            Assert.That(queryExpression.ColumnSet.Columns.Count, Is.EqualTo(2));

          
            Assert.That(queryExpression.Criteria.Filters, Is.Not.Null);
            Assert.That(queryExpression.Criteria.Filters.Count, Is.EqualTo(1));
            Assert.That(queryExpression.Criteria.FilterOperator, Is.EqualTo(LogicalOperator.And));


            var defaultFilterGroup = queryExpression.Criteria.Filters[0];
            Assert.That(defaultFilterGroup.Filters.Count, Is.EqualTo(1));
            Assert.That(defaultFilterGroup.FilterOperator, Is.EqualTo(LogicalOperator.And));

            var topLevelFilterGroup = defaultFilterGroup.Filters[0];
            Assert.That(topLevelFilterGroup.FilterOperator == LogicalOperator.Or);

            var albertEinstenFilter = topLevelFilterGroup.Filters[0];
            Assert.That(albertEinstenFilter.FilterOperator == LogicalOperator.And);
            Assert.That(albertEinstenFilter.Conditions, Is.Not.Null);
            Assert.That(albertEinstenFilter.Conditions.Count, Is.EqualTo(2));
            Assert.That(albertEinstenFilter.Conditions[0].Values, Contains.Item("Albert"));
            Assert.That(albertEinstenFilter.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Equal));
            Assert.That(albertEinstenFilter.Conditions[1].Values, Contains.Item("Einstein"));
            Assert.That(albertEinstenFilter.Conditions[1].Operator, Is.EqualTo(ConditionOperator.Equal));

            var maxPlanckFilter = topLevelFilterGroup.Filters[1];
            Assert.That(maxPlanckFilter.FilterOperator == LogicalOperator.And);
            Assert.That(maxPlanckFilter.Conditions, Is.Not.Null);
            Assert.That(maxPlanckFilter.Conditions.Count, Is.EqualTo(2));
            Assert.That(maxPlanckFilter.Conditions[0].Values, Contains.Item("Max"));
            Assert.That(maxPlanckFilter.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Equal));
            Assert.That(maxPlanckFilter.Conditions[1].Values, Contains.Item("Planck"));
            Assert.That(maxPlanckFilter.Conditions[1].Operator, Is.EqualTo(ConditionOperator.Equal));


        }


        [Category("Filter Operators")]
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
            // Convery the DML (SQL) statement to a SelectBuilder object which an object representation of the DML.
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
            var queryExpression = subject.CreateQueryExpression(cmd);

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
            AssertFilterExpressionContion(filterOperator, value, condition);
        }

        #region Helper Methods

        private static void AssertFilterExpressionContion(string filterOperator, object value, ConditionExpression condition)
        {

            // var condition = filterExpression.Conditions[position];
            switch (filterOperator)
            {
                case "=":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.Equal));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "<>":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.NotEqual));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case ">":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.GreaterThan));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "<":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.LessThan));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case ">=":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.GreaterEqual));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "<=":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.LessEqual));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "IS NULL":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.Null));
                    Assert.That(condition.Values, Is.Empty);
                    break;
                case "IS NOT NULL":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.NotNull));
                    Assert.That(condition.Values, Is.Empty);
                    break;
                case "LIKE":
                    if (value != null)
                    {
                        bool startsWith = false;
                        bool endsWith = false;
                        var stringVal = value.ToString();
                        startsWith = stringVal.EndsWith("%");
                        endsWith = stringVal.StartsWith("%");
                        if (startsWith && !endsWith)
                        {
                            // starts with..
                            var actualValue = stringVal.Remove(stringVal.Length - 1, 1);
                            Assert.That(condition.Operator,
                                        Is.EqualTo(ConditionOperator.BeginsWith));
                            Assert.That(condition.Values, Contains.Item(actualValue));
                        }
                        else if (endsWith && !startsWith)
                        {
                            // ends with
                            var actualValue = stringVal.Remove(0, 1);
                            Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.EndsWith));
                            Assert.That(condition.Values, Contains.Item(actualValue));
                        }
                        else
                        {
                            // like (will also do contains)
                            Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.Like));
                            Assert.That(condition.Values, Contains.Item(value));
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
                        if (startsWith && !endsWith)
                        {
                            // starts with..
                            var actualValue = stringVal.Remove(stringVal.Length - 1, 1);
                            Assert.That(condition.Operator,
                                        Is.EqualTo(ConditionOperator.DoesNotBeginWith));
                            Assert.That(condition.Values, Contains.Item(actualValue));
                        }
                        else if (endsWith && !startsWith)
                        {
                            // ends with
                            var actualValue = stringVal.Remove(0, 1);
                            Assert.That(condition.Operator,
                                        Is.EqualTo(ConditionOperator.DoesNotEndWith));
                            Assert.That(condition.Values, Contains.Item(actualValue));
                        }
                        else
                        {
                            // not like (will also do not contains)
                            Assert.That(condition.Operator,
                                        Is.EqualTo(ConditionOperator.NotLike));
                            Assert.That(condition.Values, Contains.Item(value));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case data");
                    }
                    break;
                case "IN":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.In));
                    if (value != null && value.GetType().IsArray)
                    {
                        foreach (var val in value as Array)
                        {
                            Guid guidVal;
                            if (val is string)
                            {
                                if (Guid.TryParse(val as string, out guidVal))
                                {
                                    Assert.That(condition.Values, Contains.Item(guidVal));
                                    continue;
                                }
                            }

                            Assert.That(condition.Values, Contains.Item(val));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case for IN expression.");
                    }
                    break;
                case "NOT IN":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.NotIn));
                    if (value != null && value.GetType().IsArray)
                    {
                        foreach (var val in value as Array)
                        {
                            Guid guidVal;
                            if (val is string)
                            {
                                if (Guid.TryParse(val as string, out guidVal))
                                {
                                    Assert.That(condition.Values, Contains.Item(guidVal));
                                    continue;
                                }
                            }

                            Assert.That(condition.Values, Contains.Item(val));
                        }
                    }
                    else
                    {
                        Assert.Fail("Unhandled test case for IN expression.");
                    }

                    break;

                case "CONTAINS":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.Contains));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                case "NOT CONTAINS":
                    Assert.That(condition.Operator, Is.EqualTo(ConditionOperator.DoesNotContain));
                    Assert.That(condition.Values, Contains.Item(value));
                    break;
                default:
                    Assert.Fail("Unhandled test case.");
                    break;
            }
        }

        private string GenerateTestSqlWithJoinAndFilters(string joinType, string filterOperator, object filterValue, string filterFormatString)
        {
            // Arrange
            // Formulate DML (SQL) statement from test case data.
            string filterColumnName = "C.firstname";
            var filterOnMainEntity = GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnName);

            string filterColumnOnJoinedTable = "A.name";
            var filterOnJoinedTable = GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnOnJoinedTable);

            var filterGroupOperator = "AND";
            var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.name, A.addressline1 From contact C {0} JOIN address A on C.contactid = A.contactid Where {1} {2} {3} ", joinType, filterOnMainEntity, filterGroupOperator, filterOnJoinedTable);
            // var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.addressline1 From contact C {0} JOIN address A on C.contactid = A.contactid", joinType);
            return sql;
        }

        private string GenerateTestSqlWithNestedJoinAndFilters(string joinType, string filterOperator, object filterValue, string filterFormatString)
        {
            // Arrange
            // Formulate DML (SQL) statement from test case data.
            string filterColumnName = "C.firstname";
            var filterOnMainEntity = GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnName);

            string filterColumnOnJoinedTable = "A.name";
            var filterOnJoinedTable = GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnOnJoinedTable);

            string filterColumnOnNestedJoinedTable = "AC.firstname";
            var filterOnNestedJoinedTable = GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnOnNestedJoinedTable);

            var filterGroupOperator = "AND";
            var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.name, A.addressline1 From contact C {0} JOIN address A on C.contactid = A.contactid {0} JOIN account AC on A.addressid = AC.addressid Where {1} {2} {3} {2} {4}", joinType, filterOnMainEntity, filterGroupOperator, filterOnJoinedTable, filterOnNestedJoinedTable);
            // var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.addressline1 From contact C {0} JOIN address A on C.contactid = A.contactid", joinType);
            return sql;
        }

        private static string GetSqlFilterString(string filterOperator, object filterValue, string filterFormatString, string filterColumnName)
        {
            string sqlFilterString;
            if (filterValue == null || !filterValue.GetType().IsArray)
            {
                sqlFilterString = string.Format(filterFormatString, filterColumnName, filterOperator, filterValue);
                return sqlFilterString;
            }
            var formatArgs = new List<object>();
            formatArgs.Add(filterColumnName);
            formatArgs.Add(filterOperator);
            var args = filterValue as IEnumerable;
            foreach (var arg in args)
            {
                formatArgs.Add(arg);
            }
            sqlFilterString = string.Format(filterFormatString, formatArgs.ToArray());
            return sqlFilterString;
        }
  
        #endregion
    }
}


