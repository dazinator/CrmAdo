using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmAdo.Metadata
{
    public class CrmEntityMetadata
    {

        public CrmEntityMetadata(string entityName)
            : this(entityName, new List<AttributeInfo>(), null)
        {
        }

        public CrmEntityMetadata(string entityName, List<AttributeInfo> attributes, string primaryIdAttributeName)
        {
            Attributes = attributes;
            EntityName = entityName;
            PrimaryIdAttribute = primaryIdAttributeName;
        }

        public string Timestamp { get; set; }
        public string EntityName { get; set; }

        public string PrimaryIdAttribute { get; set; }

        public List<AttributeInfo> Attributes { get; set; }

        /// <summary>
        /// This lock is taken when the metadata "Refresh" method is run, as during that time the object can be modified with the latest updates.
        /// </summary>
        private static object _Lock = new object();

        public void Refresh(List<AttributeInfo> modifiedFields, List<Guid> deletedFields)
        {
            lock (_Lock)
            {
                foreach (var deletedField in deletedFields)
                {
                    var existingAtt = Attributes.FirstOrDefault(a => a.MetadataId == deletedField);
                    if (existingAtt != null)
                    {
                        Attributes.Remove(existingAtt);
                    }
                }

                foreach (var modifiedField in modifiedFields)
                {
                    var existingAtt = Attributes.FirstOrDefault(a => a.MetadataId == modifiedField.MetadataId);
                    if (existingAtt != null)
                    {
                        Attributes.Remove(existingAtt);
                        Attributes.Add(modifiedField);
                    }
                }
            }

        }

        public bool IsPseudo { get; set; }

        public AttributeInfo AddPseudoAttribute(string name, AttributeTypeCode attTypeCode, AttributeTypeDisplayName attDisplayName = null)
        {
            var factory = new AttributeInfoFactory();
            var attInfo = factory.CreatePseudo(this.EntityName, name, attTypeCode, attDisplayName);
            this.Attributes.Add(attInfo);
            return attInfo;
        }

    }
}