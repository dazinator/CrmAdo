using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrmAdo.EntityFramework.Utils;

namespace CrmAdo.EntityFramework.Metadata
{
    public class DynamicsCrmPropertyExtensions : ReadOnlyDynamicsCrmPropertyExtensions
    {
        public DynamicsCrmPropertyExtensions(Property property)
            : base(property)
        {
        }

        public new virtual string Column
        {
            get { return base.Column; }
           
            set
            {
                //Check.NullButNotEmpty(value, "value");

                ((Property)Property)[DynamicsCrmNameAnnotation] = value;
            }
        }

       
        public new virtual string ColumnType
        {
            get { return base.ColumnType; }
           
            set
            {
                //Check.NullButNotEmpty(value, "value");

                ((Property)Property)[DynamicsCrmColumnTypeAnnotation] = value;
            }
        }

       
        public new virtual string DefaultExpression
        {
            get { return base.DefaultExpression; }
            //[param: CanBeNull]
            set
            {
              //  Check.NullButNotEmpty(value, "value");

                ((Property)Property)[DynamicsCrmDefaultExpressionAnnotation] = value;
            }
        }

        public new virtual object DefaultValue
        {
            get { return base.DefaultValue; }
           // [param: CanBeNull]
            set
            {
                var typedAnnotation = new TypedAnnotation(value);

                ((Property)Property)[DynamicsCrmDefaultValueTypeAnnotation] = typedAnnotation.TypeString;
                ((Property)Property)[DynamicsCrmDefaultValueAnnotation] = typedAnnotation.ValueString;
            }
        }

      
        public new virtual string ComputedExpression
        {
            get { return base.ComputedExpression(); }           
            set
            {
              //  Check.NullButNotEmpty(value, nameof(value));

                ((Property)Property)[DynamicsCrmComputedExpressionAnnotation] = value;
            }
        }

        
        public new virtual string SequenceName
        {
            get { return base.SequenceName; }
           // [param: CanBeNull]
            set
            {
           //     Check.NullButNotEmpty(value, "value");

                ((Property)Property)[DynamicsCrmSequenceNameAnnotation] = value;
            }
        }

    
        public new virtual string SequenceSchema
        {
            get { return base.SequenceSchema; }
        //    [param: CanBeNull]
            set
            {
         //       Check.NullButNotEmpty(value, "value");

                ((Property)Property)[DynamicsCrmSequenceSchemaAnnotation] = value;
            }
        }

       
        public new virtual DynamicsCrmValueGenerationStrategy? ValueGenerationStrategy
        {
            get { return base.ValueGenerationStrategy; }
           // [param: CanBeNull]
            set
            {
                var property = ((Property)Property);

                if (value == null)
                {
                    property[DynamicsCrmValueGenerationAnnotation] = null;
                    property.GenerateValueOnAdd = null;
                }
                else
                {
                    var propertyType = Property.PropertyType;

                    if (value == DynamicsCrmValueGenerationStrategy.Identity
                        && (!propertyType.IsInteger()
                            || propertyType == typeof(byte)
                            || propertyType == typeof(byte?)))
                    {
                        throw new ArgumentException(Strings.IdentityBadType(
                            Property.Name, Property.EntityType.Name, propertyType.Name));
                    }

                    if (value == DynamicsCrmValueGenerationStrategy.Sequence
                        && !propertyType.IsInteger())
                    {
                        throw new ArgumentException(Strings.SequenceBadType(
                            Property.Name, Property.EntityType.Name, propertyType.Name));
                    }

                    // TODO: Issue #777: Non-string annotations
                    property[DynamicsCrmValueGenerationAnnotation] = value.ToString();
                    property.GenerateValueOnAdd = true;
                }
            }
        }
    }
}
