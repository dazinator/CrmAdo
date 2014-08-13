using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
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
    internal class CrmObjectSelector : DataObjectSelector
    {
        protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }

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
                if (typeName.Equals(CrmObjectTypes.Root, StringComparison.OrdinalIgnoreCase))
                {
                    comm.CommandText = "SELECT name FROM organization";
                }
                else
                {
                    throw new NotSupportedException();
                }

                return new AdoDotNetReader(comm.ExecuteReader());
            }
            finally
            {
                Site.UnlockProviderObject();
            }
        }

    }
}
