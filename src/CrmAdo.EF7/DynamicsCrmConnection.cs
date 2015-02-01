using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Storage;
using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.EntityFramework
{

    public class DynamicsCrmConnection : RelationalConnection
    {

        public static DbProviderFactory _DbProviderFactory = DbProviderFactories.GetFactory(CrmAdo.CrmAdoConstants.Invariant);


        /// <summary>
        ///     This constructor is intended only for use when creating test doubles that will override members
        ///     with mocked or faked behavior. Use of this constructor for other purposes may result in unexpected
        ///     behavior including but not limited to throwing <see cref="NullReferenceException" />.
        /// </summary>
        protected DynamicsCrmConnection()
        {
        }

        public DynamicsCrmConnection(DbContextService<IDbContextOptions> options, ILoggerFactory loggerFactory)
            : base(options, loggerFactory)
        {
        }

        protected override DbConnection CreateDbConnection()
        {
            // TODO: Consider using DbProviderFactory to create connection instance
            // Issue #774
            var conn = _DbProviderFactory.CreateConnection();
            conn.ConnectionString = ConnectionString;
            return conn;
        }

        public virtual DynamicsCrmConnection CreateDiscoveryServiceConnection()
        {
            throw new NotImplementedException();

            //var builder = new CrmConnectionStringBuilder { ConnectionString = ConnectionString };
            ////builder. = "master";

            //// TODO use clone connection method once implimented see #1406
            //var options = new DbContextOptions();
            //options.UseDynamicsCrm(builder.ConnectionString).CommandTimeout(CommandTimeout);

            //return new DynamicsCrmConnection(new DbContextService<IDbContextOptions>(() => options), LoggerFactory);
        }
    }


}
