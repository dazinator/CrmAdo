using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;

namespace CrmAdo.Dynamics.Metadata
{
    public class InMemoryCachedCrmMetaDataProvider : ICrmMetaDataProvider
    {
        private static readonly ConcurrentDictionary<string, CrmEntityMetadata> _Metadata = new ConcurrentDictionary<string, CrmEntityMetadata>();

        private IEntityMetadataRepository _repository;
        private MetadataConverter _metadataConverter;

        public InMemoryCachedCrmMetaDataProvider(IEntityMetadataRepository repository)
        {
            _repository = repository;
            this._metadataConverter = new MetadataConverter();
        }

        /// <summary>
        /// Returns the metadata for an entity.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public CrmEntityMetadata GetEntityMetadata(string entityName)
        {
            var changes = _Metadata.GetOrAdd(entityName, p =>
            {
                Debug.WriteLine("Retrieving metadata for entity: " + entityName, "Metadata");

                var pseudoMetadata = CheckPseudoEntity(entityName);
                if (pseudoMetadata != null)
                {
                    return pseudoMetadata;
                }
                var metadata = _repository.GetChanges(entityName, null);
                var entMeta = metadata.EntityMetadata[0];
                var atts = _metadataConverter.ConvertAttributeInfoList(entMeta.Attributes);
                var result = new CrmEntityMetadata(entityName, atts);
                result.Timestamp = metadata.ServerVersionStamp;
                return result;
            });

            return changes;
        }

        #region Pseudo Entities

        private CrmEntityMetadata CheckPseudoEntity(string entityName)
        {
            CrmEntityMetadata metadata = null;
            switch (entityName.ToLower())
            {
                case "entitymetadata":
                    metadata = BuildPseudoEntityMetadata();
                    break;

                case "attributemetadata":
                    metadata = BuildPseudoAttributeMetadata();
                    break;

                case "onetomanyrelationship":
                    metadata = BuildPseudoOneToManyMetadata();
                    break;

                case "manytomanyrelationship":
                    metadata = BuildPseudoManyToManyMetadata();
                    break;

                default:
                    return null;
            }
            metadata.IsPseudo = true;
            return metadata;
        }

        private CrmEntityMetadata BuildPseudoManyToManyMetadata()
        {
            throw new NotImplementedException();
        }

        private CrmEntityMetadata BuildPseudoOneToManyMetadata()
        {
            throw new NotImplementedException();
        }

        private CrmEntityMetadata BuildPseudoAttributeMetadata()
        {
            throw new NotImplementedException();
        }

        private CrmEntityMetadata BuildPseudoEntityMetadata()
        {
            var metadata = new CrmEntityMetadata("entitymetadata");         
          
      
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

        /// <summary>
        /// Ensures the metadata is refreshed and uptodate and returns it.
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public CrmEntityMetadata RefreshEntityMetadata(string entityName)
        {
            bool isPresent = true;
            var result = _Metadata.GetOrAdd(entityName, p =>
                {
                    isPresent = false;

                    var pseudoMetadata = CheckPseudoEntity(entityName);
                    if (pseudoMetadata != null)
                    {
                        return pseudoMetadata;
                    }

                    var metadata = _repository.GetChanges(entityName, null);
                    var entMeta = metadata.EntityMetadata[0];

                    var atts = _metadataConverter.ConvertAttributeInfoList(entMeta.Attributes);
                    var crment = new CrmEntityMetadata(entityName, atts);
                    crment.Timestamp = metadata.ServerVersionStamp;
                    return crment;
                });


            if (result.IsPseudo || !isPresent)
            {
                // it's either not real crm metadata (pseudo) or it wasn't reteived from the cache, 
                // in either of those cases, return it as its currently the latest.
                return result;
            }

            // Check for any changes to this metadata and update it with the deltas.         
            Debug.WriteLine("Refreshing metadata for entity: " + entityName, "Metadata");
            var changes = _repository.GetChanges(entityName, result.Timestamp);
            // update existing metadata..
            var latestEntityMetadata = changes.EntityMetadata[0];

            // Detect new / deleted fields.
            List<AttributeMetadata> modifiedFields = null;
            List<Guid> deletedFields = null;

            if (latestEntityMetadata.HasChanged.GetValueOrDefault())
            {
                modifiedFields = latestEntityMetadata.Attributes.Where(att => att.HasChanged.GetValueOrDefault()).ToList();
                deletedFields = changes.DeletedMetadata != null &&
                                changes.DeletedMetadata.ContainsKey(DeletedMetadataFilters.Attribute)
                                    ? changes.DeletedMetadata[DeletedMetadataFilters.Attribute].ToList()
                                    : null;
                // Work out what was changed..
                // var deletedFields = latestEntityMetadataResponse.DeletedMetadata.Where(att => att.HasChanged.GetValueOrDefault()).ToList();

            }

            // Loop through all metadata items, and add missing change units.
            bool hasSchemaChanges = (modifiedFields != null && modifiedFields.Any()) || (deletedFields != null && deletedFields.Any());
            if (hasSchemaChanges)
            {
                Debug.WriteLine("Updating metadata for entity: " + entityName, "Metadata");
                var modifiedAttInfoList = _metadataConverter.ConvertAttributeInfoList(modifiedFields);
                result.Refresh(modifiedAttInfoList, deletedFields);
            }

            return result;
        }



        #endregion

    }




}
