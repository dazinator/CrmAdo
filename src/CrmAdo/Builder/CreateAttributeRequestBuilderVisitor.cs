using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using SQLGeneration.Builders;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.Dynamics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using CrmAdo.Util;
using CrmAdo.Operations;
using CrmAdo.Core;

namespace CrmAdo.Visitor
{
    /// <summary>
    /// A <see cref="BuilderVisitor"/> that builds a <see cref="CreateAttributeRequest"/> when it visits an <see cref="AlterBuilder"/> 
    /// </summary>
    public class CreateAttributeRequestBuilderVisitor : BaseOrganizationRequestBuilderVisitor<CreateAttributeRequest>
    {

        public CreateAttributeRequestBuilderVisitor(ICrmMetaDataProvider metadataProvider, ConnectionSettings settings)
            : this(null, metadataProvider, settings)
        {

        }

        public CreateAttributeRequestBuilderVisitor(DbParameterCollection parameters, ICrmMetaDataProvider metadataProvider, ConnectionSettings settings)
            : base(metadataProvider, settings)
        {
            // Request = new CreateAttributeRequest();
            Parameters = parameters;
           // MetadataProvider = metadataProvider;
            this.FilterForForeignKeyConstraint = false;
        }

       // public OrganizationRequest Request { get; set; }
        public CreateOneToManyRequest CreateOneToManyRequest { get; set; }
        public DbParameterCollection Parameters { get; set; }
      //  private ICrmMetaDataProvider MetadataProvider { get; set; }

        private AttributeMetadata CurrentAttribute { get; set; }
        private AddColumns CurrentAddColumns { get; set; }
        private ColumnDefinition CurrentColumnDefinition { get; set; }
        private ForeignKeyConstraint CurrentForeignKeyConstraint { get; set; }
        private CascadeConfiguration CurrentCascadeConfiguration { get; set; }
        private bool? CurrentDefaultBooleanAttributeValue { get; set; }
        private int DefaultLanguageCode = 1033;

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

            string entityLogicalName = this.AlterTableName.ToLower();
            this.Request = BuildCreateRequest(entityLogicalName, item);
        }

        protected OrganizationRequest BuildCreateRequest(string entitySchemaName, ColumnDefinition item)
        {
            if (item.DataType == null)
            {
                throw new InvalidOperationException("You must specify a datatype.");
            }

            OrganizationRequest result = null;
            AttributeMetadata attMetadata = null;
            ForeignKeyConstraint fkConstraint = null;
            OneToManyRelationshipMetadata relationship = null;

            switch (item.DataType.Name.ToUpper())
            {
                case "BIT":
                    attMetadata = BuildCreateBooleanAttribute();
                    break;
                case "DATE":
                    attMetadata = BuildCreateDateAttribute();
                    break;
                case "DATETIME":
                    attMetadata = BuildCreateDateTimeAttribute();
                    break;
                case "DECIMAL":
                    attMetadata = BuildCreateDecimalAttribute();
                    break;
                case "FLOAT":
                    attMetadata = BuildCreateFloatAttribute();
                    break;
                case "INT":
                    fkConstraint = FindFirstForeignKeyConstraint();
                    if (fkConstraint != null)
                    {
                        // must be a picklist
                        attMetadata = BuildCreatePicklistAttribute();
                        break;
                    }
                    attMetadata = BuildCreateIntAttribute();
                    break;
                case "UNIQUEIDENTIFIER":
                    // If this is a unique identifier that has a foreign key constriant then it is a lookup.
                    fkConstraint = FindFirstForeignKeyConstraint();
                    if (fkConstraint == null)
                    {
                        throw new NotSupportedException("Crm does not allow creation of GUID columns. If you meant to create a lookup column you must include a foreign key constraint to an options table.");
                    }
                    relationship = BuildOneToManyRelationshipMetadata(fkConstraint);
                    var lookup = BuildCreateLookupAttribute();
                    attMetadata = lookup;
                    break;
                case "NVARCHAR":
                    if (item.DataType.HasMax)
                    {
                        attMetadata = BuildCreateMemoAttribute();
                        break;
                    }
                    attMetadata = BuildCreateStringAttribute();
                    break;
                case "MONEY":
                    attMetadata = BuildCreateMoneyAttribute();
                    break;
                default:
                    throw new NotSupportedException("DataType: " + item.Name + " is not supported.");
            }

            string attributeSchemaName = item.Name;
            string attributeLogicalName = attributeSchemaName.ToLower();
            attMetadata.SchemaName = attributeSchemaName;
            attMetadata.LogicalName = attributeLogicalName;
            attMetadata.RequiredLevel = new AttributeRequiredLevelManagedProperty(AttributeRequiredLevel.None);
            attMetadata.DisplayName = new Label(attributeSchemaName, DefaultLanguageCode);
            // If this is a one to many attribute the request is a create one to many request.
            if (relationship != null)
            {
                CreateOneToManyRequest = BuildCreateOneToManyRequest(relationship, (LookupAttributeMetadata)attMetadata);
                return CreateOneToManyRequest;
            }
            var entityLogicalName = entitySchemaName.ToLower();           
            result = BuildCreateAttributeRequest(entityLogicalName, attMetadata);
            return result;
        }

