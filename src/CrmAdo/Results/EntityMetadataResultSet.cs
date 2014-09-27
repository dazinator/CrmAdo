using CrmAdo.Dynamics.Metadata;
using CrmAdo.Results;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{

    public class EntityMetadataResultSet : ResultSet
    {

        internal class DenormalisedMetadataResult
        {
            public EntityMetadata EntityMetadata { get; set; }
            public AttributeMetadata AttributeMetadata { get; set; }
            public OneToManyRelationshipMetadata OneToManyRelationship { get; set; }
            public ManyToManyRelationshipMetadata ManyToManyRelationship { get; set; }

            public object GetEntityMetadataValue(string propertyname)
            {
                switch (propertyname.ToLower())
                {
                    case "activitytypemask":
                        return EntityMetadata.ActivityTypeMask;
                    case "autocreateaccessteams":
                        return EntityMetadata.AutoCreateAccessTeams;
                    case "autoroutetoownerqueue":
                        return EntityMetadata.AutoRouteToOwnerQueue;
                    case "canbeinmanytomany":
                        return EntityMetadata.CanBeInManyToMany.Value;
                    case "canbeprimaryentityinrelationship":
                        return EntityMetadata.CanBePrimaryEntityInRelationship.Value;
                    case "canberelatedentityinrelationship":
                        return EntityMetadata.CanBeRelatedEntityInRelationship.Value;
                    case "cancreateattributes":
                        return EntityMetadata.CanCreateAttributes.Value;
                    case "cancreatecharts":
                        return EntityMetadata.CanCreateCharts.Value;
                    case "cancreateforms":
                        return EntityMetadata.CanCreateForms.Value;
                    case "cancreateviews":
                        return EntityMetadata.CanCreateViews.Value;
                    case "canmodifyadditionalsettings":
                        return EntityMetadata.CanModifyAdditionalSettings.Value;
                    case "cantriggerworkflow":
                        return EntityMetadata.CanTriggerWorkflow.Value;
                    case "description":
                        return EntityMetadata.Description;
                    case "displaycollectionname":
                        return EntityMetadata.DisplayCollectionName;
                    case "displayname":
                        return EntityMetadata.DisplayName;
                    case "haschanged":
                        return EntityMetadata.HasChanged;
                    case "iconlargename":
                        return EntityMetadata.IconLargeName;
                    case "iconmediumname":
                        return EntityMetadata.IconMediumName;
                    case "iconsmallname":
                        return EntityMetadata.IconSmallName;
                    case "introducedversion":
                        return EntityMetadata.IntroducedVersion;
                    case "isactivity":
                        return EntityMetadata.IsActivity;
                    case "isactivityparty":
                        return EntityMetadata.IsActivityParty;
                    case "isairupdated":
                        return EntityMetadata.IsAIRUpdated;
                    case "isauditenabled":
                        return EntityMetadata.IsAuditEnabled.Value;
                    case "isavailableoffline":
                        return EntityMetadata.IsAvailableOffline;
                    case "isbusinessprocessenabled":
                        return EntityMetadata.IsBusinessProcessEnabled;
                    case "ischildentity":
                        return EntityMetadata.IsChildEntity;
                    case "isconnectionsenabled":
                        return EntityMetadata.IsConnectionsEnabled.Value;
                    case "iscustomentity":
                        return EntityMetadata.IsCustomEntity;
                    case "iscustomizable":
                        return EntityMetadata.IsCustomizable.Value;
                    case "isdocumentmanagementenabled":
                        return EntityMetadata.IsDocumentManagementEnabled;
                    case "isduplicatedetectionenabled":
                        return EntityMetadata.IsDuplicateDetectionEnabled.Value;
                    case "isenabledforcharts":
                        return EntityMetadata.IsEnabledForCharts;
                    case "isenabledfortrace":
                        return EntityMetadata.IsEnabledForTrace;
                    case "isimportable":
                        return EntityMetadata.IsImportable;
                    case "isintersect":
                        return EntityMetadata.IsIntersect;
                    case "ismailmergeenabled":
                        return EntityMetadata.IsMailMergeEnabled.Value;
                    case "ismanaged":
                        return EntityMetadata.IsManaged;
                    case "ismappable":
                        return EntityMetadata.IsMappable.Value;
                    case "isquickcreateenabled":
                        return EntityMetadata.IsQuickCreateEnabled;
                    case "isreadingpaneenabled":
                        return EntityMetadata.IsReadingPaneEnabled;
                    case "isreadonlyinmobileclient":
                        return EntityMetadata.IsReadOnlyInMobileClient.Value;
                    case "isrenameable":
                        return EntityMetadata.IsRenameable.Value;
                    case "isvalidforadvancedfind":
                        return EntityMetadata.IsValidForAdvancedFind;
                    case "isvalidforqueue":
                        return EntityMetadata.IsValidForQueue.Value;
                    case "isvisibleinmobile":
                        return EntityMetadata.IsVisibleInMobile.Value;
                    case "isvisibleinmobileclient":
                        return EntityMetadata.IsVisibleInMobileClient.Value;
                    case "logicalname":
                        return EntityMetadata.LogicalName;
                    case "metadataid":
                        return EntityMetadata.MetadataId;
                    case "objecttypecode":
                        return EntityMetadata.ObjectTypeCode;
                    case "ownershiptype":
                        return EntityMetadata.OwnershipType;
                    case "primaryidattribute":
                        return EntityMetadata.PrimaryIdAttribute;
                    case "primaryimageattribute":
                        return EntityMetadata.PrimaryImageAttribute;
                    case "primarynameattribute":
                        return EntityMetadata.PrimaryNameAttribute;
                    case "recurrencebaseentitylogicalname":
                        return EntityMetadata.RecurrenceBaseEntityLogicalName;
                    case "reportviewname":
                        return EntityMetadata.ReportViewName;
                    case "schemaname":
                        return EntityMetadata.SchemaName;

                    default:
                        return null;
                }
            }

            public object GetAttributeMetadataValue(string propertyname)
            {
                if (AttributeMetadata == null)
                {
                    return null;
                }
                switch (propertyname.ToLower())
                {
                    case "attributeof":
                        return AttributeMetadata.AttributeOf;
                    case "attributetype":
                        return AttributeMetadata.AttributeType.Value.ToString();
                    case "attributetypename":
                        return AttributeMetadata.AttributeTypeName.Value;
                    case "canbesecuredforcreate":
                        return AttributeMetadata.CanBeSecuredForCreate;
                    case "canbesecuredforread":
                        return AttributeMetadata.CanBeSecuredForRead;
                    case "canbesecuredforupdate":
                        return AttributeMetadata.CanBeSecuredForUpdate;
                    case "canmodifyadditionalsettings":
                        return AttributeMetadata.CanModifyAdditionalSettings.Value;
                    case "columnnumber":
                        return AttributeMetadata.ColumnNumber;
                    case "deprecatedversion":
                        return AttributeMetadata.DeprecatedVersion;
                    case "description":
                        return AttributeMetadata.Description;
                    case "displayname":
                        return AttributeMetadata.DisplayName;
                    case "entitylogicalname":
                        return AttributeMetadata.EntityLogicalName;
                    case "introducedversion":
                        return AttributeMetadata.IntroducedVersion;
                    case "isauditenabled":
                        return AttributeMetadata.IsAuditEnabled.Value;
                    case "iscustomattribute":
                        return AttributeMetadata.IsCustomAttribute;
                    case "iscustomizable":
                        return AttributeMetadata.IsCustomizable.Value;
                    case "ismanaged":
                        return AttributeMetadata.IsManaged;
                    case "isprimaryid":
                        return AttributeMetadata.IsPrimaryId;
                    case "isprimaryname":
                        return AttributeMetadata.IsPrimaryName;
                    case "isrenameable":
                        return AttributeMetadata.IsRenameable.Value;
                    case "issecured":
                        return AttributeMetadata.IsSecured;
                    case "isvalidforadvancedfind":
                        return AttributeMetadata.IsValidForAdvancedFind.Value;
                    case "isvalidforcreate":
                        return AttributeMetadata.IsValidForCreate;
                    case "isvalidforread":
                        return AttributeMetadata.IsValidForRead;
                    case "isvalidforupdate":
                        return AttributeMetadata.IsValidForUpdate;
                    case "linkedattributeid":
                        return AttributeMetadata.LinkedAttributeId;
                    case "logicalname":
                        return AttributeMetadata.LogicalName;
                    case "metadataid":
                        return AttributeMetadata.MetadataId;
                    case "requiredlevel":
                        return AttributeMetadata.RequiredLevel.Value.ToString();
                    case "schemaname":
                        return AttributeMetadata.SchemaName;
                    case "isprimarykey":
                        return AttributeMetadata.LogicalName == EntityMetadata.PrimaryIdAttribute;

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

        private DenormalisedMetadataResult[] _Results = null;
        private int _ResultCount = 0;

        public EntityMetadataResultSet(CrmDbCommand command, OrganizationRequest request, List<ColumnMetadata> columnMetadata)
            : base(command, request, columnMetadata)
        {

        }

        internal DenormalisedMetadataResult[] Results { get { return _Results; } set { _Results = value; _ResultCount = _Results.Count(); } }

        public override bool HasResults()
        {
            return Results != null && Results.Any();
        }

        public override int ResultCount()
        {
            return _ResultCount;
        }

        public override DbDataReader GetReader(DbConnection connection = null)
        {
            return new CrmDbMetadataReader(this, connection);
        }

        public override object GetScalar()
        {
            throw new NotImplementedException();
        }
    }



}
