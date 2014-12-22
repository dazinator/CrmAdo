using CrmAdo.Tests.Sandbox;
using CrmAdo.Tests.Support;
using NUnit.Framework;

namespace CrmAdo.Tests
{

    [TestFixture(Category = "Select Statement")]
    public class SelectStatementProjectionVisitorTests : BaseOrganisationRequestBuilderVisitorTest
    {

        [Category("Projecting")]
        [Test(Description = "Should support *")]
        public void Should_Support_Select_Statement_Containing_Star()
        {
            // Arrange
            var sql = "Select * From contact";
            using (var sandbox = RequestProviderTestsSandbox.Create())
            {

                // Act
                var queryExpression = GetQueryExpression(sql);
                // Assert
                Assert.That(queryExpression.ColumnSet.AllColumns == true);
                Assert.That(queryExpression.EntityName == "contact");
            }
        }

        [Category("Projecting")]
        [Test(Description = "Should support Column Names")]
        public void Should_Support_Select_Statement_Containing_Column_Names()
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname From contact";
            using (var sandbox = RequestProviderTestsSandbox.Create())
            {

                // Act
                var queryExpression = GetQueryExpression(sql);
                // Assert
                Assert.That(queryExpression.ColumnSet.AllColumns == false);
                Assert.That(queryExpression.ColumnSet.Columns[0] == "contactid");
                Assert.That(queryExpression.ColumnSet.Columns[1] == "firstname");
                Assert.That(queryExpression.ColumnSet.Columns[2] == "lastname");
                Assert.That(queryExpression.EntityName == "contact");
            }
        }

    }
}