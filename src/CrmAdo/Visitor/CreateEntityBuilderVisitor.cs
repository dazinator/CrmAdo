using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.Dynamics.Metadata;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using CrmAdo.Core;

namespace CrmAdo.Visitor
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="CreateEntityRequest"/> when it visits an <see cref="CreateBuilder"/> 
    /// </summary>
    public class CreateEntityRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public const int NameMaxLength = 4000;
        public const int DefaultNameMaxLength = 200;
        public ICrmMetadataNamingProvider _SchemaNameProvider;

        private CrmMetadataNamingConvention _NamingConvention;

        public CreateEntityRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
            : this(null, metadataProvider)
        {

        }

        public CreateEntityRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider)
            : this(parameters, metadataProvider, new CrmAdoCrmMetadataNamingProvider())
        {

        }

        public CreateEntityRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider, ICrmMetadataNamingProvider schemaNameProvider)
        {
            Request = new CreateEntityRequest();
            Parameters = parameters;
            MetadataProvider = metadataProvider;
            _SchemaNameProvider = schemaNameProvider;
            _NamingConvention = schemaNameProvider.GetAttributeNamingConvention();
        }

        public CreateEntityRequest Request { get; set; }
        public DbParameterCollection Parameters { get; set; }
        private ICrmMetaDataProvider MetadataProvider { get; set; }
        private EntityMetadataBuilder EntityBuilder { get; set; }

        private bool IsVisitingCandidateIdColumn { get; set; }
        private bool HasFoundPrimaryKey { get; set; }

        private bool IsVisitingCandidateNameColumn { get; set; }
        private bool HasFoundNameColumn { get; set; }

        public int? ColumnSize { get; set; }

        #region Visit Methods

        protected override void VisitCreate(CreateBuilder item)
        {
            GuardCreateBuilder(item);
            item.CreateObject.Accept(this);
            Request = EntityBuilder.ToCreateEntityRequest();
            EntityBuilder = null;
        }

        protected override void VisitCreateTableDefinition(CreateTableDefinition item)
        {

            var entityName = _NamingConvention.GetEntityLogicalName(item.Name);
          
            EntityBuilder = EntityConstruction.ConstructEntity(entityName);
            EntityBuilder.SchemaName(_NamingConvention.GetEntitySchemaName(entityName))
                         .DisplayName(_NamingConvention.GetEntityDisplayName(entityName))
                         .DisplayCollectionName(_NamingConvention.GetEntityDisplayCollectionName(entityName));

            if (item.Columns != null && item.Columns.Any())
            {
                foreach (var col in item.Columns)
                {
                    ((IVisitableBuilder)col).Accept(this);
                }
            }

            if (!HasFoundPrimaryKey)
            {
                throw new NotSupportedException("Dynamics CRM requires that you specify a single PRIMARY KEY column. This should be of datatype UNIQUEIDENTIFIER");
            }

            if (!HasFoundNameColumn)
            {
                throw new NotSupportedException("Dynamics CRM requires that you specify the primary name column. This should be of datatype VARCHAR or NVARCHAR.");
            }

        }

        protected override void VisitColumnDefinition(ColumnDefinition item)
        {
            string columnName = item.Name;

            // We are looking for 2 columns. 
            // The primary key column:
            if (HasFoundPrimaryKey && HasFoundNameColumn)
            {
                throw new NotSupportedException("You can only specify a Primary Key column and a Primary Name column in the CREATE TABLE statement, as CRM requires that these 2 columns are specified when a new entity is created. You can only add additional columns after the entity has been created, via use of ALTER statements.");
            }

            // Visit the datatype first.
            ((IVisitableBuilder)item.DataType).Accept(this);

            // visit constraints second.
            if (item.Constraints != null)
            {
                foreach (var constraint in item.Constraints)
                {
                    ((IVisitableBuilder)constraint).Accept(this);
                }
            }

            if (!string.IsNullOrEmpty(item.Collation))
            {
                throw new NotSupportedException("Collation not supported.");
            }

            if (item.Default != null)
            {
                ((IVisitableBuilder)item.Default).Accept(this);
            }

            if (item.AutoIncrement != null)
            {
                ((IVisitableBuilder)item.AutoIncrement).Accept(this);
            }

            // Nullability.
            AttributeRequiredLevel attRequiredLevel = AttributeRequiredLevel.None;
            if (item.IsNullable.HasValue)
            {
                if (item.IsNullable.Value)
                {
                    attRequiredLevel = AttributeRequiredLevel.None;
                }
                else
                {
                    attRequiredLevel = AttributeRequiredLevel.ApplicationRequired;
                }
            }

            // If we found a UNIQUEIDENTIFIER column but no primary key constraint then throw.
            if (IsVisitingCandidateIdColumn)
            {
                if (!HasFoundPrimaryKey)
                {
                    throw new NotSupportedException(string.Format("Column: '{0}' must have a PRIMARY KEY constraint.", columnName));
                }

                // we found id column.
                var expectedColumnName = _NamingConvention.GetEntityIdAttributeLogicalName(EntityBuilder.Entity.LogicalName);
                if (columnName.ToLower() != expectedColumnName)
                {
                    throw new NotSupportedException(string.Format("As the Column '{0}' is the primary key column it should be named {1}. This is a requirement of dynamics CRM.", item.Name, expectedColumnName));
                }

                IsVisitingCandidateIdColumn = false;
            }

            if (IsVisitingCandidateNameColumn)
            {
                if (this.ColumnSize == null)
                {
                    throw new InvalidOperationException(string.Format("Column: {0}, Unable to establish column size.", item.Name));
                }


                var schemaName = _NamingConvention.GetAttributeSchemaName(item.Name);
                var displayName = _NamingConvention.GetAttributeDisplayName(item.Name);
                var description = _NamingConvention.GetAttributeDescription(item.Name);

                EntityBuilder.WithPrimaryAttribute(schemaName, displayName, description, attRequiredLevel, this.ColumnSize.Value, StringFormat.Text);
                HasFoundNameColumn = true;
                IsVisitingCandidateNameColumn = false;
            }
        }

        protected override void VisitDataType(DataType item)
        {
            switch (item.Name.ToUpper())
            {
                case "UNIQUEIDENTIFIER":
                    // This is a candiate for the id column that we require.
                    if (HasFoundPrimaryKey)
                    {
                        throw new NotSupportedException("You must specify a single UNIQUEIDENTIFIER column in the CREATE TABLE statement, which is the PRIMARY KEY for the table. CRM only allows a signle ID column and a single NAME column to be specified when a new entity is created. Once the table is created, you can use ALTER to add in any additional columns as required.");
                    }
                    IsVisitingCandidateIdColumn = true;
                    break;

                case "VARCHAR":
                case "NVARCHAR":
                    if (HasFoundNameColumn)
                    {
                        throw new NotSupportedException("You must specify a single VARCHAR or NVARCHAR column in the CREATE TABLE statement, which is the primary name attribute for the new CRM entity. CRM only allows a signle ID column and a single NAME column to be specified when a new entity is created. Once the entity is created, you can use ALTER to add in any additional columns as required.");
                    }
                    IsVisitingCandidateNameColumn = true;
                    break;
                default:
                    throw new NotSupportedException("You must specify a single VARCHAR or NVARCHAR column in the CREATE TABLE statement, which is the primary name attribute for the new CRM entity. CRM only allows a signle ID column and a single NAME column to be specified when a new entity is created. Once the entity is created, you can use ALTER to add in any additional columns as required.");

            }


            if (IsVisitingCandidateNameColumn)
            {

                // Size only applies to name column.
                if (item.HasMax)
                {
                    this.ColumnSize = NameMaxLength;
                    return;
                }

                if (!item.Arguments.Any())
                {
                    this.ColumnSize = DefaultNameMaxLength;
                    return;
                }

                if (item.Arguments.Count() > 1)
                {
                    throw new NotSupportedException(string.Format("For Datatype {0}, Only a single size argument is expected.", item.Name));
                }

                var sizeArg = item.Arguments.Single();
                ((IVisitableBuilder)sizeArg).Accept(this);
            }
            else
            {
                if (item.HasMax)
                {
                    throw new NotSupportedException(string.Format("For Datatype {0}, no 'max' size argument is expected.", item.Name));
                }
                if (item.Arguments.Any())
                {
                    throw new NotSupportedException(string.Format("For Datatype {0}, no size arguments are expected.", item.Name));
                }
            }

        }

        protected override void VisitPrimaryKeyConstraint(PrimaryKeyConstraint item)
        {
            if (IsVisitingCandidateIdColumn)
            {
                // we found the primary key!
                this.HasFoundPrimaryKey = true;
            }
            else
            {
                throw new NotSupportedException("Primary Key Constraint can only be present on a single column of datatype UNIQUEIDENTITIFER.");
            }
        }

        protected override void VisitUniqueConstraint(UniqueConstraint item)
        {
            // Unique constraints are allowed if the column is the primary key..
            if (IsVisitingCandidateIdColumn)
            {

            }
            else
            {
                throw new NotSupportedException("Unique constraints are not supported.");
            }

        }

        protected override void VisitForeignKeyConstraint(ForeignKeyConstraint item)
        {
            throw new NotSupportedException("Due to a limitation of the Dynamics CRM API, You cannot add foreign key or other custom columns when creating the table. Please create the table first, then use an ALTER statement after table creation to add in any custom columns.");
        }

        protected override void VisitAutoIncrement(AutoIncrement item)
        {
            throw new NotSupportedException("Identity / AutoIncrement is not supported by CRM.");
        }

        protected override void VisitDefaultConstraint(DefaultConstraint item)
        {
            throw new NotSupportedException("Default constraints are not supported.");
        }

        protected override void VisitNumericLiteral(NumericLiteral item)
        {
            this.ColumnSize = System.Convert.ToInt32(item.Value);
            base.VisitNumericLiteral(item);
        }

        #endregion

        private void GuardCreateBuilder(CreateBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            if (builder.CreateObject == null)
            {
                throw new ArgumentException("The create statement must specify a supported object to create, i.e a database or a table.");
            }
            // could check if the create object is database and make sure it has a name,
            // could check if the create object is a table and make sure it has the id columnd and name column specified.
        }

        private object GetParamaterValue(string paramName)
        {
            if (!Parameters.Contains(paramName))
            {
                throw new InvalidOperationException("Missing parameter value for parameter named: " + paramName);
            }
            var param = Parameters[paramName];
            return param.Value;
        }

    }
}
