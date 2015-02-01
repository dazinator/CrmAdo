using CrmAdo.EntityFramework.Metadata;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework
{

    public class ReadOnlyDynamicsCrmModelExtensions : ReadOnlyRelationalModelExtensions, IDynamicsCrmModelExtensions
    {
        protected const string DynamicsCrmValueGenerationAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.ValueGeneration;
        protected const string DynamicsCrmSequenceAnnotation = DynamicsCrmAnnotationNames.Prefix + RelationalAnnotationNames.Sequence;
        protected const string DynamicsCrmDefaultSequenceNameAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.DefaultSequenceName;
        protected const string DynamicsCrmDefaultSequenceSchemaAnnotation = DynamicsCrmAnnotationNames.Prefix + DynamicsCrmAnnotationNames.DefaultSequenceSchema;

        public ReadOnlyDynamicsCrmModelExtensions(IModel model)
            : base(model)
        {
        }

        public virtual DynamicsCrmValueGenerationStrategy? ValueGenerationStrategy
        {
            get
            {
                // TODO: Issue #777: Non-string annotations
                var value = Model[DynamicsCrmValueGenerationAnnotation];
                return value == null ? null : (DynamicsCrmValueGenerationStrategy?)Enum.Parse(typeof(DynamicsCrmValueGenerationStrategy), value);
            }
        }

        public virtual string DefaultSequenceName
        {
            get { return Model[DynamicsCrmDefaultSequenceNameAnnotation]; }
        }

        public virtual string DefaultSequenceSchema
        {
            get { return Model[DynamicsCrmDefaultSequenceSchemaAnnotation]; }
        }

        public override IReadOnlyList<Sequence> Sequences
        {
            get
            {
                var sqlServerSequences = (
                        from a in Model.Annotations
                        where a.Name.StartsWith(DynamicsCrmSequenceAnnotation)
                        select Sequence.Deserialize(a.Value))
                    .ToList();

                return base.Sequences
                    .Where(rs => !sqlServerSequences.Any(ss => ss.Name == rs.Name && ss.Schema == rs.Schema))
                    .Concat(sqlServerSequences)
                    .ToList();
            }
        }

        public override Sequence TryGetSequence(string name, string schema = null)
        {
            //Check.NotEmpty(name, "name");
            // Check.NullButNotEmpty(schema, "schema");

            return FindSequence(DynamicsCrmSequenceAnnotation + schema + "." + name)
                   ?? base.TryGetSequence(name, schema);
        }
    }

}
