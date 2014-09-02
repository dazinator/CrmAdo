using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System.Text;

namespace CrmAdo.Dynamics.Metadata
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
        public static Type GetCrmAgnosticType(this AttributeMetadata metadata)
        {
            switch (metadata.AttributeType.GetValueOrDefault())
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
        public static string GetSqlDataTypeName(this AttributeMetadata metadata)
        {
            if (metadata.AttributeType == null)
            {
                return string.Empty;
            }
            switch (metadata.AttributeType)
            {
                case AttributeTypeCode.String:
                case AttributeTypeCode.Memo:
                    return "nvarchar";
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                    return "uniqueidentifier";
                case AttributeTypeCode.Virtual:
                    if (metadata.GetType() == typeof(ImageAttributeMetadata))
                    {
                        return "image";
                    }
                    //if (metadata.LogicalAttributeName == "entityimage")
                    //{

                    //}
                    return "nvarchar";
                case AttributeTypeCode.Double:
                    return "float";
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.Picklist:
                    return "integer";
                case AttributeTypeCode.Boolean:
                    return "bit";
                default:
                    return metadata.AttributeType.Value.ToString();
            }
        }

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

                var maxNumber = decimal.Parse(maxNumberBuilder.ToString());
                metadata.MaxValue = maxNumber;
                metadata.MinValue = -maxNumber;

            }

            return true;

        }


    }
}