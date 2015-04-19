using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Tests.Support;
using CrmAdo.Tests.Sandbox;

namespace CrmAdo.Tests
{
    [Category("Batch Statement")]
    [TestFixture()]
    public class BatchVisitorTests : BaseOrganisationRequestBuilderVisitorTest
    {

        [Test(Description = "Should support a a batch of sql statements via executemultiplerequest")]
        public void Should_Support_Batch_Of_Statements_Via_ExecuteMultipleRequest()
        {
            // Arrange

            Guid id = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");

            string insertSql = string.Format("INSERT INTO contact (contactid, firstname, lastname) VALUES ('{0}','billy','bob');", id);
            string insertSqlWithOutputClause = "INSERT INTO contact (firstname, lastname) OUTPUT INSERTED.contactid VALUES ('bat','man');";
            string updateSql = string.Format("UPDATE contact SET firstname = 'john', lastname = 'doe' WHERE contactid = '{0}';", id);
            string updateSqlWithOutputClause = string.Format("UPDATE contact SET firstname = 'johny', lastname = 'doe' OUTPUT INSERTED.modifiedon WHERE contactid = '{0}';", id);
            string deleteSql = string.Format("DELETE FROM contact WHERE contactid = '{0}';", id);
            string selectSql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname ASC;";


            var sql = insertSql + insertSqlWithOutputClause + updateSql + updateSqlWithOutputClause + deleteSql + selectSql;

            using (var sandbox = RequestProviderTestsSandbox.Create())
            {

                var cmd = new CrmDbCommand(sandbox.FakeCrmDbConnection);
                cmd.CommandText = sql;

                // Act
                var executeMultipleRequest = GetOrganizationRequest<ExecuteMultipleRequest>(cmd);

                // Assert
                Assert.That(executeMultipleRequest, Is.Not.Null);
                Assert.That(executeMultipleRequest.Requests, Is.Not.Null);

                // the first statement should translate to a CreateRequest
                var createRequest = executeMultipleRequest.Requests[0];
                AssertRequestMessageTypeIs<CreateRequest>(createRequest);

                // the second statement should translate to a CreateRequest (no retreive as the output of id doesn't require a seperate retrieve)
                var createRequestNumber2 = executeMultipleRequest.Requests[1];
                AssertRequestMessageTypeIs<CreateRequest>(createRequestNumber2);             

                // the third statement should translate to a UpdateRequest
                var updateRequest = executeMultipleRequest.Requests[2];
                AssertRequestMessageTypeIs<UpdateRequest>(updateRequest);

                // the fourth statement should translate to a UpdateRequest, anda RetrieveRequest
                var updateRequestNumber2 = executeMultipleRequest.Requests[3];
                AssertRequestMessageTypeIs<UpdateRequest>(updateRequestNumber2);
                var retrieveRequestForUpdateOutputClause = executeMultipleRequest.Requests[4];
                AssertRequestMessageTypeIs<RetrieveRequest>(retrieveRequestForUpdateOutputClause);

                // the fifth statement should translate to a DeleteRequest
                var deleteRequest = executeMultipleRequest.Requests[5];
                AssertRequestMessageTypeIs<DeleteRequest>(deleteRequest);

                // the sixth statement should translate to a RetrieveMultipleRequest
                var retrieveMultipleRequest = executeMultipleRequest.Requests[6];
                AssertRequestMessageTypeIs<RetrieveMultipleRequest>(retrieveMultipleRequest);              
            }

        }

        public void AssertRequestMessageTypeIs<T>(OrganizationRequest message)
            where T : OrganizationRequest
        {
            Assert.That(message, Is.Not.Null);
            Assert.That(message, Is.TypeOf<T>());
        }


    }
}