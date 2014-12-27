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
    [Category("Update Statement")]
    [TestFixture()]
    public class UpdateStatementVisitorTests : BaseOrganisationRequestBuilderVisitorTest
    {

        [Test(Description = "Should support Update of a single entity with named columns")]
        public void Should_Support_Update_Statement_Of_Single_Entity_With_Named_Columns()
        {
            // Arrange
            var sql = "UPDATE contact SET firstname = 'john', lastname = 'doe' WHERE contactid = '9bf20a16-6034-48e2-80b4-8349bb80c3e2'";
            using (var sandbox = RequestProviderTestsSandbox.Create())
            {
                var cmd = new CrmDbCommand(sandbox.FakeCrmDbConnection);
                cmd.CommandText = sql;

                // Act
                var request = GetOrganizationRequest<UpdateRequest>(cmd);

                // Assert
                Entity targetEntity = request.Target;
                Assert.That(targetEntity.Attributes.ContainsKey("firstname"));
                Assert.That((string)targetEntity.Attributes["firstname"], Is.EqualTo("john"));
                Assert.That(targetEntity.Attributes.ContainsKey("lastname"));
                Assert.That((string)targetEntity.Attributes["lastname"], Is.EqualTo("doe"));
            }

        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Update into integer field from numeric literal")]
        public void Should_Support_Update_Of_Integer_From_Numeric_Literal()
        {
            // Arrange
            int val = 1012525;
            string fieldName = "address1_utcoffset";
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<int>(val, fieldName, val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Update into long field from numeric literal")]
        public void Should_Support_Update_Of_Long_From_Numeric_Literal()
        {
            long val = 10225225;
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<long>(val, "versionnumber", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Update into decimal field from numeric literal")]
        public void Should_Support_Update_Of_decimal_From_Numeric_Literal()
        {
            decimal val = 1.2525m;
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<decimal>(val, "exchangerate", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Update into double field from numeric literal")]
        public void Should_Support_Update_Of_double_From_Numeric_Literal()
        {
            double val = 987654321098765d;
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<double>(val, "address2_latitude", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Update into bool field from numeric literal")]
        public void Should_Support_Update_Of_bool_From_Numeric_Literal()
        {
            bool val = true;
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<bool>(val, "donotpostalmail", "1");
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Update into bool field from numeric literal")]
        public void Should_Support_Update_Of_OptionSetValue_From_Numeric_Literal()
        {
            int optVal = 1000000;
            OptionSetValue desiredValue = new OptionSetValue(optVal);
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<OptionSetValue>(desiredValue, "address2_shippingmethodcode", optVal.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Update into Money field from numeric literal")]
        public void Should_Support_Update_Of_Money_From_Numeric_Literal()
        {
            // Arrange
            decimal val = 1012525.95m;
            Money money = new Money(val);
            string fieldName = "aging90_base";
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<Money>(money, fieldName, val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("String Literal")]
        [Test(Description = "Should support Update into datetime field from string literal")]
        public void Should_Support_Update_Of_datetime_From_String_Literal()
        {
            DateTime dateValue = DateTime.UtcNow;
            string stringVal = dateValue.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK", CultureInfo.InvariantCulture);

            string sqlValue = string.Format("'{0}'", stringVal);
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<DateTime>(dateValue, "birthdate", sqlValue);
        }

        [Category("String Literal")]
        [Test(Description = "Should support Update into string field from string literal")]
        public void Should_Support_Update_Of_string_From_String_Literal()
        {
            string stringValue = "bob";
            string sqlValue = string.Format("'{0}'", stringValue);
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<string>(stringValue, "telephone2", sqlValue);
        }

        [Category("String Literal")]
        [Test(Description = "Should support Update into guid field from string literal")]
        public void Should_Support_Update_Of_guid_From_String_Literal()
        {
            Guid guidVal = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
            string sqlValue = string.Format("'{0}'", guidVal.ToString());
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<Guid>(guidVal, "processid", sqlValue);
        }

        [Category("String Literal")]
        [Test(Description = "Should support Update into entityreference field from guid string literal guid")]
        public void Should_Support_Update_Of_entityReference_From_String_Literal_Guid()
        {
            Guid guidVal = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
            EntityReference entRef = new EntityReference(String.Empty, guidVal);
            string sqlValue = string.Format("'{0}'", guidVal);
            Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<EntityReference>(entRef, "modifiedby", sqlValue);
        }

        [Test(Description = "Should support Update of a single entity with output columns")]
        public void Should_Support_Update_Statement_Of_Single_Entity_With_Output_Columns()
        {
            // Arrange
            var sql = "UPDATE contact SET firstname = 'john', lastname = 'doe' OUTPUT INSERTED.modifiedon WHERE contactid = '9bf20a16-6034-48e2-80b4-8349bb80c3e2'";
            
            using (var sandbox = RequestProviderTestsSandbox.Create())
            {

                var cmd = new CrmDbCommand(sandbox.FakeCrmDbConnection);
                cmd.CommandText = sql;

                // Act
                var orgCommand = GetOrgCommand(cmd, System.Data.CommandBehavior.Default);
                var req = orgCommand.Request as ExecuteMultipleRequest;

                // Assert
                Assert.That(req, Is.Not.Null);
                Assert.That(req.Requests.Count, Is.EqualTo(2));

                var createRequest = (UpdateRequest)req.Requests[0];
                var retrieveRequest = (RetrieveRequest)req.Requests[1];

                Entity targetEntity = createRequest.Target;

                Assert.That(targetEntity.Attributes.ContainsKey("firstname"));
                Assert.That(targetEntity.Attributes.ContainsKey("lastname"));

                var targetRetrieve = retrieveRequest.Target;
                Assert.That(targetRetrieve.LogicalName, Is.EqualTo("contact"));
                var idGuid = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
                Assert.That(targetRetrieve.Id, Is.EqualTo(idGuid));
                Assert.That(retrieveRequest.ColumnSet.Columns.Contains("modifiedon"));

                Assert.That(orgCommand.Columns, Is.Not.Null);
                Assert.That(orgCommand.Columns.Count, Is.GreaterThan(0));

                var outputColumn = orgCommand.Columns[0];
                Assert.That(outputColumn.ColumnName, Is.EqualTo("modifiedon"));
                Assert.That(outputColumn.AttributeMetadata, Is.Not.Null);
                var attMetadata = outputColumn.AttributeMetadata;
                Assert.That(attMetadata.AttributeType, Is.EqualTo(AttributeTypeCode.DateTime));

                Assert.That(orgCommand.OperationType, Is.EqualTo(Enums.CrmOperation.UpdateWithRetrieve));

            }

        }
     
        [Test(Description = "Should support Update of a single entity with output ALL columns")]
        public void Should_Support_Update_Statement_Of_Single_Entity_With_Output_All_Columns()
        {
            // Arrange
            var sql = "UPDATE contact SET firstname = 'john', lastname = 'doe' OUTPUT INSERTED.* WHERE contactid = '9bf20a16-6034-48e2-80b4-8349bb80c3e2'";

            // set up fake metadata provider.
            // var fakeMetadataProvider = MockRepository.GenerateMock<ICrmMetaDataProvider>();
            // var fakeMetadata = GetFakeContactMetadata();
            //  fakeMetadataProvider.Stub(a => a.GetEntityMetadata("contact")).Return(fakeMetadata);

            using (var sandbox = RequestProviderTestsSandbox.Create())
            {

                var cmd = new CrmDbCommand(sandbox.FakeCrmDbConnection);
                cmd.CommandText = sql;

                // Act
                var orgCommand = GetOrgCommand(cmd, System.Data.CommandBehavior.Default);
                var req = orgCommand.Request as ExecuteMultipleRequest;

                // var req = GetOrganizationRequest<ExecuteMultipleRequest>(cmd);

                // Assert
                Assert.That(req, Is.Not.Null);
                Assert.That(req.Requests.Count, Is.EqualTo(2));
            
             
                var createRequest = (UpdateRequest)req.Requests[0];
                var retrieveRequest = (RetrieveRequest)req.Requests[1];

                Entity targetEntity = createRequest.Target;

               // Assert.That(targetEntity.Attributes.ContainsKey("contactid"));
                Assert.That(targetEntity.Attributes.ContainsKey("firstname"));
                Assert.That(targetEntity.Attributes.ContainsKey("lastname"));

                var targetRetrieve = retrieveRequest.Target;
                Assert.That(targetRetrieve.LogicalName, Is.EqualTo("contact"));
                var idGuid = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
                Assert.That(targetRetrieve.Id, Is.EqualTo(idGuid));
                Assert.That(retrieveRequest.ColumnSet.AllColumns);

                Assert.That(orgCommand.Columns, Is.Not.Null);
                Assert.That(orgCommand.Columns.Count, Is.GreaterThan(0));

                foreach (var outputColumn in orgCommand.Columns)
                {
                    Assert.That(outputColumn.ColumnName, Is.Not.Null);
                    Assert.That(outputColumn.ColumnName, Is.Not.EqualTo(""));
                    Assert.That(outputColumn.AttributeMetadata, Is.Not.Null);
                    var attMetadata = outputColumn.AttributeMetadata;
                    Assert.That(attMetadata.AttributeType, Is.Not.Null);
                }

                Assert.That(orgCommand.OperationType, Is.EqualTo(Enums.CrmOperation.UpdateWithRetrieve));


            }




        }


        #region Helper Methods

        private void Test_That_Sql_Update_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<T>(T assertValue, string fieldname, string sqlLiteralValue)
        {
            var existingContactGuid = Guid.NewGuid();
            string sqlFormatString = "UPDATE contact SET " + fieldname + " = {0} WHERE contactid = '{1}'";
            var sqlWithValue = string.Format(sqlFormatString, sqlLiteralValue, existingContactGuid);

            using (var sandbox = RequestProviderTestsSandbox.Create())
            {

                var cmd = new CrmDbCommand(sandbox.FakeCrmDbConnection);
                cmd.CommandText = sqlWithValue;
                var request = GetOrganizationRequest<UpdateRequest>(cmd);

                Entity targetEntity = request.Target;
                AssertAttributeIsValue<T>(targetEntity, fieldname, assertValue);
            }
        }

        //private CrmEntityMetadata GetFakeContactMetadata()
        //{
        //    var path = Environment.CurrentDirectory;
        //    var fileName = System.IO.Path.Combine(path, "MetadataFiles\\contactMetadata.xml");

        //    using (var reader = new XmlTextReader(fileName))
        //    {
        //        var deserialised = EntityMetadataUtils.DeserializeMetaData(reader);
        //        var crmMeta = new CrmEntityMetadata();
        //        var atts = new List<AttributeMetadata>();
        //        atts.AddRange(deserialised.Attributes);
        //        crmMeta.Attributes = atts;
        //        crmMeta.EntityName = "contact";
        //        return crmMeta;
        //    }
        //}

        private void AssertAttributeIsValue<T>(Entity ent, string attributeName, T val)
        {
            Assert.That(ent.Attributes.ContainsKey(attributeName));
            Assert.That(ent[attributeName], Is.TypeOf(typeof(T)));

            var att = (T)ent[attributeName];
            var reference = att as EntityReference;
            if (reference != null)
            {
                Assert.That(reference.Id == (val as EntityReference).Id);
                Assert.That(reference.LogicalName, Is.EqualTo((val as EntityReference).LogicalName));
                Assert.That(reference.Name == (val as EntityReference).Name);
            }
            else
            {
                Assert.That(att, Is.EqualTo(val));
            }

        }

        #endregion
    }
}