using System.Collections;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    [TestFixture()]
    public class SelectStatementWhereFilterConjunctionTests : CrmQueryExpressionProviderTestsBase
    {


        [Category("Filtering")]
        [Test]
        [TestCase(TestName = "Should support conjunctions using AND and OR.")]
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




        #region Helper Methods







        #endregion
    }
}


