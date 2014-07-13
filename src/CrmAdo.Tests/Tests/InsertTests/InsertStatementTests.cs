using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;

namespace CrmAdo.Tests
{
    [Obsolete]
    [Category("Insert Statement")]
    [TestFixture()]
    public class InsertStatementTests : CrmQueryExpressionProviderTestsBase
    {

        [Test(Description = "Should support Insert of a single entity with named columns")]
        public void Should_Support_Insert_Statement_Of_Single_Entity_With_Named_Columns()
        {
            // Arrange
            var sql = "INSERT INTO contact (contactid, firstname, lastname) VALUES ('9bf20a16-6034-48e2-80b4-8349bb80c3e2','billy','bob')";
         
            // set up fake metadata provider.
            var fakeMetadataProvider = MockRepository.GenerateMock<ICrmMetaDataProvider>();
            var fakeMetadata = GetFakeContactMetadata();
            fakeMetadataProvider.Stub(a => a.GetEntityMetadata("contact")).Return(fakeMetadata);
            var fakeConn = MockRepository.GenerateMock<CrmDbConnection>();
            fakeConn.Stub(a => a.MetadataProvider).Return(fakeMetadataProvider);

            var cmd = new CrmDbCommand(fakeConn);
            cmd.CommandText = sql;

            // Act
            var createRequest = GetOrganizationRequest<CreateRequest>(cmd);
            
            // Assert
            Entity targetEntity = createRequest.Target;

            Assert.That(targetEntity.Attributes.ContainsKey("contactid"));
            Assert.That(targetEntity.Attributes.ContainsKey("firstname"));
            Assert.That(targetEntity.Attributes.ContainsKey("lastname"));

        }

