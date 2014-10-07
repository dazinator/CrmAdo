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

            var dbConnection = new CrmDbConnection(mockServiceProvider);
            var selectCommand = new CrmDbCommand(dbConnection);
            selectCommand.CommandText = "SELECT * FROM contact";

            IList<Entity> fakeContactsData = GenerateFakeEntities("contact", 100);

            var response = new RetrieveMultipleResponse
            {
                Results = new ParameterCollection
 {
 { "EntityCollection", new EntityCollection(fakeContactsData){EntityName = "contact"} }
 }
            };



            fakeOrgService.Stub(f => f.Execute(Arg<OrganizationRequest>.Matches(new OrganizationRequestMessageConstraint<RetrieveMultipleRequest>())))
                .WhenCalled(x =>
                {
                    var request = ((RetrieveMultipleRequest)x.Arguments[0]);

                }).Return(response);



            //var conn = MockRepository.GenerateStub<CrmDbConnection>();
            //var fakeSelectCommand = MockRepository.GenerateStub<CrmDbCommand>();
            //fakeSelectCommand.Connection = conn;
            //conn.Stub(a => a.State).Return(ConnectionState.Open);

            // fakeSelectCommand.Stub(m => m.Connection).Return(conn);
            //  fakeSelectCommand.Connection = conn;
            // mockSelectCommand.CommandText = "SELECT * FROM CONTACT";
            var subject = CreateTestSubject();
            subject.SelectCommand = selectCommand;

            var ds = new DataSet();
            var result = subject.Fill(ds);

            Assert.NotNull(ds);
            Assert.NotNull(ds.Tables);
            Assert.That(ds.Tables.Count, NUnit.Framework.Is.EqualTo(1));

            var table = ds.Tables[0];



            //var results = new EntityResultSet(null, null, null);
            //results.ColumnMetadata = new List<ColumnMetadata>();

            //var firstName = MockRepository.GenerateMock<AttributeInfo>();
            //var lastname = MockRepository.GenerateMock<AttributeInfo>();
            //var firstNameC = new ColumnMetadata(firstName);
            //var lastnameC = new ColumnMetadata(lastname);



            //subject.
        }

        private IList<Entity> GenerateFakeEntities(string entityName, int howMany)
        {

            var metadataProvider = new FakeContactMetadataProvider();
            var metadata = metadataProvider.GetEntityMetadata(entityName);
            Random rand = new Random();

            List<Entity> results = new List<Entity>();

            // Used for generating random dates.
            DateTime minCrmDate = new DateTime(1900, 1, 1);
            int crmDayRange = (DateTime.Today - minCrmDate).Days;

            for (int i = 0; i < howMany; i++)
            {
                var ent = new Microsoft.Xrm.Sdk.Entity(entityName);
                ent.Id = Guid.NewGuid();

                foreach (var a in metadata.Attributes)
                {
                    switch (a.AttributeType.Value)
                    {
                        case AttributeTypeCode.BigInt:
                            var randomBigInt = (long)rand.NextLong(0, Int64.MaxValue);
                            ent[a.LogicalName] = randomBigInt;
                            break;
                        case AttributeTypeCode.Boolean:
                            int randomBoolInt = rand.Next(0, 1);
                            ent[a.LogicalName] = randomBoolInt == 1;
                            break;
                        case AttributeTypeCode.CalendarRules:
                            break;
                        case AttributeTypeCode.Customer:
                            int randomCustomerInt = rand.Next(0, 1);
                            string customerentity = "contact";
                            Guid customerId = Guid.NewGuid();
                            if (randomCustomerInt == 1)
                            {
                                customerentity = "account";
                            }
                            EntityReference customerRef = new EntityReference(customerentity, customerId);
                            ent[a.LogicalName] = customerRef;
                            break;
                        case AttributeTypeCode.DateTime:
                            DateTime randomDate = rand.NextCrmDate(minCrmDate, crmDayRange);
                            ent[a.LogicalName] = randomDate;
                            break;
                        case AttributeTypeCode.Decimal:
                            var decAtt = (DecimalAttributeInfo)a;
                            var scale = decAtt.GetNumericScale();
                            byte byteScale = (byte)scale;
                            var randomDecimal = rand.NextDecimal(byteScale);
                            ent[a.LogicalName] = randomDecimal;
                            break;
                        case AttributeTypeCode.Double:
                            var doubleAtt = (DoubleAttributeInfo)a;
                            var doubleScale = doubleAtt.GetNumericScale();
                            byte byteDoubleScale = (byte)doubleScale;
                            // todo apply precision / scale
                            var randomDouble = rand.NextDouble();
                            ent[a.LogicalName] = randomDouble;
                            break;
                        case AttributeTypeCode.EntityName:
                            break;
                        case AttributeTypeCode.Integer:
                            ent[a.LogicalName] = rand.Next();
                            break;
                        case AttributeTypeCode.Lookup:
                            break;
                        case AttributeTypeCode.ManagedProperty:
                            break;
                        case AttributeTypeCode.Memo:
                            var randomMemoString = string.Format("Test Memo String {0}", DateTime.UtcNow.Ticks.ToString());
                            ent[a.LogicalName] = randomMemoString;
                            break;
                        case AttributeTypeCode.Money:
                            var moneyAtt = (MoneyAttributeInfo)a;
                            var mscale = moneyAtt.GetNumericScale();
                            byte bytemScale = (byte)mscale;
                            var randomMoneyDecimal = rand.NextDecimal(bytemScale);
                            var randMoney = new Money(randomMoneyDecimal);
                            ent[a.LogicalName] = randMoney;
                            break;
                        case AttributeTypeCode.Owner:
                            EntityReference ownerRef = new EntityReference("systemuser", Guid.NewGuid());
                            ent[a.LogicalName] = ownerRef;
                            break;
                        case AttributeTypeCode.PartyList:
                            break;
                        case AttributeTypeCode.Picklist:
                            OptionSetValue optValue = new OptionSetValue(rand.Next());
                            ent[a.LogicalName] = optValue;
                            break;
                        case AttributeTypeCode.State:
                            // todo randomise active and inactive.
                            ent[a.LogicalName] = 0;
                            break;
                        case AttributeTypeCode.Status:
                            // todo randomise active and inactive.
                            ent[a.LogicalName] = 1;
                            break;
                        case AttributeTypeCode.String:
                            var randomString = string.Format("TestString{0}", DateTime.UtcNow.Ticks.ToString());
                            ent[a.LogicalName] = randomString;
                            break;
                        case AttributeTypeCode.Uniqueidentifier:
                            ent[a.LogicalName] = Guid.NewGuid();
                            break;
                        case AttributeTypeCode.Virtual:
                            break;
                        default:
                            break;
                    }

                }

                results.Add(ent);
            }

            return results;
        }

    }

    public static class RandomExtensions
    {
        public static long NextLong(this Random random, long min, long max)
        {
            byte[] buf = new byte[8];
            random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return (Math.Abs(longRand % (max - min)) + min);
        }

        public static DateTime NextCrmDate(this Random random, DateTime minDate, int dayRange)
        {
            return minDate.AddDays(random.Next(dayRange));
        }

        public static decimal NextDecimal(this Random r, byte scale)
        {
            var s = scale;
            var a = (int)(uint.MaxValue * r.NextDouble());
            var b = (int)(uint.MaxValue * r.NextDouble());
            var c = (int)(uint.MaxValue * r.NextDouble());
            var n = r.NextDouble() >= 0.5;
            return new Decimal(a, b, c, n, s);
        }
    }
}
