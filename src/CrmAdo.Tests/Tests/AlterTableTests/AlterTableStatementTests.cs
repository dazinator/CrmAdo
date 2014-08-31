using CrmAdo.Tests.Support;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Tests
{

    [Obsolete]
    [Category("Alter Table Statement")]
    [TestFixture()]
    public class AlterTableStatementTests : BaseOrganisationRequestBuilderVisitorTest
    {
        #region Bool

        [Test(Description = "Should support adding a new boolean attribute.")]
        public void Can_Add_Boolean_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} BIT", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(BooleanAttributeMetadata)));

            var attMeta = (BooleanAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Boolean);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.BooleanType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            //  Assert.That(attMeta.DefaultValue, Is.EqualTo(int.MinValue));
            // CrmAdo should create a default yes / no option set, whihc should be able to be altered later if desired. 
            Assert.That(attMeta.OptionSet, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption, Is.Not.Null);
            Assert.That(attMeta.OptionSet.FalseOption, Is.Not.Null);

            // True and False values should be specified.
            Assert.That(attMeta.OptionSet.TrueOption.Value, Is.GreaterThan(-1));
            Assert.That(attMeta.OptionSet.FalseOption.Value, Is.GreaterThan(-1));
            // True and False values shouldn't be the same.
            Assert.That(attMeta.OptionSet.FalseOption.Value, Is.Not.EqualTo(attMeta.OptionSet.TrueOption.Value));

            // True and False Labels should be specified.
            Assert.That(attMeta.OptionSet.TrueOption.Label, Is.Not.Null);
            Assert.That(attMeta.OptionSet.FalseOption.Label, Is.Not.Null);

            // True and False Localised Labels should be specified.
            Assert.That(attMeta.OptionSet.FalseOption.Label.UserLocalizedLabel, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption.Label.UserLocalizedLabel, Is.Not.Null);

            // True and False Localised Labels shouldn't be the same.
            Assert.That(attMeta.OptionSet.FalseOption.Label.UserLocalizedLabel.Label, Is.Not.EqualTo(attMeta.OptionSet.TrueOption.Label.UserLocalizedLabel.Label));



        }

        [Test(Description = "Should support adding a new boolean attribute with a default value of true.")]
        public void Can_Add_Boolean_Attribute_With_Default_Value_True()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} BIT DEFAULT 1", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(BooleanAttributeMetadata)));

            var attMeta = (BooleanAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Boolean);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.BooleanType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            //  Assert.That(attMeta.DefaultValue, Is.EqualTo(int.MinValue));
            // CrmAdo should create a default yes / no option set, whihc should be able to be altered later if desired. 
            Assert.That(attMeta.OptionSet, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption, Is.Not.Null);
            Assert.That(attMeta.OptionSet.FalseOption, Is.Not.Null);

            // True and False values should be specified.
            Assert.That(attMeta.OptionSet.TrueOption.Value, Is.GreaterThan(-1));
            Assert.That(attMeta.OptionSet.FalseOption.Value, Is.GreaterThan(-1));
            // True and False values shouldn't be the same.
            Assert.That(attMeta.OptionSet.FalseOption.Value, Is.Not.EqualTo(attMeta.OptionSet.TrueOption.Value));

            // True and False Labels should be specified.
            Assert.That(attMeta.OptionSet.TrueOption.Label, Is.Not.Null);
            Assert.That(attMeta.OptionSet.FalseOption.Label, Is.Not.Null);

            // True and False Localised Labels should be specified.
            Assert.That(attMeta.OptionSet.FalseOption.Label.UserLocalizedLabel, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption.Label.UserLocalizedLabel, Is.Not.Null);

            // True and False Localised Labels shouldn't be the same.
            Assert.That(attMeta.OptionSet.FalseOption.Label.UserLocalizedLabel.Label, Is.Not.EqualTo(attMeta.OptionSet.TrueOption.Label.UserLocalizedLabel.Label));

            Assert.That(attMeta.DefaultValue, Is.Not.Null);
            Assert.That(attMeta.DefaultValue, Is.EqualTo(true));

        }

        [Test(Description = "Should support adding a new boolean attribute with a default value of false.")]
        public void Can_Add_Boolean_Attribute_With_Default_Value_False()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} BIT DEFAULT 0", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(BooleanAttributeMetadata)));

            var attMeta = (BooleanAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Boolean);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.BooleanType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            //  Assert.That(attMeta.DefaultValue, Is.EqualTo(int.MinValue));
            // CrmAdo should create a default yes / no option set, whihc should be able to be altered later if desired. 
            Assert.That(attMeta.OptionSet, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption, Is.Not.Null);
            Assert.That(attMeta.OptionSet.FalseOption, Is.Not.Null);

            // True and False values should be specified.
            Assert.That(attMeta.OptionSet.TrueOption.Value, Is.GreaterThan(-1));
            Assert.That(attMeta.OptionSet.FalseOption.Value, Is.GreaterThan(-1));
            // True and False values shouldn't be the same.
            Assert.That(attMeta.OptionSet.FalseOption.Value, Is.Not.EqualTo(attMeta.OptionSet.TrueOption.Value));

            // True and False Labels should be specified.
            Assert.That(attMeta.OptionSet.TrueOption.Label, Is.Not.Null);
            Assert.That(attMeta.OptionSet.FalseOption.Label, Is.Not.Null);

            // True and False Localised Labels should be specified.
            Assert.That(attMeta.OptionSet.FalseOption.Label.UserLocalizedLabel, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption.Label.UserLocalizedLabel, Is.Not.Null);

            // True and False Localised Labels shouldn't be the same.
            Assert.That(attMeta.OptionSet.FalseOption.Label.UserLocalizedLabel.Label, Is.Not.EqualTo(attMeta.OptionSet.TrueOption.Label.UserLocalizedLabel.Label));

            Assert.That(attMeta.DefaultValue, Is.Not.Null);
            Assert.That(attMeta.DefaultValue, Is.EqualTo(false));

        }

        #endregion

        #region DateTime

        [Test(Description = "Should support adding a new datetime attribute.")]
        public void Can_Add_DateTime_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} DATETIME", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(DateTimeAttributeMetadata)));

            var attMeta = (DateTimeAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.DateTime);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.DateTimeType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            Assert.That(attMeta.Format == DateTimeFormat.DateAndTime);


        }

        [Test(Description = "Should support adding a date only attribute.")]
        public void Can_Add_Date_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} DATE", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(DateTimeAttributeMetadata)));

            var attMeta = (DateTimeAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.DateTime);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.DateTimeType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            Assert.That(attMeta.Format == DateTimeFormat.DateOnly);


        }

        #endregion

        #region Decimal

        [Test(Description = "Should support adding a new decimal attribute.")]
        public void Can_Add_Decimal_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            // the max number of digits that can be stored in crm decimal field. = 12 + 10 == 22
            int maxPrecision = DecimalAttributeMetadata.MaxSupportedValue.ToString().Length + DecimalAttributeMetadata.MaxSupportedPrecision;

            // The max number of digits that can appear after the decimal point. = 10.
            int maxScale = DecimalAttributeMetadata.MaxSupportedPrecision;

            // The min number of digits that can appear after the decimal point. = 0.
            int minScale = DecimalAttributeMetadata.MinSupportedPrecision;

            // The default precision for a decimal field is the max precision, plus the minimum scale. = 12 + 0 = 12.
            int defaultprecision = DecimalAttributeMetadata.MaxSupportedValue.ToString().Length + minScale;


            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} DECIMAL", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(DecimalAttributeMetadata)));

            var attMeta = (DecimalAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Decimal);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.DecimalType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            Assert.That(attMeta.MinValue, Is.EqualTo(DecimalAttributeMetadata.MinSupportedValue));
            Assert.That(attMeta.MaxValue, Is.EqualTo(DecimalAttributeMetadata.MaxSupportedValue));
            // attMeta.Precision
            Assert.That(attMeta.Precision, Is.EqualTo(minScale));

        }

        [Test(Description = "Should support adding a new decimal attribute with precision.")]
        public void Can_Add_Decimal_Attribute_With_Precision()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            // the max number of digits that can be stored in crm decimal field. = 12 + 10 == 22
            // without specifying scale, the max number of digits is therefore 12.
            int maxCrmPrecisionValue = DecimalAttributeMetadata.MaxSupportedValue.ToString().Length;

            // Lets create a decimal field with random precision between 1 and 12.
            int precision = new Random().Next(1, maxCrmPrecisionValue);

            Console.WriteLine("Precision is " + precision);

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} DECIMAL({2})", entityName, newColumnName, precision);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(DecimalAttributeMetadata)));

            var attMeta = (DecimalAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Decimal);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.DecimalType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));

            // we should have a default min and max value that is not greater in length that the precision we have specified. 
            Assert.That(attMeta.MinValue, Is.Not.Null);
            Assert.That(attMeta.MaxValue, Is.Not.Null);
            Assert.That(attMeta.MinValue.ToString().Length, Is.EqualTo(precision));
            Assert.That(attMeta.MaxValue.ToString().Length, Is.EqualTo(precision));

            // What dynamics sdk refers to as the 'precision' here is actually what we refer to as the 'scale' - the number of digits allowed after the decimal point.
            // as we never speficied a scale this should default to 0.
            Assert.That(attMeta.Precision, Is.EqualTo(DecimalAttributeMetadata.MinSupportedPrecision));

        }

        [Test(Description = "Should support adding a new decimal attribute with precision and scale.")]
        public void Can_Add_Decimal_Attribute_With_Precision_And_Scale()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            // the max number of digits that can be stored in crm decimal field. = 12 + 10 == 22

            int maxCrmPrecisionValue = DecimalAttributeMetadata.MaxSupportedValue.ToString().Length;

            // Create random scale between min and max allowed.
            int scale = new Random().Next(DecimalAttributeMetadata.MinSupportedPrecision, DecimalAttributeMetadata.MaxSupportedPrecision);

            // Create a decimal field precision between 1 + s and 12 + s.
            int precision = new Random().Next(0 + scale, maxCrmPrecisionValue + scale);

            Console.WriteLine("Precision is " + precision + ", scale is: " + scale);

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} DECIMAL({2},{3})", entityName, newColumnName, precision, scale);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(DecimalAttributeMetadata)));

            var attMeta = (DecimalAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Decimal);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.DecimalType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));

            // we should have a default min and max value that is not greater in length than the precision we specified. 
            Assert.That(attMeta.MinValue, Is.Not.Null);
            Assert.That(attMeta.MaxValue, Is.Not.Null);
            Assert.That(attMeta.MinValue.ToString().Length, Is.EqualTo(precision));
            Assert.That(attMeta.MaxValue.ToString().Length, Is.EqualTo(precision));

            // What dynamics sdk refers to as the 'precision' here is actually what we refer to as the 'scale' - the number of digits allowed after the decimal point.
            // as we speficied a scale this should equal that.
            Assert.That(attMeta.Precision, Is.EqualTo(scale));

        }

        #endregion

        #region Double

        [Test(Description = "Should support adding a new double attribute.")]
        public void Can_Add_Double_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            // the max number of digits that can be stored in crm decimal field. = 12 + 10 == 22
            int maxPrecision = DoubleAttributeMetadata.MaxSupportedValue.ToString().Length + DoubleAttributeMetadata.MaxSupportedPrecision;

            // The max number of digits that can appear after the decimal point. = 10.
            int maxScale = DoubleAttributeMetadata.MaxSupportedPrecision;

            // The min number of digits that can appear after the decimal point. = 0.
            int minScale = DoubleAttributeMetadata.MinSupportedPrecision;

            // The default precision for a decimal field is the max precision, plus the minimum scale. = 12 + 0 = 12.
            int defaultprecision = DoubleAttributeMetadata.MaxSupportedValue.ToString().Length + minScale;

            // NEED TO SEE HOW DYNAMICS CRM STORES DOUBLE ATTRIBUTES.. CRM LETS YOU SPECIFY PRECISION AND SCALE, BUT FLOAT DATATYPE DOESNT.
            // SO PERHAPS FLOAT IS WRONG DATATYPE TO USE FOR DOUBLE ATTRIBUTES, PERHAPS NUMERIC(P,S) INSTEAD.

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} FLOAT", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(DoubleAttributeMetadata)));

            var attMeta = (DoubleAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Double);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.DoubleType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            Assert.That(attMeta.MinValue, Is.EqualTo(DoubleAttributeMetadata.MinSupportedValue));
            Assert.That(attMeta.MaxValue, Is.EqualTo(DoubleAttributeMetadata.MaxSupportedValue));
            // attMeta.Precision
            Assert.That(attMeta.Precision, Is.EqualTo(minScale));
            
        }

        [Test(Description = "Should support adding a new double attribute with precision.")]
        public void Can_Add_Double_Attribute_With_Precision()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            // the max number of digits that can be stored in crm decimal field. = 12 + 10 == 22
            // without specifying scale, the max number of digits is therefore 12.
            int maxCrmPrecisionValue = DoubleAttributeMetadata.MaxSupportedValue.ToString().Length;

            // Lets create a decimal field with random precision between 1 and 12.
            int precision = new Random().Next(1, maxCrmPrecisionValue);

            Console.WriteLine("Precision is " + precision);

            // NEED TO SEE HOW DYNAMICS CRM STORES DOUBLE ATTRIBUTES.. CRM LETS YOU SPECIFY PRECISION AND SCALE, BUT FLOAT DATATYPE DOESNT.
            // SO PERHAPS FLOAT IS WRONG DATATYPE TO USE FOR DOUBLE ATTRIBUTES, PERHAPS NUMERIC(P,S) INSTEAD.
            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} FLOAT({2})", entityName, newColumnName, precision);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(DoubleAttributeMetadata)));

            var attMeta = (DoubleAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Double);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.DoubleType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));

            // we should have a default min and max value that is not greater in length that the precision we have specified. 
            Assert.That(attMeta.MinValue, Is.Not.Null);
            Assert.That(attMeta.MaxValue, Is.Not.Null);
            Assert.That(attMeta.MinValue.ToString().Length, Is.EqualTo(precision));
            Assert.That(attMeta.MaxValue.ToString().Length, Is.EqualTo(precision));

            // What dynamics sdk refers to as the 'precision' here is actually what we refer to as the 'scale' - the number of digits allowed after the decimal point.
            // as we never speficied a scale this should default to 0.
            Assert.That(attMeta.Precision, Is.EqualTo(DoubleAttributeMetadata.MinSupportedPrecision));

        }

        [Test(Description = "Should support adding a new double attribute with precision and scale.")]
        public void Can_Add_Double_Attribute_With_Precision_And_Scale()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            // the max number of digits that can be stored in crm decimal field. = 12 + 10 == 22

            int maxCrmPrecisionValue = DoubleAttributeMetadata.MaxSupportedValue.ToString().Length;

            // Create random scale between min and max allowed.
            int scale = new Random().Next(DoubleAttributeMetadata.MinSupportedPrecision, DoubleAttributeMetadata.MaxSupportedPrecision);

            // Create a decimal field precision between 1 + s and 12 + s.
            int precision = new Random().Next(0 + scale, maxCrmPrecisionValue + scale);

            Console.WriteLine("Precision is " + precision + ", scale is: " + scale);
            // NEED TO SEE HOW DYNAMICS CRM STORES DOUBLE ATTRIBUTES.. CRM LETS YOU SPECIFY PRECISION AND SCALE, BUT FLOAT DATATYPE DOESNT.
            // SO PERHAPS FLOAT IS WRONG DATATYPE TO USE FOR DOUBLE ATTRIBUTES, PERHAPS NUMERIC(P,S) INSTEAD.
            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} FLOAT({2},{3})", entityName, newColumnName, precision, scale);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(DoubleAttributeMetadata)));

            var attMeta = (DoubleAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Double);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.DoubleType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));

            // we should have a default min and max value that is not greater in length than the precision we specified. 
            Assert.That(attMeta.MinValue, Is.Not.Null);
            Assert.That(attMeta.MaxValue, Is.Not.Null);
            Assert.That(attMeta.MinValue.ToString().Length, Is.EqualTo(precision));
            Assert.That(attMeta.MaxValue.ToString().Length, Is.EqualTo(precision));

            // What dynamics sdk refers to as the 'precision' here is actually what we refer to as the 'scale' - the number of digits allowed after the decimal point.
            // as we speficied a scale this should equal that.
            Assert.That(attMeta.Precision, Is.EqualTo(scale));

        }

        #endregion

        #region Image


        #endregion

        #region Integer

        [Test(Description = "Should support adding a new integer attribute.")]
        public void Can_Add_Integer_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} INT", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(IntegerAttributeMetadata)));

            var attMeta = (IntegerAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Integer);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.IntegerType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            Assert.That(attMeta.MinValue, Is.EqualTo(int.MinValue));
            Assert.That(attMeta.MaxValue, Is.EqualTo(int.MaxValue));
            attMeta.Format = IntegerFormat.None;


        }

        #endregion

        #region Lookup


        #endregion

        #region Memo


        #endregion

        #region Money


        #endregion

        #region Picklist


        #endregion

        #region State


        #endregion

        #region Status


        #endregion

        #region String

        [Test(Description = "Should support adding a new string attribute.")]
        public void Can_Add_String_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} VARCHAR", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(StringAttributeMetadata)));

            var stringMeta = (StringAttributeMetadata)attMetadata;

            Assert.That(stringMeta.AttributeType == AttributeTypeCode.String);
            Assert.That(stringMeta.AttributeTypeName == AttributeTypeDisplayName.StringType);
            Assert.That(stringMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(stringMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));

        }

        [Test(Description = "Should support adding a new string attribute with a specified max size.")]
        public void Can_Add_String_Attribute_MaxSize()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();
            int maxLength = 100;

            string commandText = string.Format(@"ALTER TABLE {0} ADD COLUMN {1} VARCHAR({2})", entityName, newColumnName, maxLength.ToString());

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(StringAttributeMetadata)));

            var stringMeta = (StringAttributeMetadata)attMetadata;

            Assert.That(stringMeta.AttributeType == AttributeTypeCode.String);
            Assert.That(stringMeta.AttributeTypeName == AttributeTypeDisplayName.StringType);
            Assert.That(stringMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(stringMeta.MaxLength, Is.EqualTo(maxLength));
            Assert.That(stringMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));

        }

        #endregion

















    }
}
