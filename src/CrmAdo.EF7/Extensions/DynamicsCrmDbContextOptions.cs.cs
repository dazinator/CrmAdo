using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.SqlServer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework.Extensions
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
        }
   
}
