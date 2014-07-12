using System;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using SQLGeneration.Generators;
using SQLGeneration.Builders;
using CrmAdo.Tests.WIP;
using Microsoft.Xrm.Sdk.Messages;
using CrmAdo.Tests.Support;
using Microsoft.Xrm.Sdk;
using System.Collections.Generic;
using System.Collections;

namespace CrmAdo.Tests.Visitor
{  

    [TestFixture()]
    public class SelectStatementJoinTests : BaseOrganisationRequestBuilderVisitorTest
    {

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

            // Act
            var queryExpression = GetQueryExpression(sql);

            //Assert

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
            Assert.That(queryExpression.ColumnSet.Columns.Count, Is.EqualTo(3));
        }

        [Category("Joins")]
        [Test]
        [TestCase("INNER", TestName = "Should support Nested Inner Joins")]
        [TestCase("LEFT", TestName = "Should support Nested Left Joins")]
        public void Should_Support_Nested_Joins(string joinType)
        {

            var sql = string.Format("Select C.contactid, C.firstname, C.lastname, AC.name, A.addressline1 From contact C {0} JOIN address A on C.contactid = A.contactid {0} JOIN account AC on A.addressid = AC.addressid", joinType);

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

            // Act
            var queryExpression = GetQueryExpression(sql);

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

            var addressEntity = queryExpression.LinkEntities[0];
            Assert.That(addressEntity.LinkEntities, Is.Not.Null);
            Assert.That(addressEntity.LinkEntities[0], Is.Not.Null);
            Assert.That(addressEntity.LinkEntities[0].LinkFromEntityName, Is.EqualTo("address"));
            Assert.That(addressEntity.LinkEntities[0].LinkToEntityName, Is.EqualTo("account"));
            Assert.That(addressEntity.LinkEntities[0].LinkFromAttributeName, Is.EqualTo("addressid"));
            Assert.That(addressEntity.LinkEntities[0].LinkToAttributeName, Is.EqualTo("addressid"));
            Assert.That(addressEntity.LinkEntities[0].EntityAlias, Is.EqualTo("AC"));
            Assert.That(addressEntity.LinkEntities[0].JoinOperator, Is.EqualTo(join));
            Assert.That(addressEntity.LinkEntities[0].Columns, Is.Not.Null);
            Assert.That(addressEntity.LinkEntities[0].Columns.Columns, Contains.Item("name"));

        }

        [Category("Joins")]
        [Test]
        [TestCase("INNER", "=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Inner Joins with filter conditions on the Joined table")]
        [TestCase("LEFT", "=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Left Joins with filter condition on the Joined table")]
        public void Should_Support_Joins_With_Filters_On_The_Joined_Table(string joinType, string filterOperator, object filterValue, string filterFormatString)
        {
            var sql = GetTestSqlWithJoinAndFilters(joinType, filterOperator, filterValue, filterFormatString);

            // Ask our test subject to Convert the SelectBuilder to a Query Expression.
            // Act
            var queryExpression = GetQueryExpression(sql);

            // There should be filter criteria on the main entity and also on the link entity.
            //var defaultConditons = queryExpression.Criteria.Conditions;
            var defaultConditons = queryExpression.Criteria.Filters[0].Conditions;
            Assert.That(defaultConditons.Count, Is.EqualTo(2), "There should be two conditions.");

            var condition = defaultConditons[0];
            AssertUtils.AssertFilterExpressionContion("firstname", filterOperator, filterValue, condition);

            var joinCondition = defaultConditons[1];
            AssertUtils.AssertFilterExpressionContion("name", filterOperator, filterValue, joinCondition);
        }

        [Category("Joins")]
        [Test]
        [TestCase("INNER", "=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Nested Inner Joins with filter conditions on the Joined tables")]
        [TestCase("LEFT", "=", "SomeValue", "{0} {1} '{2}'", TestName = "Should support Nested Left Joins with filter conditions on the Joined tables")]
        public void Should_Support_Nested_Joins_With_Filters_On_The_Nested_Joined_Tables(string joinType, string filterOperator, object filterValue, string filterFormatString)
        {

            var sql = GenerateTestSqlWithNestedJoinAndFilters(joinType, filterOperator, filterValue, filterFormatString);

            // Act
            var queryExpression = GetQueryExpression(sql);

            // Assert
            // the filter should have 3 conditions, one for main entity attribue, and others for the linked entities..
            // var defaultFilter = queryExpression.Criteria; //.Filters[0];
            var defaultFilter = queryExpression.Criteria.Filters[0];
            var defaultFilterConditions = defaultFilter.Conditions;

            Assert.That(defaultFilterConditions.Count, Is.EqualTo(3), "Wrong number of conditions.");

            var condition = defaultFilterConditions[0];
            Assert.That(condition.EntityName, Is.EqualTo("contact"));
            AssertUtils.AssertFilterExpressionContion("firstname", filterOperator, filterValue, condition);

            var joinCondition = defaultFilterConditions[1];
            Assert.That(joinCondition.EntityName, Is.EqualTo("A"));
            AssertUtils.AssertFilterExpressionContion("name", filterOperator, filterValue, joinCondition);

            var nestedjoinCondition = defaultFilterConditions[2];
            Assert.That(nestedjoinCondition.EntityName, Is.EqualTo("AC"));
            AssertUtils.AssertFilterExpressionContion("firstname", filterOperator, filterValue, nestedjoinCondition);

        }


        private string GetTestSqlWithJoinAndFilters(string joinType, string filterOperator, object filterValue, string filterFormatString)
        {
            // Arrange
            // Formulate DML (SQL) statement from test case data.
            string filterColumnName = "C.firstname";
            var filterOnMainEntity = Utils.GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnName);

            string filterColumnOnJoinedTable = "A.name";
            var filterOnJoinedTable = Utils.GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnOnJoinedTable);

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
            var filterOnMainEntity = Utils.GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnName);

            string filterColumnOnJoinedTable = "A.name";
            var filterOnJoinedTable = Utils.GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnOnJoinedTable);

            string filterColumnOnNestedJoinedTable = "AC.firstname";
            var filterOnNestedJoinedTable = Utils.GetSqlFilterString(filterOperator, filterValue, filterFormatString, filterColumnOnNestedJoinedTable);

            var filterGroupOperator = "AND";
            var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.name, A.addressline1 From contact C {0} JOIN address A on C.contactid = A.contactid {0} JOIN account AC on A.addressid = AC.addressid Where {1} {2} {3} {2} {4}", joinType, filterOnMainEntity, filterGroupOperator, filterOnJoinedTable, filterOnNestedJoinedTable);
            // var sql = string.Format("Select C.contactid, C.firstname, C.lastname, A.addressline1 From contact C {0} JOIN address A on C.contactid = A.contactid", joinType);
            return sql;
        }

    }
}