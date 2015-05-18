using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System.Text;

namespace CrmAdo.Util
{
    public static class EntityMetadataUtils
    {

        /// <summary>Serialize metadata</summary>
        /// <param name="metaData">Metadata to serialize</param>
        /// <param name="formatting">Formatting, determines if indentation and line feeds are used in the file</param>
        public static string SerializeMetaData(this EntityMetadata metaData, Formatting formatting)
        {

            using (var stringWriter = new StringWriter())
            {

                var serializer = new DataContractSerializer(typeof(EntityMetadata), null, int.MaxValue, false, false, null, new KnownTypesResolver());
                var writer = new XmlTextWriter(stringWriter)
                    {
                        Formatting = formatting
                    };
                serializer.WriteObject(writer, metaData);

                writer.Close();

                return stringWriter.ToString();
            }

        }

        /// <summary>
        /// Deserialises the xml into the EntityMetadata object.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static EntityMetadata DeserializeMetaData(XmlReader reader)
        {
            var serializer = new DataContractSerializer(typeof(EntityMetadata), null, int.MaxValue, false, false, null, new KnownTypesResolver());
            var entity = (EntityMetadata)serializer.ReadObject(reader);
            return entity;
        }

        /// <summary>
        /// Get's the non-sdk type, in other words, get's the non dyanmics sdk specific type used to represent this attribute type
        /// in a crm agnostic way.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static Type GetCrmAgnosticType(this AttributeTypeCode attType)
        {
            switch (attType)
            {
                case AttributeTypeCode.BigInt:
                    return typeof(long);
                case AttributeTypeCode.Boolean:
                    return typeof(bool);
                case AttributeTypeCode.CalendarRules:
                    return typeof(string);
                case AttributeTypeCode.Customer:
                    return typeof(Guid);
                case AttributeTypeCode.DateTime:
                    return typeof(DateTime);
                case AttributeTypeCode.Decimal:
                    return typeof(decimal);
                case AttributeTypeCode.Double:
                    return typeof(double);
                case AttributeTypeCode.EntityName:
                    return typeof(string);
                case AttributeTypeCode.Integer:
                    return typeof(int);
                case AttributeTypeCode.Lookup:
                    return typeof(Guid);
                case AttributeTypeCode.ManagedProperty:
                    return typeof(bool);
                case AttributeTypeCode.Memo:
                    return typeof(string);
                case AttributeTypeCode.Money:
                    return typeof(decimal);
                case AttributeTypeCode.Owner:
                    return typeof(Guid);
                case AttributeTypeCode.PartyList:
                    return typeof(string);
                case AttributeTypeCode.Picklist:
                    return typeof(int);
                case AttributeTypeCode.State:
                    return typeof(int);
                case AttributeTypeCode.Status:
                    return typeof(int);
                case AttributeTypeCode.String:
                    return typeof(string);
                case AttributeTypeCode.Uniqueidentifier:
                    return typeof(Guid);
                case AttributeTypeCode.Virtual:
                    return typeof(string);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the sql datatype name for the attribute type. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static string GetSqlDataTypeName(this AttributeTypeCode attType, AttributeTypeDisplayName attTypeDisplayName)
        {
            switch (attType)
            {
                case AttributeTypeCode.String:
                case AttributeTypeCode.Memo:
                case AttributeTypeCode.EntityName:
                    return "nvarchar";
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Uniqueidentifier:
                    return "uniqueidentifier";

                case AttributeTypeCode.Virtual:
                    if (attTypeDisplayName != null && attTypeDisplayName.Value == AttributeTypeDisplayName.ImageType.Value)
                    {
                        return "image";
                    }
                    return "nvarchar";
                case AttributeTypeCode.Double:
                    return "float";
                case AttributeTypeCode.Decimal:
                    return "decimal";
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.Picklist:
                case AttributeTypeCode.Integer:
                    return "int";
                case AttributeTypeCode.Boolean:
                case AttributeTypeCode.ManagedProperty:
                    return "bit";
                case AttributeTypeCode.DateTime:
                    return "datetime";
                case AttributeTypeCode.Money:
                    return "money";
                case AttributeTypeCode.BigInt:
                    return "bigint";
                default:
                    return attType.ToString().ToLower();
            }
        }

        public static int GetLength(this AttributeMetadata attribute)
        {
            switch (attribute.AttributeType)
            {
                case AttributeTypeCode.BigInt:
                    return 8;
                case AttributeTypeCode.Boolean:
                    return 1;
                case AttributeTypeCode.CalendarRules:
                    return 0;
                case AttributeTypeCode.Customer:
                    return 16;
                case AttributeTypeCode.DateTime:
                    return 8;
                case AttributeTypeCode.Decimal:
                    return 9;
                case AttributeTypeCode.Double:
                    return 8;
                case AttributeTypeCode.EntityName:
                    return 200;
                case AttributeTypeCode.Integer:
                    // var i = (IntegerAttributeMetadata)attribute;
                    //  i.ma
                    return 4;
                case AttributeTypeCode.Lookup:
                    return 16;
                case AttributeTypeCode.ManagedProperty:
                    return 1;
                case AttributeTypeCode.Memo:
                    var m = (MemoAttributeMetadata)attribute;
                    return m.MaxLength.GetValueOrDefault();
                case AttributeTypeCode.Money:
                    return 8;
                case AttributeTypeCode.Owner:
                    return 16;
                case AttributeTypeCode.PartyList:
                    return 0;
                case AttributeTypeCode.Picklist:
                    return 4;
                case AttributeTypeCode.State:
                    return 4;
                case AttributeTypeCode.Status:
                    return 4;
                case AttributeTypeCode.String:
                    var s = (StringAttributeMetadata)attribute;
                    return s.MaxLength.GetValueOrDefault();

                case AttributeTypeCode.Uniqueidentifier:
                    return 16;
                case AttributeTypeCode.Virtual:
                    return 0;
                default:
                    throw new NotSupportedException();
            }
        }

        public static int GetSqlPrecision(this AttributeMetadata attribute)
        {
            switch (attribute.AttributeType)
            {
                case AttributeTypeCode.BigInt:
                    return 19;
                case AttributeTypeCode.Boolean:
                    return 1;
                case AttributeTypeCode.CalendarRules:
                    return 255;
                case AttributeTypeCode.Customer:
                    return 255;
                case AttributeTypeCode.DateTime:
                    return 23;
                case AttributeTypeCode.Decimal:
                    // return GetSqlPrecision()
                    return 9;
                case AttributeTypeCode.Double:
                    return 8;
                case AttributeTypeCode.EntityName:
                    return 200;
                case AttributeTypeCode.Integer:
                    // var i = (IntegerAttributeMetadata)attribute;
                    //  i.ma
                    return 4;
                case AttributeTypeCode.Lookup:
                    return 16;
                case AttributeTypeCode.ManagedProperty:
                    return 1;
                case AttributeTypeCode.Memo:
                    var m = (MemoAttributeMetadata)attribute;
                    return m.MaxLength.GetValueOrDefault();
                case AttributeTypeCode.Money:
                    return 8;
                case AttributeTypeCode.Owner:
                    return 16;
                case AttributeTypeCode.PartyList:
                    return 0;
                case AttributeTypeCode.Picklist:
                    return 4;
                case AttributeTypeCode.State:
                    return 4;
                case AttributeTypeCode.Status:
                    return 4;
                case AttributeTypeCode.String:
                    var s = (StringAttributeMetadata)attribute;
                    return s.MaxLength.GetValueOrDefault();

                case AttributeTypeCode.Uniqueidentifier:
                    return 16;
                case AttributeTypeCode.Virtual:
                    return 0;
                default:
                    throw new NotSupportedException();
            }
        }

        #region Decimal

        /// <summary>
        /// Gets the sql precision for the crm decimal attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int MaxSupportedSqlPrecision(this DecimalAttributeMetadata metadata)
        {
            // = 12 + max scale of 10 = 22 in total.      
            var crmPrecision = Math.Max(Math.Truncate(Math.Abs(DecimalAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DecimalAttributeMetadata.MinSupportedValue)).ToString().Length);

            //  int crmPrecision = Math.Max(Math.Truncate(DecimalAttributeMetadata.MaxSupportedValue).ToString().Length, Math.Truncate(DecimalAttributeMetadata.MinSupportedValue).ToString().Length);
            return crmPrecision + DecimalAttributeMetadata.MaxSupportedPrecision;
        }

        /// <summary>
        /// Gets the sql precision for the crm decimal attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static bool IsSqlPrecisionSupported(this DecimalAttributeMetadata metadata, int precision, int scale)
        {
            if (precision < scale)
            {
                throw new ArgumentOutOfRangeException("precision must be equal to or greater than scale.");
            }

            if (scale < DecimalAttributeMetadata.MinSupportedPrecision || scale > DecimalAttributeMetadata.MaxSupportedPrecision)
            {
                return false;
            }

            // = 12
            int crmMaxValueLengthWithoutPrecision = Math.Max(DecimalAttributeMetadata.MinSupportedValue.ToString().Length, DecimalAttributeMetadata.MaxSupportedValue.ToString().Length);
            if (precision - scale > crmMaxValueLengthWithoutPrecision)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Gets the default sql precision for the crm decimal attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int DefaultSqlPrecision(this DecimalAttributeMetadata metadata)
        {
            var precision = Math.Max(Math.Truncate(Math.Abs(DecimalAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DecimalAttributeMetadata.MinSupportedValue)).ToString().Length);
            return precision + DefaultSqlScale(metadata);
        }

        /// <summary>
        /// Gets the default sql scale for the crm decimal attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int DefaultSqlScale(this DecimalAttributeMetadata metadata)
        {
            int scale = DecimalAttributeMetadata.MinSupportedPrecision;
            return scale;
        }

        /// <summary>
        /// Sets the decimal size according to the sql precision and scale arguments. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static bool SetFromSqlPrecisionAndScale(this DecimalAttributeMetadata metadata, int precision, int scale)
        {
            if (precision < scale)
            {
                throw new ArgumentOutOfRangeException("precision must be equal to or greater than scale.");
            }

            if (scale < DecimalAttributeMetadata.MinSupportedPrecision || scale > DecimalAttributeMetadata.MaxSupportedPrecision)
            {
                throw new ArgumentOutOfRangeException("scale is not within min and max crm values.");
            }

            // = 12
            var crmMaxValueLengthWithoutPrecision = Math.Max(Math.Truncate(Math.Abs(DecimalAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DecimalAttributeMetadata.MinSupportedValue)).ToString().Length);
            if (precision - scale > crmMaxValueLengthWithoutPrecision)
            {
                throw new ArgumentOutOfRangeException("The precision is greater than the maximum value crm will allow.");
            }

            metadata.Precision = scale;

            // need to set appropriate min and max values.
            // If the precision is equal to the max precision allowed, then set min and max values allowed. 
            if (precision == crmMaxValueLengthWithoutPrecision)
            {
                metadata.MinValue = (decimal)DecimalAttributeMetadata.MinSupportedValue;
                metadata.MaxValue = (decimal)DecimalAttributeMetadata.MaxSupportedValue;
            }
            else
            {
                // the min value should be a series of 9's to the specified precision and scale.
                var maxNumberBuilder = new StringBuilder();
                for (int i = 0; i < precision - scale; i++)
                {
                    maxNumberBuilder.Append("9");
                }
                if (scale > 0)
                {
                    maxNumberBuilder.Append(".");
                    for (int i = 0; i < scale; i++)
                    {
                        maxNumberBuilder.Append("9");
                    }
                }

                decimal maxD;
                if (!decimal.TryParse(maxNumberBuilder.ToString(), out maxD))
                {
                    throw new FormatException(string.Format("Unable to parse {0} as a decimal.", maxNumberBuilder.ToString()));
                }

                var maxNumber = decimal.Parse(maxNumberBuilder.ToString());
                metadata.MaxValue = maxNumber;
                metadata.MinValue = -maxNumber;

            }

            return true;

        }

        #endregion

        #region Money

        /// <summary>
        /// Gets the sql precision for the crm money attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int MaxSupportedSqlPrecision(this MoneyAttributeMetadata metadata)
        {
            var crmPrecision = Math.Max(Math.Truncate(Math.Abs(MoneyAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(MoneyAttributeMetadata.MinSupportedValue)).ToString().Length);
            return crmPrecision + MoneyAttributeMetadata.MaxSupportedPrecision;
        }

        /// <summary>
        /// Gets the sql precision for the crm money attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static bool IsSqlPrecisionSupported(this MoneyAttributeMetadata metadata, int precision, int scale)
        {
            if (precision < scale)
            {
                throw new ArgumentOutOfRangeException("precision must be equal to or greater than scale.");
            }

            if (scale < MoneyAttributeMetadata.MinSupportedPrecision || scale > MoneyAttributeMetadata.MaxSupportedPrecision)
            {
                return false;
            }
            int crmMaxValueLengthWithoutPrecision = Math.Max(MoneyAttributeMetadata.MinSupportedValue.ToString().Length, MoneyAttributeMetadata.MaxSupportedValue.ToString().Length);
            if (precision - scale > crmMaxValueLengthWithoutPrecision)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Gets the default sql precision for the crm money attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int DefaultSqlPrecision(this MoneyAttributeMetadata metadata)
        {
            var precision = Math.Max(Math.Truncate(Math.Abs(MoneyAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(MoneyAttributeMetadata.MinSupportedValue)).ToString().Length);
            return precision + DefaultSqlScale(metadata);
        }

        /// <summary>
        /// Gets the default sql scale for the crm money attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int DefaultSqlScale(this MoneyAttributeMetadata metadata)
        {
            int scale = MoneyAttributeMetadata.MinSupportedPrecision;
            return scale;
        }

        /// <summary>
        /// Sets the money max size and max and min values, according to the specified sql precision and scale arguments. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static bool SetFromSqlPrecisionAndScale(this MoneyAttributeMetadata metadata, int precision, int scale)
        {
            if (precision < scale)
            {
                throw new ArgumentOutOfRangeException("precision must be equal to or greater than scale.");
            }

            if (scale < MoneyAttributeMetadata.MinSupportedPrecision || scale > MoneyAttributeMetadata.MaxSupportedPrecision)
            {
                throw new ArgumentOutOfRangeException("scale is not within min and max crm values.");
            }

            var crmMaxValueLengthWithoutPrecision = Math.Max(Math.Truncate(Math.Abs(MoneyAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(MoneyAttributeMetadata.MinSupportedValue)).ToString().Length);
            if (precision - scale > crmMaxValueLengthWithoutPrecision)
            {
                throw new ArgumentOutOfRangeException("The precision is greater than the maximum value crm will allow.");
            }

            metadata.Precision = scale;

            // need to set appropriate min and max values.
            // If the precision is equal to the max precision allowed, then set min and max values allowed. 
            if (precision == crmMaxValueLengthWithoutPrecision)
            {
                metadata.MinValue = (double)MoneyAttributeMetadata.MinSupportedValue;
                metadata.MaxValue = (double)MoneyAttributeMetadata.MaxSupportedValue;
            }
            else
            {
                // the min value should be a series of 9's to the specified precision and scale.
                var maxNumberBuilder = new StringBuilder();
                for (int i = 0; i < precision - scale; i++)
                {
                    maxNumberBuilder.Append("9");
                }
                if (scale > 0)
                {
                    maxNumberBuilder.Append(".");
                    for (int i = 0; i < scale; i++)
                    {
                        maxNumberBuilder.Append("9");
                    }
                }

                var maxNumber = double.Parse(maxNumberBuilder.ToString());
                metadata.MaxValue = maxNumber;
                metadata.MinValue = -maxNumber;
            }

            // finallty, as we are setting precision and scale explicitly, the precision source should be set to honour our precision.
            //When the PrecisionSource is set to zero (0), the MoneyAttributeMetadata.Precision value is used.
            //When the PrecisionSource is set to one (1), the Organization.PricingDecimalPrecision value is used.
            //When the PrecisionSource is set to two (2), the TransactionCurrency.CurrencyPrecision value is used.
            metadata.PrecisionSource = 0;

            return true;

        }

        #endregion

        #region Double

        /// <summary>
        /// Gets the sql precision for the crm double attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int MaxSupportedSqlPrecision(this DoubleAttributeMetadata metadata)
        {
            var crmPrecision = Math.Max(Math.Truncate(Math.Abs(DoubleAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DoubleAttributeMetadata.MinSupportedValue)).ToString().Length);
            return crmPrecision + DoubleAttributeMetadata.MaxSupportedPrecision;
        }

        /// <summary>
        /// Gets the sql precision for the crm double attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static bool IsSqlPrecisionSupported(this DoubleAttributeMetadata metadata, int precision, int scale)
        {
            if (precision < scale)
            {
                throw new ArgumentOutOfRangeException("precision must be equal to or greater than scale.");
            }

            if (scale < DoubleAttributeMetadata.MinSupportedPrecision || scale > DoubleAttributeMetadata.MaxSupportedPrecision)
            {
                return false;
            }
            int crmMaxValueLengthWithoutPrecision = Math.Max(DoubleAttributeMetadata.MinSupportedValue.ToString().Length, DoubleAttributeMetadata.MaxSupportedValue.ToString().Length);
            if (precision - scale > crmMaxValueLengthWithoutPrecision)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// Gets the default sql precision for the crm double attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int DefaultSqlPrecision(this DoubleAttributeMetadata metadata)
        {
            var precision = Math.Max(Math.Truncate(Math.Abs(DoubleAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DoubleAttributeMetadata.MinSupportedValue)).ToString().Length);
            return precision + DefaultSqlScale(metadata);
        }

        /// <summary>
        /// Gets the default sql scale for the crm double attribute. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static int DefaultSqlScale(this DoubleAttributeMetadata metadata)
        {
            int scale = DoubleAttributeMetadata.MinSupportedPrecision;
            return scale;
        }

        /// <summary>
        /// Sets the double max size and max and min values, according to the specified sql precision and scale arguments. 
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public static bool SetFromSqlPrecisionAndScale(this DoubleAttributeMetadata metadata, int precision, int scale)
        {
            if (precision < scale)
            {
                throw new ArgumentOutOfRangeException("precision must be equal to or greater than scale.");
            }

            if (scale < DoubleAttributeMetadata.MinSupportedPrecision || scale > DoubleAttributeMetadata.MaxSupportedPrecision)
            {
                throw new ArgumentOutOfRangeException("scale is not within min and max crm values.");
            }

            var crmMaxValueLengthWithoutPrecision = Math.Max(Math.Truncate(Math.Abs(DoubleAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DoubleAttributeMetadata.MinSupportedValue)).ToString().Length);
            if (precision - scale > crmMaxValueLengthWithoutPrecision)
            {
                throw new ArgumentOutOfRangeException("The precision is greater than the maximum value crm will allow.");
            }

            metadata.Precision = scale;

            // need to set appropriate min and max values.
            // If the precision is equal to the max precision allowed, then set min and max values allowed. 
            if (precision == crmMaxValueLengthWithoutPrecision)
            {
                metadata.MinValue = (double)DoubleAttributeMetadata.MinSupportedValue;
                metadata.MaxValue = (double)DoubleAttributeMetadata.MaxSupportedValue;
            }
            else
            {
                // the min value should be a series of 9's to the specified precision and scale.
                var maxNumberBuilder = new StringBuilder();
                for (int i = 0; i < precision - scale; i++)
                {
                    maxNumberBuilder.Append("9");
                }
                if (scale > 0)
                {
                    maxNumberBuilder.Append(".");
                    for (int i = 0; i < scale; i++)
                    {
                        maxNumberBuilder.Append("9");
                    }
                }

                double maxNumber;
                if (!double.TryParse(maxNumberBuilder.ToString(), out maxNumber))
                {
                    throw new FormatException(string.Format("Unable to parse {0} as a double.", maxNumberBuilder.ToString()));
                }

                metadata.MaxValue = maxNumber;
                metadata.MinValue = -maxNumber;
            }

            return true;

        }

        #endregion

    }
}