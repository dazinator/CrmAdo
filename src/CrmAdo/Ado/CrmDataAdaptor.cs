using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{
    //[Designer("CrmAdo.DesignTime.DataAdapterDesigner, CrmAdo")]
    //[ToolboxItem("CrmAdo.DesignTime.DataAdapterToolboxItem, CrmAdo")]
    [DefaultEvent("RowUpdated")]
    public class CrmDataAdapter : DbDataAdapter, IDbDataAdapter, IDataAdapter, ICloneable
    {

        private CrmDbCommand _selectCommand;
        private CrmDbCommand _insertCommand;
        private CrmDbCommand _updateCommand;
        private CrmDbCommand _deleteCommand;

        /*
         * Inherit from Component through DbDataAdapter. The event
         * mechanism is designed to work with the Component.Events
         * property. These variables are the keys used to find the
         * events in the components list of events.
         */
        static private readonly object EventRowUpdated = new object();
        static private readonly object EventRowUpdating = new object();

        public CrmDataAdapter()
            : base()
        {
        }

        public CrmDataAdapter(string selectCommandText, CrmDbConnection connection)
            : base()
        {
             // var dbprovider = DbProviderFactories.GetFactory(CrmAdoConstants.Invariant);
              this.SelectCommand = new CrmDbCommand(connection, selectCommandText);
              this.SelectCommand.CommandText = selectCommandText;
              this.SelectCommand.Connection = connection;
        }

        public CrmDataAdapter(CrmDbCommand selectCommand)
            : base()
        {
            this.SelectCommand = selectCommand;
        }

        private CrmDataAdapter(CrmDataAdapter from)
            : base(from)
        {

        }

        [DefaultValue(null)]
        [Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public new CrmDbCommand SelectCommand
        {
            get { return _selectCommand; }
            set { _selectCommand = value; }
        }

        IDbCommand IDbDataAdapter.SelectCommand
        {
            get { return _selectCommand; }
            set { _selectCommand = (CrmDbCommand)value; }
        }

        [DefaultValue(null)]
        [Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public new CrmDbCommand InsertCommand
        {
            get { return _insertCommand; }
            set { _insertCommand = value; }
        }

        IDbCommand IDbDataAdapter.InsertCommand
        {
            get { return _insertCommand; }
            set { _insertCommand = (CrmDbCommand)value; }
        }

        [DefaultValue(null)]
        [Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public new CrmDbCommand UpdateCommand
        {
            get { return _updateCommand; }
            set { _updateCommand = value; }
        }

        IDbCommand IDbDataAdapter.UpdateCommand
        {
            get { return _updateCommand; }
            set { _updateCommand = (CrmDbCommand)value; }
        }

        [DefaultValue(null)]
        [Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public new CrmDbCommand DeleteCommand
        {
            get
            {
                return this._deleteCommand;
            }
            set
            {
                this._deleteCommand = value;
            }
        }

        IDbCommand IDbDataAdapter.DeleteCommand
        {
            get { return _deleteCommand; }
            set { _deleteCommand = (CrmDbCommand)value; }
        }

        override protected RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
        {
            return new CrmDataAdapterRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
        }

        override protected RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
        {
            return new CrmDataAdapterRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
        }

        override protected void OnRowUpdating(RowUpdatingEventArgs value)
        {
            CrmDataAdapterRowUpdatingEventHandler handler = (CrmDataAdapterRowUpdatingEventHandler)Events[EventRowUpdating];
            if ((null != handler) && (value is CrmDataAdapterRowUpdatingEventArgs))
            {
                handler(this, (CrmDataAdapterRowUpdatingEventArgs)value);
            }
        }

        override protected void OnRowUpdated(RowUpdatedEventArgs value)
        {
            CrmDataAdapterRowUpdatedEventHandler handler = (CrmDataAdapterRowUpdatedEventHandler)Events[EventRowUpdated];
            if ((null != handler) && (value is CrmDataAdapterRowUpdatedEventArgs))
            {
                handler(this, (CrmDataAdapterRowUpdatedEventArgs)value);
            }
        }

        public event CrmDataAdapterRowUpdatingEventHandler RowUpdating
        {
            add { Events.AddHandler(EventRowUpdating, value); }
            remove { Events.RemoveHandler(EventRowUpdating, value); }
        }

        public event CrmDataAdapterRowUpdatedEventHandler RowUpdated
        {
            add { Events.AddHandler(EventRowUpdated, value); }
            remove { Events.RemoveHandler(EventRowUpdated, value); }
        }

        object ICloneable.Clone()
        {
            return new CrmDataAdapter(this);
        }
    }

    public delegate void CrmDataAdapterRowUpdatingEventHandler(object sender, CrmDataAdapterRowUpdatingEventArgs e);
    public delegate void CrmDataAdapterRowUpdatedEventHandler(object sender, CrmDataAdapterRowUpdatedEventArgs e);

    public class CrmDataAdapterRowUpdatingEventArgs : RowUpdatingEventArgs
    {
        public CrmDataAdapterRowUpdatingEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
            : base(row, command, statementType, tableMapping)
        {
        }

        // Hide the inherited implementation of the command property.
        new public CrmDbCommand Command
        {
            get { return (CrmDbCommand)base.Command; }
            set { base.Command = value; }
        }
    }

    public class CrmDataAdapterRowUpdatedEventArgs : RowUpdatedEventArgs
    {
        public CrmDataAdapterRowUpdatedEventArgs(DataRow row, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
            : base(row, command, statementType, tableMapping)
        {
        }

        // Hide the inherited implementation of the command property.
        new public CrmDbCommand Command
        {
            get { return (CrmDbCommand)base.Command; }
        }
    }

}

