using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm.Metadata
{
    public class ReadOnlyDynamicsCrmPropertyExtensions : ReadOnlyRelationalPropertyExtensions, IDynamicsCrmPropertyExtensions
    {
        protected const string DynamicsCrmNameAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.ColumnName;
        protected const string DynamicsCrmColumnTypeAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.ColumnType;
        protected const string DynamicsCrmDefaultExpressionAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.ColumnDefaultExpression;
        protected const string DynamicsCrmValueGenerationAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.ValueGeneration;
        protected const string DynamicsCrmSequenceNameAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.SequenceName;
        protected const string DynamicsCrmSequenceSchemaAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.SequenceSchema;
        protected const string DynamicsCrmDefaultValueAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.ColumnDefaultValue;
        protected const string DynamicsCrmDefaultValueTypeAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.ColumnDefaultValueType;
        protected const string DynamicsCrmComputedExpressionAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.ColumnComputedExpression;

        public ReadOnlyDynamicsCrmPropertyExtensions(IProperty property)
            : base(property)
        {
        }

        public override string Column
        {
            get { return Property[DynamicsCrmNameAnnotation] ?? base.Column; }
        }

        public override string ColumnType
        {
            get { return Property[DynamicsCrmColumnTypeAnnotation] ?? base.ColumnType; }
        }

        public override string DefaultExpression
        {
            get { return Property[DynamicsCrmDefaultExpressionAnnotation] ?? base.DefaultExpression; }
        }

        public override object DefaultValue
        {
            get
            {
                return new TypedAnnotation(Property[DynamicsCrmDefaultValueTypeAnnotation], Property[DynamicsCrmDefaultValueAnnotation]).Value
                       ?? base.DefaultValue;
            }
        }

        public virtual string ComputedExpression
        {
            get { return Property[DynamicsCrmComputedExpressionAnnotation]; }
        }      

        public virtual DynamicsCrmValueGenerationStrategy? ValueGenerationStrategy
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Property[DynamicsCrmValueGenerationAnnotation];
                return value == null ? null : (DynamicsCrmValueGenerationStrategy?)Enum.Parse(typeof(DynamicsCrmValueGenerationStrategy), value);
            }
        }

        public virtual string SequenceName
        {
            get { return Property[DynamicsCrmSequenceNameAnnotation]; }
        }

        public virtual string SequenceSchema
        {
            get { return Property[DynamicsCrmSequenceSchemaAnnotation]; }
        }

        public virtual Sequence TryGetSequence()
        {
            var modelExtensions = Property.EntityType.Model.DynamicsCrm();

            if (ValueGenerationStrategy != DynamicsCrmValueGenerationStrategy.Sequence
                && (ValueGenerationStrategy != null
                    || modelExtensions.ValueGenerationStrategy != DynamicsCrmValueGenerationStrategy.Sequence))
            {
                return null;
            }

            var sequenceName = SequenceName
                               ?? modelExtensions.DefaultSequenceName
                               ?? Sequence.DefaultName;

            var sequenceSchema = SequenceSchema
                                 ?? modelExtensions.DefaultSequenceSchema;

            return modelExtensions.TryGetSequence(sequenceName, sequenceSchema)
                   ?? new Sequence(Sequence.DefaultName);
        }
    }
}