        [ExpectedException(typeof(InvalidOperationException))]
        [Test(Description = "Does not support Insert of an entity with all default values")]
        public void Does_Not_Support_Insert_Of_An_Entity_With_All_Default_Values()
        {
            // Arrange
            var sql = "INSERT INTO contact DEFAULT VALUES";
            // Act
            var createRequest = GetOrganizationRequest<CreateRequest>(sql);

            Entity targetEntity = createRequest.Target;

            Assert.That(targetEntity.Attributes.ContainsKey("contactid"));
            Assert.That(targetEntity.Attributes.ContainsKey("firstname"));
            Assert.That(targetEntity.Attributes.ContainsKey("lastname"));

        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into integer field from numeric literal")]
        public void Should_Support_Insert_Of_Integer_From_Numeric_Literal()
        {
            // Arrange
            int val = 1012525;
            string fieldName = "address1_utcoffset";
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<int>(val, fieldName, val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into long field from numeric literal")]
        public void Should_Support_Insert_Of_Long_From_Numeric_Literal()
        {
            long val = 10225225;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<long>(val, "versionnumber", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into decimal field from numeric literal")]
        public void Should_Support_Insert_Of_decimal_From_Numeric_Literal()
        {
            decimal val = 1.2525m;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<decimal>(val, "exchangerate", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into double field from numeric literal")]
        public void Should_Support_Insert_Of_double_From_Numeric_Literal()
        {
            double val = 987654321098765d;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<double>(val, "address2_latitude", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into bool field from numeric literal")]
        public void Should_Support_Insert_Of_bool_From_Numeric_Literal()
        {
            bool val = true;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<bool>(val, "donotpostalmail", "1");
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into bool field from numeric literal")]
        public void Should_Support_Insert_Of_OptionSetValue_From_Numeric_Literal()
        {
            int optVal = 1000000;
            OptionSetValue desiredValue = new OptionSetValue(optVal);
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<OptionSetValue>(desiredValue, "address2_shippingmethodcode", optVal.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into Money field from numeric literal")]
        public void Should_Support_Insert_Of_Money_From_Numeric_Literal()
        {
            // Arrange
            decimal val = 1012525.95m;
            Money money = new Money(val);
            string fieldName = "aging90_base";
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<Money>(money, fieldName, val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("String Literal")]
        [Test(Description = "Should support Insert into datetime field from string literal")]
        public void Should_Support_Insert_Of_datetime_From_String_Literal()
        {
            DateTime dateValue = DateTime.UtcNow;
            string stringVal = dateValue.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK", CultureInfo.InvariantCulture);

            string sqlValue = string.Format("'{0}'", stringVal);
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<DateTime>(dateValue, "birthdate", sqlValue);
        }

        [Category("String Literal")]
        [Test(Description = "Should support Insert into string field from string literal")]
        public void Should_Support_Insert_Of_string_From_String_Literal()
        {
            string stringValue = "bob";
            string sqlValue = string.Format("'{0}'", stringValue);
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<string>(stringValue, "telephone2", sqlValue);
        }

        [Category("String Literal")]
        [Test(Description = "Should support Insert into guid field from string literal")]
        public void Should_Support_Insert_Of_guid_From_String_Literal()
        {
            Guid guidVal = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
            string sqlValue = string.Format("'{0}'", guidVal.ToString());
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<Guid>(guidVal, "processid", sqlValue);
        }

        [Category("String Literal")]
        [Test(Description = "Should support Insert into entityreference field from guid string literal guid")]
        public void Should_Support_Insert_Of_entityReference_From_String_Literal_Guid()
        {
            Guid guidVal = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
            EntityReference entRef = new EntityReference(String.Empty, guidVal);
            string sqlValue = string.Format("'{0}'", guidVal);
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<EntityReference>(entRef, "modifiedby", sqlValue);
        }

        [Category("String Literal")]
        [Test(Description = "Should support Insert with the entity id specified as string literal")]
        public void Should_Support_Insert_With_the_Entity_Id_Specified()
        {
            // Arrange
            Guid id = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
            var sql = string.Format("INSERT INTO contact (contactid) VALUES ('{0}')", id);

            // set up fake metadata provider.
            var fakeMetadataProvider = MockRepository.GenerateMock<ICrmMetaDataProvider>();
            var fakeMetadata = GetFakeContactMetadata();
            fakeMetadataProvider.Stub(a => a.GetEntityMetadata("contact")).Return(fakeMetadata);
            var fakeConn = MockRepository.GenerateMock<CrmDbConnection>();
            fakeConn.Stub(a => a.MetadataProvider).Return(fakeMetadataProvider);

            var cmd = new CrmDbCommand(fakeConn);
            cmd.CommandText = sql;

            // Act
            var createRequest = GetOrganizationRequest<CreateRequest>(cmd);

            Entity targetEntity = createRequest.Target;
            Assert.That(targetEntity.Id, Is.EqualTo(id));

            Assert.That(targetEntity.Attributes.ContainsKey("contactid"));
            Assert.That(targetEntity["contactid"], Is.EqualTo(id));
        }

        #region Helper Methods

        private void Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<T>(T assertValue, string fieldname, string sqlLiteralValue)
        {
            string sqlFormatString = "INSERT INTO contact (" + fieldname + ")";
            sqlFormatString = sqlFormatString + " VALUES ({0})";
            var sqlWithValue = string.Format(sqlFormatString, sqlLiteralValue);

            // set up fake metadata provider.
            var fakeMetadataProvider = MockRepository.GenerateMock<ICrmMetaDataProvider>();
            var fakeMetadata = GetFakeContactMetadata();
            fakeMetadataProvider.Stub(a => a.GetEntityMetadata("contact")).Return(fakeMetadata);
            var fakeConn = MockRepository.GenerateMock<CrmDbConnection>();
            fakeConn.Stub(a => a.MetadataProvider).Return(fakeMetadataProvider);

            var cmd = new CrmDbCommand(fakeConn);
            cmd.CommandText = sqlWithValue;
            var createRequest = GetOrganizationRequest<CreateRequest>(cmd);

            Entity targetEntity = createRequest.Target;
            AssertAttributeIsValue<T>(targetEntity, fieldname, assertValue);
        }

        private CrmEntityMetadata GetFakeContactMetadata()
        {
            var path = Environment.CurrentDirectory;
            var fileName = System.IO.Path.Combine(path, "MetadataFiles\\contactMetadata.xml");

            using (var reader = new XmlTextReader(fileName))
            {
                var deserialised = EntityMetadataUtils.DeserializeMetaData(reader);
                var crmMeta = new CrmEntityMetadata();
                var atts = new List<AttributeMetadata>();
                atts.AddRange(deserialised.Attributes);
                crmMeta.Attributes = atts;
                crmMeta.EntityName = "contact";
                return crmMeta;
            }
        }

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