using CrmAdo.Ado;
using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace CrmAdo
{
    public class CrmDbProviderFactory : DbProviderFactory
    {

        public const string Invariant = "System.Data.DynamicsCrm.CrmAdo";

        /// <summary>
        /// Every provider factory must have an Instance public field
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Every provider implementation must have an Instance field, this constraint is enforced by the .NET provider pattern.")]
        public static CrmDbProviderFactory Instance = new CrmDbProviderFactory();
        
        public override DbConnection CreateConnection()
        {
            return new CrmDbConnection();
        }

        public override DbCommand CreateCommand()
        {
            return new CrmDbCommand();
        }

        public override DbParameter CreateParameter()
        {
            return new CrmParameter();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new CrmConnectionStringBuilder();
        }

        #region Not Implemented Yet

        public override bool CanCreateDataSourceEnumerator
        {
            get { return false; }
        }

        public override DbCommandBuilder CreateCommandBuilder()
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

        public override System.Security.CodeAccessPermission CreatePermission(System.Security.Permissions.PermissionState state)
        {
            throw new NotSupportedException();
        }

        #endregion

    }
}
