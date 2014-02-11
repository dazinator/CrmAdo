using System;
using System.Collections.Generic;
using System.Data;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;

namespace DynamicsCrmDataProvider.Tests
{
    [TestFixture()]
    public class CrmDbDataReaderTests : BaseTest<CrmDbDataReader>
    {

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_Throw_Argument_Null_When_Constructed_With_Null_Results()
        {
            EntityResultSet results = null;
            var subject = new CrmDbDataReader(results);
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Data_Reader()
        {
            var results = new EntityResultSet();
            results.ColumnMetadata = new List<AttributeMetadata>();
            results.Results = new EntityCollection(new List<Entity>());
            var subject = CreateTestSubject(results);
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Data_Reader_With_Connection_And_Results()
        {
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            var results = new EntityResultSet();
            results.ColumnMetadata = new List<AttributeMetadata>();
            var firstName = MockRepository.GenerateMock<AttributeMetadata>();
            var lastname = MockRepository.GenerateMock<AttributeMetadata>();
            results.ColumnMetadata.Add(firstName);
            results.ColumnMetadata.Add(lastname);
            results.Results = new EntityCollection(new List<Entity>());
            var result = new Entity("contact");
            result.Id = Guid.NewGuid();
            result["firstname"] = "joe";
            result["lastname"] = "schmoe";
            results.Results.Entities.Add(result);
            var subject = CreateTestSubject(results, conn);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Should_Close_Connection_When_Finished_Reading()
        {
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            var results = new EntityResultSet();
            results.ColumnMetadata = new List<AttributeMetadata>();
            var firstName = MockRepository.GenerateMock<AttributeMetadata>();
            var lastname = MockRepository.GenerateMock<AttributeMetadata>();
            firstName.LogicalName = "firstname";
            lastname.LogicalName = "lastname";
            firstName.Stub(a => a.AttributeType).Return(AttributeTypeCode.String);
            lastname.Stub(a => a.AttributeType).Return(AttributeTypeCode.String);

            results.Results = new EntityCollection(new List<Entity>());
            var result = new Entity("contact");
            result.Id = Guid.NewGuid();
            result["firstname"] = "joe";
            result["lastname"] = "schmoe";
            results.Results.Entities.Add(result);
            var subject = CreateTestSubject(conn, results);
            foreach (var r in subject)
            {

            }
            subject.Close();
            conn.AssertWasCalled(o => o.Close(), options => options.Repeat.Once());


        }



    }
}
