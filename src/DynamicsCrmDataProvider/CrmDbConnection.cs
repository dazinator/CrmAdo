using System;
using System.Data;
using System.Data.Common;
using DynamicsCrmDataProvider.Dynamics;
using Microsoft.Xrm.Sdk;

namespace DynamicsCrmDataProvider
{
    /// <summary>
    /// Represents a connection to Dynamics Crm.
    /// </summary>
    public class CrmDbConnection : DbConnection
    {
        private ICrmServiceProvider _CrmServiceProvider = null;

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

        #region Not Implemented
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        public override void ChangeDatabase(string databaseName)
        {
            // TODO: change crm organisation for connection?
            throw new NotImplementedException();
        }

        public override string Database
        {
            get { throw new NotImplementedException(); }
        }

        public override string DataSource
        {
            get { throw new NotImplementedException(); }
        }

        public override string ServerVersion
        {
            get { throw new NotImplementedException(); }
        }


        #endregion

    }
}