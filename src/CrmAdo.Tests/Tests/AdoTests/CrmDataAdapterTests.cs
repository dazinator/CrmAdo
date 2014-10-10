using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Ado;
using CrmAdo.Dynamics;
using Rhino.Mocks.Constraints;
using Microsoft.Xrm.Sdk.Messages;
using CrmAdo.Dynamics.Metadata;
using CrmAdo.Tests.Support;

namespace CrmAdo.Tests
{

    public class OrganizationRequestMessageConstraint<T> : AbstractConstraint where T : OrganizationRequest
    {
        //private string _EntityName;

        public OrganizationRequestMessageConstraint()
        {
            //_EntityName = entityName;
        }

        public override bool Eval(object actual)
        {
            if (actual != null)
            {
                if (actual.GetType() == typeof(T))
                {
                    return true;
                }
            }
            return false;
        }

        public override string Message { get { return "The organisation request message was not of type: " + typeof(T).Name; } }

    }

    // Constraint use
    //Arg<User>.Matches(new UserConstraint(expectedUser));


    [Category("ADO")]
    [Category("DataAdapter")]
    [TestFixture()]
    public class CrmDataAdapterTests : BaseTest<CrmDataAdapter>
    {
        [Test]
        public void Should_Be_Able_To_Create_A_New_CrmDataAdapter()
        {
            var subject = CreateTestSubject();
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [Test]
        public void Should_Throw_When_Filling_Dataset_With_No_Select_Command()
        {
            var subject = CreateTestSubject();
            var ds = new DataSet();
            var result = subject.Fill(ds);



            //var conn = MockRepository.GenerateMock<CrmDbConnection>();
            //var results = new EntityResultSet(null, null, null);
            //results.ColumnMetadata = new List<ColumnMetadata>();

            //var firstName = MockRepository.GenerateMock<AttributeInfo>();
            //var lastname = MockRepository.GenerateMock<AttributeInfo>();
            //var firstNameC = new ColumnMetadata(firstName);
            //var lastnameC = new ColumnMetadata(lastname);



            //subject.
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [Test]
        public void Should_Throw_When_Filling_Dataset_And_No_Select_Command_Connection()
        {
            var subject = CreateTestSubject();
            var mockSelectCommand = MockRepository.GenerateMock<CrmDbCommand>();
            subject.SelectCommand = mockSelectCommand;
            var ds = new DataSet();
            var result = subject.Fill(ds);
        }

        [Test]
        public void Should_Be_Able_To_Fill_Data_Set()
        {

            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider.Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeMetadataProvider = new FakeContactMetadataProvider();
            var dbConnection = new CrmDbConnection(mockServiceProvider, fakeMetadataProvider);
            var selectCommand = new CrmDbCommand(dbConnection);
            selectCommand.CommandText = "SELECT * FROM contact";

            var entityDataGenerator = new EntityDataGenerator();
            IList<Entity> fakeContactsData = entityDataGenerator.GenerateFakeEntities("contact", 100);

            // This is the fake reponse that the org service will return when its requested to get the data.
            var response = new RetrieveMultipleResponse
            {
                Results = new ParameterCollection
 {
 { "EntityCollection", new EntityCollection(fakeContactsData){EntityName = "contact"} }
 }
            };

            // Setup fake org service to return fake response.
            fakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<RetrieveMultipleRequest>())))
                .WhenCalled(x =>
                {
                    var request = ((RetrieveMultipleRequest)x.Arguments[0]);
                }).Return(response);

            // Act
            var ds = new DataSet();
            var subject = CreateTestSubject();
            subject.SelectCommand = selectCommand;
            var result = subject.Fill(ds);

            // Assert
            Assert.NotNull(ds);
            Assert.NotNull(ds.Tables);
            Assert.That(ds.Tables.Count, NUnit.Framework.Is.EqualTo(1));

            var table = ds.Tables[0];
        }       

    }  

   
}
