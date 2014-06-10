using System.Data;
using System.Data.Common;

namespace CrmAdo
{
    /// <summary>
    /// Represents a transaction with Crm.
    /// </summary>
    public class CrmDbTransaction : DbTransaction
    {
        private CrmDbConnection _Connection ;
        private IsolationLevel _IsolationLevel;

        public CrmDbTransaction(CrmDbConnection connection)
        {
            _Connection = connection;
            _IsolationLevel = IsolationLevel.ReadCommitted;
        }

        public override void Commit()
        {
            //TODO: Signal plugin to commit.
            // throw new NotImplementedException();
        }

        public override void Rollback()
        {
            //TODO: Signal plugin to throw / abort.
            // throw new NotImplementedException();
        }

        protected override DbConnection DbConnection
        {
            get { return _Connection; }
        }

        public override IsolationLevel IsolationLevel
        {
            get { return _IsolationLevel; }
        }
    }
}