using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using CrmAdo.Tests.Support;
using CrmAdo.Tests.Sandbox;

namespace CrmAdo.Tests
{
    [TestFixture(Category = "Issues")]
    public class IssueTests : BaseOrganisationRequestBuilderVisitorTest
    {
        [Category("Github Issues")]       
        [TestCase(true, TestName = "Issue #73 - Should treat column names as case sensitive when CaseSensitiveColumnNames setting is on.")]
        [TestCase(false, TestName = "Issue #73 - Should use ToLower() on column names when CaseSensitiveColumnNames setting is off.")]
        public void Should_Preserve_Casing(bool caseSensitive)
        {
            // Arrange
            var sql = "Select contactid, firstname, lastname, MADE_UpField From contact";
            using (var sandbox = RequestProviderTestsSandbox.Create())
            {

                sandbox.FakeCrmDbConnection.Settings.CaseSensitiveColumnNames = caseSensitive;
                // Act

                var queryExpression = GetQueryExpression(sandbox.FakeCrmDbConnection, sql);
                // Assert
                Assert.That(queryExpression.ColumnSet != null);

                Assert.That(queryExpression.ColumnSet.Columns.Contains("firstname"));
                Assert.That(queryExpression.ColumnSet.Columns.Contains("lastname"));

                if(caseSensitive)
                {
                    Assert.That(queryExpression.ColumnSet.Columns.Contains("MADE_UpField"));
                }
                else
                {
                    Assert.That(queryExpression.ColumnSet.Columns.Contains("made_upfield"));
                }
                              

            }
        }

       


    }
}