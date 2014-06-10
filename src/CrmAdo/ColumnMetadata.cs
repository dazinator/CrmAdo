using System;
using System.Linq;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmAdo
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

        public bool HasAlias { get { return _hasAlias; } }

        public virtual string ColumnName { get { return _columnName; } }

        public virtual string LogicalAttributeName { get { return AttributeMetadata.LogicalName; } }

        public virtual string GetDataTypeName()
        {
            if (AttributeMetadata.AttributeType == null)
            {
                return string.Empty;
            }
            switch (AttributeMetadata.AttributeType)
            {
                case AttributeTypeCode.String:
                case AttributeTypeCode.Memo:
                    return "nvarchar";
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                    return "uniqueidentifier";
                case AttributeTypeCode.Virtual:
                    if (this.LogicalAttributeName == "entityimage")
                    {
                        return "image";
                    }
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
                    return AttributeMetadata.AttributeType.Value.ToString();
            }
        }

        public virtual AttributeTypeCode AttributeType()
        {
            if (AttributeMetadata.AttributeType != null) return AttributeMetadata.AttributeType.Value;
            return AttributeTypeCode.String;
        }

        public virtual Type GetFieldType()
        {
            return AttributeMetadata.GetCrmAgnosticType();
        }

        /// <summary>
        /// Compares the name that could include an alias to see if it matches the same logical name.
        /// </summary>
        /// <returns></returns>
        public bool IsSameLogicalName(string aliasedName)
        {
            if (aliasedName.Contains("."))
            {
                var segments = aliasedName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var name = segments.Last();
                return name.ToLower() == this.LogicalAttributeName;
            }
            else
            {
                return aliasedName.ToLower() == this.LogicalAttributeName;
            }
        }
    }
}