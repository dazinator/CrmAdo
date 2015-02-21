using Microsoft.Data.Entity.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace Microsoft.Data.Entity.DynamicsCrm.Metadata
{
    public class DynamicsCrmIndexBuilder
    {
         private readonly Index _index;

         public DynamicsCrmIndexBuilder(Index index)
        {
           // Check.NotNull(index, "index");

            _index = index;
        }

         public virtual DynamicsCrmIndexBuilder Name(string name)
        {
           // Check.NullButNotEmpty(name, "name");

            _index.DynamicsCrm().Name = name;

            return this;
        }

         public virtual DynamicsCrmIndexBuilder Clustered(bool isClustered = true)
        {
            _index.DynamicsCrm().IsClustered = isClustered;

            return this;
        }
    }
}
