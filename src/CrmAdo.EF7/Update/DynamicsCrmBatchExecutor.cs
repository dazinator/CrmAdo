using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Data.Entity.DynamicsCrm.Update
{

    public class DynamicsCrmBatchExecutor : BatchExecutor
    {
        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected DynamicsCrmBatchExecutor()
        {
        }

        public DynamicsCrmBatchExecutor(
           DynamicsCrmTypeMapper typeMapper,
            DbContextService<DbContext> context,
            ILoggerFactory loggerFactory)
            : base(typeMapper, context, loggerFactory)
        {
        }
    }

}
