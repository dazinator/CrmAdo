using CrmAdo.EntityFramework.Metadata;
using CrmAdo.EntityFramework.Utils;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework
{
    public class DynamicsCrmValueGeneratorSelector : ValueGeneratorSelector
    {
        private readonly DynamicsCrmSequenceValueGeneratorFactory _sequenceFactory;
        private readonly SimpleValueGeneratorFactory<SequentialGuidValueGenerator> _sequentialGuidFactory;

        public DynamicsCrmValueGeneratorSelector(
            SimpleValueGeneratorFactory<GuidValueGenerator> guidFactory,
            SimpleValueGeneratorFactory<TemporaryIntegerValueGenerator> integerFactory,
            SimpleValueGeneratorFactory<TemporaryStringValueGenerator> stringFactory,
            SimpleValueGeneratorFactory<TemporaryBinaryValueGenerator> binaryFactory,
            DynamicsCrmSequenceValueGeneratorFactory sequenceFactory,
            SimpleValueGeneratorFactory<SequentialGuidValueGenerator> sequentialGuidFactory)
            : base(guidFactory, integerFactory, stringFactory, binaryFactory)
        {      

            _sequenceFactory = sequenceFactory;
            _sequentialGuidFactory = sequentialGuidFactory;
        }

        public override IValueGeneratorFactory Select(IProperty property)
        {
           // Check.NotNull(property, "property");

            var strategy = property.DynamicsCrm().ValueGenerationStrategy
                           ?? property.EntityType.Model.DynamicsCrm().ValueGenerationStrategy;

            if (property.PropertyType.IsInteger()
                && strategy == DynamicsCrmValueGenerationStrategy.Sequence)
            {
                return _sequenceFactory;
            }

            if (property.PropertyType == typeof(Guid))
            {
                return _sequentialGuidFactory;
            }

            return base.Select(property);
        }
    }
}
