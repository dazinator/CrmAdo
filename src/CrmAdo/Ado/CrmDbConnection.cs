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

        private CrmConnectionCache _ConnectionCache = null;
        private CrmConnectionInfo _ConnectionInfo = null;     

        #region Constructor

        public CrmDbConnection()
            : this(string.Empty)
        {

        }

        public CrmDbConnection(string connectionString)
        {
            this.BuildUp();
            this.CrmServiceProvider.ConnectionProvider.OrganisationServiceConnectionString = connectionString;
            _ConnectionCache = new CrmConnectionCache();
        }

        private void BuildUp()
        {
            IoC.ContainerServices.CurrentContainer().BuildUp(this);
        }    

        #endregion

        #region Dependencies

        public ICrmMetaDataProvider MetadataProvider { get; set; }        

        public ICrmServiceProvider CrmServiceProvider { get; set; }

        public ISchemaCollectionsProvider SchemaCollectionsProvider { get; set; }

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
            get { return CrmServiceProvider.ConnectionProvider.OrganisationServiceConnectionString; }
            set { CrmServiceProvider.ConnectionProvider.OrganisationServiceConnectionString = value; }
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
                        _OrganizationService = CrmServiceProvider.GetOrganisationService();
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
                connStringBuilder.ConnectionString = CrmServiceProvider.ConnectionProvider.OrganisationServiceConnectionString;
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
            return SchemaCollectionsProvider.GetSchema(this, collectionName, restrictions);
        }

        #endregion

        #region Not Implemented

        public override void ChangeDatabase(string databaseName)
        {
            // TODO: change crm organisation for connection?
            throw new NotImplementedException();
        }

        #endregion

    }
}