using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using CrmAdo;
using Microsoft.Synchronization.Data;

namespace CrmSync
{
    /// <summary>
    /// Adaptor for CrmDbCommand that will enable it to cater for Sync insert commands by setting row count paramater.
    /// </summary>
    public class SelectIncrementalChangesDbCommandAdapter : CrmDbCommand
    {
        private List<string> _Log = new List<string>();

        private CrmDbCommand _WrappedCommand;

        public SelectIncrementalChangesDbCommandAdapter(CrmDbCommand wrappedCommand)
        {
            _WrappedCommand = wrappedCommand;
        }

        public override int ExecuteNonQuery()
        {
            Debug.WriteLine("Execute non query " + DateTime.Now + " for command text: " + this.CommandText);
            PreExecuteCheck();
#if DEBUG
            Console.WriteLine("Selecting incremental changes between " + this.Parameters[0].Value + " and " + this.Parameters[1].Value);
#endif
            return _WrappedCommand.ExecuteNonQuery();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            Debug.WriteLine("Execute non query " + DateTime.Now + " for command text: " + this.CommandText);
            PreExecuteCheck();
#if DEBUG
            Console.WriteLine("Selecting incremental changes between " + this.Parameters[0].Value + " and " + this.Parameters[1].Value);
#endif
            return _WrappedCommand.ExecuteReader(behavior);
        }

        public override object ExecuteScalar()
        {
            Debug.WriteLine("Execute Scalar " + DateTime.Now + " for command text: " + this.CommandText);
            PreExecuteCheck();
            return _WrappedCommand.ExecuteScalar();
        }

        protected void PreExecuteCheck()
        {
            // if last anchor is currently dbnull (which it will be on very first sync) then change it to 0;
            var param = this.Parameters["@" + SyncSession.SyncLastReceivedAnchor];
            if (param != null)
            {
                if (param.Value == DBNull.Value)
                {
                    param.Value = 0L;
                }
                else if (param.Value is int)
                {
                    param.Value = System.Convert.ToInt64(param.Value);
                }
            }
        }

        public override string CommandText
        {
            get
            {
                return _WrappedCommand.CommandText;
            }
            set
            {
                _WrappedCommand.CommandText = value;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _WrappedCommand.Parameters; }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            // also dispose of wrapped command.
            Debug.WriteLine("disposing of wrapped command");
            _WrappedCommand.Dispose();
        }
    }
}