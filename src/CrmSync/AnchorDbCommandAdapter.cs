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
    /// Adaptor for CrmDbCommand that will enable it to cater for Sync Anchor commands.
    /// </summary>
    public class AnchorDbCommandAdapter : CrmDbCommand
    {
        private List<string> _Log = new List<string>();

        private CrmDbCommand _WrappedCommand;

        public AnchorDbCommandAdapter(CrmDbCommand wrappedCommand)
        {
            _WrappedCommand = wrappedCommand;
        }

        public override int ExecuteNonQuery()
        {
            Debug.WriteLine("Execute non query " + DateTime.Now + " for command text: " + this.CommandText);
            SetAnchor();
            return 1;
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public override object ExecuteScalar()
        {
            Debug.WriteLine("Execute Scalar " + DateTime.Now + " for command text: " + this.CommandText);
            return SetAnchor();
        }

        protected object SetAnchor()
        {
            var param = this.Parameters["@" + SyncSession.SyncNewReceivedAnchor];
            var entityName = this.CommandText;
            // todo should be able to select max versionnumber from contact..
            _WrappedCommand.CommandText = this.CommandText;
            var lastrowversion = _WrappedCommand.ExecuteScalar();
            Debug.WriteLine("last versionnumber is " + lastrowversion);
            param.Value = lastrowversion;
            return lastrowversion;
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