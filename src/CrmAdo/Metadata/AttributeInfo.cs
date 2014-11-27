using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.Dynamics.Metadata;

namespace CrmAdo.Metadata
{
    public abstract class AttributeInfo : IColumnMetadata      
    {

        public string AttributeOf { get; set; }

        public AttributeTypeCode? AttributeType { get; set; }
        public AttributeTypeDisplayName AttributeTypeDisplayName { get; set; }

        public bool? CanBeSecuredForCreate { get; set; }

        public bool? CanBeSecuredForRead { get; set; }

        public bool? CanBeSecuredForUpdate { get; set; }

        public bool? CanModifyAdditionalSettings { get; set; }

        public int? ColumnNumber { get; set; }

        public string DeprecatedVersion { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public string EntityLogicalName { get; set; }

        public string IntroducedVersion { get; set; }

        public bool? IsAuditEnabled { get; set; }

        public bool? IsCustomAttribute { get; set; }

        public bool? IsCustomizable { get; set; }

        public bool? IsManaged { get; set; }

        public bool? IsPrimaryId { get; set; }

        public bool? IsPrimaryName { get; set; }

        public bool? IsRenameable { get; set; }

        public bool? IsSecured { get; set; }

        public bool? IsValidForAdvancedFind { get; set; }

        public bool? IsValidForCreate { get; set; }

        public bool? IsValidForRead { get; set; }

        public bool? IsValidForUpdate { get; set; }

        public Guid? LinkedAttributeId { get; set; }

        public string LogicalName { get; set; }

        public AttributeRequiredLevelManagedProperty RequiredLevel { get; set; }

        public string SchemaName { get; set; }

        public Guid MetadataId { get; set; }

        public virtual string GetSqlDataTypeName()
        {
            return AttributeType.GetValueOrDefault().GetSqlDataTypeName(this.AttributeTypeDisplayName);
        }

        public virtual Type GetFieldType()
        {
            return AttributeType.GetValueOrDefault().GetCrmAgnosticType();
        }

        public bool IsPseudo { get; set; }

        #region IColumnMetadata

        public string DataType
        {
            get
            {
                return GetSqlDataTypeName();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual int IdentitySeed
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual int IdentityIncrement
        {
            get
            {
                return 0;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual bool IsIdentity
        {
            get
            {
                return this.IsPrimaryId.GetValueOrDefault();
            }          
        }

        private int _Length = 0;
        public virtual int Length
        {
            get
            {
                return _Length;
            }
            set
            {
                _Length = value;
            }
        }

        public virtual bool Nullable
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual int GetNumericPrecision()
        {
            return 255;
        }

        public virtual int GetNumericScale()
        {
            return 255;
        }

        #endregion       

       
        

    }
}
