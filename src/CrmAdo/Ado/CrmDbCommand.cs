using CrmAdo.Core;
using CrmAdo.Visitor;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;

namespace CrmAdo
{
    /// <summary>
    /// Represents a command to execute against Dynamics Crm.
    /// </summary>
    public class CrmDbCommand : DbCommand
    {
        private DbConnection _DbConnection;
        private string _CommandText = string.Empty;
        private IOrgCommandExecutor _CrmCommandExecutor;
        private CommandType _CommandType;
        private CrmParameterCollection _ParameterCollection;
        private IOrganisationCommandProvider _OrganisationCommandProvider;

        #region Constructor
        public CrmDbCommand()
            : this(null)
        {
        }
        public CrmDbCommand(CrmDbConnection connection)
            : this(connection, string.Empty)
        {
        }

        public CrmDbCommand(CrmDbConnection connection, string commandText)
            : this(connection, CrmOrgCommandExecutor.Instance)
        {
            _CommandText = commandText;
        }

        public CrmDbCommand(CrmDbConnection connection, IOrgCommandExecutor crmCommandExecutor)
            : this(connection, crmCommandExecutor, new SqlGenerationOrganizationCommandProvider())
        {
        }
        public CrmDbCommand(CrmDbConnection connection, IOrgCommandExecutor crmCommandExecutor, IOrganisationCommandProvider organisationCommandProvider)
        {

            if (crmCommandExecutor == null)
            {
                throw new ArgumentNullException("crmCommandExecutor");
            }

            if (organisationCommandProvider == null)
            {
                throw new ArgumentNullException("organisationCommandProvider");
            }

            _DbConnection = connection;
            _CrmCommandExecutor = crmCommandExecutor;
            _OrganisationCommandProvider = organisationCommandProvider;
            _CommandType = CommandType.Text;
            _ParameterCollection = new CrmParameterCollection();
        }
        #endregion

        #region Properties

        public override string CommandText
        {
            get { return _CommandText; }
            set { _CommandText = value; }
        }

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType
        {
            get { return _CommandType; }
            set { _CommandType = value; }
        }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection DbConnection
        {
            get { return _DbConnection; }
            set { _DbConnection = value; }
        }

        public CrmDbConnection CrmDbConnection
        {
            get { return DbConnection as CrmDbConnection; }
        }

        protected override DbTransaction DbTransaction { get; set; }

        public override bool DesignTimeVisible { get; set; }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _ParameterCollection; }
        }

        #endregion

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            EnsureHasCommandText();
            EnsureOpenConnection();

            // Generate the IOrgCommand.
            IOrgCommand orgCommand = ToOrgCommand(behavior);
            // Execute the IOrgCommand and get the results.
            var results = _CrmCommandExecutor.ExecuteCommand(orgCommand, behavior);

            DbDataReader reader;
            if (behavior == CommandBehavior.CloseConnection)
            {
                reader = results.GetReader(_DbConnection);
            }
            else
            {
                reader = results.GetReader();
            }
            return reader;
        }

        public override int ExecuteNonQuery()
        {
            Debug.WriteLine("CrmDbCommand.ExecuteNonQuery()", "CrmDbCommand");
            EnsureHasCommandText();
            EnsureOpenConnection();
            // Generate the IOrgCommand.
            IOrgCommand orgCommand = ToOrgCommand(CommandBehavior.Default);
            // Execute the IOrgCommand and return the result.
            return _CrmCommandExecutor.ExecuteNonQueryCommand(orgCommand);
        }

        private IOrgCommand ToOrgCommand(CommandBehavior behavior)
        {
            var orgCommand = _OrganisationCommandProvider.GetOrganisationCommand(this, behavior);
            return orgCommand;
        }

        public override object ExecuteScalar()
        {
            Debug.WriteLine("CrmDbCommand.ExecuteScalar()", "CrmDbCommand");
            EnsureHasCommandText();
            EnsureOpenConnection();
            // If the first column of the first row in the result set is not found, a null reference is returned. 
            // If the value in the database is null, the query returns DBNull.Value.
            // Generate the IOrgCommand.
            IOrgCommand orgCommand = ToOrgCommand(CommandBehavior.Default);
            // Execute the IOrgCommand and get the results.
            var results = _CrmCommandExecutor.ExecuteCommand(orgCommand, CommandBehavior.Default);
            //  var results = _CrmCommandExecutor.ExecuteCommand(this, CommandBehavior.Default);
            return results.GetScalar();
        }

        protected void EnsureOpenConnection()
        {
            Debug.WriteLine("CrmDbCommand.EnsureOpenConnection()", "CrmDbCommand");
            // must have a valid and open connection
            if (_DbConnection == null || _DbConnection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must be valid and open");
        }

        private void EnsureHasCommandText()
        {
            if (string.IsNullOrWhiteSpace(this.CommandText))
            {
                throw new InvalidOperationException("Command must have command text.");
            }
        }

        protected override DbParameter CreateDbParameter()
        {
            return new CrmParameter();
        }

        #region Not Implemented

        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}