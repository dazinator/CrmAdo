using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.DdexProvider
{
    /// <summary>
    /// Represents constant string values for all the supported data object
    /// types.  This list must be in sync with the data object support XML.
    /// </summary>
    public static class CrmObjectTypes
    {
        public const string Root = "Root";
        public const string User = "Users";
        public const string CrmTable = "Tables";
        public const string CrmColumn = "Columns";
        public const string PluginAssembly = "PluginAssembly";
      
        //public const string Table = "Table";
        //public const string TableColumn = "TableColumn";
        //public const string Index = "Index";
        //public const string IndexColumn = "IndexColumn";
        //public const string ForeignKey = "ForeignKey";
        //public const string ForeignKeyColumn = "ForeignKeyColumn";
        //public const string View = "View";
        //public const string ViewColumn = "ViewColumn";
        //public const string StoredProcedure = "StoredProcedure";
        //public const string StoredProcedureParameter = "StoredProcedureParameter";
        //public const string StoredProcedureColumn = "StoredProcedureColumn";
        //public const string Function = "Function";
        //public const string FunctionParameter = "FunctionParameter";
        //public const string FunctionColumn = "FunctionColumn";
        //public const string UserDefinedType = "UserDefinedType";
    }
}
