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
        private ICrmCommandExecutor _CrmCommandExecutor;
        private CommandType _CommandType;
        private CrmParameterCollection _ParameterCollection;

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
            : this(connection, commandText, new CrmCommandExecutor(connection))
        {
        }
        public CrmDbCommand(CrmDbConnection connection, string commandText, ICrmCommandExecutor crmCommandExecutor)
        {
            _DbConnection = connection;
            _CommandText = commandText;
            _CrmCommandExecutor = crmCommandExecutor;
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

        internal CrmDbConnection CrmDbConnection
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
            Debug.WriteLine("CrmDbCommand.ExecuteReader(b)", "CrmDbCommand");
            EnsureOpenConnection();

            if ((behavior & CommandBehavior.KeyInfo) > 0)
                Debug.WriteLine("Behavior includes KeyInfo");

            if ((behavior & CommandBehavior.SchemaOnly) > 0)
                Debug.WriteLine("Behavior includes SchemaOnly");

            // only implement CloseConnection and "all other"
            if ((behavior & CommandBehavior.CloseConnection) > 0)
            {
                Debug.WriteLine("Behavior includes CloseConnection");
                var results = _CrmCommandExecutor.ExecuteCommand(this);
                var reader = new CrmDbDataReader(results, _DbConnection);
                return reader;
            }
            else
            {
                var results = _CrmCommandExecutor.ExecuteCommand(this);
                var reader = new CrmDbDataReader(results);
                return reader;
            }
        }

        public override int ExecuteNonQuery()
        {
            Debug.WriteLine("CrmDbCommand.ExecuteNonQuery()", "CrmDbCommand");
            EnsureOpenConnection();
            return _CrmCommandExecutor.ExecuteNonQueryCommand(this);
        }

        public override object ExecuteScalar()
        {
            Debug.WriteLine("CrmDbCommand.ExecuteScalar()", "CrmDbCommand");
            EnsureOpenConnection();
            // Use the ExecuteScalar method to retrieve a single value (for example, an aggregate value) from a database. This requires less code than using the ExecuteReader method and performing the operations necessary to generate the single value using the data returned by a DbDataReader.
            // If the first column of the first row in the result set is not found, a null reference (Nothing in Visual Basic) is returned. If the value in the database is null, the query returns DBNull.Value.
            var results = _CrmCommandExecutor.ExecuteCommand(this);
            if (results != null && results.Results != null && results.Results.Entities != null && results.Results.Entities.Any())
            {
                var first = results.Results.Entities[0];
                if (first.Attributes.Any())
                {
                    var value = first.Attributes.FirstOrDefault().Value;
                    return CrmDbTypeConverter.ToDbType(value);
                }
            }
            return null;
        }

        protected void EnsureOpenConnection()
        {
            Debug.WriteLine("CrmDbCommand.EnsureOpenConnection()", "CrmDbCommand");
            // must have a valid and open connection
            if (_DbConnection == null || _DbConnection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must be valid and open");
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