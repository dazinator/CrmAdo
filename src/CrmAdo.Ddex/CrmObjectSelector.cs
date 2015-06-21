using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{   

    /// <summary>
    /// Represents a custom data object selector to supplement or replace
    /// the schema collections supplied by the .NET Framework Data Provider
    /// for Crm.  Many of the enumerations here are required for full
    /// support of the built in data design scenarios.
    /// </summary>
    public class CrmObjectSelector : AdoDotNetObjectSelector
    {
        public CrmObjectSelector()
            : base()
        {

        }

        public CrmObjectSelector(IVsDataConnection connection)
            : base(connection)
        {


        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            // DataSourceInformation;
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            //   DataSourceInformation.IdentifierCloseQuote
            // Execute a SQL statement to get the property values
            DbConnection conn = Site.GetLockedProviderObject() as DbConnection;
            Debug.Assert(conn != null, "Invalid provider object.");
            if (conn == null)
            {
                // This should never occur
                throw new NotSupportedException();
            }
            try
            {
                // Ensure the connection is open
                if (Site.State != DataConnectionState.Open)
                {
                    Site.Open();
                }

                // Create a command object       
                DbCommand comm = (DbCommand)conn.CreateCommand();

                // Choose and format SQL based on the type
                bool isHandled;
                comm.CommandText = GetCommandText(typeName, out isHandled, restrictions);
                if (isHandled)
                {
                    var r = comm.ExecuteReader();
                    var reader = new AdoDotNetReader(r);
                    return reader;
                }
                else
                {
                    //  object[] par = parameters
                    if (parameters == null)
                    {
                        parameters = new object[] { typeName };
                    }


                    //if(typeName.ToLower()  == "Columns")
                    //{
                    //    par[0] = typeName;
                    //}

                    var tables = base.SelectObjects(typeName, restrictions, properties, parameters);
                    //#if DEBUG
                    //                    while (tables.Read())
                    //                    {
                    //                        Debug.Write(tables.GetItem("table_name"));
                    //                    }
                    //#endif

                    return tables;
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;

            }
            finally
            {
                Site.UnlockProviderObject();
            }
        }
        
        private string GetCommandText(string typeName, out bool isHandled, object[] restrictions)
        {
            isHandled = false;
            if (string.IsNullOrWhiteSpace(typeName) || typeName.Equals(CrmObjectTypes.Root, StringComparison.OrdinalIgnoreCase))
            {
                isHandled = true;
                return "SELECT name FROM organization";
            }

            //if (typeName.Equals(CrmObjectTypes.CrmTable, StringComparison.OrdinalIgnoreCase))
            //{
            //    return "SELECT * FROM entitymetadata";
            //}

            //if (typeName.Equals(CrmObjectTypes.CrmColumn, StringComparison.OrdinalIgnoreCase))
            //{
            //    if (restrictions == null)
            //    {
            //        throw new ArgumentNullException("must provide entity name restriction");
            //    }
            //    var entityName = restrictions.First();
            //    var commandText = "SELECT entitymetadata.PrimaryIdAttribute, attributemetadata.* FROM entitymetadata INNER JOIN attributemetadata ON entitymetadata.MetadataId = attributemetadata.MetadataId WHERE entitymetadata.LogicalName = '{0}'";
            //    return string.Format(commandText, entityName);
            //}

            if (typeName.Equals(CrmObjectTypes.PluginAssembly, StringComparison.OrdinalIgnoreCase))
            {
                isHandled = true;
                //if (restrictions == null)
                //{
                //    throw new ArgumentNullException("must provide entity name restriction");
                //}
                //  var entityName = restrictions.First();
                var commandText = "SELECT * FROM pluginassembly";
                return commandText;
                //   return string.Format(commandText, entityName);
            }

            return string.Empty;
        }

    }


}
