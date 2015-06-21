using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    /// <summary>
    /// Represents a custom data source information class that is able to
    /// provide data source information values that require some form of
    /// computation, perhaps based on an active connection.
    /// </summary>
    internal class CrmSourceInformation : AdoDotNetSourceInformation
    {
        #region Constructors


        public CrmSourceInformation(IVsDataConnection site)
            : base(site)
        {
            AddProperties();
        }

        public CrmSourceInformation()
            : base()
        {
            AddProperties();
        }

        private void AddProperties()
        {
            AddProperty(DefaultSchema, "dbo");
            AddProperty(CatalogSeparator, ".");
            AddProperty(CatalogSupported, true);
            AddProperty(CatalogSupportedInDml, true);
            AddProperty(CrmSourceInformation.ColumnAliasSupported, false);
            AddProperty(CrmSourceInformation.ColumnSupported, true);
            AddProperty(CrmSourceInformation.IdentifierCloseQuote, "]");
            AddProperty(CrmSourceInformation.IdentifierOpenQuote, "[");
            AddProperty(CrmSourceInformation.LikeClausePercent, "%");
            AddProperty(CrmSourceInformation.ParameterPrefix, "@");
            AddProperty(CrmSourceInformation.SchemaSeparator, ".");
            AddProperty(CrmSourceInformation.SchemaSupported, true);
            AddProperty(CrmSourceInformation.SchemaSupportedInDml, true);
            AddProperty(CrmSourceInformation.ServerSeparator, ".");
            AddProperty(CrmSourceInformation.SupportsQuotedIdentifierParts, true);
            AddProperty(CrmSourceInformation.SupportsVerifySql, false);
            AddProperty(CrmSourceInformation.TableAliasSupported, true);
            AddProperty(CrmSourceInformation.TableSupported, true);
            AddProperty(CrmSourceInformation.UserSupported, false);
            AddProperty(CrmSourceInformation.ViewSupported, true);
        }    

        #endregion

        #region Protected Methods

        /// <summary>
        /// RetrieveValue is called once per property that was identified
        /// as existing but without a value (specified in the constructor).
        /// For the purposes of this sample, only one property needs to be
        /// computed - DefaultSchema.  To retrieve this value a SQL statement
        /// is executed.
        /// </summary>
        protected override object RetrieveValue(string propertyName)
        {

            if (propertyName.Equals("DefaultSchema", StringComparison.OrdinalIgnoreCase))
            {
                if (Site.State != DataConnectionState.Open)
                {
                    Site.Open();
                }
                CrmDbConnection conn = (CrmDbConnection)Connection;
                Debug.Assert(conn != null, "Invalid provider object.");
                if (conn != null)
                {
                    try
                    {
                        return "dbo";
                    }
                    catch (DbException)
                    {
                        // We let the base class apply default behavior
                    }
                }
            }
            else if (propertyName.Equals(CrmSourceInformation.DefaultCatalog, StringComparison.OrdinalIgnoreCase))
            {
                if (Site.State != DataConnectionState.Open)
                {
                    Site.Open();
                }
                CrmDbConnection conn = (CrmDbConnection)Connection;
                Debug.Assert(conn != null, "Invalid provider object.");
                if (conn != null)
                {
                    try
                    {
                        return conn.ConnectionInfo.OrganisationName;
                    }
                    catch (DbException)
                    {
                        // We let the base class apply default behavior
                    }
                }
            }

            var val = base.RetrieveValue(propertyName);
            return val;

        }

        #endregion
    }
}
