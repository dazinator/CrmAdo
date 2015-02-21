using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm.Metadata
{

    public class DynamicsCrmKeyExtensions : ReadOnlyDynamicsCrmKeyExtensions
        {
            public DynamicsCrmKeyExtensions(Key key)
                : base(key)
            {
            }
         
            public new virtual string Name
            {
                get { return base.Name; }
                //[param: CanBeNull]
                set
                {
                  //  Check.NullButNotEmpty(value, "value");

                    ((Key)Key)[DynamicsCrmNameAnnotation] = value;
                }
            }

            //[CanBeNull]
            public new virtual bool? IsClustered
            {
                get { return base.IsClustered; }
             //   [param: CanBeNull]
                set
                {
                    // TODO: Issue #777: Non-string annotations
                    // TODO: Issue #700: Annotate associated index object instead
                    ((Key)Key)[DynamicsCrmClusteredAnnotation] = value == null ? null : value.ToString();
                }
            }
       

    }
}
