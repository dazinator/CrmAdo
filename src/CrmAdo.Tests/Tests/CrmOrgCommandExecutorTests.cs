using CrmAdo.Core;
using CrmAdo.Dynamics;
using CrmAdo.Tests.Support;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace CrmAdo.Tests.Tests
{

    [TestFixture()]
    [Category("Command Execution")]
    public class CrmOrgCommandExecutorTests : BaseTest<CrmOrgCommandExecutor>
    {
        [Test()]
        [Category("Execute Non Query")]
        public void Should_Be_Able_To_Execute_An_Insert_Using_ExecuteNonQuery()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                var insertCommand = new CrmDbCommand(dbConnection);
                insertCommand.CommandText = "INSERT INTO contact (firstname, lastname) VALUES ('JO','SCHMO')";
                insertCommand.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationOrganizationCommandProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOrganisationCommand(insertCommand, System.Data.CommandBehavior.Default);



                // This is the fake reponse that the org service will return when its requested to get the data.
                Guid expectedId = Guid.NewGuid();
                var response = new CreateResponse
                {
                    Results = new ParameterCollection
 {
 { "id", expectedId }
 }
                };

                //                   Results = new ParameterCollection
                //{
                //{ "EntityCollection", new EntityCollection(fakeContactsData){EntityName = "contact"} }
                //}





                // Setup fake org service to return fake response.
                sandbox.FakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<CreateRequest>())))
                    .WhenCalled(x =>
                    {
                        var request = ((CreateRequest)x.Arguments[0]);
                    }).Return(response);

                // Act                          

                var sut = CrmOrgCommandExecutor.Instance;

                dbConnection.Open();

                var result = sut.ExecuteNonQueryCommand(orgCommand);
                Assert.That(result == 1);




            }

        }

        [Test()]
        [Category("Execute With ResultSet")]
        public void Should_Be_Able_To_Execute_An_Insert_Using_ExecuteReader_And_Get_Back_Inserted_Id()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                var insertCommand = new CrmDbCommand(dbConnection);
                insertCommand.CommandText = "INSERT INTO contact (firstname, lastname) VALUES ('JO','SCHMO')";
                insertCommand.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationOrganizationCommandProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOrganisationCommand(insertCommand, System.Data.CommandBehavior.Default);

                // This is the fake reponse that the org service will return when its requested to get the data.
                Guid expectedId = Guid.NewGuid();
                var response = new CreateResponse
                {
                    Results = new ParameterCollection
 {
 { "id", expectedId }
 }
                };

                //                   Results = new ParameterCollection
                //{
                //{ "EntityCollection", new EntityCollection(fakeContactsData){EntityName = "contact"} }
                //}





                // Setup fake org service to return fake response.
                sandbox.FakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<CreateRequest>())))
                    .WhenCalled(x =>
                    {
                        var request = ((CreateRequest)x.Arguments[0]);
                    }).Return(response);

                // Act                          

                var sut = CrmOrgCommandExecutor.Instance;

                dbConnection.Open();

                var results = sut.ExecuteCommand(orgCommand, System.Data.CommandBehavior.Default);
                // results should contain record

                Assert.That(results.ResultCount() == 1);
                Assert.That(results.HasColumnMetadata());

                var reader = results.GetReader();
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 1);

                while (reader.Read())
                {
                    Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(expectedId));
                }




            }

        }

        [Test()]
        [Category("Execute With ResultSet")]
        public void Should_Be_Able_To_Execute_An_Insert_With_Output_Clause_And_Get_Back_Values()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                var insertCommand = new CrmDbCommand(dbConnection);
                insertCommand.CommandText = "INSERT INTO contact (contactid, firstname, lastname) OUTPUT INSERTED.createdon, INSERTED.contactid VALUES ('9bf20a16-6034-48e2-80b4-8349bb80c3e2','JO','SCHMO')";
                insertCommand.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationOrganizationCommandProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOrganisationCommand(insertCommand, System.Data.CommandBehavior.Default);

                // This is a fake CreateResponse that will be returned to our sut at test time.
                Guid expectedId = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
                var createResponse = new CreateResponse
                {
                    Results = new ParameterCollection
                    {
                        { "id", expectedId }
                    }
                };

                // This is a fake RetrieveResponse that will be returned to our sut at test time.
                var resultEntity = new Entity("contact");
                resultEntity.Id = expectedId;
                resultEntity["contactid"] = expectedId;
                var createdOnDate = DateTime.UtcNow;
                resultEntity["createdon"] = createdOnDate;
                var retrieveResponse = new RetrieveResponse
                {
                    Results = new ParameterCollection
                    {
                        { "Entity", resultEntity }
                    }
                };

                // This is a fake ExecuteMultipleResponse that will be returned to our sut at test time.
                var responses = new ExecuteMultipleResponseItemCollection();
                var createResponseItem = new ExecuteMultipleResponseItem();
                createResponseItem.RequestIndex = 0;
                createResponseItem.Response = createResponse;
                responses.Add(createResponseItem);

                var retrieveResponseItem = new ExecuteMultipleResponseItem();
                retrieveResponseItem.RequestIndex = 1;
                retrieveResponseItem.Response = retrieveResponse;
                responses.Add(retrieveResponseItem);

                var executeMultipleResponse = new ExecuteMultipleResponse
                {
                    Results = new ParameterCollection
                    {
                        { "Responses", responses },
                        { "IsFaulted", false}
                    }
                };

                // Setup fake org service to return fake response.
                sandbox.FakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<ExecuteMultipleRequest>())))
                    .WhenCalled(x =>
                    {
                        var request = ((ExecuteMultipleRequest)x.Arguments[0]);

                    }).Return(executeMultipleResponse);

                // Act                        

                var sut = CrmOrgCommandExecutor.Instance;
                dbConnection.Open();
                var results = sut.ExecuteCommand(orgCommand, System.Data.CommandBehavior.Default);

                // Assert  
                
                // Should have 1 result, with the 2 output fields - createdon, and contactid
                Assert.That(results.ResultCount() == 1);
                Assert.That(results.HasColumnMetadata());

                var reader = results.GetReader();
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 2);

                while (reader.Read())
                {
                    Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(createdOnDate));
                    Assert.That(reader.GetGuid(1), NUnit.Framework.Is.EqualTo(expectedId));
                }

            }

        }


    }
}
