using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Xrm.Sdk.Metadata;

namespace CrmAdo.Tests
{
    /// <summary>
    /// Fake metadata provider that loads information from local test files.
    /// </summary>
    public class FakeContactMetadataProvider : ICrmMetaDataProvider
    {
        private Dictionary<string, CrmEntityMetadata> _Metadata = null;
        private MetadataConverter _MetadataConverter = new MetadataConverter();

        public FakeContactMetadataProvider()
        {
            _Metadata = new Dictionary<string, CrmEntityMetadata>();
        }

        public CrmEntityMetadata GetEntityMetadata(string entityName)
        {
            if (!_Metadata.ContainsKey(entityName))
            {
                var meta = LoadTestMetadataForEntity(entityName);
                _Metadata[entityName] = meta;
                return meta;
            }
            return _Metadata[entityName];
        }

        public CrmEntityMetadata RefreshEntityMetadata(string entityName)
        {
            // nothing to do in test mode.
            return _Metadata[entityName];
        }

        private CrmEntityMetadata LoadTestMetadataForEntity(string entityName)
        {
            var path = Environment.CurrentDirectory;
            var shortFileName = entityName + "Metadata.xml";

            var fileName = System.IO.Path.Combine(path, "MetadataFiles\\", shortFileName);

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Could not find metadata file for entity " + entityName);
            }

            using (var reader = new XmlTextReader(fileName))
            {
                var deserialised = EntityMetadataUtils.DeserializeMetaData(reader);
                var crmMeta = new CrmEntityMetadata();
                var atts = new List<AttributeMetadata>();
                atts.AddRange(deserialised.Attributes);
                crmMeta.Attributes = _MetadataConverter.ConvertAttributeInfoList(atts);
                crmMeta.EntityName = "contact";
                return crmMeta;
            }
        }

    }
}