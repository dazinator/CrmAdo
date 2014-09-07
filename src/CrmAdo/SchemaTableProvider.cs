using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.Xrm.Sdk.Metadata;
using CrmAdo.Dynamics.Metadata;

namespace CrmAdo
{
    /// <summary>
    /// An implementation of a provider that generates the schema table, based on Column Metadata.
    /// </summary>
    public class SchemaTableProvider : ISchemaTableProvider
    {
        public DataTable GetSchemaTable(IEnumerable<ColumnMetadata> columns)
        {
            //Note to self: See here for an alternate implementation info: http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(v=vs.110).aspx

            var schemaTable = new DataTable("SchemaTable");
            schemaTable.Locale = CultureInfo.InvariantCulture;


            schemaTable.Columns.Add(new DataColumn("DataTypeName", typeof(string)));
            schemaTable.Columns.Add(new DataColumn("IsIdentity", typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.AllowDBNull, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.BaseColumnName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.BaseSchemaName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.BaseTableName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.ColumnName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.ColumnOrdinal, typeof(int)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.ColumnSize, typeof(int)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.DataType, typeof(Type)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsAliased, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsExpression, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsKey, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsLong, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.IsUnique, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.NonVersionedProviderType, typeof(int)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.NumericPrecision, typeof(short)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.NumericScale, typeof(short)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableColumn.ProviderType, typeof(string)));
            //  table.Columns.Add(new DataColumn(SchemaTableColumn.ProviderType, typeof(int)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsReadOnly, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.ProviderSpecificDataType, typeof(Type)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsRowVersion, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.IsHidden, typeof(bool)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.BaseCatalogName, typeof(string)));
            schemaTable.Columns.Add(new DataColumn(SchemaTableOptionalColumn.BaseServerName, typeof(string)));

            int ordinal = 0;
            foreach (var columnMetadata in columns)
            {
                var row = schemaTable.Rows.Add();
                var attMeta = columnMetadata.AttributeMetadata;
                row[SchemaTableColumn.AllowDBNull] = !attMeta.IsPrimaryId;
                row[SchemaTableColumn.BaseColumnName] = attMeta.LogicalName;
                row[SchemaTableColumn.BaseSchemaName] = null;
                row[SchemaTableColumn.BaseTableName] = attMeta.EntityLogicalName;
                row[SchemaTableColumn.ColumnName] = columnMetadata.ColumnName;
                row[SchemaTableColumn.ColumnOrdinal] = ordinal;

                // set column size
                row[SchemaTableColumn.ColumnSize] = int.MaxValue;
                row[SchemaTableColumn.DataType] = attMeta.GetFieldType();
                row[SchemaTableColumn.IsAliased] = columnMetadata.HasAlias;
                row[SchemaTableColumn.IsExpression] = false;
                row[SchemaTableColumn.IsKey] = false; //for multi part keys // columnMetadata.AttributeMetadata.IsPrimaryId;
                row[SchemaTableColumn.IsLong] = false;
                // only id must be unique.

                row[SchemaTableColumn.IsUnique] = false;  //for timestamp columns only. //columnMetadata.AttributeMetadata.IsPrimaryId;
                row[SchemaTableColumn.NonVersionedProviderType] = (int)attMeta.AttributeType;

                var haveMinAndMax = attMeta as IHaveMinMaxAndScaleValues;
                if (haveMinAndMax != null)
                {
                    row[SchemaTableColumn.NumericPrecision] = haveMinAndMax.GetNumericPrecision();
                    row[SchemaTableColumn.NumericScale] = haveMinAndMax.GetNumericScale(); // dynamics uses the term precision in its metadata to refer to the number of decimal places - which is actually the "scale"!
                }
              
                row[SchemaTableColumn.ProviderType] = attMeta.AttributeType.ToString();

                // some other optional columns..
                row["DataTypeName"] = attMeta.GetSqlDataTypeName();
                row[SchemaTableOptionalColumn.IsReadOnly] = !columnMetadata.AttributeMetadata.IsValidForUpdate.GetValueOrDefault() && !columnMetadata.AttributeMetadata.IsValidForCreate.GetValueOrDefault();
                row["IsIdentity"] = attMeta.IsPrimaryId;
                row[SchemaTableOptionalColumn.IsAutoIncrement] = false;
                // row[SchemaTableOptionalColumn.IsRowVersion] = false;
                // row[SchemaTableOptionalColumn.IsHidden] = false;
                // could possibly add a column with correct datatype etc..

                ordinal++;
            }

            return schemaTable;

        }
    }
}