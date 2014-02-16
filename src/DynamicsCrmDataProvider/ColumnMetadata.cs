using System;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsCrmDataProvider
{
    public class ColumnMetadata
    {
        private string _columnName;

        protected ColumnMetadata()
        {

        }

        public ColumnMetadata(AttributeMetadata attributeMetadata, string entityAlias = "")
        {
            AttributeMetadata = attributeMetadata;
            this.EntityAlias = entityAlias;
            if (!string.IsNullOrEmpty(entityAlias))
            {
                this._columnName = string.Format("{0}.{1}", entityAlias, attributeMetadata.LogicalName);
            }
            else
            {
                this._columnName = attributeMetadata.LogicalName;
            }
        }

        public virtual AttributeMetadata AttributeMetadata { get; set; }

        public string EntityAlias { get; set; }

        public virtual string ColumnName { get { return _columnName; } }

        public virtual string ColumnDataType()
        {
            if (AttributeMetadata.AttributeType != null) return AttributeMetadata.AttributeType.Value.ToString();
            return string.Empty;
        }

        public virtual AttributeTypeCode AttributeType()
        {
            if (AttributeMetadata.AttributeType != null) return AttributeMetadata.AttributeType.Value;
            return AttributeTypeCode.String;
        }
    }

    //public class ColumnMetadata
    //{
    //    public string Name { get; set; }
    //    public string DataTypeName
    //    {
    //        get { return this.AttributeTypeCode.ToString(); }
    //    }
    //    public AttributeTypeCode AttributeTypeCode { get; set; }
    //    public Type DataType { get; set; }
    //}
}