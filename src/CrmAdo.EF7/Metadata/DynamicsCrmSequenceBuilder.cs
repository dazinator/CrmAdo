using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace CrmAdo.EntityFramework.Metadata
{
    public class DynamicsCrmSequenceBuilder
    {
         private Sequence _sequence;

         public DynamicsCrmSequenceBuilder(Sequence sequence)
        {
            //Check.NotNull(sequence, "sequence");

            _sequence = sequence;
        }

        public virtual DynamicsCrmSequenceBuilder IncrementBy(int increment)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                increment,
                _sequence.MinValue,
                _sequence.MaxValue,
                _sequence.Type);

            model.DynamicsCrm().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual DynamicsCrmSequenceBuilder Start(long startValue)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                startValue,
                _sequence.IncrementBy,
                _sequence.MinValue,
                _sequence.MaxValue,
                _sequence.Type);

            model.DynamicsCrm().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual DynamicsCrmSequenceBuilder Type<T>()
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                _sequence.IncrementBy,
                _sequence.MinValue,
                _sequence.MaxValue,
                typeof(T));

            model.DynamicsCrm().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual DynamicsCrmSequenceBuilder Max(long maximum)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                _sequence.IncrementBy,
                _sequence.MinValue,
                maximum,
                _sequence.Type);

            model.DynamicsCrm().AddOrReplaceSequence(_sequence);

            return this;
        }

        public virtual DynamicsCrmSequenceBuilder Min(long minimum)
        {
            var model = (Model)_sequence.Model;

            _sequence = new Sequence(
                _sequence.Name,
                _sequence.Schema,
                _sequence.StartValue,
                _sequence.IncrementBy,
                minimum,
                _sequence.MaxValue,
                _sequence.Type);

            model.DynamicsCrm().AddOrReplaceSequence(_sequence);

            return this;
        }

    }
}
