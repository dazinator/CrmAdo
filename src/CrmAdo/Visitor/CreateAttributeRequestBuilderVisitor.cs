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

namespace CrmAdo.Visitor
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="CreateAttributeRequest"/> when it visits an <see cref="AlterBuilder"/> 
    /// </summary>
    public class CreateAttributeRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor
    {

        public CreateAttributeRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider)
            : this(null, metadataProvider)
        {

        }

        public CreateAttributeRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider)
        {
            // Request = new CreateAttributeRequest();
            Parameters = parameters;
            MetadataProvider = metadataProvider;
            this.FilterForForeignKeyConstraint = false;
        }

        public OrganizationRequest Request { get; set; }

        public DbParameterCollection Parameters { get; set; }
        private ICrmMetaDataProvider MetadataProvider { get; set; }

        private AttributeMetadata CurrentAttribute { get; set; }
        private AddColumns CurrentAddColumns { get; set; }
        private ColumnDefinition CurrentColumnDefinition { get; set; }
        private ForeignKeyConstraint CurrentForeignKeyConstraint { get; set; }
        private CascadeConfiguration CurrentCascadeConfiguration { get; set; }

        private double? CurrentNumericLiteralValue { get; set; }
        private bool? HasConstraints { get; set; }

        private string AlterTableName { get; set; }

        #region Visit Methods       

        protected override void VisitAlterTableDefinition(AlterTableDefinition item)
        {
            this.AlterTableName = item.Name;
            IVisitableBuilder alteration = item.Alteration;
            alteration.Accept(this);
        }

        protected override void VisitAddColumns(AddColumns item)
        {
            this.CurrentAddColumns = item;
            if (item.Columns != null && item.Columns.Any())
            {
                if (item.Columns.Count() > 1)
                {
                    throw new NotSupportedException("Only a single column can be added at a time.");
                }
                IVisitableBuilder first = item.Columns.First();
                first.Accept(this);
            }

        }

        protected override void VisitColumnDefinition(ColumnDefinition item)
        {
            this.CurrentColumnDefinition = item;
            if (item.DataType == null)
            {
                throw new InvalidOperationException("You must specify a datatype.");
            }

            ((IVisitableBuilder)item.DataType).Accept(this);

            this.CurrentAttribute.LogicalName = item.Name.ToLower();
            this.CurrentAttribute.SchemaName = item.Name;
            this.CurrentAttribute.RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None);

            if (item.Default != null)
            {
                ((IVisitableBuilder)item.Default).Accept(this);
            }

        }

        protected override void VisitDataType(DataType item)
        {
            int languageCode = 1033;
            CreateAttributeRequest createAttRequest = null;
            CreateOneToManyRequest createOneToManyRequest = null;

            switch (item.Name.ToUpper())
            {
                case "BIT":
                    // Create a boolean attribute
                    createAttRequest = new CreateAttributeRequest();
                    var optionSet = new BooleanOptionSetMetadata(
                            new OptionMetadata(new Label("True", languageCode), 1),
                            new OptionMetadata(new Label("False", languageCode), 0)
                            );
                    CurrentAttribute = new BooleanAttributeMetadata(optionSet);
                    createAttRequest.Attribute = CurrentAttribute;
                    createAttRequest.EntityName = this.AlterTableName.ToLower();

                    //var boolAttribute = new BooleanAttributeMetadata()
                    //{
                    //    // Set base properties
                    //    SchemaName = schemaName,
                    //    DisplayName = new Label(displayName, languageCode),
                    //    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    //    Description = new Label(description, languageCode),
                    //    // Set extended properties
                    //    OptionSet = 
                    //};
                    //this.Request.Attribute = boolAttribute;

                    break;
                case "DATE":

                    // Create a date time attribute
                    createAttRequest = new CreateAttributeRequest();
                    DateTimeFormat dtFormat = DateTimeFormat.DateOnly;
                    CurrentAttribute = new DateTimeAttributeMetadata(dtFormat);
                    createAttRequest.Attribute = CurrentAttribute;
                    createAttRequest.EntityName = this.AlterTableName.ToLower();
                    //{
                    //    // Set base properties
                    //    SchemaName = schemaName,
                    //    DisplayName = new Label(displayName, languageCode),
                    //    RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel),
                    //    Description = new Label(description, languageCode),
                    //    // Set extended properties
                    //    Format = format,
                    //    ImeMode = imeMode
                    //};

                    break;
                case "DATETIME":
                    createAttRequest = new CreateAttributeRequest();
                    createAttRequest.EntityName = this.AlterTableName.ToLower();
                    DateTimeFormat dateTimeFormat = DateTimeFormat.DateAndTime;
                    CurrentAttribute = new DateTimeAttributeMetadata(dateTimeFormat);
                    createAttRequest.Attribute = CurrentAttribute;
                    break;
                case "DECIMAL":

                    // Define the primary attribute for the entity
                    // Create a integer attribute	
                    createAttRequest = new CreateAttributeRequest();
                    createAttRequest.EntityName = this.AlterTableName.ToLower();
                    var decimalMetadata = new DecimalAttributeMetadata();
                    CurrentAttribute = decimalMetadata;
                    createAttRequest.Attribute = CurrentAttribute;

                    if (item.Arguments != null && item.Arguments.Any())
                    {
                        // first is scale, second is precision.
                        var argsCount = item.Arguments.Count();
                        if (argsCount > 2)
                        {
                            throw new InvalidOperationException("Datatype can have a maximum of 2 size arguments.");
                        }

                        int precision = decimalMetadata.DefaultSqlPrecision();
                        int scale = decimalMetadata.DefaultSqlScale();
                        if (argsCount >= 1)
                        {
                            var sqlPrecisionArg = item.Arguments.First();
                            ((IVisitableBuilder)sqlPrecisionArg).Accept(this);
                            if (CurrentNumericLiteralValue != null)
                            {
                                precision = Convert.ToInt32(CurrentNumericLiteralValue);
                                CurrentNumericLiteralValue = null;
                            }

                        }

                        if (argsCount >= 2)
                        {
                            int? sqlScale = null;
                            var sqlScaleArg = item.Arguments.Skip(1).Take(1);
                            ((IVisitableBuilder)sqlScaleArg).Accept(this);
                            if (CurrentNumericLiteralValue != null)
                            {
                                sqlScale = Convert.ToInt32(CurrentNumericLiteralValue);
                                CurrentNumericLiteralValue = null;
                            }
                            scale = sqlScale.Value;
                        }

                        decimalMetadata.SetFromSqlPrecisionAndScale(precision, scale);
                    }

                    //int languageCode = 1033;
                    //var att = new DecimalAttributeMetadata()
                    //{
                    //    // Set base properties
                    //    SchemaName = schemaName,
                    //    DisplayName = new Label(schemaName, languageCode),
                    //    RequiredLevel = new AttributeRequiredLevelManagedProperty(requiredLevel),
                    //    Description = new Label(description, languageCode),
                    //    // Set extended properties
                    //    Precision = precision,
                    //    MaxValue = max,
                    //    MinValue = min
                    //};

                    //this.Attributes.Add(att);
                    //return this;

                    break;
                case "FLOAT":
                    throw new NotImplementedException();
                // break;
                case "INT":

                    var firstForeignKey = FindFirstForeignKeyConstraint();
                    if (firstForeignKey != null)
                    {
                        // must be a picklist
                        throw new NotImplementedException();
                    }

                    // Must be an integer attribute.
                    createAttRequest = new CreateAttributeRequest();
                    createAttRequest.EntityName = this.AlterTableName.ToLower();
                    var intAttribute = new IntegerAttributeMetadata();
                    CurrentAttribute = intAttribute;
                    createAttRequest.Attribute = CurrentAttribute;
                    intAttribute.MaxValue = int.MaxValue;
                    intAttribute.MinValue = int.MinValue;

                    break;
                case "UNIQUEIDENTIFIER":

                    // If this is a unique identifier that has a foreign key constriant then it is a lookup.
                    createOneToManyRequest = new CreateOneToManyRequest();

                    var fkConstraint = FindFirstForeignKeyConstraint();
                    if (fkConstraint == null)
                    {
                        throw new NotSupportedException("Crm does not allow creation of GUID columns. If you meant to create a lookup column you must include a foreign key constraint to an options table.");
                    }

                    var oneToManyRelationship = new OneToManyRelationshipMetadata();
                    createOneToManyRequest.OneToManyRelationship = oneToManyRelationship;

                    oneToManyRelationship.ReferencedEntity = fkConstraint.ReferencedTable.Name.ToLower();
                    oneToManyRelationship.ReferencedAttribute = fkConstraint.ReferencedColumn.ToLower();

                    oneToManyRelationship.ReferencedEntity = this.AlterTableName.ToLower();
                    oneToManyRelationship.ReferencingAttribute = this.CurrentColumnDefinition.Name.ToLower();

                    oneToManyRelationship.CascadeConfiguration = new CascadeConfiguration();

                    oneToManyRelationship.CascadeConfiguration.Assign = CascadeType.NoCascade;
                    oneToManyRelationship.CascadeConfiguration.Delete = CascadeType.NoCascade;
                    oneToManyRelationship.CascadeConfiguration.Merge = CascadeType.NoCascade;
                    oneToManyRelationship.CascadeConfiguration.Reparent = CascadeType.NoCascade;
                    oneToManyRelationship.CascadeConfiguration.Share = CascadeType.NoCascade;
                    oneToManyRelationship.CascadeConfiguration.Unshare = CascadeType.NoCascade;

                    this.CurrentCascadeConfiguration = oneToManyRelationship.CascadeConfiguration;
                    if (fkConstraint.OnDeleteAction != null)
                    {
                        fkConstraint.OnDeleteAction.Accept(this);
                    }
                    if (fkConstraint.OnUpdateAction != null)
                    {
                        fkConstraint.OnUpdateAction.Accept(this);
                    }

                    if (!string.IsNullOrWhiteSpace(fkConstraint.ConstraintName))
                    {
                        oneToManyRelationship.SchemaName = fkConstraint.ConstraintName;
                    }

                    var lookupAtt = new LookupAttributeMetadata();
                    createOneToManyRequest.Lookup = lookupAtt;
                    this.CurrentAttribute = lookupAtt;
                    //OneToManyRelationship =
                    //        new OneToManyRelationshipMetadata
                    //        {
                    //            ReferencedEntity = "account",
                    //            ReferencingEntity = "campaign",
                    //            SchemaName = "new_account_campaign",
                    //            AssociatedMenuConfiguration = new AssociatedMenuConfiguration
                    //            {
                    //                Behavior = AssociatedMenuBehavior.UseLabel,
                    //                Group = AssociatedMenuGroup.Details,
                    //                Label = new Label("Account", 1033),
                    //                Order = 10000
                    //            },
                    //            CascadeConfiguration = new CascadeConfiguration
                    //            {
                    //                Assign = CascadeType.NoCascade,
                    //                Delete = CascadeType.RemoveLink,
                    //                Merge = CascadeType.NoCascade,
                    //                Reparent = CascadeType.NoCascade,
                    //                Share = CascadeType.NoCascade,
                    //                Unshare = CascadeType.NoCascade
                    //            }
                    //        },
                    //Lookup = new LookupAttributeMetadata
                    //{
                    //    SchemaName = "new_parent_accountid",
                    //    DisplayName = new Label("Account Lookup", 1033),
                    //    RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None),
                    //    Description = new Label("Sample Lookup", 1033)
                    //}


                    break;
                case "NVARCHAR":

                    if (item.HasMax)
                    {
                        // memo
                        throw new NotImplementedException("MEMO");
                    }
                    throw new NotImplementedException("STRING");

                    break;
                case "MONEY":
                    throw new NotImplementedException("MONEY");
                    break;
                default:
                    throw new NotSupportedException("DataType: " + item.Name + " is not supported.");
            }

            if (createAttRequest != null)
            {
                this.Request = createAttRequest;
            }
            else if (createOneToManyRequest != null)
            {
                this.Request = createOneToManyRequest;
            }
        }

        private ForeignKeyConstraint FindFirstForeignKeyConstraint()
        {
            if (this.CurrentColumnDefinition != null && this.CurrentColumnDefinition.Constraints != null)
            {
                this.FilterForForeignKeyConstraint = true;
                foreach (var constraint in this.CurrentColumnDefinition.Constraints)
                {
                    ((IVisitableBuilder)constraint).Accept(this);
                    if (this.CurrentForeignKeyConstraint != null)
                    {
                        var fk = this.CurrentForeignKeyConstraint;
                        this.CurrentForeignKeyConstraint = null;
                        return fk;
                    }
                }
                this.FilterForForeignKeyConstraint = false;
            }
            return null;
        }

        protected override void VisitNumericLiteral(NumericLiteral item)
        {
            this.CurrentNumericLiteralValue = item.Value;
            //   base.VisitNumericLiteral(item);
        }

        #endregion

        private object GetParamaterValue(string paramName)
        {
            if (!Parameters.Contains(paramName))
            {
                throw new InvalidOperationException("Missing parameter value for parameter named: " + paramName);
            }
            var param = Parameters[paramName];
            return param.Value;
        }


        public bool FilterForForeignKeyConstraint { get; set; }


    }
}
