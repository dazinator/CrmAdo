using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Metadata
{
    
        public class DynamicsCrmEntityTypeExtensions : ReadOnlyDynamicsCrmEntityTypeExtensions
        {
            public DynamicsCrmEntityTypeExtensions(EntityType entityType)
                : base(entityType)
            {
            }

            public new virtual string Table
            {
                get { return base.Table; }
                //[param: CanBeNull]
                set
                {
                    //Check.NullButNotEmpty(value, "value");

                    ((EntityType)EntityType)[DynamicsCrmTableAnnotation] = value;
                }
            }

         
            public new virtual string Schema
            {
                get { return base.Schema; }
              //  [param: CanBeNull]
                set
                {
                   // Check.NullButNotEmpty(value, "value");

                    ((EntityType)EntityType)[DynamicsCrmSchemaAnnotation] = value;
                }
            }
        }

    
}
