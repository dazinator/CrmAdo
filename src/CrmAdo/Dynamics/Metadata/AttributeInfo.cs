using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.Dynamics.Metadata;

namespace CrmAdo.Dynamics.Metadata
{
    public class AttributeInfo
    {
        public Guid MetadataId { get; set; }
        public string LogicalName { get; set; }
        public string EntityLogicalName { get; set; }
        public AttributeTypeCode? AttributeType { get; set; }
        public AttributeTypeDisplayName AttributeTypeDisplayName { get; set; }
        public virtual string GetSqlDataTypeName()
        {
            return AttributeType.GetValueOrDefault().GetSqlDataTypeName(this.AttributeTypeDisplayName);
        }
        public virtual Type GetFieldType()
        {
            return AttributeType.GetValueOrDefault().GetCrmAgnosticType();
        }
        public bool IsPrimaryId { get; set; }
        public bool? IsValidForUpdate { get; internal set; }
        public bool? IsValidForCreate { get; internal set; }
        public bool IsPseudo { get; set; }
    }
}
