using System;
using System.Linq;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;
using CrmAdo.Tests.Support;
using System.Text;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;

namespace CrmAdo.Tests
{
    [TestFixture(Category = "Select Statement")]
    public class SelectMetadataVisitorTests : BaseOrganisationRequestBuilderVisitorTest
    {
        [Category("TOP")]
        [Test(Description = "Should support selecting entity metadata")]
        public void Should_Support_Selecting_Entity_Metadata()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("SELECT e.MetadataId, e.HasChanged, e.ActivityTypeMask, e.Attributes, e.AutoRouteToOwnerQueue, e.CanBeInManyToMany, e.CanBePrimaryEntityInRelationship, ");
            sqlBuilder.AppendLine("e.CanBeRelatedEntityInRelationship, e.CanCreateAttributes, e.CanCreateCharts, e.CanCreateForms, e.CanCreateViews, e.CanModifyAdditionalSettings, e.CanTriggerWorkflow, ");
            sqlBuilder.AppendLine("e.Description, e.DisplayCollectionName, e.DisplayName, e.IconLargeName, e.IconMediumName, e.IconSmallName, e.IsActivity, e.IsActivityParty, e.IsAuditEnabled, ");
            sqlBuilder.AppendLine("e.IsAvailableOffline, e.IsChildEntity, e.IsConnectionsEnabled, e.IsCustomEntity, e.IsCustomizable, e.IsDocumentManagementEnabled, e.IsDuplicateDetectionEnabled, ");
            sqlBuilder.AppendLine("e.IsEnabledForCharts, e.IsImportable, e.IsIntersect, e.IsMailMergeEnabled, e.IsMailMergeEnabled, e.IsManaged, e.IsMappable, e.IsReadingPaneEnabled, e.IsRenameable, ");
            sqlBuilder.AppendLine("e.IsValidForAdvancedFind, e.IsValidForQueue, e.IsVisibleInMobile, e.LogicalName, e.IsCustomEntity, e.ManyToManyRelationships, e.ManyToOneRelationships, ");
            sqlBuilder.AppendLine("e.ObjectTypeCode, e.OneToManyRelationships, e.OwnershipType, e.PrimaryIdAttribute, e.PrimaryNameAttribute, e.Privileges, e.RecurrenceBaseEntityLogicalName, ");
            sqlBuilder.AppendLine("e.ReportViewName, e.SchemaName, e.RecurrenceBaseEntityLogicalName ");
            sqlBuilder.AppendLine("FROM EntityMetadata AS e");

            // Arrange
            var sql = sqlBuilder.ToString();
            // Act
            var queryExpression = GetOrganizationRequest<RetrieveMetadataChangesRequest>(sql);
            // Assert
            Assert.That(queryExpression, Is.Not.Null);
            Assert.That(queryExpression.Query, Is.Not.Null);


            EntityQueryExpression query = queryExpression.Query;
            Assert.That(query.Properties, Is.Not.Null);

            MetadataPropertiesExpression props = query.Properties;
            Assert.That(props.PropertyNames.Count, Is.GreaterThan(1));

        }


    }
}