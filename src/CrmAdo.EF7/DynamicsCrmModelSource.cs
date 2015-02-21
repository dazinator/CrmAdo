using Microsoft.Data.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Data.Entity.DynamicsCrm
{   

    public class DynamicsCrmModelSource : ModelSourceBase
    {
        public DynamicsCrmModelSource(DbSetFinder setFinder, ModelValidator modelValidator)
            : base(setFinder, modelValidator)
        {
        }
    }
}
