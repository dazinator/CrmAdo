using System;
using System.Data;
using System.Data.Common;
using CrmAdo.Dynamics;
using CrmAdo.Dynamics.Metadata;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using CrmAdo.Ado;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace CrmAdo
{
    /// <summary>
    /// Represents a connection to Dynamics Crm.
    /// </summary>
    public class CrmDbConnection : DbConnection
    {
        private ICrmServiceProvider _CrmServiceProvider = null;
        private ICrmMetaDataProvider _MetadataProvider = null;

        #region Constructor

        public CrmDbConnection()
            : this(string.Empty)
        {

        }

        public CrmDbConnection(string connectionString)
        {
            var connectionProvider = new ExplicitConnectionStringProviderWithFallbackToConfig();
            connectionProvider.OrganisationServiceConnectionString = connectionString;
            var credentialsProvider = new CrmClientCredentialsProvider();
            _CrmServiceProvider = new CrmServiceProvider(connectionProvider, credentialsProvider);
            _MetadataProvider = new InMemoryCachedCrmMetaDataProvider(new EntityMetadataRepository(_CrmServiceProvider));
        }

        public CrmDbConnection(ICrmServiceProvider serviceProvider)
        {
            _CrmServiceProvider = serviceProvider;
        }

        #endregion

        #region Properties

        private ConnectionState _State = ConnectionState.Closed;
        public override ConnectionState State
        {
            get { return _State; }
        }

        private IOrganizationService _OrganizationService = null;
        public IOrganizationService OrganizationService
        {
            get { return _OrganizationService; }
        }

        public override string ConnectionString
        {
            get { return _CrmServiceProvider.ConnectionProvider.OrganisationServiceConnectionString; }
            set { _CrmServiceProvider.ConnectionProvider.OrganisationServiceConnectionString = value; }
        }

        #endregion

        public override void Open()
        {
            if (_State == ConnectionState.Closed)
            {
                _State = ConnectionState.Connecting;
                _OrganizationService = _CrmServiceProvider.GetOrganisationService();

                // could do a whoami request to warmup the connection.
                //TODO:  would rather cache this information against the connection string so its not requiried on every open.
                //var req = new WhoAmIRequest();
                //var resp = (WhoAmIResponse)_OrganizationService.Execute(req);
                //var orgId = resp.OrganizationId;
                //var businessUnitId = resp.BusinessUnitId;
                //var userId = resp.UserId;

                _State = ConnectionState.Open;
            }
            else
            {
                throw new InvalidOperationException("Connection can only be opened if it is currently in the closed state.");
            }

        }

        public override void Close()
        {
            // TODO: close crm connection
            if (_OrganizationService != null)
            {
                var disposable = _OrganizationService as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                _OrganizationService = null;
            }
            _State = ConnectionState.Closed;
        }

        protected override DbCommand CreateDbCommand()
        {
            return new CrmDbCommand(this);
        }

        protected override void Dispose(bool disposing)
        {
            //TODO: Close crm connection.
            Close();
            base.Dispose(disposing);
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new CrmDbTransaction(this);
            //  throw new NotImplementedException();
        }

        public override string DataSource
        {
            get
            {
                var connStringBuilder = new CrmConnectionStringBuilder();
                connStringBuilder.ConnectionString = _CrmServiceProvider.ConnectionProvider.OrganisationServiceConnectionString;
                return connStringBuilder.Url;
            }
        }

        public override string ServerVersion
        {
            get
            {
                bool hadToOpen = false;
                try
                {
                    if (_State != ConnectionState.Open)
                    {
                        hadToOpen = true;
                        this.Open();
                    }

                    var req = new RetrieveVersionRequest();
                    var resp = (RetrieveVersionResponse)_OrganizationService.Execute(req);
                    //assigns the version to a string
                    string versionNumber = resp.Version;
                    return versionNumber;
                }
                finally
                {
                    // return connection to orginal state.
                    if (hadToOpen)
                    {
                        this.Close();
                    }
                }
                //  throw new NotImplementedException();
            }
        }

        public ICrmMetaDataProvider MetadataProvider
        {
            get { return _MetadataProvider; }
        }

        public ICrmServiceProvider CrmServiceProvider
        {
            get { return _CrmServiceProvider; }
        }

        #region Not Implemented


        public override void ChangeDatabase(string databaseName)
        {
            // TODO: change crm organisation for connection?
            throw new NotImplementedException();
        }

        public override string Database
        {
            get { throw new NotImplementedException(); }
        }


        /// <summary>
        /// Returns the supported collections
        /// </summary>
        public override DataTable GetSchema()
        {
            return this.GetSchema("MetaDataCollections");
        }

        /// <summary>
        /// Returns the schema collection specified by the collection name.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <returns>The collection specified.</returns>
        public override DataTable GetSchema(string collectionName)
        {
            return GetSchema(collectionName, null);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictions)
        {
            DataTable result = null;

            if (restrictions == null)
            {
                restrictions = new string[] { "" };
            }

            switch (collectionName.ToLower())
            {
                case "metadatacollections":
                    result = GetMetadataCollectionsDataTable();
                    break;
                case "datasourceinformation":
                    result = GetDataSourceInfoDataTable();
                    //    return NpgsqlSchema.GetDataSourceInformation();
                    break;

                case "reservedwords":
                    //   return NpgsqlSchema.GetReservedWords();
                    break;

                case "DataTypes":
                    throw new NotSupportedException();

                case "Tables":
                    // return NpgsqlSchema.GetTables(tempConn, restrictions);
                    break;

                case "Columns":
                    //  return NpgsqlSchema.GetColumns(tempConn, restrictions);
                    break;

                case "Views":
                    // return NpgsqlSchema.GetViews(tempConn, restrictions);
                    break;

                case "Indexes":
                    //  return NpgsqlSchema.GetIndexes(tempConn, restrictions);
                    break;
                case "IndexColumns":
                    //  return NpgsqlSchema.GetIndexColumns(tempConn, restrictions);
                    break;

                case "ForeignKeys":
                    // return NpgsqlSchema.GetConstraints(tempConn, restrictions, collectionName);
                    break;



                case "Restrictions":
                    // return NpgsqlSchema.GetRestrictions();
                    break;


                // custom collections for npgsql
                case "Databases":
                    //   return NpgsqlSchema.GetDatabases(tempConn, restrictions);
                    break;
                case "Schemata":
                    //  return NpgsqlSchema.GetSchemata(tempConn, restrictions);
                    break;



                case "Users":
                    //  return NpgsqlSchema.GetUsers(tempConn, restrictions);
                    break;

                case "Constraints":
                case "PrimaryKey":
                case "UniqueKeys":

                case "ConstraintColumns":
                    // return NpgsqlSchema.GetConstraintColumns(tempConn, restrictions);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("collectionName", collectionName, "Invalid collection name");
                // }
            }

            return result;          
        }       

        //private static void AddMetadataCollectionRow(DataTable dataTable, DataColumn nameCol, DataColumn restrictionsCol, DataColumn identifiersCol, string collectionName, int restrictions, int identifiers)
        //{
        //    var row = dataTable.Rows.Add();
        //    row.SetField<string>(nameCol, collectionName);
        //    row.SetField<int>(restrictionsCol, restrictions);
        //    row.SetField<int>(identifiersCol, identifiers);
        //}

        private static DataTable GetMetadataCollectionsDataTable()
        {
            throw new NotSupportedException();
            //DataTable dataTable = new DataTable("MetaDataCollections")
            //{
            //    Locale = CultureInfo.InvariantCulture
            //};
            //var nameCol = dataTable.Columns.Add("CollectionName", typeof(string));
            //var restrictionsCol = dataTable.Columns.Add("NumberOfRestrictions", typeof(int));
            //var identifiersCol = dataTable.Columns.Add("NumberOfIdentifierParts", typeof(int));
            //dataTable.BeginLoadData();

            //AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "MetaDataCollections", 0, 0);
            //AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "DataSourceInformation", 0, 0);
            //AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "DataTypes", 0, 0);
            //AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "ReservedWords", 0, 0);
            //AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "Tables", 4, 3);
            //AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "Columns", 4, 4);

            //dataTable.AcceptChanges();
            //dataTable.EndLoadData();
            //// dataTable.DefaultView.Sort = "Category ASC";
            //return dataTable;
        }

        private static DataTable GetDataSourceInfoDataTable()
        {
            //throw new NotSupportedException();
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add(DbMetaDataColumnNames.SupportedJoinOperators, typeof(int));
            dataTable.Columns.Add(DbMetaDataColumnNames.ParameterNamePattern, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.ParameterMarkerFormat, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.ParameterNameMaxLength, typeof(int));
            dataTable.Columns.Add(DbMetaDataColumnNames.DataSourceProductName, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.DataSourceProductVersion, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.NumberOfIdentifierParts, typeof(int));
            dataTable.Columns.Add("IdentifierOpenQuote", typeof(string));
            dataTable.Columns.Add("IdentifierCloseQuote", typeof(string));
            dataTable.Columns.Add("SupportsAnsi92Sql", typeof(bool));
            dataTable.Columns.Add("SupportsQuotedIdentifierParts", typeof(bool));          
            dataTable.Columns.Add("ParameterPrefix", typeof(string));
            dataTable.Columns.Add("ParameterPrefixInName", typeof(bool));           
            dataTable.Columns.Add("ColumnAliasSupported", typeof(bool));
            dataTable.Columns.Add("TableAliasSupported", typeof(bool));
            dataTable.Columns.Add("TableSupported", typeof(bool));
            dataTable.Columns.Add("UserSupported", typeof(bool));
            dataTable.Columns.Add("SchemaSupported", typeof(bool));
            dataTable.Columns.Add("CatalogSupported", typeof(bool));
            dataTable.Columns.Add("SupportsVerifySQL", typeof(bool));

            DataRow dataRow = dataTable.NewRow();

            dataRow[DbMetaDataColumnNames.SupportedJoinOperators] = 3; // Inner join and Left joins
            dataRow[DbMetaDataColumnNames.ParameterNamePattern] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            dataRow[DbMetaDataColumnNames.ParameterMarkerFormat] = "{0}";
            dataRow[DbMetaDataColumnNames.ParameterNameMaxLength] = 128;
            dataRow[DbMetaDataColumnNames.DataSourceProductName] = "CrmAdo Provider for Dynamics CRM";
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            dataRow[DbMetaDataColumnNames.DataSourceProductVersion] = version.ToString();
            dataRow[DbMetaDataColumnNames.NumberOfIdentifierParts] = 1;           
            dataRow["IdentifierOpenQuote"] = "[";
            dataRow["IdentifierCloseQuote"] = "]";
            dataRow["SupportsAnsi92Sql"] = false;
            dataRow["SupportsQuotedIdentifierParts"] = true;
            dataRow["ParameterPrefix"] = "@";
            dataRow["ParameterPrefixInName"] = true;           
            dataRow["ColumnAliasSupported"] = false;
            dataRow["TableAliasSupported"] = true;
            dataRow["TableSupported"] = true;
            dataRow["UserSupported"] = false;
            dataRow["SchemaSupported"] = false;
            dataRow["CatalogSupported"] = false;
            dataRow["SupportsVerifySQL"] = false;
            dataTable.Rows.Add(dataRow);
            return dataTable;
        }


        #endregion

    }
}