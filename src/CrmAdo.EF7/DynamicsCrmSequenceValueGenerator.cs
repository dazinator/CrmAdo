using Microsoft.Data.Entity.DynamicsCrm;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.DynamicsCrm
{

    public class DynamicsCrmSequenceValueGenerator<TValue> : HiLoValueGenerator<TValue>
    {
        private readonly SqlStatementExecutor _executor;
        private readonly DynamicsCrmConnection _connection;
        private readonly string _sequenceName;

        public DynamicsCrmSequenceValueGenerator(
           SqlStatementExecutor executor,
           DynamicsCrmSequenceValueGeneratorState generatorState,
           DynamicsCrmConnection connection)
            : base(generatorState)
        {
            // Check.NotNull(executor, nameof(executor));
            // Check.NotNull(generatorState, nameof(generatorState));
            // Check.NotNull(connection, nameof(connection));

            _sequenceName = generatorState.SequenceName;
            _executor = executor;
            _connection = connection;
        }

        protected override long GetNewHighValue()
        {
            // TODO: Parameterize query and/or delimit identifier without using SqlServerMigrationOperationSqlGenerator
            var sql = string.Format(CultureInfo.InvariantCulture, "SELECT NEXT VALUE FOR {0}", _sequenceName);
            var nextValue = _executor.ExecuteScalar(_connection, _connection.DbTransaction, sql);

            return (long)Convert.ChangeType(nextValue, typeof(long), CultureInfo.InvariantCulture);
        }

        // override 

        public override bool GeneratesTemporaryValues
        {
            get { return false; }
        }

        //public override bool GeneratesTemporaryValues()
        //{
        //    
        //}
    }

}
