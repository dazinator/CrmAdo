using CrmAdo.EntityFramework;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Data.Entity.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm
{
   

    public class DynamicsCrmValueGeneratorCache : ValueGeneratorCache
    {
        private readonly ThreadSafeDictionaryCache<string, DynamicsCrmSequenceValueGeneratorState> _sequenceGeneratorCache
            = new ThreadSafeDictionaryCache<string, DynamicsCrmSequenceValueGeneratorState>();

        public virtual DynamicsCrmSequenceValueGeneratorState GetOrAddSequenceState(IProperty property)
        {
          //  Check.NotNull(property, nameof(property));

            return _sequenceGeneratorCache.GetOrAdd(
                GetSequenceName(property),
                sequenceName => new DynamicsCrmSequenceValueGeneratorState(sequenceName, GetBlockSize(property), GetPoolSize(property)));
        }

        public virtual int GetBlockSize(IProperty property)
        {
           // Check.NotNull(property, nameof(property));

            var incrementBy = property.DynamicsCrm().TryGetSequence().IncrementBy;

            if (incrementBy <= 0)
            {
                throw new NotSupportedException(Strings.SequenceBadBlockSize(incrementBy, GetSequenceName(property)));
            }

            return incrementBy;
        }

        public virtual string GetSequenceName( IProperty property)
        {
            //Check.NotNull(property, nameof(property));
            var sequence = property.DynamicsCrm().TryGetSequence();
            return (sequence.Schema == null ? "" : (sequence.Schema + ".")) + sequence.Name;
        }

        public virtual int GetPoolSize(IProperty property)
        {
           // Check.NotNull(property, nameof(property));

            // TODO: Allow configuration without creation of derived factory type
            // Issue #778
            return 5;
        }
    }

   

}
