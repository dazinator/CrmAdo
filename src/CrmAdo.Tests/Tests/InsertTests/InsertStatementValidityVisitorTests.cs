using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NUnit.Framework;
using CrmAdo.Tests.Support;

namespace CrmAdo.Tests
{
    [Category("Validity")]
    [Category("Insert Statement")]
    [TestFixture()]
    public class InsertStatementValidityVisitorTests : BaseOrganisationRequestBuilderVisitorTest
    {

        [Test(Description = "Should Throw when Insert statement has different number of columns to values specified")]
        [ExpectedException(typeof(ArgumentException))]
        public void Should_Throw_When_Insert_Statement_Has_Different_Number_Of_Columns_To_Values_Specified()
        {
            // Arrange
            var sql = "INSERT INTO contact (firstname) VALUES (1, 'bloggs')";
            // Act
            var queryExpression = GetOrganizationRequest<CreateRequest>(sql);
        }


    }
}