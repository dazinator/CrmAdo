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
        public void Where_Clause_Equals_String_Literal_Value()
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
        public void Where_Clause_Not_Equals_String_Literal_Value()
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
