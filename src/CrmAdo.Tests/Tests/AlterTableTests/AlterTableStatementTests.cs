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

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} BIT", entityName, newColumnName);

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
            Assert.That(attMeta.OptionSet.FalseOption.Label.LocalizedLabels, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption.Label.LocalizedLabels, Is.Not.Null);

            Assert.That(attMeta.OptionSet.FalseOption.Label.LocalizedLabels, Is.Not.Empty);
            Assert.That(attMeta.OptionSet.TrueOption.Label.LocalizedLabels, Is.Not.Empty);

            var trueLabel = attMeta.OptionSet.TrueOption.Label.LocalizedLabels.First();
            var falseLabel = attMeta.OptionSet.FalseOption.Label.LocalizedLabels.First();
            // True and False Localised Labels shouldn't be the same.
            Assert.That(trueLabel.Label, Is.Not.EqualTo(falseLabel.Label));



        }

        [Test(Description = "Should support adding a new boolean attribute with a default value of true.")]
        public void Can_Add_Boolean_Attribute_With_Default_Value_True()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} BIT DEFAULT 1", entityName, newColumnName);

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

            // True and False Localised Labels should be specified.
            Assert.That(attMeta.OptionSet.FalseOption.Label.LocalizedLabels, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption.Label.LocalizedLabels, Is.Not.Null);

            Assert.That(attMeta.OptionSet.FalseOption.Label.LocalizedLabels, Is.Not.Empty);
            Assert.That(attMeta.OptionSet.TrueOption.Label.LocalizedLabels, Is.Not.Empty);

            var trueLabel = attMeta.OptionSet.TrueOption.Label.LocalizedLabels.First();
            var falseLabel = attMeta.OptionSet.FalseOption.Label.LocalizedLabels.First();
            // True and False Localised Labels shouldn't be the same.
            Assert.That(trueLabel.Label, Is.Not.EqualTo(falseLabel.Label));


            Assert.That(attMeta.DefaultValue, Is.Not.Null);
            Assert.That(attMeta.DefaultValue, Is.EqualTo(true));

        }

        [Test(Description = "Should support adding a new boolean attribute with a default value of false.")]
        public void Can_Add_Boolean_Attribute_With_Default_Value_False()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} BIT DEFAULT 0", entityName, newColumnName);

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

            // True and False Localised Labels should be specified.
            Assert.That(attMeta.OptionSet.FalseOption.Label.LocalizedLabels, Is.Not.Null);
            Assert.That(attMeta.OptionSet.TrueOption.Label.LocalizedLabels, Is.Not.Null);

            Assert.That(attMeta.OptionSet.FalseOption.Label.LocalizedLabels, Is.Not.Empty);
            Assert.That(attMeta.OptionSet.TrueOption.Label.LocalizedLabels, Is.Not.Empty);

            var trueLabel = attMeta.OptionSet.TrueOption.Label.LocalizedLabels.First();
            var falseLabel = attMeta.OptionSet.FalseOption.Label.LocalizedLabels.First();
            // True and False Localised Labels shouldn't be the same.
            Assert.That(trueLabel.Label, Is.Not.EqualTo(falseLabel.Label));


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

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} DATETIME", entityName, newColumnName);

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

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} DATE", entityName, newColumnName);

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


            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} DECIMAL", entityName, newColumnName);

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

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} DECIMAL({2})", entityName, newColumnName, precision);

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
            Assert.That(Math.Abs(attMeta.MinValue.Value).ToString().Length, Is.EqualTo(precision));
            Assert.That(Math.Abs(attMeta.MaxValue.Value).ToString().Length, Is.EqualTo(precision));

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

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} DECIMAL({2},{3})", entityName, newColumnName, precision, scale);

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
            Assert.That(Math.Abs(attMeta.MinValue.Value).ToString().Replace(".","").Length, Is.EqualTo(precision));
            Assert.That(Math.Abs(attMeta.MaxValue.Value).ToString().Replace(".","").Length, Is.EqualTo(precision));

         

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

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} FLOAT", entityName, newColumnName);

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
            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} FLOAT({2})", entityName, newColumnName, precision);

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
            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} FLOAT({2},{3})", entityName, newColumnName, precision, scale);

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

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} INT", entityName, newColumnName);

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

        [Test(Description = "Should support adding a new lookup attribute.")]
        public void Can_Add_Lookup_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string referencedEntityName = "referencedentity";

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} UNIQUEIDENTIFIER REFERENCES {2};", entityName, newColumnName, referencedEntityName);

            var request = GetOrganizationRequest<CreateOneToManyRequest>(commandText);

            var attMetadata = request.Lookup;
            var relationship = request.OneToManyRelationship;

            Assert.IsNotNull(attMetadata);
            Assert.IsNotNull(relationship);
            Assert.That(attMetadata.EntityLogicalName, Is.EqualTo(entityName.ToLower()));
            Assert.That(attMetadata, Is.AssignableTo(typeof(LookupAttributeMetadata)));

            var attMeta = (LookupAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Lookup);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.LookupType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));

            // Assert on relationship.
            Assert.That(relationship.ReferencedEntity, Is.EqualTo(referencedEntityName.ToLower()));
            Assert.That(relationship.ReferencingEntity, Is.EqualTo(entityName.ToLower()));
            //   Assert.That(relationship.SchemaName, Is.EqualTo(entityName.ToLower())); // potentially use CONSTRAINT my_name to set the schemaname?

            // assert on cascade config?
            Assert.That(relationship.RelationshipType, Is.EqualTo(RelationshipType.OneToManyRelationship));
            Assert.That(relationship.CascadeConfiguration, Is.Not.Null);

            var cascade = relationship.CascadeConfiguration;

            // By default we shouldn't have any cascaded actions because it would be dangerous to assume any,
            // dynamics only allows an entity to have 1 relationship that has a cascading action on it. If more relationships are added,
            // those new relationshops cannot have any cascading actions because its constrained to 1 relationship with cascading actions per entity.
            // therefore we assume that when users add in new foreign keys (lookups) that there are no cascading relationships. Otherwise the 2nd one
            // that is added would cause dynamics crm to throw an error.

            Assert.That(cascade.Delete, Is.EqualTo(CascadeType.NoCascade));
            Assert.That(cascade.Assign, Is.EqualTo(CascadeType.NoCascade));
            Assert.That(cascade.Merge, Is.EqualTo(CascadeType.NoCascade));
            Assert.That(cascade.Reparent, Is.EqualTo(CascadeType.NoCascade));
            Assert.That(cascade.Share, Is.EqualTo(CascadeType.NoCascade));
            Assert.That(cascade.Unshare, Is.EqualTo(CascadeType.NoCascade));

        }

        #endregion

        #region Memo

        [Test(Description = "Should support adding a new memo attribute.")]
        public void Can_Add_Memo_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();
            //  int maxLength = MemoAttributeMetadata.MaxSupportedLength;

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} NVARCHAR(MAX)", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(MemoAttributeMetadata)));

            var attMeta = (MemoAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Memo);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.MemoType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.MaxLength, Is.EqualTo(MemoAttributeMetadata.MaxSupportedLength));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));

        }

        #endregion

        #region Money

        [Test(Description = "Should support adding a new money attribute.")]
        public void Can_Add_Money_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            // the max number of digits that can be stored in crm decimal field. = 12 + 10 == 22
            int maxPrecision = MoneyAttributeMetadata.MaxSupportedValue.ToString().Length + MoneyAttributeMetadata.MaxSupportedPrecision;

            // The max number of digits that can appear after the decimal point. = 10.
            int maxScale = MoneyAttributeMetadata.MaxSupportedPrecision;

            // The min number of digits that can appear after the decimal point. = 0.
            int minScale = MoneyAttributeMetadata.MinSupportedPrecision;

            // The default precision for a decimal field is the max precision, plus the minimum scale. = 12 + 0 = 12.
            int defaultprecision = MoneyAttributeMetadata.MaxSupportedValue.ToString().Length + minScale;

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} MONEY", entityName, newColumnName);

            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(MoneyAttributeMetadata)));

            var attMeta = (MoneyAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Money);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.MoneyType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
            Assert.That(attMeta.MinValue, Is.EqualTo(MoneyAttributeMetadata.MinSupportedValue));
            Assert.That(attMeta.MaxValue, Is.EqualTo(MoneyAttributeMetadata.MaxSupportedValue));

            //When the PrecisionSource is set to zero (0), the MoneyAttributeMetadata.Precision value is used.
            //When the PrecisionSource is set to one (1), the Organization.PricingDecimalPrecision value is used.
            //When the PrecisionSource is set to two (2), the TransactionCurrency.CurrencyPrecision value is used.

            Assert.That(attMeta.PrecisionSource, Is.EqualTo(0));
            Assert.That(attMeta.Precision, Is.EqualTo(MoneyAttributeMetadata.MinSupportedPrecision));

        }

        #endregion

        #region Picklist

        [Test(Description = "Should support adding a new picklist attribute that has a local option set.")]
        public void Can_Add_Picklist_Attribute_With_Local_OptionSet()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            Dictionary<int, string> optionValues = new Dictionary<int, string>();
            optionValues.Add(10000000, "Red");
            optionValues.Add(10000001, "Green");
            optionValues.Add(10000002, "Blue");

            // Local option set tables are named by convention. entityName.AttributeName.options
            // Global option set tables are named by convention. optionsetname.options
            string optionSetTableName = string.Format("{0}.{1}.options", entityName, newColumnName);
            string optionSetOptionsLabelName = string.Format("{0}.{1}.optionlabels", entityName, newColumnName);

            var sqlBuilder = new StringBuilder();
            string addPicklistAttributeCommandText = string.Format(@"ALTER TABLE {0} ADD {1} INT REFERENCES {2};", entityName, newColumnName, optionSetTableName);

            // The sql necessary to create the picklist column. Now must provide options and option labels in same sql statement.
            sqlBuilder.AppendLine(addPicklistAttributeCommandText);

            // string createOptionSetTableCommandText = string.Format(@"CREATE TABLE {0}(optionvalue INT PRIMARY KEY);", optionSetTableName);         
            // sqlBuilder.AppendLine(createOptionSetTableCommandText);

            // NOT SURE WHTHER WE CAN CREATE THE ATTRIBUTE WITH EMPTY OPTION SET FIRST
            // THEN DO InsertOptionValueRequest to insert option values later..
            //InsertOptionValueRequest insertOptionValueRequest =
            //    new InsertOptionValueRequest
            //    {
            //        AttributeLogicalName = "new_picklist",
            //        EntityLogicalName = Contact.EntityLogicalName,
            //        Label = new Label("New Picklist Label", _languageCode)
            //    };
          
            //foreach (var item in optionValues)
            //{
            //    // Adds an option value to the option set.
            //    string insertOptionValueCommandText = string.Format(@"INSERT INTO {0}(optionvalue) VALUES({1});", optionSetTableName, item.Key);
            //    sqlBuilder.AppendLine(insertOptionValueCommandText);

            //    // Adds a few text labels for the particular option value to demonstrate that a single option can have multiple text labels associated with it.
            //    // When LCID not specified, then default used.
            //    string insertOptionLabelCommandText = string.Format(@"INSERT INTO {0}(optionvalue, labeltext) VALUES({1}, {2});", optionSetOptionsLabelName, item.Key, item.Value);
            //    sqlBuilder.AppendLine(insertOptionLabelCommandText);
            //    // Adding an australian localised label. LCID 3081
            //    string insertAnotherLabelCommandText = string.Format(@"INSERT INTO {0}(optionvalue, labeltext, lcid) VALUES({1}, {2}, {3});", optionSetOptionsLabelName, item.Key, item.Value + "-anotherlabel", 3081);
            //    sqlBuilder.AppendLine(insertAnotherLabelCommandText);
            //}


            string commandText = sqlBuilder.ToString();
            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(PicklistAttributeMetadata)));

            var attMeta = (PicklistAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Picklist);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.PicklistType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
      
            //  Assert.That(attMeta.OptionSet.OptionSetType, Is.EqualTo(OptionSetType.Picklist));     
            //Assert.That(attMeta.OptionSet, Is.Not.Null);
            //Assert.That(attMeta.OptionSet.Options, Is.Not.Null);
            //Assert.That(attMeta.OptionSet.Options.Count(), Is.EqualTo(optionValues.Count()));

            //for (int i = 0; i < optionValues.Count() - 1; i++)
            //{
            //    var option = attMeta.OptionSet.Options[0];
            //    var expectedOption = optionValues.ElementAt(i);
            //    Assert.That(option.Value, Is.EqualTo(expectedOption.Key));
            //    Assert.That(option.Label, Is.Not.Null);
            //    Assert.That(option.Label.LocalizedLabels, Is.Not.Null);
            //    Assert.That(option.Label.LocalizedLabels.Count(), Is.EqualTo(2));
            //    Assert.That(option.Label.LocalizedLabels[0].Label, Is.EqualTo(expectedOption.Value));
            //    Assert.That(option.Label.LocalizedLabels[1].Label, Is.EqualTo(expectedOption.Value + "-anotherlabel"));
            //    Assert.That(option.Label.LocalizedLabels[1].LanguageCode, Is.EqualTo(3081));
            //}
        }

        [Test(Description = "Should support adding a new picklist attribute that has a global option set and a default value.")]
        public void Can_Add_Picklist_Attribute_With_Global_OptionSet_And_Default_Value()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            Dictionary<int, string> optionValues = new Dictionary<int, string>();
            optionValues.Add(10000000, "Red");
            optionValues.Add(10000001, "Green");
            optionValues.Add(10000002, "Blue");

            int defaultValue = 10000000;

            // Local option set tables are named by convention. entityName.AttributeName.options
            // Global option set tables are named by convention. optionsetname.options
            string optionSetTableName = string.Format("{0}.{1}.options", entityName, newColumnName);
            string optionSetOptionsLabelName = string.Format("{0}.{1}.optionlabels", entityName, newColumnName);

            var sqlBuilder = new StringBuilder();
            string addPicklistAttributeCommandText = string.Format(@"ALTER TABLE {0} ADD {1} INT DEFAULT {3} REFERENCES {2};", entityName, newColumnName, optionSetTableName, defaultValue);

            // The sql necessary to create the picklist column. Now must provide options and option labels in same sql statement.
            sqlBuilder.AppendLine(addPicklistAttributeCommandText);

            // string createOptionSetTableCommandText = string.Format(@"CREATE TABLE {0}(optionvalue INT PRIMARY KEY);", optionSetTableName);         
            // sqlBuilder.AppendLine(createOptionSetTableCommandText);

            //foreach (var item in optionValues)
            //{
            //    // Adds an option value to the option set.
            //    string insertOptionValueCommandText = string.Format(@"INSERT INTO {0}(optionvalue) VALUES({1});", optionSetTableName, item.Key);
            //    sqlBuilder.AppendLine(insertOptionValueCommandText);

            //    // Adds a few text labels for the particular option value to demonstrate that a single option can have multiple text labels associated with it.
            //    // When LCID not specified, then default used.
            //    string insertOptionLabelCommandText = string.Format(@"INSERT INTO {0}(optionvalue, labeltext) VALUES({1}, {2});", optionSetOptionsLabelName, item.Key, item.Value);
            //    sqlBuilder.AppendLine(insertOptionLabelCommandText);
            //    // Adding an australian localised label. LCID 3081
            //    string insertAnotherLabelCommandText = string.Format(@"INSERT INTO {0}(optionvalue, lcid, labeltext) VALUES({1}, {2}, {3});", optionSetOptionsLabelName, item.Key, 3081, item.Value + "-anotherlabel");
            //    sqlBuilder.AppendLine(insertAnotherLabelCommandText);
            //}


            string commandText = sqlBuilder.ToString();
            var request = GetOrganizationRequest<CreateAttributeRequest>(commandText);

            var attMetadata = request.Attribute;

            Assert.IsNotNull(attMetadata);
            Assert.That(request.EntityName, Is.EqualTo(entityName.ToLower()));

            Assert.That(attMetadata, Is.AssignableTo(typeof(PicklistAttributeMetadata)));

            var attMeta = (PicklistAttributeMetadata)attMetadata;

            Assert.That(attMeta.AttributeType == AttributeTypeCode.Picklist);
            Assert.That(attMeta.AttributeTypeName == AttributeTypeDisplayName.PicklistType);
            Assert.That(attMeta.LogicalName, Is.EqualTo(newColumnName.ToLower()));
            Assert.That(attMeta.RequiredLevel.Value, Is.EqualTo(AttributeRequiredLevel.None));
          //  Assert.That(attMeta.OptionSet.OptionSetType, Is.EqualTo(OptionSetType.Picklist));
            Assert.That(attMeta.DefaultFormValue, Is.EqualTo(defaultValue));

            //Assert.That(attMeta.OptionSet, Is.Not.Null);
            //Assert.That(attMeta.OptionSet.Options, Is.Not.Null);
            //Assert.That(attMeta.OptionSet.Options.Count(), Is.EqualTo(optionValues.Count()));

            //for (int i = 0; i < optionValues.Count() - 1; i++)
            //{
            //    var option = attMeta.OptionSet.Options[0];
            //    var expectedOption = optionValues.ElementAt(i);
            //    Assert.That(option.Value, Is.EqualTo(expectedOption.Key));
            //    Assert.That(option.Label, Is.Not.Null);
            //    Assert.That(option.Label.LocalizedLabels, Is.Not.Null);
            //    Assert.That(option.Label.LocalizedLabels.Count(), Is.EqualTo(2));
            //    Assert.That(option.Label.LocalizedLabels[0].Label, Is.EqualTo(expectedOption.Value));
            //    Assert.That(option.Label.LocalizedLabels[1].Label, Is.EqualTo(expectedOption.Value + "-anotherlabel"));
            //    Assert.That(option.Label.LocalizedLabels[1].LanguageCode, Is.EqualTo(3081));
            //}
        }

        #endregion

        #region State

        // Crm doesn't allow you to create state attributes.

        #endregion

        #region Status

        // Crm doesn't allow you to create status attributes.

        #endregion

        #region String

        [Test(Description = "Should support adding a new string attribute.")]
        public void Can_Add_String_Attribute()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} NVARCHAR", entityName, newColumnName);

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
            Assert.That(stringMeta.MaxLength, Is.EqualTo(1));
        }

        [Test(Description = "Should support adding a new string attribute with a specified max size.")]
        public void Can_Add_String_Attribute_MaxSize()
        {
            // Arrange         
            string entityName = "testentity";
            string newColumnName = "newcol" + DateTime.UtcNow.Ticks.ToString();
            int maxLength = StringAttributeMetadata.MaxSupportedLength;

            string commandText = string.Format(@"ALTER TABLE {0} ADD {1} NVARCHAR({2})", entityName, newColumnName, maxLength.ToString());

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
