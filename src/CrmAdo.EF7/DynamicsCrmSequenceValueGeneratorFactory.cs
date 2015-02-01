using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework
{    
        public class DynamicsCrmSequenceValueGeneratorFactory : IValueGeneratorFactory
        {
            private readonly SqlStatementExecutor _executor;

            public DynamicsCrmSequenceValueGeneratorFactory(SqlStatementExecutor executor)
            {
               // Check.NotNull(executor, "executor");

                _executor = executor;
            }

            public virtual int GetBlockSize(IProperty property)
            {
              //  Check.NotNull(property, "property");

                var incrementBy = property.DynamicsCrm().TryGetSequence().IncrementBy;

                if (incrementBy <= 0)
                {
                    throw new NotSupportedException(Strings.SequenceBadBlockSize(incrementBy, GetSequenceName(property)));
                }

                return incrementBy;
            }

            public virtual string GetSequenceName(IProperty property)
            {
              //  Check.NotNull(property, "property");

                var sequence = property.DynamicsCrm().TryGetSequence();

                return (sequence.Schema == null ? "" : (sequence.Schema + ".")) + sequence.Name;
            }

            public virtual IValueGenerator Create(IProperty property)
            {
               // Check.NotNull(property, "property");

                return new DynamicsCrmSequenceValueGenerator(_executor, GetSequenceName(property), GetBlockSize(property));
            }

            public virtual int GetPoolSize(IProperty property)
            {
               // Check.NotNull(property, "property");

                // TODO: Allow configuration without creation of derived factory type
                // Issue #778
                return 5;
            }

            public virtual string GetCacheKey(IProperty property)
            {
               // Check.NotNull(property, "property");

                return GetSequenceName(property);
            }
        }
  
}
