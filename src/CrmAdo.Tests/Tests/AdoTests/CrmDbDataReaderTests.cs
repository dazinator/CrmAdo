using System;
using System.Collections.Generic;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;
using CrmAdo.Metadata;
using CrmAdo.Tests.Sandbox;

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
            var results = new EntityResultSet(null, null, null);
            results.ColumnMetadata = new List<ColumnMetadata>();
            results.Results = new EntityCollection(new List<Entity>());
            var subject = new CrmDbDataReader(results);
        }

        [Test]
        public void Should_Be_Able_To_Create_A_New_Data_Reader_With_Connection_And_Results()
        {
            // Arrange
            using (var sandbox = DataReaderTestsSandbox.Create())
            {
                // Act
                var subject = new CrmDbDataReader(sandbox.FakeResultSet, sandbox.FakeCrmDbConnection);
            }
        }

        [Test]
        public void Should_Close_Connection_When_Finished_Reading()
        {
            // Arrange
            using (var sandbox = DataReaderTestsSandbox.Create())
            {
                var subject = ResolveTestSubjectInstance();
                // Act
                foreach (var r in subject)
                {

                }
                // Assert
                sandbox.FakeCrmDbConnection.AssertWasCalled(o => o.Close(), options => options.Repeat.Once());
            }
        }

        [Test]
        public void Should_Be_Able_To_Get_Schema_Data_Table()
        {
            // Arrange
            using (var sandbox = DataReaderTestsSandbox.Create())
            {
                var subject = ResolveTestSubjectInstance();

                // Act
                var schema = subject.GetSchemaTable();

                // Assert
                Assert.That(schema, Is.Not.Null);

            }
        }
    }
}
