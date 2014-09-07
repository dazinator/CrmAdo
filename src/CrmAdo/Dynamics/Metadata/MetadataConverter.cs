using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Dynamics.Metadata
{
    public class MetadataConverter
    {
        public List<AttributeInfo> ConvertAttributeInfoList(IEnumerable<AttributeMetadata> attributeMetadata)
        {
            var results = new List<AttributeInfo>();
            foreach (var item in attributeMetadata)
            {
                AttributeInfo attInfo = null;
                switch (item.AttributeType.GetValueOrDefault())
                {
                    case AttributeTypeCode.Decimal:
                        var decimalMetadata = (DecimalAttributeMetadata)item;
                        var dec = new DecimalAttributeInfo();
                        dec.MinValue = decimalMetadata.MinValue;
                        dec.MaxValue = decimalMetadata.MaxValue;
                        dec.Precision = decimalMetadata.Precision;
                        attInfo = dec;
                        break;
                    case AttributeTypeCode.Double:
                        var doubleMetadata = (DoubleAttributeMetadata)item;
                        var doubleAttInfo = new DoubleAttributeInfo();
                        doubleAttInfo.MinValue = doubleMetadata.MinValue;
                        doubleAttInfo.MaxValue = doubleMetadata.MaxValue;
                        doubleAttInfo.Precision = doubleMetadata.Precision;
                        attInfo = doubleAttInfo;
                        break;
                    case AttributeTypeCode.Money:
                        var moneyMetadata = (MoneyAttributeMetadata)item;
                        var moneyAttInfo = new MoneyAttributeInfo();
                        moneyAttInfo.MinValue = moneyMetadata.MinValue;
                        moneyAttInfo.MaxValue = moneyMetadata.MaxValue;
                        moneyAttInfo.Precision = moneyMetadata.Precision;
                        moneyAttInfo.PrecisionSource = moneyMetadata.PrecisionSource;
                        attInfo = moneyAttInfo;
                        break;
                    default:
                        attInfo = new AttributeInfo();
                        break;
                }

                attInfo.AttributeType = item.AttributeType;
                attInfo.AttributeTypeDisplayName = item.AttributeTypeName;
                attInfo.EntityLogicalName = item.EntityLogicalName;
                attInfo.IsPrimaryId = item.IsPrimaryId.GetValueOrDefault();
                attInfo.IsValidForCreate = item.IsValidForCreate;
                attInfo.IsValidForUpdate = item.IsValidForUpdate;
                attInfo.LogicalName = item.LogicalName;
                attInfo.MetadataId = item.MetadataId.GetValueOrDefault();
                results.Add(attInfo);
            }
            return results;
        }
    }
}
