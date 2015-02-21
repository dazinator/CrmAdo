using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace Microsoft.Data.Entity.DynamicsCrm.Metadata
{
    public class DynamicsCrmPropertyBuilder
    {
         private readonly Property _property;

         public DynamicsCrmPropertyBuilder(Property property)
        {
           // Check.NotNull(property, "property");

            _property = property;
        }

         public virtual DynamicsCrmPropertyBuilder Column(string columnName)
        {
           // Check.NullButNotEmpty(columnName, "columnName");

            _property.DynamicsCrm().Column = columnName;

            return this;
        }

         public virtual DynamicsCrmPropertyBuilder ColumnType(string columnType)
        {
           // Check.NullButNotEmpty(columnType, "columnType");

            _property.DynamicsCrm().ColumnType = columnType;

            return this;
        }

         public virtual DynamicsCrmPropertyBuilder DefaultExpression(string expression)
        {
           // Check.NullButNotEmpty(expression, "expression");

            _property.DynamicsCrm().DefaultExpression = expression;

            return this;
        }

         public virtual DynamicsCrmPropertyBuilder DefaultValue(object value)
        {
            _property.DynamicsCrm().DefaultValue = value;

            return this;
        }

         public virtual DynamicsCrmPropertyBuilder ComputedExpression(string expression)
        {
          //  Check.NullButNotEmpty(expression, nameof(expression));

            _property.DynamicsCrm().ComputedExpression = expression;

            return this;
        }

         public virtual DynamicsCrmPropertyBuilder UseSequence()
        {
            _property.DynamicsCrm().ValueGenerationStrategy = DynamicsCrmValueGenerationStrategy.Sequence;
            _property.DynamicsCrm().SequenceName = null;
            _property.DynamicsCrm().SequenceSchema = null;

            return this;
        }

         public virtual DynamicsCrmPropertyBuilder UseSequence(string name, string schema = null)
        {
           // Check.NotEmpty(name, "name");
           // Check.NullButNotEmpty(schema, "schema");

            var sequence = _property.EntityType.Model.DynamicsCrm().GetOrAddSequence(name, schema);

            _property.DynamicsCrm().ValueGenerationStrategy = DynamicsCrmValueGenerationStrategy.Sequence;
            _property.DynamicsCrm().SequenceName = sequence.Name;
            _property.DynamicsCrm().SequenceSchema = sequence.Schema;

            return this;
        }

         public virtual DynamicsCrmPropertyBuilder UseIdentity()
        {
            _property.DynamicsCrm().ValueGenerationStrategy = DynamicsCrmValueGenerationStrategy.Identity;
            _property.DynamicsCrm().SequenceName = null;
            _property.DynamicsCrm().SequenceSchema = null;

            return this;
        }
    }
}
