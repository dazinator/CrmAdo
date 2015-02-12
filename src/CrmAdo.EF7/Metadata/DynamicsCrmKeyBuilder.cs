using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace CrmAdo.EntityFramework.Metadata
{
    public class DynamicsCrmKeyBuilder
    {
         private readonly Key _key;

         public DynamicsCrmKeyBuilder(Key key)
        {
            //Check.NotNull(key, "key");

            _key = key;
        }

         public virtual DynamicsCrmKeyBuilder Name(string name)
        {
           // Check.NullButNotEmpty(name, "name");

            _key.DynamicsCrm().Name = name;

            return this;
        }

         public virtual DynamicsCrmKeyBuilder Clustered(bool isClustered = true)
        {
            _key.DynamicsCrm().IsClustered = isClustered;

            return this;
        }
    }
}
