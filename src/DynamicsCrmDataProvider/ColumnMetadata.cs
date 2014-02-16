using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsCrmDataProvider
{
    public class ColumnMetadata
    {
        private string _columnName;
        private bool _hasAlias;

        protected ColumnMetadata()
        {

        }

        public ColumnMetadata(AttributeMetadata attributeMetadata, string entityAlias = "")
        {
            AttributeMetadata = attributeMetadata;
            this.EntityAlias = entityAlias;
            if (!string.IsNullOrEmpty(entityAlias))
            {
                _hasAlias = true;
                this._columnName = string.Format("{0}.{1}", entityAlias, attributeMetadata.LogicalName);
            }
            else
            {
                _hasAlias = false;
                this._columnName = attributeMetadata.LogicalName;
            }
        }

        public virtual AttributeMetadata AttributeMetadata { get; set; }

        public string EntityAlias { get; set; }

        public bool HasAlias { get { return _hasAlias;} }

        public virtual string ColumnName { get { return _columnName; } }

        public virtual string LogicalAttributeName { get { return AttributeMetadata.LogicalName; } }

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

        /// <summary>
        /// Compares the name that could include an alias to see if it matches the same logical name.
        /// </summary>
        /// <returns></returns>
        public bool IsSameLogicalName(string aliasedName)
        {
            if (aliasedName.Contains("."))
            {
                var segments = aliasedName.Split(new char[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                var name = segments.Last();
                return name.ToLower() == this.LogicalAttributeName;
            }
            else
            {
                return aliasedName.ToLower() == this.LogicalAttributeName;
            }
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