using Microsoft.Data.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework
{
    public class DynamicsCrmModelSource : ModelSourceBase
    {
        public DynamicsCrmModelSource(DbSetFinder setFinder)
            : base(setFinder)
        {
        }
    }
}
