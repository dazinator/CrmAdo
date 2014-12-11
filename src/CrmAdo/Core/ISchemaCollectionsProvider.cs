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

namespace CrmAdo
{
    public interface ISchemaCollectionsProvider
    {

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

        DataTable GetIndexes(CrmDbConnection crmDbConnection, string[] restrictions);

        DataTable GetIndexColumns(CrmDbConnection crmDbConnection, string[] restrictions);
    }

    public class SchemaCollectionsProvider : ISchemaCollectionsProvider
    {

        public DataTable GetMetadataCollections()
        {
            var dt = new DataTable();
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

            DataRow dataRow = dataTable.NewRow();
            dataRow[DbMetaDataColumnNames.CompositeIdentifierSeparatorPattern] = @"\.";
            dataRow[DbMetaDataColumnNames.DataSourceProductName] = "Dynamics CRM";
            dataRow[DbMetaDataColumnNames.DataSourceProductVersion] = connection.ServerVersion;
            dataRow[DbMetaDataColumnNames.DataSourceProductVersionNormalized] = connection.ServerVersion;
            dataRow[DbMetaDataColumnNames.GroupByBehavior] = GroupByBehavior.MustContainAll;
            dataRow[DbMetaDataColumnNames.IdentifierPattern] = @"([\p{L}:?@#_][\p{L}\p{N}@#$_]*)|(""(\.|"""")+"")|(\[[^\]]+\])";  // [A-Za-z0-9_#$] 
            dataRow[DbMetaDataColumnNames.IdentifierCase] = IdentifierCase.Insensitive;
            dataRow[DbMetaDataColumnNames.OrderByColumnsInSelect] = true;
            dataRow[DbMetaDataColumnNames.ParameterMarkerFormat] = "{0}";
            dataRow[DbMetaDataColumnNames.ParameterMarkerPattern] = "(@[A-Za-z0-9_$#]*)";
            dataRow[DbMetaDataColumnNames.ParameterNameMaxLength] = 128;
            dataRow[DbMetaDataColumnNames.ParameterNamePattern] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)"; //  \p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Nl}\p{Nd}
            dataRow[DbMetaDataColumnNames.QuotedIdentifierPattern] = @"\[(.*?)\]"; //TODO THIS IS WRONG AS IT MATCHES THE QUOTES AND I NEED TO EXLCUDE THEM..;
            dataRow[DbMetaDataColumnNames.QuotedIdentifierCase] = IdentifierCase.Insensitive;
            dataRow[DbMetaDataColumnNames.StatementSeparatorPattern] = @";";
            dataRow[DbMetaDataColumnNames.StringLiteralPattern] = @"('([^']|'')*')"; //TODO THIS IS WRONG AS IT MATCHES THE QUOTES AND I NEED TO EXLCUDE THEM..;
            dataRow[DbMetaDataColumnNames.SupportedJoinOperators] = SupportedJoinOperators.Inner | SupportedJoinOperators.LeftOuter;

            #region probably none of this needed

            //dataTable.Columns.Add("IdentifierOpenQuote", typeof(string));
            //dataTable.Columns.Add("IdentifierCloseQuote", typeof(string));
            //dataTable.Columns.Add("SupportsAnsi92Sql", typeof(bool));
            //dataTable.Columns.Add("SupportsQuotedIdentifierParts", typeof(bool));
            //dataTable.Columns.Add("ParameterPrefix", typeof(string));
            //dataTable.Columns.Add("ParameterPrefixInName", typeof(bool));
            //dataTable.Columns.Add("ColumnAliasSupported", typeof(bool));
            //dataTable.Columns.Add("TableAliasSupported", typeof(bool));
            //dataTable.Columns.Add("TableSupported", typeof(bool));
            //dataTable.Columns.Add("UserSupported", typeof(bool));
            //dataTable.Columns.Add("SchemaSupported", typeof(bool));
            //dataTable.Columns.Add("CatalogSupported", typeof(bool));
            //dataTable.Columns.Add("SupportsVerifySQL", typeof(bool));

            ////dataRow[DbMetaDataColumnNames.SupportedJoinOperators] = 3; // Inner join and Left joins
            ////dataRow[DbMetaDataColumnNames.ParameterNamePattern] = @"^[\p{Lo}\p{Lu}\p{Ll}\p{Lm}_@#][\p{Lo}\p{Lu}\p{Ll}\p{Lm}\p{Nd}\uff3f_@#\$]*(?=\s+|$)";
            ////dataRow[DbMetaDataColumnNames.ParameterMarkerFormat] = "{0}";
            ////dataRow[DbMetaDataColumnNames.ParameterNameMaxLength] = 128;        

            ////dataRow[DbMetaDataColumnNames.DataSourceProductName] = "CrmAdo Provider for Dynamics CRM";
            ////var version = Assembly.GetExecutingAssembly().GetName().Version;
            ////dataRow[DbMetaDataColumnNames.DataSourceProductVersion] = version.ToString();

            //dataRow["IdentifierOpenQuote"] = "[";
            //dataRow["IdentifierCloseQuote"] = "]";
            //dataRow["SupportsAnsi92Sql"] = false;
            //dataRow["SupportsQuotedIdentifierParts"] = true;
            //dataRow["ParameterPrefix"] = "@";
            //dataRow["ParameterPrefixInName"] = true;
            //dataRow["ColumnAliasSupported"] = false;
            //dataRow["TableAliasSupported"] = true;
            //dataRow["TableSupported"] = true;
            //dataRow["UserSupported"] = false;
            //dataRow["SchemaSupported"] = false;
            //dataRow["CatalogSupported"] = false;
            //dataRow["SupportsVerifySQL"] = false;

            #endregion

            dataTable.Rows.Add(dataRow);
            return dataTable;
        }

        public DataTable GetDataTypes()
        {
            var dt = new DataTable();
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
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
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

            string tableName = null;
            if (restrictions != null)
            {
                if (restrictions.Length > 0)
                {
                    tableName = restrictions[0];
                }
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                commandText = commandText + " WHERE LogicalName = '" + tableName + "'";
            }

            command.CommandText = commandText;

            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
            adapter.Fill(dataTable);
            dataTable.Columns.Add("table_catalog", typeof(string), "''");
            dataTable.Columns.Add("table_schema", typeof(string), "''");
            dataTable.Columns["logicalname"].ColumnName = "table_name";
            dataTable.Columns.Add("table_type", typeof(string), "'BASE TABLE'");
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

            string entityName = null;
            string attributeName = null;
            bool hasEntityFilter = false;
            bool hasAttributeFilter = false;

            if (restrictions != null)
            {
                if (restrictions.Length > 0)
                {
                    entityName = restrictions[0];
                }
                if (restrictions.Length > 1)
                {
                    attributeName = restrictions[1];
                }
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
            adapter.Fill(dataTable);
            dataTable.Columns.Add("table_catalog", typeof(string), "''");
            dataTable.Columns.Add("table_schema", typeof(string), "''");
            dataTable.Columns["entitylogicalname"].ColumnName = "table_name";
            dataTable.Columns["logicalname"].ColumnName = "column_name";
            dataTable.Columns["columnnumber"].ColumnName = "ordinal_position";
            dataTable.Columns["defaultvalue"].ColumnName = "column_default";
            dataTable.Columns["isnullable"].ColumnName = "is_nullable";
            dataTable.Columns["datatype"].ColumnName = "data_type";

            // dataTable.Columns["attributemetadata.length"].ColumnName = "character_maximum_length";
            dataTable.Columns.Add("character_maximum_length", typeof(int), "IIF(data_type = 'nvarchar', [maxlength], NULL)");
            dataTable.Columns.Add("character_octet_length", typeof(int), "IIF(data_type ='nvarchar',ISNULL(character_maximum_length, 0) * 2, character_maximum_length)");

            dataTable.Columns["numericprecision"].ColumnName = "numeric_precision";
            dataTable.Columns["numericprecisionradix"].ColumnName = "numeric_precision_radix";
            dataTable.Columns["numericscale"].ColumnName = "numeric_scale";

            dataTable.Columns.Add("datetime_precision", typeof(int), "IIF(data_type = 'datetime', 3, NULL)");

            dataTable.Columns.Add("character_set_catalog", typeof(string), "NULL");
            dataTable.Columns.Add("character_set_schema", typeof(string), "NULL");

            dataTable.Columns.Add("character_set_name", typeof(string), "IIF(data_type ='nvarchar', 'UNICODE', NULL)");

            dataTable.Columns.Add("collation_catalog", typeof(string), "NULL");
            dataTable.Columns.Add("is_sparse", typeof(bool), "false");
            dataTable.Columns.Add("is_column_set", typeof(bool), "false");
            dataTable.Columns.Add("is_filestream", typeof(bool), "false");

            return dataTable;

        }

        public DataTable GetViews(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            throw new NotImplementedException();
        }

        public DataTable GetViewColumns(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            throw new NotImplementedException();
        }

        public DataTable GetForeignKeys(CrmDbConnection crmDbConnection, string[] restrictions)
        {
            // throw new NotImplementedException();

            string entityName = null;
            string constraintName = null;
            bool hasEntityFilter = false;
            bool hasConstraintNameFilter = false;

            if (restrictions != null)
            {
                if (restrictions.Length > 0)
                {
                    entityName = restrictions[0];
                }
                if (restrictions.Length > 1)
                {
                    constraintName = restrictions[1];
                }
            }

            hasEntityFilter = !string.IsNullOrEmpty(entityName);
            hasConstraintNameFilter = !string.IsNullOrEmpty(constraintName);

            string commandText = "SELECT o.* FROM entitymetadata e INNER JOIN onetomanyrelationshipmetadata o ON e.MetadataId = o.MetadataId ";


            if (hasEntityFilter || hasConstraintNameFilter)
            {
                commandText += "WHERE ";
                if (hasEntityFilter)
                {
                    commandText += " (e.LogicalName = '" + entityName + "')";
                }
                if (hasEntityFilter && hasConstraintNameFilter)
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
            adapter.Fill(dataTable);

            //  dataTable.AsDataView().RowFilter = "ReferencingEntity = '" + entityName + 

            dataTable.Columns.Add("constraint_catalog", typeof(string), "''");
            dataTable.Columns.Add("constraint_schema", typeof(string), "''");
            dataTable.Columns["o.schemaname"].ColumnName = "constraint_name";

            //   dataTable.Columns.Add("CONSTRAINT_NAME", typeof(string), "''");

            dataTable.Columns.Add("table_catalog", typeof(string), "''");
            dataTable.Columns.Add("table_schema", typeof(string), "''");

            dataTable.Columns["o.referencingentity"].ColumnName = "table_name";
            dataTable.Columns.Add("constraint_type", typeof(string), "'FOREIGN KEY'");

            dataTable.Columns.Add("is_deferrable", typeof(string), "'NO'");
            dataTable.Columns.Add("initially_deferred", typeof(string), "'NO'");

            var filteredView = dataTable.AsDataView();
            if (hasEntityFilter)
            {
                filteredView.RowFilter = "table_name = '" + entityName + "'";
            }
            dataTable = filteredView.ToTable(true);

            //else
            //{               
            //    dataTable = filteredView.ToTable(true);  
            //}

            return dataTable;
        }

        public DataTable GetIndexes(CrmDbConnection crmDbConnection, string[] restrictions)
        {

            string entityName = null;
            string constraintName = null;
            bool hasEntityFilter = false;
            bool hasConstraintNameFilter = false;

            if (restrictions != null)
            {
                if (restrictions.Length > 0)
                {
                    entityName = restrictions[0];
                }
                if (restrictions.Length > 1)
                {
                    constraintName = restrictions[1];
                }
            }

            hasEntityFilter = !string.IsNullOrEmpty(entityName);
            hasConstraintNameFilter = !string.IsNullOrEmpty(constraintName);

            string commandText = "SELECT LogicalName, PrimaryIdAttribute FROM entitymetadata ";


            if (hasEntityFilter || hasConstraintNameFilter)
            {
                commandText += "WHERE ";
                if (hasEntityFilter)
                {
                    commandText += " (LogicalName = '" + entityName + "')";
                }
                if (hasEntityFilter && hasConstraintNameFilter)
                {
                    commandText += " AND ";
                }
                if (hasConstraintNameFilter)
                {
                    commandText += " (PrimaryIdAttribute = '" + constraintName + "')";
                }
            }

            var command = new CrmDbCommand(crmDbConnection);
            command.CommandText = commandText;
            var adapter = new CrmDataAdapter(command);
            var dataTable = new DataTable();
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

            dataTable.Columns.Add("constraint_catalog", typeof(string), "''");
            dataTable.Columns.Add("constraint_schema", typeof(string), "''");
            dataTable.Columns["LogicalName"].ColumnName = "table_name";
            dataTable.Columns.Add("constraint_name", typeof(string), "'PK__' + table_name + '_' + primaryidattribute");
            dataTable.Columns.Add("table_catalog", typeof(string), "''");
            dataTable.Columns.Add("table_schema", typeof(string), "''");
            dataTable.Columns.Add("index_name", typeof(string), "constraint_name");
            dataTable.Columns.Add("type_desc", typeof(string), "'CLUSTERED'");

            return dataTable;
        }
        
        public DataTable GetIndexColumns(CrmDbConnection crmDbConnection, string[] restrictions)
        {

            string entityName = null;
            string constraintName = null;           
            bool hasEntityFilter = false;
            bool hasConstraintNameFilter = false;

            if (restrictions != null)
            {
                if (restrictions.Length > 0)
                {
                    entityName = restrictions[0];
                }
                if (restrictions.Length > 1)
                {
                    constraintName = restrictions[1];                    
                }
            }

            hasEntityFilter = !string.IsNullOrEmpty(entityName);
            hasConstraintNameFilter = !string.IsNullOrEmpty(constraintName);


            string commandText = "SELECT entitymetadata.PrimaryIdAttribute, entitymetadata.LogicalName, a.LogicalName, a.ColumnNumber FROM entitymetadata JOIN attributemetadata a on entitymetadata.MetadataId = a.MetadataId WHERE (a.isprimaryid = @isPrimaryId)";


            if (hasEntityFilter || hasConstraintNameFilter)
            {
                commandText += "AND ";
                if (hasEntityFilter)
                {
                    commandText += " (entitymetadata.LogicalName = '" + entityName + "')";
                }
                if (hasEntityFilter && hasConstraintNameFilter)
                {
                    commandText += " AND ";
                }
                if (hasConstraintNameFilter)
                {
                    commandText += " (a.LogicalName = '" + constraintName + "')";
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

            dataTable.Columns.Add("constraint_catalog", typeof(string), "''");
            dataTable.Columns.Add("constraint_schema", typeof(string), "''");
            dataTable.Columns["LogicalName"].ColumnName = "table_name";
            dataTable.Columns["a.LogicalName"].ColumnName = "column_name";
            dataTable.Columns.Add("constraint_name", typeof(string), "'PK__' + IsNull(table_name, '') + '_' + IsNull(column_name, PrimaryIdAttribute)");
            dataTable.Columns.Add("table_catalog", typeof(string), "''");
            dataTable.Columns.Add("table_schema", typeof(string), "''");

            dataTable.Columns["a.columnnumber"].ColumnName = "ordinal_position";
            dataTable.Columns.Add("KeyType", typeof(Byte), "36"); // 36 = uniqueidentitifer datatype - all pk indexes in crm are uniqueidentifiers.

          //  dataTable.Columns.Add("type_desc", typeof(string), "'CLUSTERED'");

            dataTable.Columns.Add("index_name", typeof(string), "constraint_name");    
            return dataTable;
        }
    }
}
