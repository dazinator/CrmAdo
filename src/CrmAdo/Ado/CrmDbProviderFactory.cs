using CrmAdo.Ado;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CrmAdo
{
    public class CrmDbProviderFactory : DbProviderFactory
    {      

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

        public override DbDataAdapter CreateDataAdapter()
        {
            return new CrmDataAdapter();
        }

        #region Not Implemented Yet

        public override bool CanCreateDataSourceEnumerator
        {
            get { return false; }
        }

        public override DbCommandBuilder CreateCommandBuilder()
        {
            //return new DbCommandBuilder()
            return new CrmCommandBuilder();
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

        private static object _EntityFrameworkServices;

        public object GetService(Type serviceType)
        {
            // In legacy Entity Framework, this is the entry point for obtaining CrmAdo's
            // implementation of DbProviderServices. We use reflection for all types to
            // avoid any dependencies on EF stuff in this project.

            if (serviceType != null && serviceType.FullName == "System.Data.Common.DbProviderServices")
            {
                // User has requested a legacy EF DbProviderServices implementation. Check our cache first.
                if (_EntityFrameworkServices != null)
                    return _EntityFrameworkServices;

                // First time, attempt to find the Npgsql.EntityFrameworkLegacy assembly and load the type via reflection
                var assemblyName = typeof(CrmDbProviderFactory).Assembly.GetName();
                assemblyName.Name = "CrmEF";
                Assembly npgsqlEfAssembly;
                try
                {
                    npgsqlEfAssembly = Assembly.Load(assemblyName.FullName);
                }
                catch (Exception e)
                {
                    throw new Exception("Could not load CrmEF assembly, is it installed?", e);
                }

                Type providerServicesType;
                if ((providerServicesType = npgsqlEfAssembly.GetType("CrmEF.CrmEfProviderServices")) == null ||
                    providerServicesType.GetProperty("Instance") == null)
                    throw new Exception("CrmEF assembly does not seem to contain the correct type!");

                return _EntityFrameworkServices = providerServicesType.InvokeMember("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null, new object[0]);
            }

            return null;
        }


    }
}
