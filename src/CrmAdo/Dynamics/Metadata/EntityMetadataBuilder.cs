using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;

namespace CrmAdo.Dynamics.Metadata
{
    /// <summary>
    /// Single responsbility: To provide a fluent API for constructing Crm Entity Metadata.
    /// </summary>
    public class EntityMetadataBuilder
    {

        public EntityMetadata Entity { get; set; }

        public StringAttributeMetadata PrimaryAttribute { get; set; }

        public EntityMetadataBuilder(string entityName)
        {
            //Initialise Meatdata
            Entity = new EntityMetadata
                {
                    LogicalName = entityName.ToLower(),
                    SchemaName = entityName,
                    IsActivity = false,
                    IsActivityParty = false,
                    OwnershipType = OwnershipTypes.UserOwned
                };
        }

        public CreateEntityRequest ToCreateEntityRequest()
        {
            // Create the entity and then add in aatributes.
            var createrequest = new CreateEntityRequest();
            createrequest.Entity = Entity;
            createrequest.PrimaryAttribute = PrimaryAttribute;
            return createrequest;
        }

        public EntityMetadataBuilder IsActivity()
        {
            this.Entity.IsActivity = true;
            return this;
        }

        public EntityMetadataBuilder IsActivityParty()
        {
            this.Entity.IsActivityParty = true;
            return this;
        }

        public EntityMetadataBuilder OwnershipType(OwnershipTypes ownershipType)
        {
            this.Entity.OwnershipType = ownershipType;
            return this;
        }

        public EntityMetadataBuilder Description(string description, int lcid = 1033)
        {
            this.Entity.Description = new Label(description, lcid);
            return this;
        }

        public EntityMetadataBuilder DisplayCollectionName(string displayCollectionName, int lcid = 1033)
        {
            this.Entity.DisplayCollectionName = new Label(displayCollectionName, lcid);
            return this;
        }

        public EntityMetadataBuilder DisplayName(string displayName, int lcid = 1033)
        {
            this.Entity.DisplayName = new Label(displayName, lcid);
            return this;
        }

        public EntityMetadataBuilder WithPrimaryAttribute(string schemaName, string displayName, string description,
                                                         AttributeRequiredLevel requiredLevel,
                                                         int maxLength, StringFormat format)
        {
            // Define the primary attribute for the entity
            var newAtt = new StringAttributeMetadata
            {
                SchemaName = schemaName,
                RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel),
                MaxLength = maxLength,
                Format = format,
                DisplayName = new Label(displayName, 1033),
                Description = new Label(description, 1033),
                LogicalName = schemaName.ToLower()
            };
            this.PrimaryAttribute = newAtt;
            return this;
        }

    }

    //public interface ICrmUpdater
    //{
    //    void Update(IOrganizationService orgService);
    //}

    //public class CrmUpdater : ICrmUpdater
    //{

    //    private EntityMetadata _EntityMetadata;
    //    private List<AttributeMetadata> _Attributes;
    //    private StringAttributeMetadata _PrimaryAttribute;

    //    public CrmUpdater(EntityMetadata entityMetadata, StringAttributeMetadata primaryAttribute, List<AttributeMetadata> attributes)
    //    {
    //        _EntityMetadata = entityMetadata;
    //        _PrimaryAttribute = primaryAttribute;
    //        _Attributes = attributes;
    //    }

    //    public void Update(IOrganizationService orgService)
    //    {
    //        // This is the metadata for the all of the atributes we defined:
    //        var attributesMetadata = _Attributes;
    //        // This is the metadata for the entity we defined:
    //        var entityMetadata = _EntityMetadata;

    //        // Create the entity and then add in aatributes.
    //        var createrequest = new CreateEntityRequest();
    //        createrequest.Entity = entityMetadata;
    //        createrequest.PrimaryAttribute = _PrimaryAttribute;

    //        var createResponse = (CreateEntityResponse)orgService.Execute(createrequest);

    //        try
    //        {
    //            foreach (var attributeMetadata in attributesMetadata)
    //            {
    //                var createAttributeRequest = new CreateAttributeRequest
    //                {
    //                    EntityName = entityMetadata.LogicalName,
    //                    Attribute = attributeMetadata
    //                };


    //                var createAttResponse = (CreateAttributeResponse)orgService.Execute(createAttributeRequest);
    //            }

    //        }
    //        catch (Exception ex)
    //        {
    //            throw;
    //        }

    //    }
    //}
}