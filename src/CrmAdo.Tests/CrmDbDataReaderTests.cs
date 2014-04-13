using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;

namespace CrmAdo.Tests
{
    [Category("ADO")]
    [Category("DataReader")]
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
            var results = new EntityResultSet(null, null);
            results.ColumnMetadata = new List<ColumnMetadata>();
            results.Results = new EntityCollection(new List<Entity>());
            var subject = CreateTestSubject(results);
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Data_Reader_With_Connection_And_Results()
        {
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            var results = new EntityResultSet(null,null);
            results.ColumnMetadata = new List<ColumnMetadata>();
            var firstName = MockRepository.GenerateMock<AttributeMetadata>();
            var lastname = MockRepository.GenerateMock<AttributeMetadata>();
            var firstNameC = new ColumnMetadata(firstName);
            var lastnameC = new ColumnMetadata(lastname);


            results.ColumnMetadata.Add(firstNameC);
            results.ColumnMetadata.Add(lastnameC);
            results.Results = new EntityCollection(new List<Entity>());
            var result = new Entity("contact");
            result.Id = Guid.NewGuid();
            result["firstname"] = "joe";
            result["lastname"] = "schmoe";
            results.Results.Entities.Add(result);
            var subject = CreateTestSubject(results, conn);
        }

        [Test]
        public void Should_Close_Connection_When_Finished_Reading()
        {
            var conn = MockRepository.GenerateMock<CrmDbConnection>();
            var results = new EntityResultSet(null,null);
            results.ColumnMetadata = new List<ColumnMetadata>();

            //   var firstName = new AttributeMetadata();
            //  var lastname = new AttributeMetadata();

            var firstNameC = MockRepository.GenerateMock<ColumnMetadata>();
            firstNameC.Stub(a => a.ColumnName).Return("firstname");
            firstNameC.Stub(a => a.GetDataTypeName()).Return("string");
            firstNameC.Stub(a => a.AttributeType()).Return(AttributeTypeCode.String);

            var lastnameC = MockRepository.GenerateMock<ColumnMetadata>();
            lastnameC.Stub(a => a.ColumnName).Return("lastname");
            lastnameC.Stub(a => a.GetDataTypeName()).Return("string");
            lastnameC.Stub(a => a.AttributeType()).Return(AttributeTypeCode.String);
            //  lastnameC.AttributeMetadata

            // var firstName = MockRepository.GenerateMock<AttributeMetadata>();
            //var lastname = MockRepository.GenerateMock<AttributeMetadata>();
            //  firstName.LogicalName = "firstname";
            //  lastname.LogicalName = "lastname";

            //lastname.Stub(a => a.LogicalName).Return("lastname");
            //firstName.Stub(a => a.AttributeType).Return(AttributeTypeCode.String);
            //lastname.Stub(a => a.AttributeType).Return(AttributeTypeCode.String);

            results.ColumnMetadata.Add(firstNameC);
            results.ColumnMetadata.Add(lastnameC);
            results.Results = new EntityCollection(new List<Entity>());
            var result = new Entity("contact");
            result.Id = Guid.NewGuid();
            result["firstname"] = "joe";
            result["lastname"] = "schmoe";
            results.Results.Entities.Add(result);
            var subject = new CrmDbDataReader(results, conn);
            foreach (var r in subject)
            {

            }
            conn.AssertWasCalled(o => o.Close(), options => options.Repeat.Once());

        }



    }
}
