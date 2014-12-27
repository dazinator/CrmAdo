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
    [Category("Request Execution")]
    public class CrmOrgCommandExecutorTests : BaseTest<CrmOrgCommandExecutor>
    {
         [Test()]
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


    }
}