        private MoneyAttributeMetadata BuildCreateMoneyAttribute()
        {

            //DateTimeFormat dtFormat = DateTimeFormat.DateOnly;
            var moneyAttribute = new MoneyAttributeMetadata();
            int moneyPrecision = moneyAttribute.DefaultSqlPrecision();
            int moneyScale = moneyAttribute.DefaultSqlScale();
            var dataType = this.CurrentColumnDefinition.DataType;

            if (dataType.Arguments != null && dataType.Arguments.Any())
            {
                // first is scale, second is precision.
                var argsCount = dataType.Arguments.Count();
                if (argsCount > 2)
                {
                    throw new InvalidOperationException("Datatype can have a maximum of 2 size arguments.");
                }

                if (argsCount >= 1)
                {
                    var sqlPrecisionArg = dataType.Arguments.First();
                    ((IVisitableBuilder)sqlPrecisionArg).Accept(this);
                    if (CurrentNumericLiteralValue != null)
                    {
                        moneyPrecision = Convert.ToInt32(CurrentNumericLiteralValue);
                        CurrentNumericLiteralValue = null;
                    }

                }

                if (argsCount >= 2)
                {
                    int? sqlScale = null;
                    var sqlScaleArg = dataType.Arguments.Skip(1).Take(1).Single();
                    ((IVisitableBuilder)sqlScaleArg).Accept(this);
                    if (CurrentNumericLiteralValue != null)
                    {
                        sqlScale = Convert.ToInt32(CurrentNumericLiteralValue);
                        CurrentNumericLiteralValue = null;
                    }
                    moneyScale = sqlScale.Value;
                }
            }

            moneyAttribute.SetFromSqlPrecisionAndScale(moneyPrecision, moneyScale);
            return moneyAttribute;

        }

        private StringAttributeMetadata BuildCreateStringAttribute()
        {
            // var createAttRequest = new CreateAttributeRequest();
            //DateTimeFormat dtFormat = DateTimeFormat.DateOnly;
            var stringAttribute = new StringAttributeMetadata();
            var dataType = this.CurrentColumnDefinition.DataType;
            int maxLength = 1; // default string max length 1.
            if (dataType.Arguments != null && dataType.Arguments.Any())
            {
                // first is scale, second is precision.

                var argsCount = dataType.Arguments.Count();
                if (argsCount > 1)
                {
                    throw new InvalidOperationException("Datatype can have a maximum of 1 size arguments.");
                }

                var maxLengthArg = dataType.Arguments.First();
                ((IVisitableBuilder)maxLengthArg).Accept(this);
                if (CurrentNumericLiteralValue != null)
                {
                    maxLength = Convert.ToInt32(CurrentNumericLiteralValue);
                    CurrentNumericLiteralValue = null;
                }
            }
            stringAttribute.MaxLength = maxLength;
            return stringAttribute;

        }

        private MemoAttributeMetadata BuildCreateMemoAttribute()
        {
            //DateTimeFormat dtFormat = DateTimeFormat.DateOnly;
            var memoAttribute = new MemoAttributeMetadata();
            memoAttribute.MaxLength = MemoAttributeMetadata.MaxSupportedLength;
            return memoAttribute;
        }

