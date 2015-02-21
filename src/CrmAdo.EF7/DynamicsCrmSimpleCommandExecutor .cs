using CrmAdo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm
{
    public class DynamicsCrmSimpleCommandExecutor
    {
        private const int DefaultCommandTimeout = 1;

        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public DynamicsCrmSimpleCommandExecutor(string connectionString)
            : this(connectionString, DefaultCommandTimeout)
        {
        }

        public DynamicsCrmSimpleCommandExecutor(string connectionString, int commandTimeout)
        {
            // Check.NotEmpty(connectionString, "connectionString");

            _connectionString = connectionString;
            _commandTimeout = commandTimeout;
        }

        public virtual async Task<T> ExecuteScalarAsync<T>(string commandText, CancellationToken cancellationToken, params object[] parameters)
        {
            // Check.NotEmpty(commandText, "commandText");
            // Check.NotNull(parameters, "parameters");

            using (var connection = new CrmDbConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).WithCurrentCulture();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.CommandTimeout = _commandTimeout;

                    return (T)await command.ExecuteScalarAsync(cancellationToken).WithCurrentCulture();
                }
            }
        }
    }
}
