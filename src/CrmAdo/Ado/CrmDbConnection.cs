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
using System.Collections.Concurrent;
using Microsoft.Xrm.Sdk.Query;

namespace CrmAdo
{      

    /// <summary>
    /// Represents a connection to Dynamics Crm.
    /// </summary>
    public class CrmDbConnection : DbConnection
    {
        private ICrmServiceProvider _CrmServiceProvider = null;
        private ICrmMetaDataProvider _MetadataProvider = null;
        private CrmConnectionCache _ConnectionCache = null;
        private CrmConnectionInfo _ConnectionInfo = null;

        private bool _InitialisedVersion = false;
        // private string _CrmVersion = string.Empty;

        #region Constructor

        public CrmDbConnection()
            : this(string.Empty)
        {

        }

        public CrmDbConnection(string connectionString)
            : this(new CrmServiceProvider(new ExplicitConnectionStringProviderWithFallbackToConfig() { OrganisationServiceConnectionString = connectionString }, new CrmClientCredentialsProvider()))
        {
        }

        public CrmDbConnection(ICrmServiceProvider serviceProvider)
            : this(serviceProvider, new InMemoryCachedCrmMetaDataProvider(new EntityMetadataRepository(serviceProvider)))
        {
        }

        public CrmDbConnection(ICrmServiceProvider serviceProvider, ICrmMetaDataProvider metadataProvider)
        {
            _CrmServiceProvider = serviceProvider;
            _MetadataProvider = metadataProvider;          
            _ConnectionCache = new CrmConnectionCache();
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
                ExecuteWithinStateTransition(
                    ConnectionState.Connecting,
                    f =>
                    {
                        _OrganizationService = _CrmServiceProvider.GetOrganisationService();
                        _ConnectionInfo = _ConnectionCache.GetConnectionInfo(this);
                    },
                    ConnectionState.Open);
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

        public CrmConnectionInfo ConnectionInfo
        {
            get
            {
                if (_ConnectionInfo == null)
                {
                    ExecuteAndOpenIfNeccessary(a =>
                    {
                        _ConnectionInfo = _ConnectionCache.GetConnectionInfo(this);
                    });
                }

                return _ConnectionInfo;
            }
        }

        public override string ServerVersion
        {
            get
            {
                return ConnectionInfo.ServerVersion;
            }
        }

        public override string Database
        {
            get
            {
                return ConnectionInfo.OrganisationName;
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

        #region StateWrappers

        /// <summary>
        /// Enusures that whilst a Func<CrmDbConnection> is executed, the connection transitions to the correct state.
        /// Afterwards the connection transitions to either the successful state, or the broken state.         
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="execute"></param>
        private TResult ExecuteWithinStateTransition<TResult>(ConnectionState stateWhileExecuting, Func<CrmDbConnection, TResult> execute, ConnectionState transitionToStateOnSuccess)
        {
            // bool hadToChangeState = false;
            ConnectionState _previousState = this._State;
            try
            {
                if (_State != stateWhileExecuting)
                {
                    //  hadToChangeState = true;
                    this._State = stateWhileExecuting;
                }
                var result = execute(this);
                _State = transitionToStateOnSuccess;
                return result;
            }
            catch (Exception)
            {
                _State = ConnectionState.Broken;
                throw;
            }

        }

        /// <summary>
        /// Enusures that whilst a Action<CrmDbConnection> is executed, the connection transitions to the correct state.
        /// Afterwards the connection transitions to either the successful state, or the broken state.         
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="execute"></param>
        private void ExecuteWithinStateTransition(ConnectionState stateWhileExecuting, Action<CrmDbConnection> execute, ConnectionState transitionToStateOnSuccess)
        {
            // bool hadToChangeState = false;
            ConnectionState _previousState = this._State;
            try
            {
                if (_State != stateWhileExecuting)
                {
                    //  hadToChangeState = true;
                    this._State = stateWhileExecuting;
                }
                execute(this);
                _State = transitionToStateOnSuccess;
                return;
            }
            catch (Exception)
            {
                _State = ConnectionState.Broken;
                throw;
            }

        }

        /// <summary>
        /// Enusures that prior to executing an Action<CrmDbConnection>, the connection is Open(). If it is not open already it will be opened. If it is necessary to Open() it then
        /// after executing the connection will be Closed() otherwise it will remain in an Open state.     
        /// </summary>      
        /// <param name="execute"></param>
        private void ExecuteAndOpenIfNeccessary(Action<CrmDbConnection> execute)
        {
            bool hadToOpen = false;
            ConnectionState previousState = _State;
            try
            {
                if (_State != ConnectionState.Open)
                {
                    hadToOpen = true;
                    this.Open();
                }

                execute(this);

            }
            finally
            {
                // return connection to orginal state.
                if (hadToOpen)
                {
                    this.Close();
                }
            }

        }


        #endregion

        #region Not Implemented

        public override void ChangeDatabase(string databaseName)
        {
            // TODO: change crm organisation for connection?
            throw new NotImplementedException();
        }

        #region Schema

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
                    result = GetReservedWords();
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

        public static DataTable GetReservedWords()
        {
            DataTable table = new DataTable("ReservedWords");
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("ReservedWord", typeof(string));

            // List of keywords taken from SQLGeneration library token registry grammer.
            string[] keywords = new[]
            {
                "TOP",
                "UPDATE",
                "VALUES",
                "WHERE",
                "WITH",
                "BETWEEN",
                "AND",
                "OR",
                "DELETE",
                "ALL",
                "ANY",
                "SOME",
                "FROM",
                "GROUP",
                "BY",
                "HAVING",
                "INSERT",
                "INTO",
                "IS",
                "FULL",
                "OUTER",
                "JOIN",
                "INNER",
                "LEFT",
                "RIGHT",
                "CROSS",
                "IN",
                "LIKE",
                "NOT",
                "NULLS",
                "NULL",
                "ORDER",
                "ASC",
                "DESC",
                "PERCENT",
                "SELECT",
                "UNION",
                "INTERSECT",
                "EXCEPT",
                "MINUS",
                "SET",
                "ON",
                "AS",
                "EXISTS",
                "OVER",
                "PARTITION",
                "ROWS",
                "RANGE",
                "UNBOUNDED",
                "PRECEDING",
                "FOLLOWING",
                "CURRENT",
                "ROW",
                "CASE",
                "WHEN",
                "THEN",
                "ELSE",
                "END",
                "CREATE", /// DDL
                "DATABASE",
                "TABLE",
                "PRIMARY",
                "KEY",
                "COLLATE",
                "CONSTRAINT",
                "IDENTITY",
                "DEFAULT",
                "ROWGUIDCOL",
                "UNIQUE",
                "CLUSTERED",
                "NONCLUSTERED",
                "FOREIGN",
                "REFERENCES",
                "NO",
                "ACTION",
                "CASCADE",
                "FOR",
                "REPLICATION",
                "CHECK",
                "ALTER",
                "MODIFY",
                "CURRENT",
                "COLUMN",
                "ADD",
                "DROP",
                "PERSISTED",
                "SPARSE"               
            };
            foreach (string keyword in keywords)
            {
                table.Rows.Add(keyword);
            }
            return table;
        }

        #endregion

        #endregion

    }
}