        private OneToManyRelationshipMetadata BuildOneToManyRelationshipMetadata(ForeignKeyConstraint fkConstraint)
        {

            var oneToManyRelationship = new OneToManyRelationshipMetadata();
            oneToManyRelationship.ReferencedEntity = fkConstraint.ReferencedTable.Name.ToLower();
            if (!string.IsNullOrWhiteSpace(fkConstraint.ReferencedColumn))
            {
                oneToManyRelationship.ReferencedAttribute = fkConstraint.ReferencedColumn.ToLower();
            }
            var referencingAttributeName = this.CurrentColumnDefinition.Name.ToLower();
            oneToManyRelationship.ReferencingEntity = this.AlterTableName.ToLower();
            // oneToManyRelationship.ReferencingAttribute = this.CurrentColumnDefinition.Name.ToLower();

            oneToManyRelationship.CascadeConfiguration = new CascadeConfiguration();

            oneToManyRelationship.CascadeConfiguration.Assign = CascadeType.NoCascade;
            oneToManyRelationship.CascadeConfiguration.Delete = CascadeType.Restrict;
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
            else
            {
                // generate schema name.
                oneToManyRelationship.SchemaName = string.Format("{0}_{1}_{2}", oneToManyRelationship.ReferencedEntity, oneToManyRelationship.ReferencingEntity, referencingAttributeName);
                //   throw new NotSupportedException("You must specify a constraint name for the foregin key constraint. This should be prefixed with the crm publisher prefix, e.g 'new_entity1_entity2'");
            }
            return oneToManyRelationship;
        }

        private CreateOneToManyRequest BuildCreateOneToManyRequest(OneToManyRelationshipMetadata relationship, LookupAttributeMetadata lookup)
        {
            var createOneToManyRequest = new CreateOneToManyRequest();
            createOneToManyRequest.OneToManyRelationship = relationship;
            createOneToManyRequest.Lookup = lookup;
            return createOneToManyRequest;
        }

        private LookupAttributeMetadata BuildCreateLookupAttribute()
        {
            var lookupAtt = new LookupAttributeMetadata();

            return lookupAtt;
        }

        private IntegerAttributeMetadata BuildCreateIntAttribute()
        {
            // Must be an integer attribute.        
            var intAttribute = new IntegerAttributeMetadata();
            intAttribute.MaxValue = int.MaxValue;
            intAttribute.MinValue = int.MinValue;
            intAttribute.Format = IntegerFormat.None;
            return intAttribute;
        }

        private PicklistAttributeMetadata BuildCreatePicklistAttribute()
        {
            throw new NotImplementedException();
        }

        private DoubleAttributeMetadata BuildCreateFloatAttribute()
        {
            var doubleAttribute = new DoubleAttributeMetadata();
            int doublePrecision = doubleAttribute.DefaultSqlPrecision();
            int doubleScale = doubleAttribute.DefaultSqlScale();
            var dataType = this.CurrentColumnDefinition.DataType;

            if (dataType.Arguments != null && dataType.Arguments.Any())
            {
                // first is scale, second is precision.
                var argsCount = dataType.Arguments.Count();
                if (argsCount > 2)
                {
                    throw new InvalidOperationException("Datatype can have a maximum of 2 size arguments.");
                }

                if (argsCount >= 1)
                {
                    var sqlPrecisionArg = dataType.Arguments.First();
                    ((IVisitableBuilder)sqlPrecisionArg).Accept(this);
                    if (CurrentNumericLiteralValue != null)
                    {
                        doublePrecision = Convert.ToInt32(CurrentNumericLiteralValue);
                        CurrentNumericLiteralValue = null;
                    }

                }

                if (argsCount >= 2)
                {
                    int? sqlScale = null;
                    var sqlScaleArg = dataType.Arguments.Skip(1).Take(1).Single();
                    ((IVisitableBuilder)sqlScaleArg).Accept(this);
                    if (CurrentNumericLiteralValue != null)
                    {
                        sqlScale = Convert.ToInt32(CurrentNumericLiteralValue);
                        CurrentNumericLiteralValue = null;
                    }
                    doubleScale = sqlScale.Value;
                }
            }

            doubleAttribute.SetFromSqlPrecisionAndScale(doublePrecision, doubleScale);
            return doubleAttribute;
        }

