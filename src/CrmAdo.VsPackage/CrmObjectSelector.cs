﻿using Microsoft.VisualStudio.Data.Framework;
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
                comm.CommandText = GetCommandText(typeName);            

                return new AdoDotNetReader(comm.ExecuteReader());
            }
            finally
            {
                Site.UnlockProviderObject();
            }
        }

        private string GetCommandText(string typeName)
        {
            if (typeName.Equals(CrmObjectTypes.Root, StringComparison.OrdinalIgnoreCase))
            {
                return "SELECT name FROM organization";
            }

            if (typeName.Equals(CrmObjectTypes.Table, StringComparison.OrdinalIgnoreCase))
            {
                return "SELECT * FROM entitymetadata";
            }

            throw new NotSupportedException();
        }

    }
}
