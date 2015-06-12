using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Metadata
{
    public class AttributeInfoFactory
    {

        protected virtual AttributeInfo CreateBigInt(AttributeMetadata attributeMetadata)
        {
            var attMeta = (BigIntAttributeMetadata)attributeMetadata;
            var attInfo = new BigIntAttributeInfo();
            attInfo.MinValue = attMeta.MinValue;
            attInfo.MaxValue = attMeta.MaxValue;
            return attInfo;
        }

        protected virtual AttributeInfo CreateBoolean(AttributeMetadata attributeMetadata)
        {
            var attMeta = (BooleanAttributeMetadata)attributeMetadata;
            var attInfo = new BooleanAttributeInfo();
            attInfo.DefaultValue = attMeta.DefaultValue;
            return attInfo;
        }

        protected virtual AttributeInfo CreateDateTime(AttributeMetadata attributeMetadata)
        {
            var attMeta = (DateTimeAttributeMetadata)attributeMetadata;
            var attInfo = new DateTimeAttributeInfo();
            return attInfo;
        }

        protected virtual AttributeInfo CreateDecimal(AttributeMetadata attributeMetadata)
        {
            var decimalMetadata = (DecimalAttributeMetadata)attributeMetadata;
            var dec = new DecimalAttributeInfo();
            dec.MinValue = decimalMetadata.MinValue;
            dec.MaxValue = decimalMetadata.MaxValue;
            dec.Precision = decimalMetadata.Precision;
            return dec;
        }

        protected virtual AttributeInfo CreateDouble(AttributeMetadata attributeMetadata)
        {
            var doubleMetadata = (DoubleAttributeMetadata)attributeMetadata;
            var doubleAttInfo = new DoubleAttributeInfo();
            doubleAttInfo.MinValue = doubleMetadata.MinValue;
            doubleAttInfo.MaxValue = doubleMetadata.MaxValue;
            doubleAttInfo.Precision = doubleMetadata.Precision;
            return doubleAttInfo;
        }

        protected virtual AttributeInfo CreateGuid(AttributeMetadata attributeMetadata)
        {
            // var attMeta = (GuidAttributeMetadata)attributeMetadata;
            var attInfo = new GuidAttributeInfo();
            return attInfo;
        }

        protected virtual AttributeInfo CreateInteger(AttributeMetadata attributeMetadata)
        {
            var attMeta = (IntegerAttributeMetadata)attributeMetadata;
            var attInfo = new IntegerAttributeInfo();
            attInfo.MinValue = attMeta.MinValue;
            attInfo.MaxValue = attMeta.MaxValue;
            return attInfo;
        }

        protected virtual AttributeInfo CreateMoney(AttributeMetadata attributeMetadata)
        {
            var moneyMetadata = (MoneyAttributeMetadata)attributeMetadata;
            var moneyAttInfo = new MoneyAttributeInfo();
            moneyAttInfo.MinValue = moneyMetadata.MinValue;
            moneyAttInfo.MaxValue = moneyMetadata.MaxValue;
            moneyAttInfo.Precision = moneyMetadata.Precision;
            moneyAttInfo.PrecisionSource = moneyMetadata.PrecisionSource;
            return moneyAttInfo;
        }

        protected virtual AttributeInfo CreateString(AttributeMetadata attributeMetadata)
        {
            var attMeta = (StringAttributeMetadata)attributeMetadata;
            var attInfo = new StringAttributeInfo();
            attInfo.Length = attMeta.MaxLength.GetValueOrDefault();
            return attInfo;
        }

        protected virtual AttributeInfo CreateMemo(AttributeMetadata attributeMetadata)
        {
            var attMeta = (MemoAttributeMetadata)attributeMetadata;
            var attInfo = new MemoAttributeInfo();
            attInfo.Length = attMeta.MaxLength.GetValueOrDefault();
            return attInfo;
        }

        protected virtual AttributeInfo CreateStatus(AttributeMetadata attributeMetadata)
        {
            var attMeta = (StatusAttributeMetadata)attributeMetadata;
            var attInfo = new StatusAttributeInfo(attMeta.OptionSet);
            attInfo.DefaultValue = attMeta.DefaultFormValue;
            return attInfo;
        }

        protected virtual AttributeInfo CreateState(AttributeMetadata attributeMetadata)
        {
            var attMeta = (StateAttributeMetadata)attributeMetadata;
            var attInfo = new StateAttributeInfo(attMeta.OptionSet);
            attInfo.DefaultValue = attMeta.DefaultFormValue;
            return attInfo;
        }

        protected virtual AttributeInfo CreatePicklist(AttributeMetadata attributeMetadata)
        {
            var attMeta = (PicklistAttributeMetadata)attributeMetadata;
            var attInfo = new PicklistAttributeInfo(attMeta.OptionSet);
            attInfo.DefaultValue = attMeta.DefaultFormValue;
            return attInfo;
        }

        protected virtual AttributeInfo CreateVirtual(AttributeMetadata attributeMetadata)
        {
            //var attMeta = (VirtualAttributeInfo)attributeMetadata;
            var attInfo = new VirtualAttributeInfo();
            return attInfo;
        }

        protected virtual AttributeInfo CreatePartyList(AttributeMetadata attributeMetadata)
        {
            var attInfo = new PartyListAttributeInfo();
            return attInfo;
        }

        protected virtual AttributeInfo CreateOwner(AttributeMetadata attributeMetadata)
        {
            var attInfo = new OwnerAttributeInfo();
            return attInfo;
        }

        protected virtual AttributeInfo CreateManagedProperty(AttributeMetadata attributeMetadata)
        {
            var attMeta = (ManagedPropertyAttributeMetadata)attributeMetadata;
            var attInfo = new ManagedPropertyAttributeInfo();
            return attInfo;
        }

        protected virtual AttributeInfo CreateLookup(AttributeMetadata attributeMetadata)
        {
            var attMeta = (LookupAttributeMetadata)attributeMetadata;           
            var attInfo = new LookupAttributeInfo();
            attInfo.Targets = attMeta.Targets;
            return attInfo;
        }

        protected virtual AttributeInfo CreateEntityName(AttributeMetadata attributeMetadata)
        {
            var attMeta = (EntityNameAttributeMetadata)attributeMetadata;
            var attInfo = new EntityNameAttributeInfo();
            attInfo.DefaultValue = attMeta.DefaultFormValue;
            return attInfo;
        }

        protected virtual AttributeInfo CreateCustomer(AttributeMetadata attributeMetadata)
        {
            // var attMeta = (CustomerAttributeInfo)attributeMetadata;
            var attInfo = new CustomerAttributeInfo();
            return attInfo;
        }

        protected virtual AttributeInfo CreateCalendarRules(AttributeMetadata attributeMetadata)
        {
            var attInfo = new CalendarRulesAttributeInfo();
            return attInfo;
        }

        public virtual AttributeInfo Create(AttributeMetadata attributeMetadata)
        {
            if (attributeMetadata == null)
            {
                return null;
            }

            AttributeInfo newAtt;

            if (attributeMetadata.AttributeType.HasValue)
            {
                switch (attributeMetadata.AttributeType.GetValueOrDefault())
                {
                    case AttributeTypeCode.Decimal:
                        newAtt = CreateDecimal(attributeMetadata);
                        break;
                    case AttributeTypeCode.Double:
                        newAtt = CreateDouble(attributeMetadata);
                        break;
                    case AttributeTypeCode.Money:
                        newAtt = CreateMoney(attributeMetadata);
                        break;
                    case AttributeTypeCode.BigInt:
                        newAtt = CreateBigInt(attributeMetadata);
                        break;
                    case AttributeTypeCode.Boolean:
                        newAtt = CreateBoolean(attributeMetadata);
                        break;
                    case AttributeTypeCode.CalendarRules:
                        newAtt = CreateCalendarRules(attributeMetadata);
                        break;
                    case AttributeTypeCode.Customer:
                        newAtt = CreateCustomer(attributeMetadata);
                        break;
                    case AttributeTypeCode.DateTime:
                        newAtt = CreateDateTime(attributeMetadata);
                        break;
                    case AttributeTypeCode.EntityName:
                        newAtt = CreateEntityName(attributeMetadata);
                        break;
                    case AttributeTypeCode.Integer:
                        newAtt = CreateInteger(attributeMetadata);
                        break;
                    case AttributeTypeCode.Lookup:
                        newAtt = CreateLookup(attributeMetadata);
                        break;
                    case AttributeTypeCode.ManagedProperty:
                        newAtt = CreateManagedProperty(attributeMetadata);
                        break;
                    case AttributeTypeCode.Memo:
                        newAtt = CreateMemo(attributeMetadata);
                        break;
                    case AttributeTypeCode.Owner:
                        newAtt = CreateOwner(attributeMetadata);
                        break;
                    case AttributeTypeCode.PartyList:
                        newAtt = CreatePartyList(attributeMetadata);
                        break;
                    case AttributeTypeCode.Picklist:
                        newAtt = CreatePicklist(attributeMetadata);
                        break;
                    case AttributeTypeCode.State:
                        newAtt = CreateState(attributeMetadata);
                        break;
                    case AttributeTypeCode.Status:
                        newAtt = CreateStatus(attributeMetadata);
                        break;
                    case AttributeTypeCode.String:
                        newAtt = CreateString(attributeMetadata);
                        break;
                    case AttributeTypeCode.Uniqueidentifier:
                        newAtt = CreateGuid(attributeMetadata);
                        break;
                    case AttributeTypeCode.Virtual:
                        newAtt = CreateVirtual(attributeMetadata);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                newAtt = CreateVirtual(attributeMetadata);
            }

            newAtt.AttributeOf = attributeMetadata.AttributeOf;
            newAtt.AttributeType = attributeMetadata.AttributeType;
            newAtt.AttributeTypeDisplayName = attributeMetadata.AttributeTypeName;
            newAtt.CanBeSecuredForCreate = attributeMetadata.CanBeSecuredForCreate;
            newAtt.CanBeSecuredForRead = attributeMetadata.CanBeSecuredForRead;
            newAtt.CanBeSecuredForUpdate = attributeMetadata.CanBeSecuredForUpdate;
            newAtt.CanModifyAdditionalSettings = GetBooleanManagedValue(attributeMetadata.CanModifyAdditionalSettings);
            newAtt.ColumnNumber = attributeMetadata.ColumnNumber;
            //newAtt.DataType = attributeMetadata.GetSqlDataType
            newAtt.DeprecatedVersion = attributeMetadata.DeprecatedVersion;
            newAtt.Description = GetLabelText(attributeMetadata.Description);
            newAtt.DisplayName = GetLabelText(attributeMetadata.DisplayName);

            newAtt.EntityLogicalName = attributeMetadata.EntityLogicalName;
            // newAtt.IdentityIncrement;
            // newAtt.IdentitySeed;
            newAtt.IntroducedVersion = attributeMetadata.IntroducedVersion;
            newAtt.IsAuditEnabled = GetBooleanManagedValue(attributeMetadata.IsAuditEnabled);
            newAtt.IsCustomAttribute = attributeMetadata.IsCustomAttribute;
            newAtt.IsCustomizable = GetBooleanManagedValue(attributeMetadata.IsCustomizable);
            // newAtt.IsIdentity = GetBooleanManagedValue(attributeMetadata.is);
            newAtt.IsManaged = attributeMetadata.IsManaged;
            newAtt.IsPrimaryId = attributeMetadata.IsPrimaryId.GetValueOrDefault();
            newAtt.IsPrimaryName = attributeMetadata.IsPrimaryName.GetValueOrDefault();
            newAtt.IsPseudo = false;
            newAtt.IsRenameable = GetBooleanManagedValue(attributeMetadata.IsRenameable);
            newAtt.IsSecured = attributeMetadata.IsSecured;
            newAtt.IsValidForAdvancedFind = GetBooleanManagedValue(attributeMetadata.IsValidForAdvancedFind);
            newAtt.IsValidForCreate = attributeMetadata.IsValidForCreate;
            newAtt.IsValidForRead = attributeMetadata.IsValidForRead;
            newAtt.IsValidForUpdate = attributeMetadata.IsValidForUpdate;
            //newAtt.Length
            newAtt.LinkedAttributeId = attributeMetadata.LinkedAttributeId;
            newAtt.LogicalName = attributeMetadata.LogicalName;
            newAtt.MetadataId = attributeMetadata.MetadataId.GetValueOrDefault();
            newAtt.RequiredLevel = attributeMetadata.RequiredLevel;
            newAtt.SchemaName = attributeMetadata.SchemaName;
            return newAtt;
        }

        public virtual AttributeInfo CreatePseudo(string entityName, string logicalName, AttributeTypeCode attTypeCode, AttributeTypeDisplayName attDisplayName = null)
        {
            var newAtt = new PseudoAttributeInfo();
            newAtt.AttributeType = attTypeCode;
            newAtt.AttributeOf = string.Empty;
            newAtt.AttributeType = attTypeCode;
            if (attDisplayName == null)
            {
                newAtt.AttributeTypeDisplayName = GetAttributeTypeDisplayName(attTypeCode);
            }
            else
            {
                newAtt.AttributeTypeDisplayName = attDisplayName;
            }

            newAtt.CanBeSecuredForCreate = false;
            newAtt.CanBeSecuredForRead = false;
            newAtt.CanBeSecuredForUpdate = false;
            newAtt.CanModifyAdditionalSettings = false;
            // newAtt.ColumnNumber = attributeMetadata.ColumnNumber;
            //newAtt.DataType = attributeMetadata.GetSqlDataType
            newAtt.DeprecatedVersion = string.Empty;
            newAtt.Description = string.Empty;
            newAtt.DisplayName = string.Empty;

            newAtt.EntityLogicalName = entityName;
            // newAtt.IdentityIncrement;
            // newAtt.IdentitySeed;
            newAtt.IntroducedVersion = string.Empty;
            newAtt.IsAuditEnabled = false;
            newAtt.IsCustomAttribute = true;
            newAtt.IsCustomizable = false;
            // newAtt.IsIdentity = GetBooleanManagedValue(attributeMetadata.is);
            newAtt.IsManaged = false;
            newAtt.IsPrimaryId = false;
            newAtt.IsPrimaryName = false;
            newAtt.IsPseudo = true;
            newAtt.IsRenameable = false;
            newAtt.IsSecured = false;
            newAtt.IsValidForAdvancedFind = false;
            newAtt.IsValidForCreate = false;
            newAtt.IsValidForRead = false;
            newAtt.IsValidForUpdate = false;
            //newAtt.Length
            newAtt.LinkedAttributeId = null;
            newAtt.LogicalName = logicalName;
            newAtt.MetadataId = Guid.Empty;
            newAtt.RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None);
            newAtt.SchemaName = logicalName;
            return newAtt;

        }

        private bool? GetBooleanManagedValue(Microsoft.Xrm.Sdk.BooleanManagedProperty booleanManagedProperty)
        {
            if (booleanManagedProperty != null)
            {
                return booleanManagedProperty.Value;
            }
            return null;
        }

        private string GetLabelText(Microsoft.Xrm.Sdk.Label label)
        {
            if (label != null && label.UserLocalizedLabel != null)
            {
                return label.UserLocalizedLabel.Label;
            }
            return string.Empty;

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
