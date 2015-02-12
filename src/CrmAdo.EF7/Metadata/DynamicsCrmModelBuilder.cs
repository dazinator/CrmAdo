using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace CrmAdo.EntityFramework.Metadata
{
    public class DynamicsCrmModelBuilder
    {
         private readonly Model _model;

         public DynamicsCrmModelBuilder( Model model)
        {
           // Check.NotNull(model, "model");

            _model = model;
        }

         public virtual DynamicsCrmModelBuilder UseIdentity()
        {
            _model.DynamicsCrm().ValueGenerationStrategy = DynamicsCrmValueGenerationStrategy.Identity;

            return this;
        }

         public virtual DynamicsCrmModelBuilder UseSequence()
        {
            var extensions = _model.DynamicsCrm();

            extensions.ValueGenerationStrategy = DynamicsCrmValueGenerationStrategy.Sequence;
            extensions.DefaultSequenceName = null;
            extensions.DefaultSequenceSchema = null;

            return this;
        }

         public virtual DynamicsCrmModelBuilder UseSequence(string name, string schema = null)
        {
           // Check.NotEmpty(name, "name");
           // Check.NullButNotEmpty(schema, "schema");

            var extensions = _model.DynamicsCrm();

            var sequence = extensions.GetOrAddSequence(name, schema);

            extensions.ValueGenerationStrategy = DynamicsCrmValueGenerationStrategy.Sequence;
            extensions.DefaultSequenceName = sequence.Name;
            extensions.DefaultSequenceSchema = sequence.Schema;

            return this;
        }

         public virtual DynamicsCrmSequenceBuilder Sequence(string name = null, string schema = null)
        {
           // Check.NullButNotEmpty(name, "name");
           // Check.NullButNotEmpty(schema, "schema");

            return new DynamicsCrmSequenceBuilder(_model.DynamicsCrm().GetOrAddSequence(name, schema));
        }
    }
}
