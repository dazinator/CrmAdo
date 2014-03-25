using System;
using System.Globalization;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    [TestFixture()]
    public class InsertStatementTests : CrmQueryExpressionProviderTestsBase
    {

        [Category("Insert")]
        [Test(Description = "Should support Insert of a single entity with named columns")]
        public void Should_Support_Insert_Statement_Of_Single_Entity_With_Named_Columns()
        {
            // Arrange
            var sql = "INSERT INTO contact (contactid, firstname, lastname) VALUES ('9bf20a16-6034-48e2-80b4-8349bb80c3e2','billy','bob')";
            // Act
            var createRequest = GetOrganizationRequest<CreateRequest>(sql);

            Entity targetEntity = createRequest.Target;

            Assert.That(targetEntity.Attributes.ContainsKey("contactid"));
            Assert.That(targetEntity.Attributes.ContainsKey("firstname"));
            Assert.That(targetEntity.Attributes.ContainsKey("lastname"));

        }

        [Category("Insert")]
        [Category("Numeric Literal")]
        [TestCase("integerfield", 1, typeof(int), Description = "Should support Insert into integer field from numeric literal")]
        public void Should_Support_Insert_Of_Integer_From_Numeric_Literal()
        {
            // Arrange
            int val = 1012525;
            string fieldName = "integerfield";
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<int>(val, fieldName, val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Insert")]
        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into long field from numeric literal")]
        public void Should_Support_Insert_Of_Long_From_Numeric_Literal()
        {
            long val = 10225225;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<long>(val, "longfield", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Insert")]
        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into decimal field from numeric literal")]
        public void Should_Support_Insert_Of_decimal_From_Numeric_Literal()
        {
            decimal val = 1.2525m;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<decimal>(val, "decimalfield", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Insert")]
        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into double field from numeric literal")]
        public void Should_Support_Insert_Of_double_From_Numeric_Literal()
        {
            double val = 11.2525d;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<double>(val, "doublefield", val.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Insert")]
        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into bool field from numeric literal")]
        public void Should_Support_Insert_Of_bool_From_Numeric_Literal()
        {
            bool val = true;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<bool>(val, "doublefield", "1");
        }

        [Category("Insert")]
        [Category("Numeric Literal")]
        [Test(Description = "Should support Insert into bool field from numeric literal")]
        public void Should_Support_Insert_Of_OptionSetValue_From_Numeric_Literal()
        {
            int optVal = 1000000;
            OptionSetValue desiredValue = new OptionSetValue(optVal);
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<OptionSetValue>(desiredValue, "optionfield", optVal.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Insert")]
        [Category("String Literal")]
        [Test(Description = "Should support Insert into datetime field from string literal")]
        public void Should_Support_Insert_Of_datetime_From_String_Literal()
        {
            DateTime dateValue = DateTime.UtcNow;
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<DateTime>(dateValue, "datefield", dateValue.ToString(CultureInfo.InvariantCulture));
        }

        [Category("Insert")]
        [Category("String Literal")]
        [Test(Description = "Should support Insert into string field from string literal")]
        public void Should_Support_Insert_Of_string_From_String_Literal()
        {
            string stringValue = "bob";
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<string>(stringValue, "stringfield", stringValue);
        }


        [Category("Insert")]
        [Category("String Literal")]
        [Test(Description = "Should support Insert into guid field from string literal")]
        public void Should_Support_Insert_Of_guid_From_String_Literal()
        {
            Guid guidVal = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<Guid>(guidVal, "guidfield", guidVal.ToString());
        }

        [Category("Insert")]
        [Category("String Literal")]
        [Test(Description = "Should support Insert into guid field from string literal")]
        public void Should_Support_Insert_Of_entityReference_From_String_Literal()
        {
            Guid guidVal = Guid.Parse("9bf20a16-6034-48e2-80b4-8349bb80c3e2");
            EntityReference entRef = new EntityReference("testEntity", guidVal);
            Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<EntityReference>(entRef, "entityreffield", guidVal.ToString());
        }


        #region Helper Methods

        private void Test_That_Sql_Insert_Statement_With_A_Literal_Value_Has_The_Value_Translated_To<T>(T assertValue, string fieldname, string sqlLiteralValue)
        {
            string sqlFormatString = "INSERT INTO contact (" + fieldname + ")";
            sqlFormatString = sqlFormatString + " VALUES {0}";
            var sqlWithValue = string.Format(sqlFormatString, sqlLiteralValue);
            var createRequest = GetOrganizationRequest<CreateRequest>(sqlWithValue);
            Entity targetEntity = createRequest.Target;
            AssertAttributeIsValue<T>(targetEntity, fieldname, assertValue);
        }

        private void AssertAttributeIsValue<T>(Entity ent, string attributeName, T val)
        {
            Assert.That(ent.Attributes.ContainsKey(attributeName));
            Assert.That(ent[attributeName], Is.TypeOf(typeof(T)));
            Assert.That((T)ent[attributeName], Is.EqualTo(val));
        }

        #endregion
    }
}