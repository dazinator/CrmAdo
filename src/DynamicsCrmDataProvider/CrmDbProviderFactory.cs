using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsCrmDataProvider
{
    public class CrmDbProviderFactory : DbProviderFactory
    {

        /// <summary>
        /// Every provider factory must have an Instance public field
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Every provider implementation must have an Instance field, this constraint is enforced by the .NET provider pattern.")]
        public static CrmDbProviderFactory Instance = new CrmDbProviderFactory();

        public override bool CanCreateDataSourceEnumerator
        {
            get { return false; }
        }
        
        public override DbConnection CreateConnection()
        {
            return new CrmDbConnection();
        }

        public override DbCommand CreateCommand()
        {
            return new CrmDbCommand();
        }

        #region Not Implemented Yet

        public override DbCommandBuilder CreateCommandBuilder()
        {
            throw new NotSupportedException();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            throw new NotSupportedException();
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            throw new NotSupportedException();
        }

        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            throw new NotSupportedException();
        }

        public override DbParameter CreateParameter()
        {
            throw new NotSupportedException();
        }

        public override System.Security.CodeAccessPermission CreatePermission(System.Security.Permissions.PermissionState state)
        {
            throw new NotSupportedException();
        }

        #endregion

    }
}
