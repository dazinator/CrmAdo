using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Relational;

namespace Microsoft.Data.Entity.DynamicsCrm
{

    public class DynamicsCrmDbContextOptions : RelationalDbContextOptions
    {
        public DynamicsCrmDbContextOptions(DbContextOptions options)
            : base(options)
        { }

        public virtual DynamicsCrmDbContextOptions MaxBatchSize(int maxBatchSize)
        {
            ((IDbContextOptions)Options)
                .AddOrUpdateExtension<DynamicsCrmOptionsExtension>(x => x.MaxBatchSize = maxBatchSize);

            return this;
        }

        public virtual DynamicsCrmDbContextOptions CommandTimeout(int? commandTimeout)
        {
            ((IDbContextOptions)Options)
                .AddOrUpdateExtension<DynamicsCrmOptionsExtension>(x => x.CommandTimeout = commandTimeout);

            return this;
        }

        public virtual DynamicsCrmDbContextOptions MigrationsAssembly(string assemblyName)
        {
           // Check.NotEmpty(assemblyName, nameof(assemblyName));

            ((IDbContextOptions)Options)
                .AddOrUpdateExtension<DynamicsCrmOptionsExtension>(x => x.MigrationsAssembly = assemblyName);

            return this;
        }
    }

}
