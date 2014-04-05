using System;
using System.Globalization;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using NUnit.Framework;

namespace CrmAdo.Tests
{
    [Category("Type Conversion")]
    [TestFixture()]
    public class SdkAttributeTypeConverterTests : BaseTest<DynamicsAttributeTypeProvider>
    {

        [Test(Description = "Should get BigInt from long")]
        public void Should_Get_BigInt_From_Long()
        {
            long x = 8467L;
            var sut = CreateTestSubject();
            var result = sut.GetBigInt((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get BigInt from int")]
        public void Should_Get_BigInt_From_int()
        {
            int x = 8467;
            long y = Convert.ToInt64(x);
            var sut = CreateTestSubject();
            var result = sut.GetBigInt((object)x);
            Assert.That(result, Is.EqualTo(y));
        }

        [Test(Description = "Should get BigInt from string")]
        public void Should_Get_BigInt_From_String()
        {
            long x = 8467L;
            var stringLong = x.ToString();
            var sut = CreateTestSubject();
            var result = sut.GetBigInt((object)stringLong);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get bool from bool")]
        public void Should_Get_Bool_From_Bool()
        {
            bool x = true;
            var sut = CreateTestSubject();
            var result = sut.GetBoolean((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get bool from int 1")]
        public void Should_Get_Bool_From_Int_1()
        {
            int x = 1;
            var sut = CreateTestSubject();
            var result = sut.GetBoolean((object)x);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test(Description = "Should get bool from int 0")]
        public void Should_Get_Bool_From_Int_0()
        {
            int x = 0;
            var sut = CreateTestSubject();
            var result = sut.GetBoolean((object)x);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test(Description = "Should get bool from string")]
        public void Should_Get_Bool_From_String()
        {
            bool x = true;
            var stringVal = x.ToString();
            var sut = CreateTestSubject();
            var result = sut.GetBoolean((object)stringVal);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get datetime from datetime")]
        public void Should_Get_DateTime_From_DateTime()
        {
            DateTime x = DateTime.UtcNow;
            var sut = CreateTestSubject();
            var result = sut.GetDateTime(x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get datetime from string")]
        public void Should_Get_DateTime_From_string()
        {
            DateTime x = DateTime.UtcNow;
            string stringVal = x.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK", CultureInfo.InvariantCulture);
            var sut = CreateTestSubject();
            var result = sut.GetDateTime((object)stringVal);
            Assert.That(result.Kind, Is.EqualTo(x.Kind));
            Assert.That(result, Is.EqualTo(x));

        }

        [Test(Description = "Should get datetime from string with less precision")]
        public void Should_Get_DateTime_From_string_with_less_precision()
        {
            DateTime x = DateTime.UtcNow;
            string stringVal = x.ToString("yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture);
            var sut = CreateTestSubject();
            var result = sut.GetDateTime((object)stringVal);
            Assert.That(result.Kind, Is.EqualTo(x.Kind));
            Assert.That(result.Date, Is.EqualTo(x.Date));
            Assert.That(result.Hour, Is.EqualTo(x.Hour));
            Assert.That(result.Minute, Is.EqualTo(x.Minute));
            Assert.That(result.Second, Is.EqualTo(x.Second));
            Assert.That(result.Millisecond, Is.EqualTo(x.Millisecond));
        }

        [Test(Description = "Should get Decimal from Decimal")]
        public void Should_Get_Decimal_From_Decimal()
        {
            decimal x = 8467m;
            var sut = CreateTestSubject();
            var result = sut.GetDecimal((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get Decimal from string")]
        public void Should_Get_Decimal_From_string()
        {
            decimal x = 8467m;
            var stringVal = x.ToString(CultureInfo.InvariantCulture);
            var sut = CreateTestSubject();
            var result = sut.GetDecimal((object)stringVal);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get Double from Double")]
        public void Should_Get_Double_From_Double()
        {
            double x = 8467D;
            var sut = CreateTestSubject();
            var result = sut.GetDouble((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get Double from string")]
        public void Should_Get_Double_From_string()
        {
            double x = 8467D;
            var stringVal = x.ToString(CultureInfo.InvariantCulture);
            var sut = CreateTestSubject();
            var result = sut.GetDouble((object)stringVal);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get EntityName from string")]
        public void Should_Get_EntityName_From_String()
        {
            string x = "name";
            var sut = CreateTestSubject();
            var result = sut.GetEntityName((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get Integer from Integer")]
        public void Should_Get_Integer_From_Integer()
        {
            int x = 8467;
            var sut = CreateTestSubject();
            var result = sut.GetInteger((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get Integer from string")]
        public void Should_Get_Integer_From_string()
        {
            int x = 8467;
            var stringValue = x.ToString(CultureInfo.InvariantCulture);
            var sut = CreateTestSubject();
            var result = sut.GetInteger((object)stringValue);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get Memo from String")]
        public void Should_Get_Memo_From_String()
        {
            string x = "memo text &*(%$/#~!";
            var sut = CreateTestSubject();
            var result = sut.GetMemo((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get Money from Decimal")]
        public void Should_Get_Money_From_Decimal()
        {
            decimal x = 8467.99m;
            var sut = CreateTestSubject();
            var result = sut.GetMoney((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(x));
        }

        [Test(Description = "Should get Picklist from int")]
        public void Should_Get_Picklist_From_int()
        {
            int x = 100000000;
            var sut = CreateTestSubject();
            var result = sut.GetPicklist((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(x));
        }

        [Test(Description = "Should get State from int")]
        public void Should_Get_State_From_int()
        {
            int x = 0;
            var sut = CreateTestSubject();
            var result = sut.GetState((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(x));
        }

        [Test(Description = "Should get Status from int")]
        public void Should_Get_Status_From_int()
        {
            int x = 1;
            var sut = CreateTestSubject();
            var result = sut.GetStatus((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(x));
        }

        [Test(Description = "Should get string from string")]
        public void Should_Get_String_From_String()
        {
            string x = "some val*&^%$£!";
            var sut = CreateTestSubject();
            var result = sut.GetString((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get UniqueIdentitifer from Guid String")]
        public void Should_Get_UniqueIdentitifer_From_Guid_String()
        {
            Guid x = Guid.NewGuid();
            var stringVal = x.ToString();
            var sut = CreateTestSubject();
            var result = sut.GetUniqueIdentifier((object)stringVal);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get UniqueIdentitifer from Guid")]
        public void Should_Get_UniqueIdentitifer_From_Guid()
        {
            Guid x = Guid.NewGuid();
            var sut = CreateTestSubject();
            var result = sut.GetUniqueIdentifier((object)x);
            Assert.That(result, Is.EqualTo(x));
        }

        [Test(Description = "Should get Customer from Guid")]
        public void Should_Get_Customer_From_Guid()
        {
            Guid x = Guid.NewGuid();
            var sut = CreateTestSubject();
            var result = sut.GetCustomer((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(x));
        }

        [Test(Description = "Should get Customer from Guid string")]
        public void Should_Get_Customer_From_Guid_String()
        {
            Guid x = Guid.NewGuid();
            var stringVal = x.ToString();
            var sut = CreateTestSubject();
            var result = sut.GetCustomer((object)stringVal);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(x));
        }

        [Test(Description = "Should get Customer from 'contact' string")]
        public void Should_Get_Customer_From_Contact_String()
        {
            string x = "contact";
            var sut = CreateTestSubject();
            var result = sut.GetCustomer((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.LogicalName, Is.EqualTo(x));
        }

        [Test(Description = "Should get Customer from 'account' string")]
        public void Should_Get_Customer_From_Account_String()
        {
            string x = "account";
            var sut = CreateTestSubject();
            var result = sut.GetCustomer((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.LogicalName, Is.EqualTo(x));
        }

        [Test(Description = "Should get Lookup from Guid string")]
        public void Should_Get_Lookup_From_Guid_String()
        {
            Guid x = Guid.NewGuid();
            var stringVal = x.ToString();
            var sut = CreateTestSubject();
            var result = sut.GetLookup((object)stringVal);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(x));
        }

        [Test(Description = "Should get Lookup from Guid")]
        public void Should_Get_Lookup_From_Guid()
        {
            Guid x = Guid.NewGuid();
            var sut = CreateTestSubject();
            var result = sut.GetLookup((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(x));
        }

        [Test(Description = "Should get Owner from Guid string")]
        public void Should_Get_Owner_From_Guid_String()
        {
            Guid x = Guid.NewGuid();
            var stringVal = x.ToString();
            var sut = CreateTestSubject();
            var result = sut.GetOwner((object)stringVal);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(x));
        }

        [Test(Description = "Should get Owner from Guid")]
        public void Should_Get_Owner_From_Guid()
        {
            Guid x = Guid.NewGuid();
            var sut = CreateTestSubject();
            var result = sut.GetOwner((object)x);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(x));
        }



    }
}