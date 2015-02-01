using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Metadata
{    
    public class DynamicsCrmModelExtensions : ReadOnlyDynamicsCrmModelExtensions
    {
        public DynamicsCrmModelExtensions(Model model) 
            : base(model)
        {
        }

       // [CanBeNull]
        public new virtual DynamicsCrmValueGenerationStrategy? ValueGenerationStrategy
        {
            get { return base.ValueGenerationStrategy; }
          //  [param: CanBeNull]
            set
            {
                // TODO: Issue #777: Non-string annotations
                ((Model)Model)[DynamicsCrmValueGenerationAnnotation] = value == null ? null : value.ToString();
            }
        }

        public new virtual string DefaultSequenceName
        {
            get { return base.DefaultSequenceName; }
           // [param: CanBeNull]
            set
            {
               // Check.NullButNotEmpty(value, "value");

                ((Model)Model)[DynamicsCrmDefaultSequenceNameAnnotation] = value;
            }
        }

        public new virtual string DefaultSequenceSchema
        {
            get { return base.DefaultSequenceSchema; }
           // [param: CanBeNull]
            set
            {
               // Check.NullButNotEmpty(value, "value");

                ((Model)Model)[DynamicsCrmDefaultSequenceSchemaAnnotation] = value;
            }
        }

        public virtual Sequence AddOrReplaceSequence(Sequence sequence)
        {
           // Check.NotNull(sequence, "sequence");

            var model = (Model)Model;
            sequence.Model = model;
            model[DynamicsCrmSequenceAnnotation + sequence.Schema + "." + sequence.Name] = sequence.Serialize();

            return sequence;
        }

        public virtual Sequence GetOrAddSequence(string name = null, string schema = null)
        {
           // Check.NullButNotEmpty(name, "name");
           // Check.NullButNotEmpty(schema, "schema");

            name = name ?? Sequence.DefaultName;

            return ((Model)Model).DynamicsCrm().TryGetSequence(name, schema)
                   ?? AddOrReplaceSequence(new Sequence(name, schema));
        }
    }
}
