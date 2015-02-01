using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmAdo.EntityFramework.Update
{

    public class DynamicsCrmModificationCommandBatchFactory : ModificationCommandBatchFactory
    {
        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected DynamicsCrmModificationCommandBatchFactory()
        {
        }

        public DynamicsCrmModificationCommandBatchFactory(
            DynamicsCrmSqlGenerator sqlGenerator)
            : base(sqlGenerator)
        {
        }

        public override ModificationCommandBatch Create(IDbContextOptions options)
        {
            // Check.NotNull(options, "options");

            var optionsExtension = options.Extensions.OfType<DynamicsCrmOptionsExtension>().FirstOrDefault();

            int? maxBatchSize = null;
            if (optionsExtension != null)
            {
                maxBatchSize = optionsExtension.MaxBatchSize;
            }


            return new DynamicsCrmModificationCommandBatch((DynamicsCrmSqlGenerator)SqlGenerator, maxBatchSize);
        }
    }

}
