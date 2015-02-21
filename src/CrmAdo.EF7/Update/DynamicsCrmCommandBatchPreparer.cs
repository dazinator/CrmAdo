using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Data.Entity.DynamicsCrm.Update
{
    
        public class DynamicsCrmCommandBatchPreparer : CommandBatchPreparer
        {
            public DynamicsCrmCommandBatchPreparer(
                 DynamicsCrmModificationCommandBatchFactory modificationCommandBatchFactory,
                 ParameterNameGeneratorFactory parameterNameGeneratorFactory,
                 ModificationCommandComparer modificationCommandComparer)
                : base(modificationCommandBatchFactory, parameterNameGeneratorFactory, modificationCommandComparer)
            {
            }

            public override IRelationalPropertyExtensions GetPropertyExtensions(IProperty property)
            {
                //Check.NotNull(property, "property");

                return property.DynamicsCrm();
            }

            public override IRelationalEntityTypeExtensions GetEntityTypeExtensions(IEntityType entityType)
            {
               // Check.NotNull(entityType, "entityType");

                return entityType.DynamicsCrm();
            }
        }
    
}
