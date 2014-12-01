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

        DataTable GetReservedWords();

        DataTable GetDataTypes();


        DataTable GetTables(CrmDbConnection crmDbConnection);

        void GetColumns(CrmDbConnection crmDbConnection);

        void GetViews(CrmDbConnection crmDbConnection);

        void GetRestrictions(CrmDbConnection crmDbConnection);
    }

    public class SchemaCollectionsProvider : ISchemaCollectionsProvider
    {

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

        public DataTable GetMetadataCollections()
        {
            DataTable dataTable = new DataTable("MetaDataCollections")
            {
                Locale = CultureInfo.InvariantCulture
            };
            var nameCol = dataTable.Columns.Add("CollectionName", typeof(string));
            var restrictionsCol = dataTable.Columns.Add("NumberOfRestrictions", typeof(int));
            var identifiersCol = dataTable.Columns.Add(DbMetaDataColumnNames.NumberOfIdentifierParts, typeof(int));
            dataTable.BeginLoadData();

            AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "MetaDataCollections", 0, 0);
            AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "DataSourceInformation", 0, 0);
            AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "DataTypes", 0, 0);
            AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "ReservedWords", 0, 0);
            AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "Tables", 1, 0);
            AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "Views", 1, 0);
            AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "Columns", 2, 1);
            AddMetadataCollectionRow(dataTable, nameCol, restrictionsCol, identifiersCol, "Restrictions", 0, 0);


            dataTable.AcceptChanges();
            dataTable.EndLoadData();
            // dataTable.DefaultView.Sort = "Category ASC";
            return dataTable;
        }

        private static void AddMetadataCollectionRow(DataTable dataTable, DataColumn nameCol, DataColumn restrictionsCol, DataColumn identifiersCol, string collectionName, int restrictions, int identifiers)
        {
            var row = dataTable.Rows.Add();
            row.SetField<string>(nameCol, collectionName);
            row.SetField<int>(restrictionsCol, restrictions);
            row.SetField<int>(identifiersCol, identifiers);
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


        public DataTable GetTables(CrmDbConnection crmDbConnection)
        {
            throw new NotImplementedException();
        }

        public void GetColumns(CrmDbConnection crmDbConnection)
        {
            throw new NotImplementedException();
        }

        public void GetViews(CrmDbConnection crmDbConnection)
        {
            throw new NotImplementedException();
        }

        public void GetRestrictions(CrmDbConnection crmDbConnection)
        {
            throw new NotImplementedException();
        }
    }
}
