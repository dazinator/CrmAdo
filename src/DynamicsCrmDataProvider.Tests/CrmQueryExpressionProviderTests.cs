using System;
using System.Data;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
        public void Should_Create_Query_Expression_For_All_Columns_Of_Entity()
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
        public void Should_Create_Query_Expression_For_Named_Columns_Of_Entity()
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
        public void Should_Throw_When_No_Columns_Selected()
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
        public void Should_Throw_When_No_From_Entity_Specified()
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
        public void Should_Throw_When_Not_A_Select_Statement()
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
        public void Should_Create_Query_Expression_With_Where_Equals_Criteria_And_String_Literals()
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact Where firstname = 'Julius' and lastname = 'Caeser'";
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
            Assert.That(queryExpression.Criteria.Conditions.Count, Is.EqualTo(2));
            Assert.That(queryExpression.Criteria.FilterOperator, Is.EqualTo(LogicalOperator.And));
            Assert.That(queryExpression.Criteria.Conditions[0].AttributeName == "firstname");
            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.Equal));
            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item("Julius"));

            Assert.That(queryExpression.Criteria.Conditions[1].AttributeName == "lastname");
            Assert.That(queryExpression.Criteria.Conditions[1].Operator, Is.EqualTo(ConditionOperator.Equal));
            Assert.That(queryExpression.Criteria.Conditions[1].Values, Contains.Item("Caeser"));

        }

        [Test]
        public void Should_Create_Query_Expression_With_Where_Equals_Criteria_And_Numeric_Values()
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
        public void Should_Create_Query_Expression_With_Where_Not_Equals_Criteria_And_String_Literals()
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact Where firstname <> 'Julius' and lastname <> 'Caeser'";
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
            Assert.That(queryExpression.Criteria.Conditions.Count, Is.EqualTo(2));
            Assert.That(queryExpression.Criteria.FilterOperator, Is.EqualTo(LogicalOperator.And));
            Assert.That(queryExpression.Criteria.Conditions[0].AttributeName == "firstname");
            Assert.That(queryExpression.Criteria.Conditions[0].Operator, Is.EqualTo(ConditionOperator.NotEqual));
            Assert.That(queryExpression.Criteria.Conditions[0].Values, Contains.Item("Julius"));
            Assert.That(queryExpression.Criteria.Conditions[1].AttributeName == "lastname");
            Assert.That(queryExpression.Criteria.Conditions[1].Operator, Is.EqualTo(ConditionOperator.NotEqual));
            Assert.That(queryExpression.Criteria.Conditions[1].Values, Contains.Item("Caeser"));

        }
     
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Throw_When_Query_Contains_Where_Clause_Equals_Condition_With_No_Attribute_Name()
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
