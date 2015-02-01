using Microsoft.Data.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework
{
    public class DynamicsCrmValueGeneratorCache : ValueGeneratorCache
    {
        public DynamicsCrmValueGeneratorCache(DynamicsCrmValueGeneratorSelector selector)
            : base(selector)
        {
        }
    }

   

}
