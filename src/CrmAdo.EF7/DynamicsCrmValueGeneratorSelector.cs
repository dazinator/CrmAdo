using Microsoft.Data.Entity;
using Microsoft.Data.Entity.DynamicsCrm.Metadata;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm
{
  
    public class DynamicsCrmValueGeneratorSelector : ValueGeneratorSelector
    {
        private readonly DynamicsCrmValueGeneratorCache _cache;
        private readonly DynamicsCrmSequenceValueGeneratorFactory _sequenceFactory;
        private readonly ValueGeneratorFactory<SequentialGuidValueGenerator> _sequentialGuidFactory;
        private readonly DynamicsCrmConnection _connection;

        public DynamicsCrmValueGeneratorSelector(
            DynamicsCrmValueGeneratorCache cache,
           ValueGeneratorFactory<GuidValueGenerator> guidFactory,
           TemporaryIntegerValueGeneratorFactory integerFactory,
           ValueGeneratorFactory<TemporaryStringValueGenerator> stringFactory,
           ValueGeneratorFactory<TemporaryBinaryValueGenerator> binaryFactory,
            DynamicsCrmSequenceValueGeneratorFactory sequenceFactory,
           ValueGeneratorFactory<SequentialGuidValueGenerator> sequentialGuidFactory,
            DynamicsCrmConnection connection)
            : base(guidFactory, integerFactory, stringFactory, binaryFactory)
        {
          //  Check.NotNull(cache, nameof(cache));
          //  Check.NotNull(sequenceFactory, nameof(sequenceFactory));
          //  Check.NotNull(sequentialGuidFactory, nameof(sequentialGuidFactory));
          //  Check.NotNull(connection, nameof(connection));

            _cache = cache;
            _sequenceFactory = sequenceFactory;
            _sequentialGuidFactory = sequentialGuidFactory;
            _connection = connection;
        }

        public override ValueGenerator Select(IProperty property)
        {
            //Check.NotNull(property, nameof(property));

            var strategy = property.DynamicsCrm().ValueGenerationStrategy
                           ?? property.EntityType.Model.DynamicsCrm().ValueGenerationStrategy;

            if (property.PropertyType.IsInteger()
                && strategy == DynamicsCrmValueGenerationStrategy.Sequence)
            {
                return _sequenceFactory.Create(property, _cache.GetOrAddSequenceState(property), _connection);
            }

            return _cache.GetOrAdd(property, Create);
        }

        public override ValueGenerator Create(IProperty property)
        {
            //Check.NotNull(property, nameof(property));

            return property.PropertyType == typeof(Guid)
                ? _sequentialGuidFactory.Create(property)
                : base.Create(property);
        }
    }
}
