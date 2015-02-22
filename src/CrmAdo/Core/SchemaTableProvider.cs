using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.Xrm.Sdk.Metadata;
using CrmAdo.Dynamics;
using CrmAdo.Metadata;

namespace CrmAdo.Core
{
    /// <summary>
    /// An implementation of a provider that generates the schema table, based on Column Metadata.
    /// </summary>
    public class SchemaTableProvider : ISchemaTableProvider
    {
        public DataTable GetSchemaTable(DbConnection connection, IEnumerable<ColumnMetadata> columns)
        {
            //Note to self: See here for an alternate implementation info: http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(v=vs.110).aspx
            CrmDbConnection conn = (CrmDbConnection)connection;

            var schemaTable = new DataTable("SchemaTable");
            schemaTable.Locale = CultureInfo.InvariantCulture;


            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.AllowDBNull, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.BaseColumnName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.BaseServerName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.BaseTableName, typeof(string)));


            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.ColumnName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(int)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.ColumnSize, typeof(int)));
            schemaTable.Columns.Add(new DataColumn("DataTypeName", typeof(string)));

            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.DataType, typeof(Type))); // THIS was not present in documentation.
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsAliased, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn("IsColumnSet", typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsExpression, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsHidden, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn("IsIdentity", typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsKey, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsLong, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsUnique, typeof(bool)));




            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.NonVersionedProviderType, typeof(int)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.NumericPrecision, typeof(short)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.NumericScale, typeof(short)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.ProviderSpecificDataType, typeof(Type)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.ProviderType, typeof(string)));

            schemaTable.Columns.Add(new DataColumn("UdtAssemblyQualifiedName", typeof(string)));
            schemaTable.Columns.Add(new DataColumn("XmlSchemaCollectionDatabase", typeof(string)));
            schemaTable.Columns.Add(new DataColumn("XmlSchemaCollectionName", typeof(string)));
            schemaTable.Columns.Add(new DataColumn("XmlSchemaCollectionOwningSchema", typeof(string)));
            //  table.Columns.Add(new DataColumn(SchemaTableColumn.ProviderType, typeof(int)));  


            int ordinal = 0;
            foreach (var columnMetadata in columns)
            {
                var row = schemaTable.Rows.Add();
                var attMeta = columnMetadata.AttributeMetadata;
                bool isPrimaryId = attMeta.IsPrimaryId.HasValue && attMeta.IsPrimaryId.Value;
                row[SchemaTableColumn.AllowDBNull] = !isPrimaryId;

                if (conn != null)
                {
                    row[SchemaTableOptionalColumn.BaseCatalogName] = conn.ConnectionInfo.OrganisationName;
                }
                else
                {
                    row[SchemaTableOptionalColumn.BaseCatalogName] = null;
                }

                row[SchemaTableColumn.BaseColumnName] = attMeta.LogicalName;
                row[SchemaTableColumn.BaseSchemaName] = "dbo";
                //   row[SchemaTableOptionalColumn.BaseServerName] = "dbo";
                row[SchemaTableColumn.BaseTableName] = attMeta.EntityLogicalName;
                row[SchemaTableColumn.ColumnName] = columnMetadata.ColumnName;
                row[SchemaTableColumn.ColumnOrdinal] = ordinal; // columnMetadata.AttributeMetadata.ColumnNumber;

                // set column size
                row[SchemaTableColumn.ColumnSize] = columnMetadata.AttributeMetadata.Length;
                row["DataTypeName"] = attMeta.GetSqlDataTypeName();
                row[SchemaTableColumn.IsAliased] = columnMetadata.HasAlias;
                row["IsColumnSet"] = false;
                row[SchemaTableColumn.IsExpression] = false;

                row[SchemaTableColumn.DataType] = attMeta.GetFieldType();
                row[SchemaTableOptionalColumn.IsAutoIncrement] = false;
                row["IsHidden"] = false; // !attMeta.IsValidForRead;


                row["IsIdentity"] = isPrimaryId;

                row[SchemaTableColumn.IsKey] = isPrimaryId; // false; //for multi part keys // columnMetadata.AttributeMetadata.IsPrimaryId;
                row[SchemaTableColumn.IsLong] = false;
                row[SchemaTableOptionalColumn.IsReadOnly] = !columnMetadata.AttributeMetadata.IsValidForUpdate.GetValueOrDefault() && !columnMetadata.AttributeMetadata.IsValidForCreate.GetValueOrDefault();
                row[SchemaTableOptionalColumn.IsRowVersion] = false; //columnMetadata.AttributeMetadata.LogicalName == "versionnumber";

                row[SchemaTableColumn.IsUnique] = false;  //for timestamp columns only. //columnMetadata.AttributeMetadata.IsPrimaryId;
                row[SchemaTableColumn.NonVersionedProviderType] = (int)attMeta.AttributeType;

                //var haveMinAndMax = attMeta as IColumnMetadata;
                //if (haveMinAndMax != null)
                //{
                if (attMeta.NumericPrecision == null)
                {
                    row[SchemaTableColumn.NumericPrecision] = 0;// DBNull.Value;
                }
                else
                {
                    row[SchemaTableColumn.NumericPrecision] = attMeta.NumericPrecision;
                }

                if (attMeta.NumericScale == null)
                {
                    row[SchemaTableColumn.NumericScale] = 0; // DBNull.Value;
                }
                else
                {
                    row[SchemaTableColumn.NumericScale] = attMeta.NumericScale;
                }

                row[SchemaTableOptionalColumn.ProviderSpecificDataType] = null; // attMeta.AttributeType;
                row[SchemaTableColumn.ProviderType] = attMeta.GetSqlDataTypeName();

                row["UdtAssemblyQualifiedName"] = null;
                row["XmlSchemaCollectionDatabase"] = null;
                row["XmlSchemaCollectionName"] = null;
                row["XmlSchemaCollectionOwningSchema"] = null;                   


                ordinal++;
            }

            return schemaTable;

        }
    }
}