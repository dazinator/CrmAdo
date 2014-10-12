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
        public void Should_Throw_When_Filling_DataSet_With_No_Select_Command()
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
        public void Should_Throw_When_Filling_DataSet_And_No_Select_Command_Connection()
        {
            var subject = CreateTestSubject();
            var mockSelectCommand = MockRepository.GenerateMock<CrmDbCommand>();
            subject.SelectCommand = mockSelectCommand;
            var ds = new DataSet();
            var result = subject.Fill(ds);
        }

        [Test]
        public void Should_Be_Able_To_Fill_DataSet()
        {

            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider.Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeMetadataProvider = new FakeContactMetadataProvider();
            var dbConnection = new CrmDbConnection(mockServiceProvider, fakeMetadataProvider);
            var selectCommand = new CrmDbCommand(dbConnection);
            selectCommand.CommandText = "SELECT * FROM contact";

            int resultCount = 100;

            var entityDataGenerator = new EntityDataGenerator();
            IList<Entity> fakeContactsData = entityDataGenerator.GenerateFakeEntities("contact", resultCount);

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
            Assert.That(table.Rows.Count, NUnit.Framework.Is.EqualTo(resultCount));

        }

        [Test]
        public void Should_Be_Able_To_Fill_DataTable()
        {

            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider.Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeMetadataProvider = new FakeContactMetadataProvider();
            var dbConnection = new CrmDbConnection(mockServiceProvider, fakeMetadataProvider);
            var selectCommand = new CrmDbCommand(dbConnection);
            selectCommand.CommandText = "SELECT * FROM contact";

            int resultCount = 100;

            var entityDataGenerator = new EntityDataGenerator();
            IList<Entity> fakeContactsData = entityDataGenerator.GenerateFakeEntities("contact", resultCount);

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
            var dt = new DataTable();
            var subject = CreateTestSubject();
            subject.SelectCommand = selectCommand;
            var result = subject.Fill(dt);

            // Assert
            Assert.That(dt.Rows.Count, NUnit.Framework.Is.EqualTo(resultCount));

        }

        [Test]
        public void Should_Be_Able_To_Fill_DataSet_Schema_Only()
        {

            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider.Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeMetadataProvider = new FakeContactMetadataProvider();
            var dbConnection = new CrmDbConnection(mockServiceProvider, fakeMetadataProvider);
            var selectCommand = new CrmDbCommand(dbConnection);
            selectCommand.CommandText = "SELECT * FROM contact";

            int resultCount = 100;

            var entityDataGenerator = new EntityDataGenerator();
            IList<Entity> fakeContactsData = entityDataGenerator.GenerateFakeEntities("contact", resultCount);

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
            subject.FillSchema(ds, SchemaType.Source);

            // var result = subject.Fill(ds);

            // Assert
            Assert.NotNull(ds);
            Assert.NotNull(ds.Tables);
            Assert.That(ds.Tables.Count, NUnit.Framework.Is.EqualTo(1));

            var table = ds.Tables[0];
            Assert.That(table.Rows.Count, NUnit.Framework.Is.EqualTo(0));
            Assert.That(table.Columns.Count, NUnit.Framework.Is.GreaterThan(0));

        }

        [Test]
        public void Should_Be_Able_To_Fill_DataTable_Schema_Only()
        {

            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider.Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeMetadataProvider = new FakeContactMetadataProvider();
            var dbConnection = new CrmDbConnection(mockServiceProvider, fakeMetadataProvider);
            var selectCommand = new CrmDbCommand(dbConnection);
            selectCommand.CommandText = "SELECT * FROM contact";

            int resultCount = 100;

            var entityDataGenerator = new EntityDataGenerator();
            IList<Entity> fakeContactsData = entityDataGenerator.GenerateFakeEntities("contact", resultCount);

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
            var dt = new DataTable();
            var subject = CreateTestSubject();
            subject.SelectCommand = selectCommand;
            subject.FillSchema(dt, SchemaType.Source);

            // var result = subject.Fill(dt);

            // Assert       
            Assert.That(dt.Rows.Count, NUnit.Framework.Is.EqualTo(0));
            Assert.That(dt.Columns.Count, NUnit.Framework.Is.GreaterThan(0));

        }

        /// <summary>
        /// Fills a dataset with fake data, then adds a row, modifies a row and deleted a row. Verifies that when the DataAdapter.Update()
        /// method is called, that the appropriate Crm Organisation Service requests are issues (Create Request, Update Request and Delete Request)
        /// to make the changes in the underlying data source (Crm).
        /// </summary>
        [Test]
        public void Should_Be_Able_To_Update_DataSet()
        {

            // Arrange
            var fakeOrgService = MockRepository.GenerateMock<IOrganizationService, IDisposable>();
            var mockServiceProvider = MockRepository.GenerateMock<ICrmServiceProvider>();
            mockServiceProvider.Stub(c => c.GetOrganisationService()).Return(fakeOrgService);

            var fakeMetadataProvider = new FakeContactMetadataProvider();
            var dbConnection = new CrmDbConnection(mockServiceProvider, fakeMetadataProvider);
            var selectCommand = new CrmDbCommand(dbConnection);
            selectCommand.CommandText = "SELECT contactid, firstname, lastname FROM contact";

            // Create a dataset, fill with schema.
            var ds = new DataSet();
            var subject = CreateTestSubject();
            subject.SelectCommand = selectCommand;
            var result = subject.FillSchema(ds, SchemaType.Source);

            var insertCommand = new CrmDbCommand(dbConnection);
            insertCommand.CommandText = "INSERT INTO contact (contactid, firstname, lastname) VALUES (@contactid, @firstname, @lastname)";
            subject.InsertCommand = insertCommand;

            var updateCommand = new CrmDbCommand(dbConnection);
            updateCommand.CommandText = "UPDATE contact SET firstname = @firstname, lastname = @lastname WHERE contactid = @contactid";
            subject.UpdateCommand = updateCommand;

            var deleteCommand = new CrmDbCommand(dbConnection);
            deleteCommand.CommandText = "DELETE FROM contact WHERE contactid = @contactid";
            subject.DeleteCommand = deleteCommand;

            // Add the parameters for the commands.  
            var contactIdParam = CrmDbProviderFactory.Instance.CreateParameter();
            contactIdParam.DbType = DbType.Guid;
            contactIdParam.Direction = ParameterDirection.Input;
            contactIdParam.ParameterName = "@contactid";
            contactIdParam.SourceColumn = "contactid";

            insertCommand.Parameters.Add(contactIdParam);
            updateCommand.Parameters.Add(contactIdParam);
            deleteCommand.Parameters.Add(contactIdParam);

            var firstNameParam = CrmDbProviderFactory.Instance.CreateParameter();
            firstNameParam.DbType = DbType.String;
            firstNameParam.Direction = ParameterDirection.Input;
            firstNameParam.ParameterName = "@firstname";
            firstNameParam.SourceColumn = "firstname";

            insertCommand.Parameters.Add(firstNameParam);
            updateCommand.Parameters.Add(firstNameParam);

            var lastNameParam = CrmDbProviderFactory.Instance.CreateParameter();
            lastNameParam.DbType = DbType.String;
            lastNameParam.Direction = ParameterDirection.Input;
            lastNameParam.ParameterName = "@lastname";
            lastNameParam.SourceColumn = "lastname";

            insertCommand.Parameters.Add(lastNameParam);
            updateCommand.Parameters.Add(lastNameParam);

            // Fill the dataset with 100 contact entities from the data source.
            int resultCount = 100;
            var entityDataGenerator = new EntityDataGenerator();
            IList<Entity> fakeContactsData = entityDataGenerator.GenerateFakeEntities("contact", resultCount);

            // This is the fake reponse that the org service will return when its requested to get the data.
            var retrieveResponse = new RetrieveMultipleResponse
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
                }).Return(retrieveResponse);

            subject.Fill(ds);

            // Now add a new contact, update an existing contact, and delete an existing contact.
            var newContact = entityDataGenerator.GenerateFakeEntities("contact", 1)[0];
            var contactDataTable = ds.Tables[0];

            var firstNameCol = contactDataTable.Columns["firstname"];
            var lastnameCol = contactDataTable.Columns["lastname"];
            var contactidcol = contactDataTable.Columns["contactid"];

            var newRow = contactDataTable.NewRow();

            newRow.SetField(firstNameCol, newContact["firstname"]);
            newRow.SetField(lastnameCol, newContact["lastname"]);
            newRow.SetField(contactidcol, newContact.Id);

            contactDataTable.Rows.Add(newRow);

            // update existing contact.
            var modifiedRow = contactDataTable.Rows[50];

            var updatedFirstName = "Jessie";
            var updatedLastName = "James";
            modifiedRow.SetField(firstNameCol, updatedFirstName);
            modifiedRow.SetField(lastnameCol, updatedLastName);

            // Delete existing contact
            var deleteRow = contactDataTable.Rows[99];
            var deleteContactId = (Guid)deleteRow[contactidcol];
            deleteRow.Delete();

            // When we call update on the dataset we need to verify that the org service is sent
            // an appropriate Create / Updated and Delete Request.
            var createResponse = new CreateResponse
            {
                Results = new ParameterCollection 
                {
                     { "id", newContact.Id }
                }
            };

            // Setup fake org service create response.
            CreateRequest capturedCreateRequest = null;

            fakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<CreateRequest>())))
                .WhenCalled(x =>
                {
                    var request = ((CreateRequest)x.Arguments[0]);
                    capturedCreateRequest = request;

                }).Return(createResponse);


            // Setup fake org service update response.
            var updateResponse = new UpdateResponse
            {
                Results = new ParameterCollection()
                {
                }
            };

            UpdateRequest capturedUpdateRequest = null;

            fakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<UpdateRequest>())))
                .WhenCalled(x =>
                {
                    var request = ((UpdateRequest)x.Arguments[0]);
                    capturedUpdateRequest = request;

                }).Return(updateResponse);

            // Setup fake org service delete response.
            var deleteResponse = new DeleteResponse
            {
                Results = new ParameterCollection()
                {
                }
            };


            DeleteRequest capturedDeleteRequest = null;

            fakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<DeleteRequest>())))
                .WhenCalled(x =>
                {
                    var request = ((DeleteRequest)x.Arguments[0]);
                    capturedDeleteRequest = request;

                }).Return(deleteResponse);



            // ACT
            subject.Update(ds);

            // ASSERT
            // A create request for the new row data should have been captured.
            // An update request for the modified row data should have been captured.
            // A delete request for the deleted row should have been captured.
            Assert.NotNull(capturedCreateRequest);
            Assert.NotNull(capturedUpdateRequest);
            Assert.NotNull(capturedDeleteRequest);

            var forCreate = capturedCreateRequest.Target;
            Assert.AreEqual(forCreate.Id, newContact.Id);
            Assert.AreEqual(forCreate["firstname"], newContact["firstname"]);
            Assert.AreEqual(forCreate["lastname"], newContact["lastname"]);

            var forUpdate = capturedUpdateRequest.Target;
            Assert.AreEqual(forUpdate.Id, modifiedRow[contactidcol]);
            Assert.AreEqual(forUpdate["firstname"], updatedFirstName);
            Assert.AreEqual(forUpdate["lastname"], updatedLastName);

            var forDelete = capturedDeleteRequest.Target;
            Assert.AreEqual(forDelete.Id, deleteContactId);



        }




    }


}
