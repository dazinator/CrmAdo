using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk.Metadata;
using CrmAdo.Metadata;
using CrmAdo.Core;
using CrmAdo.Util;

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
            _Metadata.Add("entitymetadata", BuildPseudoEntityMetadata());
            _Metadata.Add("attributemetadata", BuildPseudoAttributeMetadata());
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

                var atts = new List<AttributeMetadata>();
                atts.AddRange(deserialised.Attributes);
                var attsList = _MetadataConverter.ConvertAttributeInfoList(atts);
                var entName = "contact";
                var crmMeta = new CrmEntityMetadata(entName, attsList, deserialised.PrimaryIdAttribute);
                return crmMeta;
            }
        }
        
        private CrmEntityMetadata BuildPseudoEntityMetadata()
        {
            var metadata = new CrmEntityMetadata("entitymetadata");
            metadata.IsPseudo = true;
            metadata.AddPseudoAttribute("activitytypemask", AttributeTypeCode.Integer);
            metadata.AddPseudoAttribute("autoroutetoownerqueue", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("autocreateaccessteams", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("canbeinmanytomany", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("canbeprimaryentityinrelationship", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("canberelatedentityinrelationship", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("cancreateattributes", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("cancreatecharts", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("cancreateforms", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("cancreateviews", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("canmodifyadditionalsettings", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("cantriggerworkflow", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("description", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("displaycollectionname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("displayname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("haschanged", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("iconlargename", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("iconmediumname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("iconsmallname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("introducedversion", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("isactivity", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isactivityparty", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isairupdated", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isauditenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isavailableoffline", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isbusinessprocessenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("ischildentity", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isconnectionsenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("iscustomentity", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("iscustomizable", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isdocumentmanagementenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isduplicatedetectionenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isenabledfortrace", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isenabledforcharts", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isimportable", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isintersect", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("ismailmergeenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("ismanaged", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("ismappable", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isquickcreateenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isreadingpaneenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isreadonlyinmobileclient", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isrenameable", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isvalidforadvancedfind", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isvalidforqueue", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isvisibleinmobile", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isvisibleinmobileclient", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("logicalname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("metadataid", AttributeTypeCode.Uniqueidentifier);
            metadata.AddPseudoAttribute("objecttypecode", AttributeTypeCode.Integer);
            metadata.AddPseudoAttribute("ownershiptype", AttributeTypeCode.Integer);
            metadata.AddPseudoAttribute("primaryidattribute", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("primaryimageattribute", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("primarynameattribute", AttributeTypeCode.String);
            //metadata.AddPseudoAttribut, "privileges", AttributeTypeCode.Uniqueidentifier)));
            metadata.AddPseudoAttribute("recurrencebaseentitylogicalname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("reportviewname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("schemaname", AttributeTypeCode.String);
            return metadata;
        }

        private CrmEntityMetadata BuildPseudoAttributeMetadata()
        {
            //   AttributeMetadata x;

            var metadata = new CrmEntityMetadata("attributemetadata");
            metadata.IsPseudo = true;
            metadata.AddPseudoAttribute("attributeof", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("attributetype", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("attributetypename", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("canbesecuredforcreate", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("canbesecuredforread", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("canbesecuredforupdate", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("canmodifyadditionalsettings", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("columnnumber", AttributeTypeCode.Integer);
            metadata.AddPseudoAttribute("deprecatedversion", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("description", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("displayname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("entitylogicalname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("introducedversion", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("isauditenabled", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("iscustomattribute", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("iscustomizable", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("ismanaged", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isprimaryid", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isprimaryname", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isrenameable", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("issecured", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isvalidforadvancedfind", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isvalidforcreate", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isvalidforread", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("isvalidforupdate", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("linkedattributeid", AttributeTypeCode.Uniqueidentifier);
            metadata.AddPseudoAttribute("logicalname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("metadataid", AttributeTypeCode.Uniqueidentifier);
            metadata.AddPseudoAttribute("requiredlevel", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("schemaname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("isprimarykey", AttributeTypeCode.Boolean);
            metadata.AddPseudoAttribute("optionsetname", AttributeTypeCode.String);
            metadata.AddPseudoAttribute("optionsetoptions", AttributeTypeCode.String);
            return metadata;
        }


    }
}