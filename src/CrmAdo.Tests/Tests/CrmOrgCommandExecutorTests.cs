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
using CrmAdo.Core;

namespace CrmAdo.Tests.Tests
{

    [TestFixture()]
    [Category("Command Execution")]
    public class CrmOrgCommandExecutorTests : BaseTest<CrmOperationExecutor>
    {
        [Test()]
        [Category("Insert Statement")]
        public void Should_Be_Able_To_Execute_An_Insert_Using_ExecuteNonQuery()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                var insertCommand = new CrmDbCommand(dbConnection);
                insertCommand.CommandText = "INSERT INTO contact (firstname, lastname) VALUES ('JO','SCHMO')";
                insertCommand.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationCrmOperationProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOperation(insertCommand, System.Data.CommandBehavior.Default);



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

                var sut = CrmOperationExecutor.Instance;

                dbConnection.Open();

                var result = sut.ExecuteNonQueryOperation(orgCommand);
                Assert.That(result == 1);




            }

        }

        [Test()]
        [Category("Insert Statement")]
        public void Should_Be_Able_To_Execute_An_Insert_Using_ExecuteReader_And_Get_Back_Inserted_Id()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                var insertCommand = new CrmDbCommand(dbConnection);
                insertCommand.CommandText = "INSERT INTO contact (firstname, lastname) VALUES ('JO','SCHMO')";
                insertCommand.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationCrmOperationProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOperation(insertCommand, System.Data.CommandBehavior.Default);

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

                var sut = CrmOperationExecutor.Instance;

                dbConnection.Open();

                var commandResult = sut.ExecuteOperation(orgCommand);
                var results = commandResult.ResultSet;

                //  var results = sut.ExecuteCommand(orgCommand, System.Data.CommandBehavior.Default);
                // results should contain record

                Assert.That(results.ResultCount() == 1);
                Assert.That(results.HasColumnMetadata());

                var reader = commandResult.GetReader();
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 1);

                while (reader.Read())
                {
                    Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(expectedId));
                }




            }

        }

        [Test()]
        [Category("Insert Statement")]
        [Category("Output Clause")]
        public void Should_Be_Able_To_Execute_An_Insert_With_Output_Clause_And_Get_Back_Values()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                var insertCommand = new CrmDbCommand(dbConnection);
                insertCommand.CommandText = "INSERT INTO contact (contactid, firstname, lastname) OUTPUT INSERTED.createdon, INSERTED.contactid VALUES ('9bf20a16-6034-48e2-80b4-8349bb80c3e2','JO','SCHMO')";
                insertCommand.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationCrmOperationProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOperation(insertCommand, System.Data.CommandBehavior.Default);

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

                var sut = CrmOperationExecutor.Instance;
                dbConnection.Open();
                var commandResult = sut.ExecuteOperation(orgCommand);
                var results = commandResult.ResultSet;
                // Assert  

                // Should have 1 result, with the 2 output fields - createdon, and contactid
                Assert.That(results.ResultCount() == 1);
                Assert.That(results.HasColumnMetadata());

                var reader = commandResult.GetReader();
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 2);

                while (reader.Read())
                {
                    Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(createdOnDate));
                    Assert.That(reader.GetGuid(1), NUnit.Framework.Is.EqualTo(expectedId));
                }

            }

        }

        [Test()]
        [Category("Update Statement")]
        public void Should_Be_Able_To_Execute_An_Update_Using_ExecuteNonQuery()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                var command = new CrmDbCommand(dbConnection);
                command.CommandText = "UPDATE contact SET firstname = 'JO', lastname = 'SCHMO' WHERE contactid = '9bf20a16-6034-48e2-80b4-8349bb80c3e2'";
                command.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationCrmOperationProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOperation(command, System.Data.CommandBehavior.Default);


                // This is the fake reponse that the org service will return when its requested to get the data.
                Guid expectedId = Guid.NewGuid();
                var response = new UpdateResponse
                {
                    Results = new ParameterCollection()
                };

                // Setup fake org service to return fake response.
                sandbox.FakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<UpdateRequest>())))
                    .WhenCalled(x =>
                    {
                        var request = ((UpdateRequest)x.Arguments[0]);
                    }).Return(response);

                // Act                          

                var sut = CrmOperationExecutor.Instance;

                dbConnection.Open();

                var result = sut.ExecuteNonQueryOperation(orgCommand);
                Assert.That(result == 1);

            }

        }

        [Test()]
        [Category("Update Statement")]
        [Category("Output Clause")]
        public void Should_Be_Able_To_Execute_An_Update_With_Output_Clause_And_Get_Back_Values()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                var command = new CrmDbCommand(dbConnection);
                command.CommandText = "UPDATE contact SET firstname = 'JO', lastname = 'SCHMO' OUTPUT INSERTED.modifiedon, INSERTED.contactid WHERE contactid = '9bf20a16-6034-48e2-80b4-8349bb80c3e2'";
                command.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationCrmOperationProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOperation(command, System.Data.CommandBehavior.Default);

                // This is a fake CreateResponse that will be returned to our sut at test time.
                Guid expectedId = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
                var createResponse = new UpdateResponse
                {
                    Results = new ParameterCollection()
                };

                // This is a fake RetrieveResponse that will be returned to our sut at test time.
                var resultEntity = new Entity("contact");
                resultEntity.Id = expectedId;
                resultEntity["contactid"] = expectedId;
                var modifiedOnDate = DateTime.UtcNow;
                resultEntity["modifiedon"] = modifiedOnDate;
                var retrieveResponse = new RetrieveResponse
                {
                    Results = new ParameterCollection
                    {
                        { "Entity", resultEntity }
                    }
                };

                // This is a fake ExecuteMultipleResponse that will be returned to our sut at test time.
                var responses = new ExecuteMultipleResponseItemCollection();
                var updateResponseItem = new ExecuteMultipleResponseItem();
                updateResponseItem.RequestIndex = 0;
                updateResponseItem.Response = createResponse;
                responses.Add(updateResponseItem);

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

                var sut = CrmOperationExecutor.Instance;
                dbConnection.Open();

                var commandResult = sut.ExecuteOperation(orgCommand);
                var results = commandResult.ResultSet;

                //var results = sut.ExecuteCommand(orgCommand, System.Data.CommandBehavior.Default);

                // Assert  

                // Should have 1 result, with the 2 output fields - createdon, and contactid
                Assert.That(results.ResultCount() == 1);
                Assert.That(results.HasColumnMetadata());

                var reader = commandResult.GetReader();
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 2);

                while (reader.Read())
                {
                    Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(modifiedOnDate));
                    Assert.That(reader.GetGuid(1), NUnit.Framework.Is.EqualTo(expectedId));
                }

            }

        }

        [Test()]
        [Category("Batch Statement")]
        public void Should_Be_Able_To_Execute_A_Batch_Of_Statenents_And_Get_Multiple_Results()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                Guid id = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
                string insertSql = string.Format("INSERT INTO contact (contactid, firstname, lastname) VALUES ('{0}','billy','bob');", id);
                string insertSqlWithOutputClause = "INSERT INTO contact (firstname, lastname) OUTPUT INSERTED.contactid VALUES ('bat','man');";
                string updateSql = string.Format("UPDATE contact SET firstname = 'john', lastname = 'doe' WHERE contactid = '{0}';", id);
                string updateSqlWithOutputClause = string.Format("UPDATE contact SET firstname = 'johny', lastname = 'doe' OUTPUT INSERTED.modifiedon WHERE contactid = '{0}';", id);
                string deleteSql = string.Format("DELETE FROM contact WHERE contactid = '{0}';", id);
                string selectSql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname ASC;";

                var sql = insertSql + insertSqlWithOutputClause + updateSql + updateSqlWithOutputClause + deleteSql + selectSql;
                //var sql = updateSql + updateSqlWithOutputClause;

                var batchCommand = new CrmDbCommand(dbConnection);
                batchCommand.CommandText = sql;
                batchCommand.CommandType = System.Data.CommandType.Text;

                var provider = new SqlGenerationCrmOperationProvider(new DynamicsAttributeTypeProvider());
                var orgCommand = provider.GetOperation(batchCommand, System.Data.CommandBehavior.Default);

                // This is a fake ExecuteMultipleResponse that will be returned to our sut at test time.
                // prepare a fake org service response
                DateTime expectedOutputModifiedOnDate = DateTime.UtcNow;
                ExecuteMultipleResponseItemCollection fakeOrgServiceResponses = GetFakeOrgServiceResponse(id, expectedOutputModifiedOnDate, sandbox.FakeMetadataProvider);
                var executeMultipleResponse = new ExecuteMultipleResponse
                {
                    Results = new ParameterCollection
                    {
                        { "Responses", fakeOrgServiceResponses },
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
                var sut = CrmOperationExecutor.Instance;
                dbConnection.Open();
                var commandResult = sut.ExecuteOperation(orgCommand);

                // SHould have more results.
                Assert.That(commandResult.HasMoreResults);

                // First result for insert.
                Assert.That(commandResult.ResultSet != null);

                // Should have one row containing inserted record id.
                var reader = commandResult.GetReader();
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 1);

                while (reader.Read())
                {
                    // Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(createdOnDate));
                    Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                }


                // Move to second result for insert plus output.
                reader.NextResult();
                Assert.That(commandResult.ResultSet != null);

                // Should have one row containing inserted record id.               
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 1);

                while (reader.Read())
                {
                    // Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(createdOnDate));
                    // we didn't specify the id, so one should be generated for us.
                    Assert.That(reader.GetGuid(0), NUnit.Framework.Is.Not.Null);
                }

                // Move to third result for update.
                reader.NextResult();
                Assert.That(commandResult.ResultSet != null);

                // Move to fourth result for update plus output.
                reader.NextResult();
                Assert.That(commandResult.ResultSet != null);

                // Should have one row containing output clause values.               
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 1);

                while (reader.Read())
                {
                    Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(expectedOutputModifiedOnDate));
                    //Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                }

                // Move to fifth result for delete.
                reader.NextResult();
                Assert.That(commandResult.ResultSet != null);

                // no result set..

                // Move to sixth result for retrieve multiple.
                reader.NextResult();
                Assert.That(commandResult.ResultSet != null);

                Assert.That(commandResult.ResultSet.ResultCount() == 10);
                Assert.That(commandResult.ResultSet.HasColumnMetadata());
              
                Assert.That(reader.HasRows);
                Assert.That(reader.FieldCount == 2);
                int recordCount = 0;
                while (reader.Read())
                {
                    recordCount++;
                    Assert.That(reader.GetString(0), NUnit.Framework.Is.Not.Null);
                    //  Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                }

                Assert.That(recordCount == 10);
            }

        }

        [Test()]
        [Category("Batch Statement")]
        [Category("Stress")]
        public void Should_Be_Able_To_Execute_A_Batch_Of_Statements_And_Get_Multiple_Results_Stress()
        {
            // Arrange
            using (var sandbox = ConnectionTestsSandbox.Create())
            {

                var dbConnection = sandbox.Container.Resolve<CrmDbConnection>();

                Guid id = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
                string insertSql = string.Format("INSERT INTO contact (contactid, firstname, lastname) VALUES ('{0}','billy','bob');", id);
                string insertSqlWithOutputClause = "INSERT INTO contact (firstname, lastname) OUTPUT INSERTED.contactid VALUES ('bat','man');";
                string updateSql = string.Format("UPDATE contact SET firstname = 'john', lastname = 'doe' WHERE contactid = '{0}';", id);
                string updateSqlWithOutputClause = string.Format("UPDATE contact SET firstname = 'johny', lastname = 'doe' OUTPUT INSERTED.modifiedon WHERE contactid = '{0}';", id);
                string deleteSql = string.Format("DELETE FROM contact WHERE contactid = '{0}';", id);
                string selectSql = "SELECT TOP 10 firstname, lastname FROM contact ORDER BY lastname ASC;";

                var sql = insertSql + insertSqlWithOutputClause + updateSql + updateSqlWithOutputClause + deleteSql + selectSql;

                var provider = new SqlGenerationCrmOperationProvider(new DynamicsAttributeTypeProvider());

                // prepare a fake org service response
                DateTime expectedOutputModifiedOnDate = DateTime.UtcNow;
                ExecuteMultipleResponseItemCollection fakeOrgServiceResponses = GetFakeOrgServiceResponse(id, expectedOutputModifiedOnDate, sandbox.FakeMetadataProvider);
                var executeMultipleResponse = new ExecuteMultipleResponse
                {
                    Results = new ParameterCollection
                    {
                        { "Responses", fakeOrgServiceResponses },
                        { "IsFaulted", false}
                    }
                };

                // Setup fake org service to return fake response.
                sandbox.FakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<ExecuteMultipleRequest>())))
                    .WhenCalled(x =>
                    {
                        var request = ((ExecuteMultipleRequest)x.Arguments[0]);

                    }).Return(executeMultipleResponse);

                /// Execute a batch command multiple times..
                dbConnection.Open();
                for (int i = 0; i < 30; i++)
                {

                    var batchCommand = new CrmDbCommand(dbConnection);
                    batchCommand.CommandText = sql;
                    batchCommand.CommandType = System.Data.CommandType.Text;
                    var orgCommand = provider.GetOperation(batchCommand, System.Data.CommandBehavior.Default);


                    // Act                       
                    var sut = CrmOperationExecutor.Instance;
                    var commandResult = sut.ExecuteOperation(orgCommand);

                    // SHould have more results.
                    Assert.That(commandResult.HasMoreResults);

                    // First result for insert.
                    Assert.That(commandResult.ResultSet != null);

                    // Should have one row containing inserted record id.
                    var reader = commandResult.GetReader();
                    Assert.That(reader.HasRows);
                    Assert.That(reader.FieldCount == 1);

                    while (reader.Read())
                    {
                        // Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(createdOnDate));
                        Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                    }


                    // Move to second result for insert plus output.
                    reader.NextResult();
                    Assert.That(commandResult.ResultSet != null);

                    // Should have one row containing inserted record id.                   
                    Assert.That(reader.HasRows);
                    Assert.That(reader.FieldCount == 1);

                    while (reader.Read())
                    {
                        // Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(createdOnDate));
                        Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                    }

                    // Move to third result for update.
                    reader.NextResult();
                    Assert.That(commandResult.ResultSet != null);

                    // Move to fourth result for update plus output.
                    reader.NextResult();
                    Assert.That(commandResult.ResultSet != null);

                    // Should have one row containing output clause values.                 
                    Assert.That(reader.HasRows);
                    Assert.That(reader.FieldCount == 1);

                    while (reader.Read())
                    {
                        Assert.That(reader.GetDateTime(0), NUnit.Framework.Is.EqualTo(expectedOutputModifiedOnDate));
                        //Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                    }

                    // Move to fifth result for delete.
                    reader.NextResult();
                    Assert.That(commandResult.ResultSet != null);

                    // no result set..

                    // Move to sixth result for retrieve multiple.
                    reader.NextResult();
                    Assert.That(commandResult.ResultSet != null);
                    Assert.That(commandResult.ResultSet.ResultCount() == 10);
                    Assert.That(commandResult.ResultSet.HasColumnMetadata());
                    
                    Assert.That(reader.HasRows);
                    Assert.That(reader.FieldCount == 2);
                    int recordCount = 0;
                    while (reader.Read())
                    {
                        recordCount++;
                        Assert.That(reader.GetString(0), NUnit.Framework.Is.Not.Null);
                        //  Assert.That(reader.GetGuid(0), NUnit.Framework.Is.EqualTo(id));
                    }

                    Assert.That(recordCount == 10);
                }
            }

        }

        private ExecuteMultipleResponseItemCollection GetFakeOrgServiceResponse(Guid id, DateTime modifiedOnDate, ICrmMetaDataProvider crmMetaDataProvider)
        {
            // This is a fake ExecuteMultipleResponse that will be returned to our sut at test time.
            var responses = new ExecuteMultipleResponseItemCollection();

            // fake response for insert.
            ExecuteMultipleResponseItem createResponse = GetCreateResponseItem(0, id);
            responses.Add(createResponse);

            // fake response for second insert.
            ExecuteMultipleResponseItem createResponseWithOutput = GetCreateResponseItem(1, null);
            responses.Add(createResponseWithOutput);

            // fake response for update.
            ExecuteMultipleResponseItem updateResponseItem = GetUpdateResponseItem(2);
            responses.Add(updateResponseItem);

            ExecuteMultipleResponseItem updateResponseItemWithOutput = GetUpdateResponseItem(3);
            responses.Add(updateResponseItemWithOutput);

            // This is a fake RetrieveResponse that will be returned to our sut at test time.
            var updatedContactEntity = new Entity("contact");
            updatedContactEntity.Id = id;
            updatedContactEntity["contactid"] = id;
            updatedContactEntity["modifiedon"] = modifiedOnDate;

            ExecuteMultipleResponseItem retrieveResponseItemForUpdateOutput = GetRetrieveResponseItem(4, updatedContactEntity);
            responses.Add(retrieveResponseItemForUpdateOutput);

            ExecuteMultipleResponseItem deleteResponseItem = GetDeleteResponseItem(5);
            responses.Add(deleteResponseItem);

            var entityDataGenerator = new EntityDataGenerator(crmMetaDataProvider);
            var entities = entityDataGenerator.GenerateFakeEntities("contact", 10);
            EntityCollection fakeEntityResults = new EntityCollection(entities);
            ExecuteMultipleResponseItem retrieveMultipleResponseItem = GetRetrieveMultipleResponseItem(6, fakeEntityResults);
            responses.Add(retrieveMultipleResponseItem);

            return responses;
        }

        private ExecuteMultipleResponseItem GetRetrieveMultipleResponseItem(int requestIndex, EntityCollection resultEntities)
        {
            var retrieveMultipleResponse = new RetrieveMultipleResponse
            {
                Results = new ParameterCollection
                    {
                        { "EntityCollection", resultEntities }
                    }
            };

            var retrieveResponseItem = new ExecuteMultipleResponseItem();
            retrieveResponseItem.RequestIndex = requestIndex;
            retrieveResponseItem.Response = retrieveMultipleResponse;
            return retrieveResponseItem;
        }

        private ExecuteMultipleResponseItem GetDeleteResponseItem(int requestIndex)
        {
            var deleteResponse = new DeleteResponse
            {
                Results = new ParameterCollection()
            };

            var retrieveResponseItem = new ExecuteMultipleResponseItem();
            retrieveResponseItem.RequestIndex = requestIndex;
            retrieveResponseItem.Response = deleteResponse;
            return retrieveResponseItem;
        }

        private ExecuteMultipleResponseItem GetRetrieveResponseItem(int requestIndex, Entity updatedContactEntity)
        {
            var retrieveResponse = new RetrieveResponse
            {
                Results = new ParameterCollection
                    {
                        { "Entity", updatedContactEntity }
                    }
            };

            var retrieveResponseItem = new ExecuteMultipleResponseItem();
            retrieveResponseItem.RequestIndex = requestIndex;
            retrieveResponseItem.Response = retrieveResponse;
            return retrieveResponseItem;

        }

        private ExecuteMultipleResponseItem GetUpdateResponseItem(int requestIndex)
        {
            var updateResponse = new UpdateResponse
            {
                Results = new ParameterCollection()
            };

            var updateResponseItem = new ExecuteMultipleResponseItem();
            updateResponseItem.RequestIndex = requestIndex;
            updateResponseItem.Response = updateResponse;
            return updateResponseItem;

        }

        private ExecuteMultipleResponseItem GetCreateResponseItem(int requestIndex, Guid? specifiedId = null)
        {
            if(specifiedId == null)
            {
                specifiedId = Guid.NewGuid();
            }

            var createResponse = new CreateResponse
            {
                Results = new ParameterCollection
                    {
                        { "id", specifiedId }
                    }
            };

            var createResponseItem = new ExecuteMultipleResponseItem();
            createResponseItem.RequestIndex = requestIndex;
            createResponseItem.Response = createResponse;
            return createResponseItem;
        }



    }
}
