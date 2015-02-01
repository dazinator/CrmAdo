using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Metadata
{
   
        public class DynamicsCrmForeignKeyExtensions : ReadOnlyDynamicsCrmForeignKeyExtensions
        {
            public DynamicsCrmForeignKeyExtensions(ForeignKey foreignKey)
                : base(foreignKey)
            {
            }

            //[CanBeNull]
            public new virtual string Name
            {
                get { return base.Name; }
               // [param: CanBeNull]
                set
                {
                 //   Check.NullButNotEmpty(value, "value");

                    ((ForeignKey)ForeignKey)[DynamicsCrmNameAnnotation] = value;
                }
            }
        }
    
}