        private DecimalAttributeMetadata BuildCreateDecimalAttribute()
        {
            var decimalMetadata = new DecimalAttributeMetadata();
            int precision = decimalMetadata.DefaultSqlPrecision();
            int scale = decimalMetadata.DefaultSqlScale();
            var dataType = this.CurrentColumnDefinition.DataType;
            if (dataType.Arguments != null && dataType.Arguments.Any())
            {
                // first is scale, second is precision.
                var argsCount = dataType.Arguments.Count();
                if (argsCount > 2)
                {
                    throw new InvalidOperationException("Datatype can have a maximum of 2 size arguments.");
                }

                if (argsCount >= 1)
                {
                    var sqlPrecisionArg = dataType.Arguments.First();
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
                    var sqlScaleArg = dataType.Arguments.Skip(1).Take(1).Single();
                    ((IVisitableBuilder)sqlScaleArg).Accept(this);
                    if (CurrentNumericLiteralValue != null)
                    {
                        sqlScale = Convert.ToInt32(CurrentNumericLiteralValue);
                        CurrentNumericLiteralValue = null;
                    }
                    scale = sqlScale.Value;
                }
            }

            decimalMetadata.SetFromSqlPrecisionAndScale(precision, scale);
            return decimalMetadata;
        }

        private DateTimeAttributeMetadata BuildCreateDateTimeAttribute()
        {
            DateTimeFormat dateTimeFormat = DateTimeFormat.DateAndTime;
            var att = new DateTimeAttributeMetadata(dateTimeFormat);
            return att;
        }

        private DateTimeAttributeMetadata BuildCreateDateAttribute()
        {
            DateTimeFormat dtFormat = DateTimeFormat.DateOnly;
            var att = new DateTimeAttributeMetadata(dtFormat);
            return att;
        }

        private BooleanAttributeMetadata BuildCreateBooleanAttribute()
        {
            var optionSet = new BooleanOptionSetMetadata(
                    new OptionMetadata(new Label("True", DefaultLanguageCode), 1),
                    new OptionMetadata(new Label("False", DefaultLanguageCode), 0)
                    );
            var boolAttribute = new BooleanAttributeMetadata(optionSet);

            // Set default value.
            if (this.CurrentColumnDefinition.Default != null)
            {
                ((IVisitableBuilder)this.CurrentColumnDefinition.Default).Accept(this);
                boolAttribute.DefaultValue = this.CurrentDefaultBooleanAttributeValue;
                this.CurrentDefaultBooleanAttributeValue = null;
            }
            return boolAttribute;
        }

        private CreateAttributeRequest BuildCreateAttributeRequest(string entityLogicalName, AttributeMetadata attributeMetadata)
        {
            var createAttRequest = this.CurrentRequest;
            createAttRequest.EntityName = entityLogicalName;
            createAttRequest.Attribute = attributeMetadata;
            return createAttRequest;
        }

        protected override void VisitDefaultConstraint(DefaultConstraint item)
        {
            if (item.Value != null)
            {
                int? defaultVal = null;
                ((IVisitableBuilder)item.Value).Accept(this);
                if (CurrentNumericLiteralValue != null)
                {
                    defaultVal = Convert.ToInt32(CurrentNumericLiteralValue);
                    CurrentNumericLiteralValue = null;
                }
                if (defaultVal.HasValue)
                {
                    if (defaultVal == 1)
                    {
                        CurrentDefaultBooleanAttributeValue = true;
                    }
                    else if (defaultVal == 0)
                    {
                        CurrentDefaultBooleanAttributeValue = false;
                    }
                }
            }
            base.VisitDefaultConstraint(item);
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

        protected override void VisitForeignKeyConstraint(ForeignKeyConstraint item)
        {
            if (this.FilterForForeignKeyConstraint)
            {
                this.CurrentForeignKeyConstraint = item;
                this.FilterForForeignKeyConstraint = false;
                return;
            }
            base.VisitForeignKeyConstraint(item);
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

        public override ICrmOperation GetCommand()
        {
            var orgCommand = new CreateAttributeOperation(ResultColumnMetadata, Request);
            return orgCommand;
        }


    }
}
