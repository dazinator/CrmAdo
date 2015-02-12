using CrmAdo.EntityFramework;
using Microsoft.Data.Entity.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace

namespace Microsoft.Data.Entity
{
    public static class DynamicsCrmDatabaseExtensions
    {

        public static DynamicsCrmDatabase AsDynamicsCrm(this Database database)
        {
            //Check.NotNull(database, "database");

            var db = database as DynamicsCrmDatabase;

            if (db == null)
            {
                throw new InvalidOperationException(Strings.SqlServerNotInUse);
            }

            return db;
        }

    }
}
