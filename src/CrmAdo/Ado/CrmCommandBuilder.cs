using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo
{
    public class CrmCommandBuilder : DbCommandBuilder
    {
        public override ConflictOption ConflictOption
        {
            get
            {
                return ConflictOption.CompareRowVersion;
            }
            set
            {
                base.ConflictOption = value;
            }
        }

        /// <summary>
        /// Shadowing base adapator.
        /// </summary>
        public new CrmDataAdapter DataAdapter
        {
            get
            {
                return (CrmDataAdapter)base.DataAdapter;
            }
            set
            {
                base.DataAdapter = value;
            }
        }

        public CrmCommandBuilder()
        {
            base.QuotePrefix = "[";
            base.QuoteSuffix = "]";
        }

        public CrmCommandBuilder(CrmDataAdapter dataAdapter)
            : this()
        {
            this.DataAdapter = dataAdapter;
        }


        /// <summary>
        /// Shadowing base command.
        /// </summary>
        /// <returns></returns>
        public new CrmDbCommand GetDeleteCommand()
        {
            return (CrmDbCommand)base.GetDeleteCommand();
        }

        /// <summary>
        /// Shadowing base command.
        /// </summary>
        /// <param name="useColumnsForParameterNames"></param>
        /// <returns></returns>
        public new CrmDbCommand GetDeleteCommand(bool useColumnsForParameterNames)
        {
            return (CrmDbCommand)base.GetDeleteCommand(useColumnsForParameterNames);
        }

        /// <summary>
        /// Shadowing base command.
        /// </summary>
        /// <returns></returns>
        public new CrmDbCommand GetInsertCommand()
        {
            return (CrmDbCommand)base.GetInsertCommand();
        }

        /// <summary>
        /// Shadowing base command.
        /// </summary>
        /// <param name="useColumnsForParameterNames"></param>
        /// <returns></returns>
        public new CrmDbCommand GetInsertCommand(bool useColumnsForParameterNames)
        {
            return (CrmDbCommand)base.GetInsertCommand(useColumnsForParameterNames);
        }

        /// <summary>
        /// Shadowing base command.
        /// </summary>
        /// <returns></returns>
        public new CrmDbCommand GetUpdateCommand()
        {
            return (CrmDbCommand)base.GetUpdateCommand();
        }

        /// <summary>
        /// Shadowing base command.
        /// </summary>
        /// <param name="useColumnsForParameterNames"></param>
        /// <returns></returns>
        public new CrmDbCommand GetUpdateCommand(bool useColumnsForParameterNames)
        {
            return (CrmDbCommand)base.GetUpdateCommand(useColumnsForParameterNames);
        }

        protected override string GetParameterName(int ordinal)
        {
            return string.Concat("@p", ordinal);
        }

        protected override string GetParameterName(string name)
        {
            return string.Concat("@", name);
        }

        protected override string GetParameterPlaceholder(int ordinal)
        {
            return this.GetParameterName(ordinal);
        }

        protected override DataTable GetSchemaTable(DbCommand srcCommand)
        {
            using (var dataReader = srcCommand.ExecuteReader(CommandBehavior.SchemaOnly))
            {
                try
                {
                    return dataReader.GetSchemaTable();
                }
                finally
                {
                    dataReader.Close();
                }
            }
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
        {
            if (adapter != base.DataAdapter)
            {
                ((CrmDataAdapter)adapter).RowUpdating += CrmCommandBuilder_RowUpdating;
            }
            else
            {
                ((CrmDataAdapter)adapter).RowUpdating -= CrmCommandBuilder_RowUpdating;
            }
        }

        void CrmCommandBuilder_RowUpdating(object sender, RowUpdatingEventArgs e)
        {
            base.RowUpdatingHandler(e);
        }

        public override string QuoteIdentifier(string unquotedIdentifier)
        {
            // Escape any existing escape characters, and existing quote prefix and suffix characters.
            string escapedQuotePrefix = "\\" + QuotePrefix;
            string escapedQuoteSuffix = "\\" + QuoteSuffix;

            unquotedIdentifier = unquotedIdentifier.Replace("\\", "\\\\")
                                                   .Replace(QuotePrefix, escapedQuotePrefix)
                                                   .Replace(QuoteSuffix, escapedQuoteSuffix);
            // Now wrap in quotes.
            return string.Concat(this.QuotePrefix, unquotedIdentifier, this.QuoteSuffix);
        }

        public override string UnquoteIdentifier(string quotedIdentifier)
        {
            string escapedQuotePrefix = "\\" + QuotePrefix;
            string escapedQuoteSuffix = "\\" + QuoteSuffix;
            if (!string.IsNullOrEmpty(quotedIdentifier))
            {
                // Unescape any previously escaped characters.
                if (quotedIdentifier.StartsWith(QuotePrefix))
                {
                    quotedIdentifier = quotedIdentifier.Remove(0, QuotePrefix.Length);
                    quotedIdentifier = quotedIdentifier.Replace(escapedQuotePrefix, QuotePrefix)
                                                       .Replace(escapedQuoteSuffix, QuoteSuffix)
                                                       .Replace("\\\\", "\\");
                }
                return quotedIdentifier;
            }
            else
            {
                return quotedIdentifier;
            }
        }

        protected override void ApplyParameterInfo(DbParameter parameter, DataRow dataRow, StatementType statementType, bool whereClause)
        {
           
        }
    }
}
