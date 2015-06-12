using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CrmAdo.Core
{
    public interface ISchemaCollectionsProvider
    {

        DataTable GetSchema(CrmDbConnection crmDbConnection, string collectionName, string[] restrictions);

        DataTable GetMetadataCollections();

        DataTable GetDataSourceInfo(CrmDbConnection connection);

        DataTable GetDataTypes();

        DataTable GetReservedWords();

        DataTable GetRestrictions();

        DataTable GetUsers(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetTables(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetColumns(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetViews(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetViewColumns(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetForeignKeys(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetForeignKeyColumns(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetIndexes(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetIndexColumns(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetDatabases(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetUniqueKeys(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetUniqueKeyColumns(CrmDbConnection crmDbConnection, string[] restrictions);

    }

    public class SchemaCollectionsProvider : ISchemaCollectionsProvider
    {
        public const string DefaultSchema = "dbo";

        public DataTable GetMetadataCollections()
        {
            var dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            using (var reader = new StringReader(Properties.Resources.MetaDataCollections))
            {
                dt.ReadXml(reader);
                return dt;
            }
        }

        public DataTable GetDataSourceInfo(CrmDbConnection connection)
        {

            //   http://msdn.microsoft.com/en-us/library/ms254501%28v=vs.110%29.aspx

            DataTable dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;

            dataTable.Columns.Add(DbMetaDataColumnNames.CompositeIdentifierSeparatorPattern, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.DataSourceProductName, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.DataSourceProductVersion, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.DataSourceProductVersionNormalized, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.GroupByBehavior, typeof(GroupByBehavior));
            dataTable.Columns.Add(DbMetaDataColumnNames.IdentifierPattern, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.IdentifierCase, typeof(IdentifierCase));
            dataTable.Columns.Add(DbMetaDataColumnNames.OrderByColumnsInSelect, typeof(bool));
            dataTable.Columns.Add(DbMetaDataColumnNames.ParameterMarkerFormat, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.ParameterMarkerPattern, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.ParameterNameMaxLength, typeof(int));
            dataTable.Columns.Add(DbMetaDataColumnNames.ParameterNamePattern, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.QuotedIdentifierPattern, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.QuotedIdentifierCase, typeof(IdentifierCase));
            dataTable.Columns.Add(DbMetaDataColumnNames.StatementSeparatorPattern, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.StringLiteralPattern, typeof(string));
            dataTable.Columns.Add(DbMetaDataColumnNames.SupportedJoinOperators, typeof(SupportedJoinOperators));
            dataTable.Columns.Add("ColumnAliasSupported", typeof(bool));
            dataTable.Columns.Add("TableAliasSupported", typeof(bool));
            dataTable.Columns.Add("SchemaSupported", typeof(bool));
            dataTable.Columns.Add("CatalogSupported", typeof(bool));


            DataRow dataRow = dataTable.NewRow();
            dataRow[DbMetaDataColumnNames.CompositeIdentifierSeparatorPattern] = @"\.";
            dataRow[DbMetaDataColumnNames.DataSourceProductName] = "Dynamics CRM";
            dataRow[DbMetaDataColumnNames.DataSourceProductVersion] = connection.ServerVersion;
            dataRow[DbMetaDataColumnNames.DataSourceProductVersionNormalized] = connection.ServerVersion;
            dataRow[DbMetaDataColumnNames.GroupByBehavior] = GroupByBehavior.Unrelated;



            dataRow[DbMetaDataColumnNames.IdentifierPattern] = @"(^\[\p{Lo}\p{Lu}\p{Ll}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Nd}@$#_]*$)|(^\[[^\]\0]|\]\]+\]$)|(^\""[^\""\0]|\""\""+\""$)";
            //  dataRow[DbMetaDataColumnNames.IdentifierPattern] = @"([\p{L}:?@#_][\p{L}\p{N}@#$_]*)|(""(\.|"""")+"")|(\[[^\]]+\])";  // [A-Za-z0-9_#$] 
            dataRow[DbMetaDataColumnNames.IdentifierCase] = IdentifierCase.Insensitive;
            dataRow[DbMetaDataColumnNames.OrderByColumnsInSelect] = false;
            dataRow[DbMetaDataColumnNames.ParameterMarkerFormat] = "@{0}";
            dataRow[DbMetaDataColumnNames.ParameterMarkerPattern] = @"@[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            //dataRow[DbMetaDataColumnNames.ParameterMarkerPattern] = "(@[A-Za-z0-9_$#]*)";
            dataRow[DbMetaDataColumnNames.ParameterNameMaxLength] = 128;
            dataRow[DbMetaDataColumnNames.ParameterNamePattern] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)"; //  \p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}


            dataRow[DbMetaDataColumnNames.QuotedIdentifierPattern] = @"(([^\[]|\]\])*)";
            // dataRow[DbMetaDataColumnNames.QuotedIdentifierPattern] = @"\[(.*?)\]"; //TODO THIS IS WRONG AS IT MATCHES THE QUOTES AND I NEED TO EXLCUDE THEM..;

            dataRow[DbMetaDataColumnNames.QuotedIdentifierCase] = IdentifierCase.Insensitive;
            dataRow[DbMetaDataColumnNames.StatementSeparatorPattern] = @";";
            dataRow[DbMetaDataColumnNames.StringLiteralPattern] = @"'(([^']|'')*)'"; //TODO THIS IS WRONG AS IT MATCHES THE QUOTES AND I NEED TO EXLCUDE THEM..;
            dataRow[DbMetaDataColumnNames.SupportedJoinOperators] = SupportedJoinOperators.Inner | SupportedJoinOperators.LeftOuter;

            #region probably none of this needed

            //dataTable.Columns.Add("IdentifierOpenQuote", typeof(string));
            //dataTable.Columns.Add("IdentifierCloseQuote", typeof(string));
            //dataTable.Columns.Add("SupportsAnsi92Sql", typeof(bool));
            //dataTable.Columns.Add("SupportsQuotedIdentifierParts", typeof(bool));
            //dataTable.Columns.Add("ParameterPrefix", typeof(string));
            //dataTable.Columns.Add("ParameterPrefixInName", typeof(bool));         

            dataRow["ColumnAliasSupported"] = false;
            dataRow["TableAliasSupported"] = true;
            dataRow["SchemaSupported"] = true;
            dataRow["CatalogSupported"] = true;

            //dataTable.Columns.Add("TableSupported", typeof(bool));
            //dataTable.Columns.Add("UserSupported", typeof(bool));         
            //dataTable.Columns.Add("SupportsVerifySQL", typeof(bool));            


            //dataRow["IdentifierOpenQuote"] = "[";
            //dataRow["IdentifierCloseQuote"] = "]";
            //dataRow["SupportsAnsi92Sql"] = false;
            //dataRow["SupportsQuotedIdentifierParts"] = true;
            //dataRow["ParameterPrefix"] = "@";
            //dataRow["ParameterPrefixInName"] = true;        
            //dataRow["TableSupported"] = true;
            //dataRow["UserSupported"] = false;          
            //dataRow["SupportsVerifySQL"] = false;

            #endregion

            dataTable.Rows.Add(dataRow);
            return dataTable;
        }

        public DataTable GetDataTypes()
        {
            var dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            using (var reader = new StringReader(Properties.Resources.DataTypes))
            {
                dt.ReadXml(reader);
                return dt;
            }
        }

        public DataTable GetReservedWords()
        {
            DataTable table = new DataTable("ReservedWords");
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("ReservedWord", typeof(string));

            // List of keywords taken from SQLGeneration library token registry grammer.
            string[] keywords = new[]
            {
                "TOP",
                "UPDATE",
                "VALUES",
                "WHERE",
                "WITH",
                "BETWEEN",
                "AND",
                "OR",
                "DELETE",
                "ALL",
                "ANY",
                "SOME",
                "FROM",
                "GROUP",
                "BY",
                "HAVING",
                "INSERT",
                "INTO",
                "IS",
                "FULL",
                "OUTER",
                "JOIN",
                "INNER",
                "LEFT",
                "RIGHT",
                "CROSS",
                "IN",
                "LIKE",
                "NOT",
                "NULLS",
                "NULL",
                "ORDER",
                "ASC",
                "DESC",
                "PERCENT",
                "SELECT",
                "UNION",
                "INTERSECT",
                "EXCEPT",
                "MINUS",
                "SET",
                "ON",
                "AS",
                "EXISTS",
                "OVER",
                "PARTITION",
                "ROWS",
                "RANGE",
                "UNBOUNDED",
                "PRECEDING",
                "FOLLOWING",
                "CURRENT",
                "ROW",
                "CASE",
                "WHEN",
                "THEN",
                "ELSE",
                "END",
                "CREATE", /// DDL
                "DATABASE",
                "TABLE",
                "PRIMARY",
                "KEY",
                "COLLATE",
                "CONSTRAINT",
                "IDENTITY",
                "DEFAULT",
                "ROWGUIDCOL",
                "UNIQUE",
                "CLUSTERED",
                "NONCLUSTERED",
                "FOREIGN",
                "REFERENCES",
                "NO",
                "ACTION",
                "CASCADE",
                "FOR",
                "REPLICATION",
                "CHECK",
                "ALTER",
                "MODIFY",
                "CURRENT",
                "COLUMN",
                "ADD",
                "DROP",
                "PERSISTED",
                "SPARSE"               
            };
            foreach (string keyword in keywords)
            {
                table.Rows.Add(keyword);
            }
            return table;
        }

        public DataTable GetRestrictions()
        {
            var dt = new DataTable();
            dt.Locale = CultureInfo.InvariantCulture;
            using (var reader = new StringReader(Properties.Resources.Restrictions))
            {
                dt.ReadXml(reader);
                return dt;
            }
        }

        public DataTable GetUsers(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            var command = new CrmDbCommand(crmDbConnection);
            command.CommandText = "SELECT su.systemuserid, su.fullname, su.domainname, su.createdon, su.modifiedon FROM systemuser su";

            if (restrictions != null && restrictions.Any())
            {
                string userName = restrictions[0];
                command.CommandText = string.Format("{0} WHERE su.fullname = {1}", command.CommandText, userName);
            }
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            adapter.Fill(dataTable);
            return dataTable;
        }

        private static string GetRestrictionOrNull(int index, string[] restrictions)
        {
            if (restrictions != null)
            {
                int length = restrictions.Length;
                if (length > 0)
                {
                    if (length - 1 >= index)
                    {
                        return restrictions[index];
                    }
                }
            }

            return null;
        }

        public DataTable GetTables(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            //            table_catalog
            //table_schema
            //table_name
            //table_type
            //Type of table. Can be VIEW or BASE TABLE.

            var command = new CrmDbCommand(crmDbConnection);
            string commandText = "SELECT * FROM EntityMetadata";

            string catalog = GetRestrictionOrNull(0, restrictions);
            string schema = GetRestrictionOrNull(1, restrictions);
            string tableName = GetRestrictionOrNull(2, restrictions);
            string tableType = GetRestrictionOrNull(3, restrictions); // doesn't matter currently what tabletype restriction is specified, we only return "base tables" not views.

            if (catalog != null && catalog.ToLowerInvariant() != crmDbConnection.ConnectionInfo.OrganisationName.ToLowerInvariant())
            {
                // we only support the catalog currently connected to, can't query accross other catalogs.
                throw new ArgumentException("invalid catalog restriction. no such catalog.");
            }

            if (schema != null && schema.ToLowerInvariant() != DefaultSchema.ToLowerInvariant())
            {
                // we only support a single schema "dbo".
                throw new ArgumentException("invalid schema restriction. no such schema.");
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                commandText = commandText + " WHERE LogicalName = '" + tableName + "'";
            }

            command.CommandText = commandText;

            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            adapter.Fill(dataTable);
            dataTable.Columns.Add("TABLE_CATALOG", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(0);
            dataTable.Columns.Add("TABLE_SCHEMA", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(1);
            dataTable.Columns["logicalname"].ColumnName = "TABLE_NAME";
            dataTable.Columns["TABLE_NAME"].SetOrdinal(2);
            dataTable.Columns.Add("TABLE_TYPE", typeof(string), "'BASE TABLE'").SetOrdinal(3);
            return dataTable;

        }

        public DataTable GetColumns(CrmDbConnection crmDbConnection, string[] restrictions)
        {


            //TABLE_CATALOG
            //TABLE_SCHEMA
            //TABLE_NAME
            //COLUMN_NAME
            //ORDINAL_POSITION
            //COLUMN_DEFAULT
            //IS_NULLABLE
            //DATA_TYPE
            //CHARACTER_MAXIMUM_LENGTH
            //CHARACTER_OCTET_LENGTH
            //NUMERIC_PRECISION
            //NUMERIC_PRECISION_RADIX
            //NUMERIC_SCALE
            //DATETIME_PRECISION
            //CHARACTER_SET_CATALOG
            //CHARACTER_SET_SCHEMA
            //CHARACTER_SET_NAME
            //COLLATION_CATALOG
            //IS_SPARSE
            //IS_COLUMN_SET
            //IS_FILESTREAM
            string catalog = GetRestrictionOrNull(0, restrictions);
            string schema = GetRestrictionOrNull(1, restrictions);
            string entityName = GetRestrictionOrNull(2, restrictions);
            string attributeName = GetRestrictionOrNull(3, restrictions);
            bool hasEntityFilter = false;
            bool hasAttributeFilter = false;

            if (catalog != null && catalog.ToLowerInvariant() != crmDbConnection.ConnectionInfo.OrganisationName.ToLowerInvariant())
            {
                // we only support the catalog currently connected to, can't query accross other catalogs.
                throw new ArgumentException("invalid catalog restriction. no such catalog.");
            }

            if (schema != null && schema.ToLowerInvariant() != DefaultSchema.ToLowerInvariant())
            {
                // we only support a single schema "dbo".
                throw new ArgumentException("invalid schema restriction. no such schema.");
            }

            hasEntityFilter = !string.IsNullOrEmpty(entityName);
            hasAttributeFilter = !string.IsNullOrEmpty(attributeName);

            var commandText = "SELECT entitymetadata.PrimaryIdAttribute, attributemetadata.* FROM entitymetadata INNER JOIN attributemetadata ON entitymetadata.MetadataId = attributemetadata.MetadataId ";

            if (hasEntityFilter || hasAttributeFilter)
            {
                commandText += "WHERE ";
                if (hasEntityFilter)
                {
                    commandText += " (entitymetadata.LogicalName = '" + entityName + "')";
                }
                if (hasEntityFilter && hasAttributeFilter)
                {
                    commandText += " AND ";
                }
                if (hasAttributeFilter)
                {
                    commandText += " (attributemetadata.LogicalName = '" + attributeName + "')";
                }
            }

            var command = new CrmDbCommand(crmDbConnection);
            command.CommandText = commandText;
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;

            adapter.Fill(dataTable);
            dataTable.Columns.Add("TABLE_CATALOG", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(0);
            dataTable.Columns.Add("TABLE_SCHEMA", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(1);
            dataTable.Columns["entitylogicalname"].ColumnName = "TABLE_NAME";
            dataTable.Columns["TABLE_NAME"].SetOrdinal(2);

            dataTable.Columns["logicalname"].ColumnName = "COLUMN_NAME";
            dataTable.Columns["COLUMN_NAME"].SetOrdinal(3);

            dataTable.Columns["columnnumber"].ColumnName = "ORDINAL_POSITION";
            dataTable.Columns["ORDINAL_POSITION"].SetOrdinal(4);

            // dataTable.Columns["defaultvalue"].ColumnName = "COLUMN_DEFAULT";
            dataTable.Columns.Add("COLUMN_DEFAULT", dataTable.Columns["defaultvalue"].DataType, "IIF([defaultvalue] = '-1', '', IIF([defaultvalue] = 'TRUE', '((' + 1 + '))', IIF([defaultvalue] = 'FALSE', '((0))', '((' + [defaultvalue] + '))')))").SetOrdinal(5);

            // dataTable.Columns["COLUMN_DEFAULT"].SetOrdinal(5);
            //  dataTable.Columns["isnullable"].ColumnName = "IS_NULLABLE";
            dataTable.Columns.Add("IS_NULLABLE", typeof(string), "IIF([isnullable] = True, 'YES', 'NO')").SetOrdinal(6);
            //dataTable.Columns["IS_NULLABLE"].SetOrdinal(6);

            dataTable.Columns["datatype"].ColumnName = "DATA_TYPE";
            dataTable.Columns["DATA_TYPE"].SetOrdinal(7);

            // dataTable.Columns["attributemetadata.length"].ColumnName = "character_maximum_length";
            dataTable.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int), "IIF(data_type = 'nvarchar', [maxlength], NULL)").SetOrdinal(8);
            dataTable.Columns.Add("CHARACTER_OCTET_LENGTH", typeof(int), "IIF(data_type ='nvarchar',ISNULL(character_maximum_length, 0) * 2, character_maximum_length)").SetOrdinal(9);

            dataTable.Columns["numericprecision"].ColumnName = "NUMERIC_PRECISION";
            dataTable.Columns["NUMERIC_PRECISION"].SetOrdinal(10);

            dataTable.Columns["numericprecisionradix"].ColumnName = "NUMERIC_PRECISION_RADIX";
            dataTable.Columns["NUMERIC_PRECISION_RADIX"].SetOrdinal(11);

            dataTable.Columns["numericscale"].ColumnName = "NUMERIC_SCALE";
            dataTable.Columns["NUMERIC_SCALE"].SetOrdinal(12);

            dataTable.Columns.Add("DATETIME_PRECISION", typeof(int), "IIF(data_type = 'datetime', 3, NULL)").SetOrdinal(13);

            dataTable.Columns.Add("CHARACTER_SET_CATALOG", typeof(string), "NULL").SetOrdinal(14);
            dataTable.Columns.Add("CHARACTER_SET_SCHEMA", typeof(string), "NULL").SetOrdinal(15);

            dataTable.Columns.Add("CHARACTER_SET_NAME", typeof(string), "IIF(data_type ='nvarchar', 'UNICODE', NULL)").SetOrdinal(16);

            dataTable.Columns.Add("COLLATION_CATALOG", typeof(string), "NULL").SetOrdinal(17);
            dataTable.Columns.Add("IS_SPARSE", typeof(bool), "false").SetOrdinal(18);
            dataTable.Columns.Add("IS_COLUMN_SET", typeof(bool), "false").SetOrdinal(19);
            dataTable.Columns.Add("IS_FILESTREAM", typeof(bool), "false").SetOrdinal(20);

            return dataTable;

        }

        public DataTable GetViews(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            DataTable dataTable = new DataTable("Views");
            dataTable.Locale = CultureInfo.InvariantCulture;
            dataTable.Columns.AddRange(
                new DataColumn[]
                    {
                        new DataColumn("TABLE_CATALOG"), 
                        new DataColumn("TABLE_SCHEMA"), 
                        new DataColumn("TABLE_NAME"),
                        new DataColumn("CHECK_OPTION"), 
                        new DataColumn("IS_UPDATABLE")
                    });

            return dataTable;
            // throw new NotImplementedException();
        }

        public DataTable GetViewColumns(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            DataTable dataTable = new DataTable("ViewColumns");
            dataTable.Locale = CultureInfo.InvariantCulture;
            dataTable.Columns.AddRange(
                new DataColumn[]
                    {
                        new DataColumn("VIEW_CATALOG"), 
                        new DataColumn("VIEW_SCHEMA"), 
                        new DataColumn("VIEW_NAME"),
                        new DataColumn("TABLE_CATALOG"), 
                        new DataColumn("TABLE_SCHEMA"), 
                        new DataColumn("TABLE_NAME"), 
                        new DataColumn("COLUMN_NAME")                        
                    });

            return dataTable;
        }

        public DataTable GetIndexes(CrmDbConnection crmDbConnection, string[] restrictions)
        {

            string catalog = GetRestrictionOrNull(0, restrictions);
            string schema = GetRestrictionOrNull(1, restrictions);
            string table = GetRestrictionOrNull(2, restrictions);
            string constraintName = GetRestrictionOrNull(3, restrictions);

            bool hasEntityFilter = !string.IsNullOrEmpty(table);
            bool hasConstraintNameFilter = !string.IsNullOrEmpty(constraintName);

            string commandText = "SELECT LogicalName, PrimaryIdAttribute FROM entitymetadata ";

            if (hasEntityFilter || hasConstraintNameFilter)
            {
                commandText += "WHERE ";
                if (hasEntityFilter)
                {
                    commandText += " (LogicalName = '" + table + "')";
                }
                //if (hasEntityFilter && hasConstraintNameFilter)
                //{
                //    commandText += " AND ";
                //}
                //if (hasConstraintNameFilter)
                //{
                //    commandText += " (PrimaryIdAttribute = '" + constraintName + "')";
                //}
            }

            var command = new CrmDbCommand(crmDbConnection);
            command.CommandText = commandText;
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            adapter.Fill(dataTable);


            //          <Indexes>
            //  <constraint_catalog>PortalDarrellDev</constraint_catalog>
            //  <constraint_schema>dbo</constraint_schema>
            //  <constraint_name>PK__tmp_ms_x__3214EC0737311087</constraint_name>
            //  <table_catalog>PortalDarrellDev</table_catalog>
            //  <table_schema>dbo</table_schema>
            //  <table_name>Table</table_name>
            //  <index_name>PK__tmp_ms_x__3214EC0737311087</index_name>
            //  <type_desc>CLUSTERED</type_desc>
            //</Indexes>

            dataTable.Columns.Add("constraint_catalog", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(0);
            dataTable.Columns.Add("constraint_schema", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(1);
            dataTable.Columns.Add("constraint_name", typeof(string), "'PK__' + [LogicalName] + '_' + [PrimaryIdAttribute]").SetOrdinal(2);

            dataTable.Columns.Add("table_catalog", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(3);
            dataTable.Columns.Add("table_schema", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(4);
            dataTable.Columns.Add("table_name", typeof(string), "[LogicalName]").SetOrdinal(5);

            //dataTable.Columns["LogicalName"].ColumnName = "table_name";
            // dataTable.Columns["table_name"].SetOrdinal(5);

            dataTable.Columns.Add("index_name", typeof(string), "constraint_name").SetOrdinal(6);
            dataTable.Columns.Add("type_desc", typeof(string), "'CLUSTERED'").SetOrdinal(7);

            if (hasConstraintNameFilter)
            {
                var filteredView = dataTable.AsDataView();
                filteredView.RowFilter = "constraint_name = '" + constraintName + "'";
                dataTable = filteredView.ToTable(true);
            }

            return dataTable;
        }

        public DataTable GetIndexColumns(CrmDbConnection crmDbConnection, string[] restrictions)
        {

            string catalog = GetRestrictionOrNull(0, restrictions);
            string schema = GetRestrictionOrNull(1, restrictions);
            string table = GetRestrictionOrNull(2, restrictions);
            string constraintname = GetRestrictionOrNull(3, restrictions);
            string columnname = GetRestrictionOrNull(4, restrictions);

            bool hasEntityFilter = !string.IsNullOrEmpty(table);
            bool hasColumnFilter = !string.IsNullOrEmpty(columnname);
            bool hasConstraintNameFilter = !string.IsNullOrEmpty(constraintname);


            string commandText = "SELECT entitymetadata.PrimaryIdAttribute, entitymetadata.LogicalName, a.LogicalName, a.ColumnNumber FROM entitymetadata JOIN attributemetadata a on entitymetadata.MetadataId = a.MetadataId WHERE (a.isprimaryid = @isPrimaryId)";

            if (hasEntityFilter || hasColumnFilter)
            {
                commandText += "AND ";
                if (hasEntityFilter)
                {
                    commandText += " (entitymetadata.LogicalName = '" + table + "')";
                }
                if (hasEntityFilter && hasColumnFilter)
                {
                    commandText += " AND ";
                }
                if (hasColumnFilter)
                {
                    commandText += " (a.LogicalName = '" + columnname + "')";
                }
            }

            var command = new CrmDbCommand(crmDbConnection);
            var param = command.CreateParameter();
            param.DbType = DbType.Boolean;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = "@isPrimaryId";
            param.Value = true;
            command.Parameters.Add(param);
            command.CommandText = commandText;
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            adapter.Fill(dataTable);


            //         <IndexColumns>
            //  <constraint_catalog>PortalDarrellDev</constraint_catalog>
            //  <constraint_schema>dbo</constraint_schema>
            //  <constraint_name>PK__tmp_ms_x__3214EC0737311087</constraint_name>
            //  <table_catalog>PortalDarrellDev</table_catalog>
            //  <table_schema>dbo</table_schema>
            //  <table_name>Table</table_name>
            //  <column_name>Id</column_name>
            //  <ordinal_position>1</ordinal_position>
            //  <KeyType>36</KeyType>
            //  <index_name>PK__tmp_ms_x__3214EC0737311087</index_name>
            //</IndexColumns>

            dataTable.Columns.Add("constraint_catalog", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(0);
            dataTable.Columns.Add("constraint_schema", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(1);

            dataTable.Columns["LogicalName"].ColumnName = "table_name";
            dataTable.Columns.Add("column_name", typeof(string), "IsNull([a.LogicalName], [PrimaryIdAttribute])");

            dataTable.Columns.Add("constraint_name", typeof(string), "'PK__' + IsNull(table_name, '') + '_' + IsNull(column_name, PrimaryIdAttribute)").SetOrdinal(2);
            dataTable.Columns.Add("table_catalog", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(3);
            dataTable.Columns.Add("table_schema", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(4);

            dataTable.Columns["table_name"].SetOrdinal(5);
            dataTable.Columns["column_name"].SetOrdinal(6);
            // dataTable.Columns["a.LogicalName"].ColumnName = "column_name";

            dataTable.Columns["a.columnnumber"].ColumnName = "ordinal_position";
            dataTable.Columns["ordinal_position"].SetOrdinal(7);

            dataTable.Columns.Add("KeyType", typeof(Byte), "36").SetOrdinal(8); // 36 = uniqueidentitifer datatype - all pk indexes in crm are uniqueidentifiers.
            dataTable.Columns.Add("index_name", typeof(string), "constraint_name").SetOrdinal(9);
            //  dataTable.Columns.Add("type_desc", typeof(string), "'CLUSTERED'");


            var filteredView = dataTable.AsDataView();
            filteredView.RowFilter = "column_name = PrimaryIdAttribute";  // necessary due to #68
            dataTable = filteredView.ToTable(true);

            if (hasConstraintNameFilter)
            {
                filteredView = dataTable.AsDataView();
                filteredView.RowFilter = "constraint_name = '" + constraintname + "'";
                dataTable = filteredView.ToTable(true);
            }


            return dataTable;
        }

        public DataTable GetForeignKeys(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            // throw new NotImplementedException();

            string catalog = GetRestrictionOrNull(0, restrictions);
            string schema = GetRestrictionOrNull(1, restrictions);
            string constraintTable = GetRestrictionOrNull(2, restrictions);
            string constraintName = GetRestrictionOrNull(3, restrictions);

            //  string entityName = GetRestrictionOrNull(0, restrictions);
            // string constraintName = GetRestrictionOrNull(1, restrictions);
            bool hasConstraintTableFilter = !string.IsNullOrEmpty(constraintTable);
            bool hasConstraintNameFilter = !string.IsNullOrEmpty(constraintName);

            string commandText = "SELECT o.* FROM entitymetadata e INNER JOIN onetomanyrelationshipmetadata o ON e.MetadataId = o.MetadataId ";

            if (hasConstraintTableFilter || hasConstraintNameFilter)
            {
                commandText += "WHERE ";
                if (hasConstraintTableFilter)
                {
                    commandText += " (e.LogicalName = '" + constraintTable + "') AND (o.referencingentity = '" + constraintTable + "')";
                }
                if (hasConstraintTableFilter && hasConstraintNameFilter)
                {
                    commandText += " AND ";
                }
                if (hasConstraintNameFilter)
                {
                    commandText += " (o.SchemaName = '" + constraintName + "')";
                }
            }


            var command = new CrmDbCommand(crmDbConnection);
            command.CommandText = commandText;
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            adapter.Fill(dataTable);

            //  dataTable.AsDataView().RowFilter = "ReferencingEntity = '" + entityName + 

            dataTable.Columns.Add("CONSTRAINT_CATALOG", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(0);
            dataTable.Columns.Add("CONSTRAINT_SCHEMA", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(1);

            dataTable.Columns.Add("CONSTRAINT_NAME", typeof(string), "o.schemaname").SetOrdinal(2);

            //dataTable.Columns["o.schemaname"].ColumnName = "CONSTRAINT_NAME";
            //dataTable.Columns["CONSTRAINT_NAME"].SetOrdinal(2);

            //   dataTable.Columns.Add("CONSTRAINT_NAME", typeof(string), "''");

            dataTable.Columns.Add("TABLE_CATALOG", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(3);
            dataTable.Columns.Add("TABLE_SCHEMA", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(4);

            dataTable.Columns["o.referencingentity"].ColumnName = "TABLE_NAME";
            dataTable.Columns["TABLE_NAME"].SetOrdinal(5);

            dataTable.Columns.Add("CONSTRAINT_TYPE", typeof(string), "'FOREIGN KEY'").SetOrdinal(6);

            dataTable.Columns.Add("IS_DEFERRABLE", typeof(string), "'NO'").SetOrdinal(7);
            dataTable.Columns.Add("INITIALLY_DEFERRED", typeof(string), "'NO'").SetOrdinal(8);


            //if (hasEntityFilter)
            //{
            //    filteredView.RowFilter = "TABLE_NAME = '" + entityName + "'";
            //}
            //dataTable = filteredView.ToTable(true);

            if (hasConstraintNameFilter)
            {
                var filteredView = dataTable.AsDataView();
                filteredView.RowFilter = "CONSTRAINT_NAME = '" + constraintName + "'";
                dataTable = filteredView.ToTable(true);
            }


            //else
            //{               
            //    dataTable = filteredView.ToTable(true);  
            //}

            return dataTable;
        }

        public DataTable GetForeignKeyColumns(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            string catalog = GetRestrictionOrNull(0, restrictions);
            string schema = GetRestrictionOrNull(1, restrictions);
            string table = GetRestrictionOrNull(2, restrictions);
            string constraintname = GetRestrictionOrNull(3, restrictions);
            string columnname = GetRestrictionOrNull(4, restrictions);

            bool hasEntityFilter = !string.IsNullOrEmpty(table);
            bool hasColumnFilter = !string.IsNullOrEmpty(columnname);
            bool hasConstraintNameFilter = !string.IsNullOrEmpty(constraintname);

            string commandText = "SELECT e.*, o.* FROM entitymetadata e INNER JOIN onetomanyrelationshipmetadata o ON e.MetadataId = o.MetadataId ";

            if (hasEntityFilter || hasColumnFilter || hasConstraintNameFilter)
            {
                commandText += "WHERE ";
                if (hasEntityFilter)
                {
                    commandText += " (e.LogicalName = '" + table + "') AND (o.ReferencingEntity = '" + table + "')";
                }
                if (hasEntityFilter && (hasConstraintNameFilter || hasColumnFilter))
                {
                    commandText += " AND ";
                }
                if (hasConstraintNameFilter)
                {
                    commandText += " (o.SchemaName = '" + constraintname + "')";
                }
                if (hasConstraintNameFilter && hasColumnFilter)
                {
                    commandText += " AND ";
                }
                if (hasColumnFilter)
                {
                    commandText += " (o.ReferencingAttribute = '" + columnname + "')";
                }
            }


            var command = new CrmDbCommand(crmDbConnection);
            command.CommandText = commandText;
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            adapter.Fill(dataTable);


            dataTable.Columns.Add("constraint_catalog", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(0);
            dataTable.Columns.Add("constraint_schema", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(1);
            dataTable.Columns.Add("constraint_name", typeof(string), "o.schemaname").SetOrdinal(2);

            dataTable.Columns.Add("table_catalog", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(3);
            dataTable.Columns.Add("table_schema", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(4);
            dataTable.Columns["e.LogicalName"].ColumnName = "table_name";
            dataTable.Columns["table_name"].SetOrdinal(5);

            dataTable.Columns.Add("column_name", typeof(string), "o.ReferencingAttribute").SetOrdinal(6);
            dataTable.Columns["a.columnnumber"].ColumnName = "ordinal_position";
            dataTable.Columns["ordinal_position"].SetOrdinal(7);

            dataTable.Columns.Add("constraint_type", typeof(string), "'FOREIGN KEY'").SetOrdinal(8);
            dataTable.Columns.Add("index_name", typeof(string), "constraint_name").SetOrdinal(9);

            //  dataTable.Columns.Add("constraint_name", typeof(string), "'FK__' + IsNull(table_name, '') + '_' + IsNull(column_name, PrimaryIdAttribute)").SetOrdinal(2);

            //  dataTable.Columns["column_name"].SetOrdinal(6);       



            //   dataTable.Columns.Add("KeyType", typeof(Byte), "36").SetOrdinal(8); // 36 = uniqueidentitifer datatype - all pk indexes in crm are uniqueidentifiers.
            //  dataTable.Columns.Add("type_desc", typeof(string), "'CLUSTERED'");

            //var filteredView = dataTable.AsDataView();
            //filteredView.RowFilter = "column_name = PrimaryIdAttribute";  // necessary due to #68
            //dataTable = filteredView.ToTable(true);

            //if (hasConstraintNameFilter)
            //{
            //    var filteredView = dataTable.AsDataView();
            //    filteredView.RowFilter = "constraint_name = '" + constraintname + "'";
            //    dataTable = filteredView.ToTable(true);
            //}

            return dataTable;


        }

        public DataTable GetUniqueKeys(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            // throw new NotImplementedException();

            string entityName = GetRestrictionOrNull(0, restrictions);
            bool hasEntityFilter = !string.IsNullOrEmpty(entityName);

            string constraintName = GetRestrictionOrNull(1, restrictions);
            bool hasConstraintNameFilter = !string.IsNullOrEmpty(constraintName);

            string commandText = "SELECT * FROM entitymetadata";

            if (hasEntityFilter || hasConstraintNameFilter)
            {
                commandText += " WHERE";
                if (hasEntityFilter)
                {
                    commandText += " (LogicalName = '" + entityName + "')";
                }
                //if (hasEntityFilter && hasConstraintNameFilter)
                //{
                //    commandText += " AND ";
                //}
                //if (hasConstraintNameFilter)
                //{
                //    commandText += " (SchemaName = '" + constraintName + "')";
                //}
            }


            var command = new CrmDbCommand(crmDbConnection);
            command.CommandText = commandText;
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            adapter.Fill(dataTable);

            //  dataTable.AsDataView().RowFilter = "ReferencingEntity = '" + entityName + 

            dataTable.Columns.Add("CONSTRAINT_CATALOG", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(0);
            dataTable.Columns.Add("CONSTRAINT_SCHEMA", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(1);

            dataTable.Columns.Add("CONSTRAINT_NAME", typeof(string), "'PK__' + LogicalName + '_' + IsNull(PrimaryIdAttribute, LogicalName + 'id')").SetOrdinal(2);


            // dataTable.Columns["SchemaName"].ColumnName = "CONSTRAINT_NAME";
            // dataTable.Columns["CONSTRAINT_NAME"].SetOrdinal(2);

            //   dataTable.Columns.Add("CONSTRAINT_NAME", typeof(string), "''");

            dataTable.Columns.Add("TABLE_CATALOG", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(3);
            dataTable.Columns.Add("TABLE_SCHEMA", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(4);
            dataTable.Columns.Add("TABLE_NAME", typeof(string), "LogicalName").SetOrdinal(5);

            //    dataTable.Columns["LogicalName"].ColumnName = "TABLE_NAME";
            //      dataTable.Columns["TABLE_NAME"].SetOrdinal(5);

            dataTable.Columns.Add("CONSTRAINT_TYPE", typeof(string), "'PRIMARY KEY'").SetOrdinal(6);

            dataTable.Columns.Add("IS_DEFERRABLE", typeof(string), "'NO'").SetOrdinal(7);
            dataTable.Columns.Add("INITIALLY_DEFERRED", typeof(string), "'NO'").SetOrdinal(8);

            if (hasConstraintNameFilter)
            {
                var filteredView = dataTable.AsDataView();
                filteredView.RowFilter = "CONSTRAINT_NAME = '" + constraintName + "'";
                dataTable = filteredView.ToTable(true);
            }

            return dataTable;

            //else
            //{               
            //    dataTable = filteredView.ToTable(true);  
            //}


        }

        public DataTable GetUniqueKeyColumns(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            string catalog = GetRestrictionOrNull(0, restrictions);
            string schema = GetRestrictionOrNull(1, restrictions);
            string table = GetRestrictionOrNull(2, restrictions);
            string constraintname = GetRestrictionOrNull(3, restrictions);
            string columnname = GetRestrictionOrNull(4, restrictions);

            bool hasEntityFilter = !string.IsNullOrEmpty(table);
            bool hasColumnFilter = !string.IsNullOrEmpty(columnname);
            bool hasConstraintNameFilter = !string.IsNullOrEmpty(constraintname);


            string commandText = "SELECT entitymetadata.PrimaryIdAttribute, entitymetadata.LogicalName, a.LogicalName, a.ColumnNumber FROM entitymetadata JOIN attributemetadata a on entitymetadata.MetadataId = a.MetadataId WHERE (a.isprimaryid = @isPrimaryId)";

            if (hasEntityFilter || hasColumnFilter)
            {
                commandText += "AND ";
                if (hasEntityFilter)
                {
                    commandText += " (entitymetadata.LogicalName = '" + table + "')";
                }
                if (hasEntityFilter && hasColumnFilter)
                {
                    commandText += " AND ";
                }
                if (hasColumnFilter)
                {
                    commandText += " (a.LogicalName = '" + columnname + "')";
                }
            }

            var command = new CrmDbCommand(crmDbConnection);
            var param = command.CreateParameter();
            param.DbType = DbType.Boolean;
            param.Direction = ParameterDirection.Input;
            param.ParameterName = "@isPrimaryId";
            param.Value = true;
            command.Parameters.Add(param);
            command.CommandText = commandText;
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            adapter.Fill(dataTable);


            dataTable.Columns.Add("constraint_catalog", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(0);
            dataTable.Columns.Add("constraint_schema", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(1);

            dataTable.Columns["LogicalName"].ColumnName = "table_name";
            dataTable.Columns.Add("column_name", typeof(string), "IsNull([a.LogicalName], [PrimaryIdAttribute])");
            dataTable.Columns.Add("constraint_name", typeof(string), "'PK__' + IsNull(table_name, '') + '_' + IsNull(column_name, PrimaryIdAttribute)").SetOrdinal(2);
            dataTable.Columns.Add("table_catalog", typeof(string), string.Format("'{0}'", crmDbConnection.ConnectionInfo.OrganisationName)).SetOrdinal(3);
            dataTable.Columns.Add("table_schema", typeof(string), string.Format("'{0}'", DefaultSchema)).SetOrdinal(4);
            dataTable.Columns["table_name"].SetOrdinal(5);
            dataTable.Columns["column_name"].SetOrdinal(6);
            dataTable.Columns["a.columnnumber"].ColumnName = "ordinal_position";
            dataTable.Columns["ordinal_position"].SetOrdinal(7);

            dataTable.Columns.Add("constraint_type", typeof(string), "'FOREIGN KEY'").SetOrdinal(8);

            //   dataTable.Columns.Add("KeyType", typeof(Byte), "36").SetOrdinal(8); // 36 = uniqueidentitifer datatype - all pk indexes in crm are uniqueidentifiers.
            dataTable.Columns.Add("index_name", typeof(string), "constraint_name").SetOrdinal(9);
            //  dataTable.Columns.Add("type_desc", typeof(string), "'CLUSTERED'");

            var filteredView = dataTable.AsDataView();
            filteredView.RowFilter = "column_name = PrimaryIdAttribute";  // necessary due to #68
            dataTable = filteredView.ToTable(true);

            if (hasConstraintNameFilter)
            {
                filteredView = dataTable.AsDataView();
                filteredView.RowFilter = "constraint_name = '" + constraintname + "'";
                dataTable = filteredView.ToTable(true);
            }

            return dataTable;

        }

        public DataTable GetSchema(CrmDbConnection crmDbConnection, string collectionName, string[] restrictions)
        {
            DataTable result = null;

            if (restrictions == null)
            {
                restrictions = new string[] { "" };
            }

            switch (collectionName.ToLower())
            {
                case "metadatacollections":
                    result = GetMetadataCollections();
                    break;

                case "datasourceinformation":
                    result = GetDataSourceInfo(crmDbConnection);
                    break;

                case "reservedwords":
                    result = GetReservedWords();
                    break;

                case "databases":
                    result = GetDatabases(crmDbConnection, restrictions);
                    break;

                case "datatypes":
                    result = GetDataTypes();
                    break;

                case "restrictions":
                    result = GetRestrictions();
                    break;

                case "tables":
                    result = GetTables(crmDbConnection, restrictions);
                    break;

                case "columns":
                    result = GetColumns(crmDbConnection, restrictions);
                    break;

                case "views":
                    result = GetViews(crmDbConnection, restrictions);
                    break;

                case "viewcolumns":
                    result = GetViewColumns(crmDbConnection, restrictions);
                    break;

                case "indexes":
                    result = GetIndexes(crmDbConnection, restrictions);
                    break;

                case "indexcolumns":
                    result = GetIndexColumns(crmDbConnection, restrictions);
                    break;

                case "foreignkeys":
                    result = GetForeignKeys(crmDbConnection, restrictions);
                    break;

                case "foreignkeycolumns":
                    result = GetForeignKeyColumns(crmDbConnection, restrictions);
                    break;

                case "users":
                    result = GetUsers(crmDbConnection, restrictions);
                    break;

                case "uniquekeys":
                    result = GetUniqueKeys(crmDbConnection, restrictions);
                    break;

                case "uniquekeycolumns":
                    result = GetUniqueKeyColumns(crmDbConnection, restrictions);
                    break;

                //case "constraints":
                //case "primarykey":


                //case "constraintcolumns":
                //    break;
                default:
                    throw new ArgumentOutOfRangeException("collectionName", collectionName, "Invalid collection name");
                // }
            }

            return result;
        }

        public DataTable GetDatabases(CrmDbConnection crmDbConnection, string[] restrictions)
        {

            // database_name (System.String)	dbid (System.Int16)	create_date (System.DateTime)
            DataTable dataTable = new DataTable("Databases");
            dataTable.Locale = CultureInfo.InvariantCulture;
            dataTable.Columns.AddRange(
                new DataColumn[]
                    {
                        new DataColumn("database_name"), 
                        new DataColumn("dbid",typeof(Int16)), 
                        new DataColumn("create_date", typeof(DateTime)),
                        new DataColumn("organisationid",typeof(Guid)),
                        new DataColumn("serverversion")
                    });

            dataTable.Rows.Add(crmDbConnection.ConnectionInfo.OrganisationName, 1, DateTime.UtcNow, crmDbConnection.ConnectionInfo.OrganisationId, crmDbConnection.ConnectionInfo.ServerVersion);
            return dataTable;
        }
    }
}
