using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Ado
{
    [Designer("CrmAdo.DesignTime.DataAdapterDesigner, CrmAdo")]
    [ToolboxItem("CrmAdo.DesignTime.DataAdapterToolboxItem, CrmAdo")]
    public class CrmDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter
    {

        public event EventHandler<RowUpdatedEventArgs> RowUpdated;
        public event EventHandler<RowUpdatingEventArgs> RowUpdating;


        private CrmDbCommand _DeleteCommand;
        public new CrmDbCommand DeleteCommand
        {
            get
            {
                return this._DeleteCommand;
            }
            set
            {
                this._DeleteCommand = value;
            }
        }

        private CrmDbCommand _InsertCommand;
        public new CrmDbCommand InsertCommand
        {
            get
            {
                return this._InsertCommand;
            }
            set
            {
                this._InsertCommand = value;
            }
        }

        private CrmDbCommand _SelectCommand;
        public new CrmDbCommand SelectCommand
        {
            get
            {
                return this._SelectCommand;
            }
            set
            {
                this._SelectCommand = value;
            }
        }

        private CrmDbCommand _UpdateCommand;
        public new CrmDbCommand UpdateCommand
        {
            get
            {
                return this._UpdateCommand;
            }
            set
            {
                this._UpdateCommand = value;
            }
        }

        public CrmDataAdapter()
        {

        }

        public CrmDataAdapter(CrmDbCommand command)
        {
            this._SelectCommand = command;
        }

        public CrmDataAdapter(CrmDbConnection connection, string commandText)
        {
            this._SelectCommand = new CrmDbCommand(connection, commandText);
        }

        public CrmDataAdapter(string connectionString, string commandText)
        {
            this._SelectCommand = new CrmDbCommand(new CrmDbConnection(connectionString), commandText);
        }

        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
        {
            return new CrmDataAdaptorRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
        }

        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
        {
            return new CrmDataAdaptorRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
        }

    }
}
