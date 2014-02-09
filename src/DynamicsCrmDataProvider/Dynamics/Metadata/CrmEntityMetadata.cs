using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsCrmDataProvider.Dynamics
{
    public class CrmEntityMetadata
    {
        public string Timestamp { get; set; }
        public string EntityName { get; set; }



        public List<AttributeMetadata> Attributes { get; set; }
        private static object _Lock = new object();

        public void Refresh(List<AttributeMetadata> modifiedFields, List<Guid> deletedFields)
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
    }
}