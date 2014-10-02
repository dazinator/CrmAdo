using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmAdo.Dynamics.Metadata
{
    public class CrmEntityMetadata
    {

        public CrmEntityMetadata(string entityName)
            : this(entityName, new List<AttributeInfo>())
        {
        }

        public CrmEntityMetadata(string entityName, List<AttributeInfo> attributes)
        {
            Attributes = attributes;
            EntityName = entityName;
        }

        public string Timestamp { get; set; }
        public string EntityName { get; set; }

        public List<AttributeInfo> Attributes { get; set; }

        /// <summary>
        /// This lock is taken when the metadata "Refresh" method is run, as during that time the object can be modified with the latest updates.
        /// </summary>
        private static object _Lock = new object();

        public void Refresh(List<AttributeInfo> modifiedFields, List<Guid> deletedFields)
        {
            lock (_Lock)
            {
                foreach (var deletedField in deletedFields)
                {
                    var existingAtt = Attributes.FirstOrDefault(a => a.MetadataId == deletedField);
                    if (existingAtt != null)
                    {
                        Attributes.Remove(existingAtt);
                    }
                }

                foreach (var modifiedField in modifiedFields)
                {
                    var existingAtt = Attributes.FirstOrDefault(a => a.MetadataId == modifiedField.MetadataId);
                    if (existingAtt != null)
                    {
                        Attributes.Remove(existingAtt);
                        Attributes.Add(modifiedField);
                    }
                }
            }

        }

        public bool IsPseudo { get; set; }

        public AttributeInfo AddPseudoAttribute(string name, AttributeTypeCode attTypeCode, AttributeTypeDisplayName attDisplayName = null)
        {
            var attInfo = CreateAttributeInfo(this.EntityName, name, attTypeCode, attDisplayName);
            this.Attributes.Add(attInfo);
            return attInfo;
        }

        private AttributeInfo CreateAttributeInfo(string entityName, string name, AttributeTypeCode attTypeCode, AttributeTypeDisplayName attDisplayName = null)
        {
            var attInfo = new Dynamics.Metadata.AttributeInfo();
            attInfo.AttributeType = attTypeCode;
            if (attDisplayName == null)
            {
                attInfo.AttributeTypeDisplayName = GetAttributeTypeDisplayName(attTypeCode);
            }
            else
            {
                attInfo.AttributeTypeDisplayName = attDisplayName;
            }
            attInfo.EntityLogicalName = entityName;
            attInfo.LogicalName = name;
            attInfo.MetadataId = Guid.Empty;
            return attInfo;
        }

        private AttributeTypeDisplayName GetAttributeTypeDisplayName(AttributeTypeCode attTypeCode)
        {
            AttributeTypeDisplayName displayType = null;
            switch (attTypeCode)
            {
                case AttributeTypeCode.BigInt:
                    displayType = AttributeTypeDisplayName.BigIntType;
                    break;

                case AttributeTypeCode.Boolean:
                    displayType = AttributeTypeDisplayName.BooleanType;
                    break;

                case AttributeTypeCode.CalendarRules:
                    displayType = AttributeTypeDisplayName.CalendarRulesType;
                    break;

                case AttributeTypeCode.Customer:
                    displayType = AttributeTypeDisplayName.CustomerType;
                    break;

                case AttributeTypeCode.DateTime:
                    displayType = AttributeTypeDisplayName.DateTimeType;
                    break;

                case AttributeTypeCode.Decimal:
                    displayType = AttributeTypeDisplayName.DecimalType;
                    break;

                case AttributeTypeCode.Double:
                    displayType = AttributeTypeDisplayName.DoubleType;
                    break;

                case AttributeTypeCode.EntityName:
                    displayType = AttributeTypeDisplayName.EntityNameType;
                    break;

                case AttributeTypeCode.Integer:
                    displayType = AttributeTypeDisplayName.IntegerType;
                    break;

                case AttributeTypeCode.Lookup:
                    displayType = AttributeTypeDisplayName.LookupType;
                    break;

                case AttributeTypeCode.ManagedProperty:
                    displayType = AttributeTypeDisplayName.ManagedPropertyType;
                    break;

                case AttributeTypeCode.Memo:
                    displayType = AttributeTypeDisplayName.MemoType;
                    break;

                case AttributeTypeCode.Money:
                    displayType = AttributeTypeDisplayName.MoneyType;
                    break;

                case AttributeTypeCode.Owner:
                    displayType = AttributeTypeDisplayName.OwnerType;
                    break;

                case AttributeTypeCode.PartyList:
                    displayType = AttributeTypeDisplayName.PartyListType;
                    break;

                case AttributeTypeCode.Picklist:
                    displayType = AttributeTypeDisplayName.PicklistType;
                    break;

                case AttributeTypeCode.State:
                    displayType = AttributeTypeDisplayName.StateType;
                    break;

                case AttributeTypeCode.Status:
                    displayType = AttributeTypeDisplayName.StatusType;
                    break;

                case AttributeTypeCode.String:
                    displayType = AttributeTypeDisplayName.StringType;
                    break;

                case AttributeTypeCode.Uniqueidentifier:
                    displayType = AttributeTypeDisplayName.UniqueidentifierType;
                    break;

                case AttributeTypeCode.Virtual:
                    displayType = AttributeTypeDisplayName.VirtualType;
                    break;
            }

            return displayType;

        }

    }
}