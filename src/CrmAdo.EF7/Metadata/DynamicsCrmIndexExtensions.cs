using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Data.Entity.DynamicsCrm.Metadata
{

    public class DynamicsCrmIndexExtensions : ReadOnlyDynamicsCrmIndexExtensions
        {
            public DynamicsCrmIndexExtensions(Index index)
                : base(index)
            {
            }

           // [CanBeNull]
            public new virtual string Name
            {
                get { return base.Name; }
              //  [param: CanBeNull]
                set
                {
                  //  Check.NullButNotEmpty(value, "value");

                    ((Index)Index)[DynamicsCrmNameAnnotation] = value;
                }
            }

           // [CanBeNull]
            public new virtual bool? IsClustered
            {
                get { return base.IsClustered; }
             //   [param: CanBeNull]
                set
                {
                    // TODO: Issue #777: Non-string annotations
                    ((Index)Index)[DynamicsCrmClusteredAnnotation] = value == null ? null : value.ToString();
                }
            }
        }
   
}
