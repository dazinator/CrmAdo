using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Results
{
    public struct DenormalisedMetadataResult
    {
        public EntityMetadata EntityMetadata { get; set; }
        public AttributeMetadata AttributeMetadata { get; set; }
        public OneToManyRelationshipMetadata OneToManyRelationship { get; set; }
        public ManyToManyRelationshipMetadata ManyToManyRelationship { get; set; }

        public object GetEntityMetadataValue(string propertyname)
        {
            switch (propertyname.ToLower())
            {
                case "metadataid":
                    return EntityMetadata.MetadataId;

                default:
                    return null;
            }
        }

        public object GetAttributeMetadataValue(string propertyname)
        {
            switch (propertyname.ToLower())
            {
                case "metadataid":
                    return AttributeMetadata.MetadataId;

                default:
                    return null;
            }
        }

        public object GetOneToManyRelationshipValue(string propertyname)
        {
            switch (propertyname.ToLower())
            {
                case "metadataid":
                    return OneToManyRelationship.MetadataId;

                default:
                    return null;
            }
        }

        public object GetManyToManyRelationshipValue(string propertyname)
        {
            switch (propertyname.ToLower())
            {
                case "metadataid":
                    return ManyToManyRelationship.MetadataId;

                default:
                    return null;
            }
        }

    }
}
