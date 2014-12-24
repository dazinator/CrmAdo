using Microsoft.Xrm.Sdk.Metadata;
using System;
using CrmAdo.Dynamics.Metadata;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace CrmAdo.Dynamics
{
    public partial class EntityMetadataDataSet
    {
        public void AddSdkMetadata(EntityMetadata entity, string timestamp = null)
        {
            var row = this.EntityMetadata.NewEntityMetadataRow();
            row.MetadataId = entity.MetadataId.GetValueOrDefault();

            if (entity.ActivityTypeMask == null)
            {
                row.SetActivityTypeMaskNull();
            }
            else
            {
                row.ActivityTypeMask = entity.ActivityTypeMask.Value;
            }

            if (entity.AutoCreateAccessTeams == null)
            {
                row.SetAutoCreateAccessTeamsNull();
            }
            else
            {
                row.AutoCreateAccessTeams = entity.AutoCreateAccessTeams.Value;
            }

            if (entity.AutoRouteToOwnerQueue == null)
            {
                row.SetAutoRouteToOwnerQueueNull();
            }
            else
            {
                row.AutoRouteToOwnerQueue = entity.AutoRouteToOwnerQueue.Value;
            }

            if (entity.CanBeInManyToMany == null)
            {
                row.SetCanBeInManyToManyNull();
            }
            else
            {
                row.CanBeInManyToMany = entity.CanBeInManyToMany.Value;
            }

            if (entity.CanBePrimaryEntityInRelationship == null)
            {
                row.SetCanBePrimaryEntityInRelationshipNull();
            }
            else
            {
                row.CanBePrimaryEntityInRelationship = entity.CanBePrimaryEntityInRelationship.Value;
            }

            if (entity.CanBeRelatedEntityInRelationship == null)
            {
                row.SetCanBeRelatedEntityInRelationshipNull();
            }
            else
            {
                row.CanBeRelatedEntityInRelationship = entity.CanBeRelatedEntityInRelationship.Value;
            }

            if (entity.CanCreateAttributes == null)
            {
                row.SetCanCreateAttributesNull();
            }
            else
            {
                row.CanCreateAttributes = entity.CanCreateAttributes.Value;
            }

            if (entity.CanCreateCharts == null)
            {
                row.SetCanCreateChartsNull();
            }
            else
            {
                row.CanCreateCharts = entity.CanCreateCharts.Value;
            }

            if (entity.CanCreateForms == null)
            {
                row.SetCanCreateFormsNull();
            }
            else
            {
                row.CanCreateForms = entity.CanCreateForms.Value;
            }

            if (entity.CanCreateViews == null)
            {
                row.SetCanCreateViewsNull();
            }
            else
            {
                row.CanCreateViews = entity.CanCreateViews.Value;
            }

            if (entity.CanModifyAdditionalSettings == null)
            {
                row.SetCanModifyAdditionalSettingsNull();
            }
            else
            {
                row.CanModifyAdditionalSettings = entity.CanModifyAdditionalSettings.Value;
            }

            if (entity.CanTriggerWorkflow == null)
            {
                row.SetCanTriggerWorkflowNull();
            }
            else
            {
                row.CanTriggerWorkflow = entity.CanTriggerWorkflow.Value;
            }

            if (entity.Description == null)
            {
                row.SetDescriptionNull();
            }
            else
            {

                if (entity.Description.UserLocalizedLabel == null)
                {
                    row.SetDescriptionNull();
                }
                else
                {
                    row.Description = entity.Description.UserLocalizedLabel.Label;
                }
            }

            if (entity.DisplayName == null)
            {
                row.SetDisplayNameNull();
            }
            else
            {

                if (entity.DisplayName.UserLocalizedLabel == null)
                {
                    row.SetDisplayNameNull();
                }
                else
                {
                    row.DisplayName = entity.DisplayName.UserLocalizedLabel.Label;
                }
            }

            if (entity.DisplayCollectionName == null)
            {
                row.SetDisplayCollectionNameNull();
            }
            else
            {

                if (entity.DisplayCollectionName.UserLocalizedLabel == null)
                {
                    row.SetDisplayCollectionNameNull();
                }
                else
                {
                    row.DisplayCollectionName = entity.DisplayCollectionName.UserLocalizedLabel.Label;
                }
            }

            if (entity.HasChanged == null)
            {
                row.SetHasChangedNull();
            }
            else
            {
                row.HasChanged = entity.HasChanged.Value;
            }

            row.IconLargeName = entity.IconLargeName;
            row.IconMediumName = entity.IconMediumName;
            row.IconSmallName = entity.IconSmallName;
            row.IntroducedVersion = entity.IntroducedVersion;

            if (entity.IsActivity == null)
            {
                row.SetIsActivityNull();
            }
            else
            {
                row.IsActivity = entity.IsActivity.Value;
            }

            if (entity.IsActivityParty == null)
            {
                row.SetIsActivityPartyNull();
            }
            else
            {
                row.IsActivityParty = entity.IsActivityParty.Value;
            }

            if (entity.IsAIRUpdated == null)
            {
                row.SetIsAIRUpdatedNull();
            }
            else
            {
                row.IsAIRUpdated = entity.IsAIRUpdated.Value;
            }

            if (entity.IsAuditEnabled == null)
            {
                row.SetIsAuditEnabledNull();
            }
            else
            {
                row.IsAuditEnabled = entity.IsAuditEnabled.Value;
            }

            if (entity.IsAvailableOffline == null)
            {
                row.SetIsAvailableOfflineNull();
            }
            else
            {
                row.IsAvailableOffline = entity.IsAvailableOffline.Value;
            }

            if (entity.IsBusinessProcessEnabled == null)
            {
                row.SetIsBusinessProcessEnabledNull();
            }
            else
            {
                row.IsBusinessProcessEnabled = entity.IsBusinessProcessEnabled.Value;
            }

            if (entity.IsChildEntity == null)
            {
                row.SetIsChildEntityNull();
            }
            else
            {
                row.IsChildEntity = entity.IsChildEntity.Value;
            }


            if (entity.IsConnectionsEnabled == null)
            {
                row.SetIsConnectionsEnabledNull();
            }
            else
            {
                row.IsConnectionsEnabled = entity.IsConnectionsEnabled.Value;
            }

            if (entity.IsCustomEntity == null)
            {
                row.SetIsCustomEntityNull();
            }
            else
            {
                row.IsCustomEntity = entity.IsCustomEntity.Value;
            }

            if (entity.IsCustomizable == null)
            {
                row.SetIsCustomizableNull();
            }
            else
            {
                row.IsCustomizable = entity.IsCustomizable.Value;
            }

            if (entity.IsDocumentManagementEnabled == null)
            {
                row.SetIsDocumentManagementEnabledNull();
            }
            else
            {
                row.IsDocumentManagementEnabled = entity.IsDocumentManagementEnabled.Value;
            }

            if (entity.IsDuplicateDetectionEnabled == null)
            {
                row.SetIsDuplicateDetectionEnabledNull();
            }
            else
            {
                row.IsDuplicateDetectionEnabled = entity.IsDuplicateDetectionEnabled.Value;
            }

            if (entity.IsEnabledForCharts == null)
            {
                row.SetIsEnabledForChartsNull();
            }
            else
            {
                row.IsEnabledForCharts = entity.IsEnabledForCharts.Value;
            }

            if (entity.IsEnabledForTrace == null)
            {
                row.SetIsEnabledForTraceNull();
            }
            else
            {
                row.IsEnabledForTrace = entity.IsEnabledForTrace.Value;
            }

            if (entity.IsImportable == null)
            {
                row.SetIsImportableNull();
            }
            else
            {
                row.IsImportable = entity.IsImportable.Value;
            }

            if (entity.IsIntersect == null)
            {
                row.SetIsIntersectNull();
            }
            else
            {
                row.IsIntersect = entity.IsIntersect.Value;
            }

            if (entity.IsMailMergeEnabled == null)
            {
                row.SetIsMailMergeEnabledNull();
            }
            else
            {
                row.IsMailMergeEnabled = entity.IsMailMergeEnabled.Value;
            }


            if (entity.IsManaged == null)
            {
                row.SetIsManagedNull();
            }
            else
            {
                row.IsManaged = entity.IsManaged.Value;
            }

            if (entity.IsMappable == null)
            {
                row.SetIsMappableNull();
            }
            else
            {
                row.IsMappable = entity.IsMappable.Value;
            }

            row.IsPseudo = false;

            if (entity.IsQuickCreateEnabled == null)
            {
                row.SetIsQuickCreateEnabledNull();
            }
            else
            {
                row.IsQuickCreateEnabled = entity.IsQuickCreateEnabled.Value;
            }

            if (entity.IsReadingPaneEnabled == null)
            {
                row.SetIsReadingPaneEnabledNull();
            }
            else
            {
                row.IsReadingPaneEnabled = entity.IsReadingPaneEnabled.Value;
            }


            if (entity.IsReadOnlyInMobileClient == null)
            {
                row.SetIsReadOnlyInMobileClientNull();
            }
            else
            {
                row.IsReadOnlyInMobileClient = entity.IsReadOnlyInMobileClient.Value;
            }

            if (entity.IsRenameable == null)
            {
                row.SetIsRenameableNull();
            }
            else
            {
                row.IsRenameable = entity.IsRenameable.Value;
            }

            if (entity.IsValidForAdvancedFind == null)
            {
                row.SetIsValidForAdvancedFindNull();
            }
            else
            {
                row.IsValidForAdvancedFind = entity.IsValidForAdvancedFind.Value;
            }

            if (entity.IsValidForQueue == null)
            {
                row.SetIsValidForQueueNull();
            }
            else
            {
                row.IsValidForQueue = entity.IsValidForQueue.Value;
            }

            if (entity.IsVisibleInMobile == null)
            {
                row.SetIsVisibleInMobileNull();
            }
            else
            {
                row.IsVisibleInMobile = entity.IsVisibleInMobile.Value;
            }

            if (entity.IsVisibleInMobileClient == null)
            {
                row.SetIsVisibleInMobileClientNull();
            }
            else
            {
                row.IsVisibleInMobileClient = entity.IsVisibleInMobileClient.Value;
            }

            if (entity.LogicalName == null)
            {
                row.SetLogicalNameNull();
            }
            else
            {
                row.LogicalName = entity.LogicalName;
            }

            if (entity.ObjectTypeCode == null)
            {
                row.SetObjectTypeCodeNull();
            }
            else
            {
                row.ObjectTypeCode = entity.ObjectTypeCode.Value;
            }

            if (entity.OwnershipType == null)
            {
                row.SetOwnershipTypeNull();
            }
            else
            {
                row.OwnershipType = (int)entity.OwnershipType.Value;
            }

            if (entity.PrimaryIdAttribute == null)
            {
                row.SetPrimaryIdAttributeNull();
            }
            else
            {
                row.PrimaryIdAttribute = entity.PrimaryIdAttribute;
            }

            if (entity.PrimaryImageAttribute == null)
            {
                row.SetPrimaryImageAttributeNull();
            }
            else
            {
                row.PrimaryImageAttribute = entity.PrimaryImageAttribute;
            }

            if (entity.PrimaryNameAttribute == null)
            {
                row.SetPrimaryNameAttributeNull();
            }
            else
            {
                row.PrimaryNameAttribute = entity.PrimaryNameAttribute;
            }

            if (entity.RecurrenceBaseEntityLogicalName == null)
            {
                row.SetRecurrenceBaseEntityLogicalNameNull();
            }
            else
            {
                row.RecurrenceBaseEntityLogicalName = entity.RecurrenceBaseEntityLogicalName;
            }

            if (entity.ReportViewName == null)
            {
                row.SetReportViewNameNull();
            }
            else
            {
                row.ReportViewName = entity.ReportViewName;
            }

            if (entity.SchemaName == null)
            {
                row.SetSchemaNameNull();
            }
            else
            {
                row.SchemaName = entity.SchemaName;
            }

            if (timestamp == null)
            {
                row.SetTimestampNull();
            }
            else
            {
                row.Timestamp = timestamp;
            }

            this.EntityMetadata.AddEntityMetadataRow(row);

            foreach (var item in entity.Attributes)
            {
                this.AttributeMetadata.AddFromSdkAttribute(row, item);
            }

        }

        //public DataTable BuildMetadataResultDataTable(List<ColumnMetadata> columns)
        //{

        //    var entCols = columns.Where(c => c.AttributeMetadata.LogicalName == "entitymetadata");
        //    var attCols = columns.Where(c => c.AttributeMetadata.LogicalName == "attributemetadata");

        //    bool hasEntCols = entCols.Any();
        //    bool hasAttCols = attCols.Any();

        //    DataTable table = new DataTable();
        //    var totalColumnsCount = entCols.Count() + attCols.Count();

        //    var props = (from e in columns
        //                               join PropertyDescriptor p in TypeDescriptor.GetProperties(typeof(EntityMetadataRow))
        //                               on e.AttributeMetadata.LogicalName.ToLowerInvariant() equals p.Name.ToLowerInvariant()
        //                               select p).ToArray();


        //    PropertyDescriptor[] entityProps;
        //    PropertyDescriptor[] attributeProps;


        //    if (hasEntCols)
        //    {
        //        entityProps = (from e in entCols
        //                       join PropertyDescriptor p in TypeDescriptor.GetProperties(typeof(EntityMetadataRow))
        //                       on e.AttributeMetadata.LogicalName.ToLowerInvariant() equals p.Name.ToLowerInvariant() 
        //                       select p).ToArray();

        //        for (int i = 0; i < entityProps.Length; i++)
        //        {
        //            PropertyDescriptor prop = entityProps[i];
        //            table.Columns.Add(prop.Name, prop.PropertyType);
        //        }
        //    }
        //    else
        //    {
        //        entityProps = new PropertyDescriptor[0];
        //    }

        //    if (hasAttCols)
        //    {
        //        attributeProps = (from e in entCols
        //                       join PropertyDescriptor p in TypeDescriptor.GetProperties(typeof(AttributeMetadataRow))
        //                       on e.AttributeMetadata.LogicalName.ToLowerInvariant() equals p.Name.ToLowerInvariant() 
        //                       select p).ToArray();

        //        for (int i = 0; i < attributeProps.Length; i++)
        //        {
        //            PropertyDescriptor prop = attributeProps[i];
        //            table.Columns.Add(prop.Name, prop.PropertyType);
        //        }
        //    }
        //    else
        //    {
        //        attributeProps = new PropertyDescriptor[0];
        //    }

        //    object[] rowValues = new object[totalColumnsCount];

        //    foreach (var item in this.EntityMetadata)
        //    {
        //        if (hasEntCols)
        //        {
        //            for (int i = 0; i < entityProps.Length; i++)
        //            {
        //                rowValues[i] = entityProps[i].GetValue(item);
        //            }
        //        }

        //        if (hasAttCols)
        //        {
        //            bool isFirst = true;

        //            var attItems = item.GetAttributeMetadataRows();
        //            int attColumnStart = entityProps.Length - 1;

        //            foreach (var attItem in attItems)
        //            {
        //                for (int i = entityProps.Length - 1; i < entityProps.Length + attributeProps.Length; i++)
        //                {
        //                    rowValues[i] = entityProps[i].GetValue(item);
        //                }
        //                isFirst = false;
        //            }
        //        }





        //        table.Rows.Add(values);
        //    }



        //    return table;
        //}

        partial class AttributeMetadataDataTable
        {
            public void AddFromSdkAttribute(EntityMetadataRow parent, AttributeMetadata attribute)
            {
                var row = this.NewAttributeMetadataRow();
                row.EntityMetadataTableRow = parent;
                row.EntityMetadataId = parent.MetadataId;
                row.MetadataId = attribute.MetadataId.GetValueOrDefault();
                row.AttributeOf = attribute.AttributeOf;

                if (attribute.AttributeType == null)
                {
                    row.SetAttributeOfNull();
                }
                else
                {
                    row.AttributeType = (int)attribute.AttributeType.Value;
                }

                row.AttributeTypeName = attribute.AttributeTypeName.Value;

                if (attribute.CanBeSecuredForCreate == null)
                {
                    row.SetCanBeSecuredForCreateNull();
                }
                else
                {
                    row.CanBeSecuredForCreate = attribute.CanBeSecuredForCreate.Value;
                }

                if (attribute.CanBeSecuredForRead == null)
                {
                    row.SetCanBeSecuredForReadNull();
                }
                else
                {
                    row.CanBeSecuredForRead = attribute.CanBeSecuredForRead.Value;
                }

                if (attribute.CanBeSecuredForUpdate == null)
                {
                    row.SetCanBeSecuredForUpdateNull();
                }
                else
                {
                    row.CanBeSecuredForUpdate = attribute.CanBeSecuredForUpdate.Value;
                }

                if (attribute.CanModifyAdditionalSettings == null)
                {
                    row.SetCanModifyAdditionalSettingsNull();
                }
                else
                {
                    row.CanModifyAdditionalSettings = attribute.CanModifyAdditionalSettings.Value;
                }

                if (attribute.ColumnNumber == null)
                {
                    row.SetColumnNumberNull();
                }
                else
                {
                    row.ColumnNumber = attribute.ColumnNumber.Value;
                }

                row.DeprecatedVersion = attribute.DeprecatedVersion;


                if (attribute.Description == null)
                {
                    row.SetDescriptionNull();
                }
                else
                {

                    if (attribute.Description.UserLocalizedLabel == null)
                    {
                        row.SetDescriptionNull();
                    }
                    else
                    {
                        row.Description = attribute.Description.UserLocalizedLabel.Label;
                    }
                }

                if (attribute.DisplayName == null)
                {
                    row.SetDisplayNameNull();
                }
                else
                {

                    if (attribute.DisplayName.UserLocalizedLabel == null)
                    {
                        row.SetDisplayNameNull();
                    }
                    else
                    {
                        row.DisplayName = attribute.DisplayName.UserLocalizedLabel.Label;
                    }
                }

                row.EntityLogicalName = attribute.EntityLogicalName;
                row.IntroducedVersion = attribute.IntroducedVersion;

                if (attribute.IsAuditEnabled == null)
                {
                    row.SetIsAuditEnabledNull();
                }
                else
                {
                    row.IsAuditEnabled = attribute.IsAuditEnabled.Value;
                }

                if (attribute.IsCustomAttribute == null)
                {
                    row.SetIsCustomAttributeNull();
                }
                else
                {
                    row.IsCustomAttribute = attribute.IsCustomAttribute.Value;
                }

                if (attribute.IsCustomizable == null)
                {
                    row.SetIsCustomizableNull();
                }
                else
                {
                    row.IsCustomizable = attribute.IsCustomizable.Value;
                }

                if (attribute.IsManaged == null)
                {
                    row.SetIsManagedNull();
                }
                else
                {
                    row.IsManaged = attribute.IsManaged.Value;
                }

                if (attribute.IsPrimaryId == null)
                {
                    row.SetIsPrimaryIdNull();
                }
                else
                {
                    row.IsPrimaryId = attribute.IsPrimaryId.Value;
                }

                if (attribute.IsPrimaryName == null)
                {
                    row.SetIsPrimaryNameNull();
                }
                else
                {
                    row.IsPrimaryName = attribute.IsPrimaryName.Value;
                }

                row.IsPseudo = false; // attribute.IsPseudo;


                if (attribute.IsRenameable == null)
                {
                    row.SetIsRenameableNull();
                }
                else
                {
                    row.IsRenameable = attribute.IsRenameable.Value;
                }

                if (attribute.IsSecured == null)
                {
                    row.SetIsSecuredNull();
                }
                else
                {
                    row.IsSecured = attribute.IsSecured.Value;
                }

                if (attribute.IsValidForAdvancedFind == null)
                {
                    row.SetIsValidForAdvancedFindNull();
                }
                else
                {
                    row.IsValidForAdvancedFind = attribute.IsValidForAdvancedFind.Value;
                }

                if (attribute.IsValidForCreate == null)
                {
                    row.SetIsValidForCreateNull();
                }
                else
                {
                    row.IsValidForCreate = attribute.IsValidForCreate.Value;
                }

                if (attribute.IsValidForRead == null)
                {
                    row.SetIsValidForReadNull();
                }
                else
                {
                    row.IsValidForRead = attribute.IsValidForRead.Value;
                }

                if (attribute.IsValidForUpdate == null)
                {
                    row.SetIsValidForUpdateNull();
                }
                else
                {
                    row.IsValidForUpdate = attribute.IsValidForUpdate.Value;
                }

                if (attribute.LinkedAttributeId == null)
                {
                    row.SetLinkedAttributeIdNull();
                }
                else
                {
                    row.LinkedAttributeId = attribute.LinkedAttributeId.Value;
                }

                if (attribute.LogicalName == null)
                {
                    row.SetLogicalNameNull();
                }
                else
                {
                    row.LogicalName = attribute.LogicalName;
                }

                var attType = row.GetAttributeTypeCode();
                switch (attType)
                {
                    case AttributeTypeCode.Decimal:
                        var decAtt = (DecimalAttributeMetadata)attribute;
                        row.MinValue = decAtt.MinValue;
                        row.MaxValue = decAtt.MinValue;
                        row.Precision = decAtt.Precision.GetValueOrDefault();
                        row.SetPrecisionSourceNull();
                        break;
                    case AttributeTypeCode.Double:
                        var doubleAtt = (DoubleAttributeMetadata)attribute;
                        row.MinValue = doubleAtt.MinValue;
                        row.MaxValue = doubleAtt.MinValue;
                        row.Precision = doubleAtt.Precision.GetValueOrDefault();
                        row.SetPrecisionSourceNull();
                        break;
                    case AttributeTypeCode.Money:
                        var moneyAtt = (MoneyAttributeMetadata)attribute;
                        row.MinValue = moneyAtt.MinValue;
                        row.MaxValue = moneyAtt.MinValue;
                        row.Precision = moneyAtt.Precision.GetValueOrDefault();
                        row.PrecisionSource = moneyAtt.PrecisionSource.GetValueOrDefault();
                        break;
                    default:
                        row.SetPrecisionSourceNull();
                        row.SetPrecisionNull();
                        row.SetMaxValueNull();
                        row.SetMinValueNull();
                        break;
                }

                if (attribute.RequiredLevel == null)
                {
                    row.SetRequiredLevelNull();
                }
                else
                {
                    row.RequiredLevel = (int)attribute.RequiredLevel.Value;
                }

                if (attribute.SchemaName == null)
                {
                    row.SetSchemaNameNull();
                }
                else
                {
                    row.SchemaName = attribute.SchemaName;
                }

                this.AddAttributeMetadataRow(row);
            }
        }

        partial class AttributeMetadataRow
        {
            public virtual string GetSqlDataTypeName()
            {
                var attTypeCode = GetAttributeTypeCode();
                return attTypeCode.GetSqlDataTypeName(this.AttributeTypeName);
            }

            public virtual Type GetFieldType()
            {
                var attTypeCode = GetAttributeTypeCode();
                return attTypeCode.GetCrmAgnosticType();
            }

            public virtual AttributeTypeCode GetAttributeTypeCode()
            {
                return (AttributeTypeCode)this.AttributeType;
            }

            /// <summary>
            /// Gets the sql datatype name for the attribute type. 
            /// </summary>
            /// <param name="metadata"></param>
            /// <returns></returns>
            public static string GetSqlDataTypeName(AttributeTypeCode attTypeCode, string attributeTypeDisplayName)
            {

                switch (attTypeCode)
                {
                    case AttributeTypeCode.String:
                    case AttributeTypeCode.Memo:
                        return "nvarchar";
                    case AttributeTypeCode.Lookup:
                    case AttributeTypeCode.Owner:
                        return "uniqueidentifier";
                    case AttributeTypeCode.Virtual:
                        if (attributeTypeDisplayName != null && attributeTypeDisplayName == AttributeTypeDisplayName.ImageType.Value)
                        {
                            return "image";
                        }
                        return "nvarchar";
                    case AttributeTypeCode.Double:
                        return "float";
                    case AttributeTypeCode.State:
                    case AttributeTypeCode.Status:
                    case AttributeTypeCode.Picklist:
                        return "integer";
                    case AttributeTypeCode.Boolean:
                        return "bit";
                    default:
                        return attributeTypeDisplayName.ToString();
                }
            }

            #region Sql Precision And Scale

            /// <summary>
            /// Gets the sql precision for the crm decimal attribute. 
            /// </summary>
            /// <param name="metadata"></param>
            /// <returns></returns>
            private int GetMaxSupportedSqlPrecision()
            {
                // = 12 + max scale of 10 = 22 in total.    
                switch (this.GetAttributeTypeCode())
                {
                    case AttributeTypeCode.Decimal:
                        var crmDecimalPrecision = Math.Max(Math.Truncate(Math.Abs(DecimalAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DecimalAttributeMetadata.MinSupportedValue)).ToString().Length);
                        //  int crmPrecision = Math.Max(Math.Truncate(DecimalAttributeMetadata.MaxSupportedValue).ToString().Length, Math.Truncate(DecimalAttributeMetadata.MinSupportedValue).ToString().Length);
                        return crmDecimalPrecision + DecimalAttributeMetadata.MaxSupportedPrecision;
                    case AttributeTypeCode.Double:
                        var crmDoublePrecision = Math.Max(Math.Truncate(Math.Abs(DoubleAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DoubleAttributeMetadata.MinSupportedValue)).ToString().Length);
                        return crmDoublePrecision + DoubleAttributeMetadata.MaxSupportedPrecision;

                    case AttributeTypeCode.Money:
                        var crmMoneyPrecision = Math.Max(Math.Truncate(Math.Abs(MoneyAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(MoneyAttributeMetadata.MinSupportedValue)).ToString().Length);
                        return crmMoneyPrecision + MoneyAttributeMetadata.MaxSupportedPrecision;

                    default:
                        return 0;
                }

            }

            /// <summary>
            /// Gets the sql precision for the crm decimal attribute. 
            /// </summary>
            /// <param name="metadata"></param>
            /// <returns></returns>
            public bool IsSqlPrecisionSupported(int precision, int scale)
            {
                if (precision < scale)
                {
                    throw new ArgumentOutOfRangeException("precision must be equal to or greater than scale.");
                }

                switch (this.GetAttributeTypeCode())
                {
                    case AttributeTypeCode.Decimal:

                        if (scale < DecimalAttributeMetadata.MinSupportedPrecision || scale > DecimalAttributeMetadata.MaxSupportedPrecision)
                        {
                            return false;
                        }
                        int crmMaxValueLengthWithoutPrecision = Math.Max(DecimalAttributeMetadata.MinSupportedValue.ToString().Length, DecimalAttributeMetadata.MaxSupportedValue.ToString().Length);
                        if (precision - scale > crmMaxValueLengthWithoutPrecision)
                        {
                            return false;
                        }
                        return true;

                    case AttributeTypeCode.Double:

                        if (scale < DoubleAttributeMetadata.MinSupportedPrecision || scale > DoubleAttributeMetadata.MaxSupportedPrecision)
                        {
                            return false;
                        }
                        int crmMaxDoubleValueLengthWithoutPrecision = Math.Max(DoubleAttributeMetadata.MinSupportedValue.ToString().Length, DoubleAttributeMetadata.MaxSupportedValue.ToString().Length);
                        if (precision - scale > crmMaxDoubleValueLengthWithoutPrecision)
                        {
                            return false;
                        }
                        return true;

                    case AttributeTypeCode.Money:

                        if (scale < MoneyAttributeMetadata.MinSupportedPrecision || scale > MoneyAttributeMetadata.MaxSupportedPrecision)
                        {
                            return false;
                        }
                        int crmMaxMoneyValueLengthWithoutPrecision = Math.Max(MoneyAttributeMetadata.MinSupportedValue.ToString().Length, MoneyAttributeMetadata.MaxSupportedValue.ToString().Length);
                        if (precision - scale > crmMaxMoneyValueLengthWithoutPrecision)
                        {
                            return false;
                        }
                        return true;


                    default:
                        return false;
                }

            }

            /// <summary>
            /// Gets the default sql precision for the crm decimal attribute. 
            /// </summary>
            /// <param name="metadata"></param>
            /// <returns></returns>
            public int DefaultSqlPrecision()
            {

                switch (this.GetAttributeTypeCode())
                {
                    case AttributeTypeCode.Decimal:

                        var decPrecision = Math.Max(Math.Truncate(Math.Abs(DecimalAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DecimalAttributeMetadata.MinSupportedValue)).ToString().Length);
                        return decPrecision + DefaultSqlScale();

                    case AttributeTypeCode.Double:

                        var doublePrecision = Math.Max(Math.Truncate(Math.Abs(DoubleAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DoubleAttributeMetadata.MinSupportedValue)).ToString().Length);
                        return doublePrecision + DefaultSqlScale();

                    case AttributeTypeCode.Money:

                        var moneyPrecision = Math.Max(Math.Truncate(Math.Abs(MoneyAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(MoneyAttributeMetadata.MinSupportedValue)).ToString().Length);
                        return moneyPrecision + DefaultSqlScale();

                    default:
                        return 0;
                }


            }

            /// <summary>
            /// Gets the default sql scale for the crm decimal attribute. 
            /// </summary>
            /// <param name="metadata"></param>
            /// <returns></returns>
            public int DefaultSqlScale()
            {

                switch (this.GetAttributeTypeCode())
                {
                    case AttributeTypeCode.Decimal:

                        int decScale = DecimalAttributeMetadata.MinSupportedPrecision;
                        return decScale;

                    case AttributeTypeCode.Double:

                        int doubleScale = DoubleAttributeMetadata.MinSupportedPrecision;
                        return doubleScale;

                    case AttributeTypeCode.Money:

                        int moneyScale = MoneyAttributeMetadata.MinSupportedPrecision;
                        return moneyScale;

                    default:
                        return 0;
                }
            }

            /// <summary>
            /// Sets the decimal size according to the sql precision and scale arguments. 
            /// </summary>
            /// <param name="metadata"></param>
            /// <returns></returns>
            public bool SetFromSqlPrecisionAndScale(int precision, int scale)
            {
                if (precision < scale)
                {
                    throw new ArgumentOutOfRangeException("precision must be equal to or greater than scale.");
                }

                switch (this.GetAttributeTypeCode())
                {
                    case AttributeTypeCode.Decimal:

                        if (scale < DecimalAttributeMetadata.MinSupportedPrecision || scale > DecimalAttributeMetadata.MaxSupportedPrecision)
                        {
                            throw new ArgumentOutOfRangeException("scale is not within min and max crm values.");
                        }

                        // = 12
                        var crmDecimalMaxValueLengthWithoutPrecision = Math.Max(Math.Truncate(Math.Abs(DecimalAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DecimalAttributeMetadata.MinSupportedValue)).ToString().Length);
                        if (precision - scale > crmDecimalMaxValueLengthWithoutPrecision)
                        {
                            throw new ArgumentOutOfRangeException("The precision is greater than the maximum value crm will allow.");
                        }

                        // need to set appropriate min and max values.
                        // If the precision is equal to the max precision allowed, then set min and max values allowed. 
                        if (precision == crmDecimalMaxValueLengthWithoutPrecision)
                        {
                            this.MinValue = (decimal)DecimalAttributeMetadata.MinSupportedValue;
                            this.MaxValue = (decimal)DecimalAttributeMetadata.MaxSupportedValue;
                        }
                        else
                        {
                            // the min value should be a series of 9's to the specified precision and scale.
                            var maxNumberBuilder = new StringBuilder();
                            for (int i = 0; i < precision - scale; i++)
                            {
                                maxNumberBuilder.Append("9");
                            }
                            if (scale > 0)
                            {
                                maxNumberBuilder.Append(".");
                                for (int i = 0; i < scale; i++)
                                {
                                    maxNumberBuilder.Append("9");
                                }
                            }

                            var maxNumber = decimal.Parse(maxNumberBuilder.ToString());
                            this.MaxValue = maxNumber;
                            this.MinValue = -maxNumber;

                        }
                        break;

                    case AttributeTypeCode.Double:

                        if (scale < DoubleAttributeMetadata.MinSupportedPrecision || scale > DoubleAttributeMetadata.MaxSupportedPrecision)
                        {
                            throw new ArgumentOutOfRangeException("scale is not within min and max crm values.");
                        }

                        var crmDoubleMaxValueLengthWithoutPrecision = Math.Max(Math.Truncate(Math.Abs(DoubleAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(DoubleAttributeMetadata.MinSupportedValue)).ToString().Length);
                        if (precision - scale > crmDoubleMaxValueLengthWithoutPrecision)
                        {
                            throw new ArgumentOutOfRangeException("The precision is greater than the maximum value crm will allow.");
                        }

                        // need to set appropriate min and max values.
                        // If the precision is equal to the max precision allowed, then set min and max values allowed. 
                        if (precision == crmDoubleMaxValueLengthWithoutPrecision)
                        {
                            this.MinValue = (double)DoubleAttributeMetadata.MinSupportedValue;
                            this.MaxValue = (double)DoubleAttributeMetadata.MaxSupportedValue;
                        }
                        else
                        {
                            // the min value should be a series of 9's to the specified precision and scale.
                            var maxNumberBuilder = new StringBuilder();
                            for (int i = 0; i < precision - scale; i++)
                            {
                                maxNumberBuilder.Append("9");
                            }
                            if (scale > 0)
                            {
                                maxNumberBuilder.Append(".");
                                for (int i = 0; i < scale; i++)
                                {
                                    maxNumberBuilder.Append("9");
                                }
                            }

                            var maxNumber = double.Parse(maxNumberBuilder.ToString());
                            this.MaxValue = maxNumber;
                            this.MinValue = -maxNumber;
                        }
                        break;


                    case AttributeTypeCode.Money:

                        if (scale < MoneyAttributeMetadata.MinSupportedPrecision || scale > MoneyAttributeMetadata.MaxSupportedPrecision)
                        {
                            throw new ArgumentOutOfRangeException("scale is not within min and max crm values.");
                        }

                        var crmMoneyMaxValueLengthWithoutPrecision = Math.Max(Math.Truncate(Math.Abs(MoneyAttributeMetadata.MaxSupportedValue)).ToString().Length, Math.Truncate(Math.Abs(MoneyAttributeMetadata.MinSupportedValue)).ToString().Length);
                        if (precision - scale > crmMoneyMaxValueLengthWithoutPrecision)
                        {
                            throw new ArgumentOutOfRangeException("The precision is greater than the maximum value crm will allow.");
                        }

                        // need to set appropriate min and max values.
                        // If the precision is equal to the max precision allowed, then set min and max values allowed. 
                        if (precision == crmMoneyMaxValueLengthWithoutPrecision)
                        {
                            this.MinValue = (double)MoneyAttributeMetadata.MinSupportedValue;
                            this.MaxValue = (double)MoneyAttributeMetadata.MaxSupportedValue;
                        }
                        else
                        {
                            // the min value should be a series of 9's to the specified precision and scale.
                            var maxNumberBuilder = new StringBuilder();
                            for (int i = 0; i < precision - scale; i++)
                            {
                                maxNumberBuilder.Append("9");
                            }
                            if (scale > 0)
                            {
                                maxNumberBuilder.Append(".");
                                for (int i = 0; i < scale; i++)
                                {
                                    maxNumberBuilder.Append("9");
                                }
                            }

                            var maxNumber = double.Parse(maxNumberBuilder.ToString());
                            this.MaxValue = maxNumber;
                            this.MinValue = -maxNumber;
                        }

                        // finallty, as we are setting precision and scale explicitly, the precision source should be set to honour our precision.
                        //When the PrecisionSource is set to zero (0), the MoneyAttributeMetadata.Precision value is used.
                        //When the PrecisionSource is set to one (1), the Organization.PricingDecimalPrecision value is used.
                        //When the PrecisionSource is set to two (2), the TransactionCurrency.CurrencyPrecision value is used.
                        this.PrecisionSource = 0;
                        break;
                    default:
                        this.SetPrecisionNull();
                        this.SetPrecisionSourceNull();
                        this.SetMinValueNull();
                        this.SetMaxValueNull();
                        break;
                }

                this.Precision = scale;
                return true;

            }

            #endregion

        }

        partial class EntityMetadataDataTable
        {
            //public EntityMetadataDataTable()
            //{
            //    ReaderWriterLock = new ReaderWriterLockSlim();
            //}

            //public ReaderWriterLockSlim ReaderWriterLock { get; set; }


        }

        partial class EntityMetadataRow
        {
            ///// <summary>
            ///// This lock is taken when the metadata "Refresh" method is run, as during that time the object can be modified with the latest updates.
            ///// </summary>
            //private object _Lock = new object();

            //public void Refresh(EntityMetadata modified, List<Guid> deletedAttributes)
            //{
            //    var locker = this.tableEntityMetadata.ReaderWriterLock;

            //    try
            //    {
            //        locker.EnterWriteLock();

            //        var children = this.GetAttributeMetadataRows();

            //        var forDelete = (from a in children
            //                         join ar in deletedAttributes on a.MetadataId equals ar
            //                         select a).AsEnumerable();

            //        foreach (var item in forDelete)
            //        {
            //            //   var attTable = this.tableEntityMetadata.ChildRelations["FK_EntityMetadataTable_AttributeMetadataTable"].ChildTable as AttributeMetadataDataTable;
            //            item.Delete();
            //            //  attTable.RemoveAttributeMetadataRow(item);       
            //        }

            //        var forUpdate = (from a in children
            //                         join ar in modified.Attributes on a.MetadataId equals ar.MetadataId
            //                         select a).AsEnumerable();

            //        var attTable = this.tableEntityMetadata.ChildRelations["FK_EntityMetadataTable_AttributeMetadataTable"].ChildTable as AttributeMetadataDataTable;
            //        foreach (var u in forUpdate)
            //        {
            //            // Remove existing rows so they can be replaced.
            //            attTable.RemoveAttributeMetadataRow(u);
            //        }

            //        if (modified.HasChanged.GetValueOrDefault())
            //        {
            //            // update entity metadata row?
            //        }

            //        foreach (var item in modified.Attributes)
            //        {
            //            attTable.AddFromSdkAttribute(this, item);
            //        }
            //    }
            //    finally
            //    {
            //        locker.ExitWriteLock();
            //    }
            //}

        }

    }
}
