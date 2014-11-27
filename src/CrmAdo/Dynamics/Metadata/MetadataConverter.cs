using CrmAdo.Metadata;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Dynamics.Metadata
{
    public class MetadataConverter
    {
        public List<AttributeInfo> ConvertAttributeInfoList(IEnumerable<AttributeMetadata> attributeMetadata)
        {
            var results = new List<AttributeInfo>();
            var factory = new AttributeInfoFactory();
            foreach (var item in attributeMetadata)
            {
                AttributeInfo attInfo = factory.Create(item);               
                results.Add(attInfo);
            }
            return results;
        }
    }
}
