using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests.Support
{
    public class EntityDataGenerator
    {
        private ICrmMetaDataProvider _MetadataProvider;

        public EntityDataGenerator()
            : this(new FakeContactMetadataProvider())
        {

        }

        public EntityDataGenerator(ICrmMetaDataProvider metadataProvider)
        {
            _MetadataProvider = metadataProvider;
        }

        public IList<Entity> GenerateFakeEntities(string entityName, int howMany)
        {

            var metadataProvider = _MetadataProvider;
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

                int stateCode = rand.Next(0, 1);
                int statusCode = stateCode + 1;

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
                            var stateCodeOpt = new OptionSetValue(stateCode);
                            ent[a.LogicalName] = stateCodeOpt;
                            break;
                        case AttributeTypeCode.Status:
                            var statusCodeOpt = new OptionSetValue(statusCode);
                            // todo randomise active and inactive.
                            ent[a.LogicalName] = statusCodeOpt;
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
}
