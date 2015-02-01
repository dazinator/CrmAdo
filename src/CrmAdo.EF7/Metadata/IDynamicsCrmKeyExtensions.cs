using Microsoft.Data.Entity.Relational.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework.Metadata
{
    public interface IDynamicsCrmKeyExtensions : IRelationalKeyExtensions
    {      
        bool? IsClustered { get; }
    }

}
