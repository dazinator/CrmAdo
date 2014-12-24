using System;
using System.Linq;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk.Metadata;
using CrmAdo.Metadata;

namespace CrmAdo
{

    public class ColumnMetadata
    {
        private string _columnName;
        private bool _hasAlias;

        protected ColumnMetadata()
        {

        }

        public ColumnMetadata(AttributeInfo attMetadata, string entityAlias = "")
        {
            // AttributeMetadata = attributeMetadata;
            this.EntityAlias = entityAlias;
            this.AttributeMetadata = attMetadata;

            if (!string.IsNullOrEmpty(entityAlias))
            {
                _hasAlias = true;
                this._columnName = string.Format("{0}.{1}", entityAlias, attMetadata.LogicalName);
            }
            else
            {
                _hasAlias = false;
                this._columnName = attMetadata.LogicalName;
            }
        }

        public ColumnMetadata(string attributeName, string entityAlias = "")
        {
            // AttributeMetadata = attributeMetadata;
            this.EntityAlias = entityAlias;
            this.AttributeMetadata = null;

            if (!string.IsNullOrEmpty(entityAlias))
            {
                _hasAlias = true;
                this._columnName = string.Format("{0}.{1}", entityAlias, attributeName);
            }
            else
            {
                _hasAlias = false;
                this._columnName = attributeName;
            }
        }

        public string EntityAlias { get; set; }

        public bool HasAlias { get { return _hasAlias; } }

        public virtual string ColumnName { get { return _columnName; } }

        public AttributeInfo AttributeMetadata { get; set; }

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

                return name.ToLower() == this.AttributeMetadata.LogicalName;
            }
            else
            {
                return aliasedName.ToLower() == this.AttributeMetadata.LogicalName;
            }
        }

    }
}