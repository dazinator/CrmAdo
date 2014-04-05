using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Xrm.Sdk.Metadata;

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

            var schemaTable = new DataTable();
            schemaTable.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.ColumnName, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.DataType, typeof(string));
            schemaTable.Columns.Add("DataTypeName", typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.IsKey, typeof(bool));
            schemaTable.Columns.Add("IsIdentity", typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.IsLong, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool));
            schemaTable.Columns.Add(SchemaTableColumn.NonVersionedProviderType, typeof(string));
            schemaTable.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.NumericScale, typeof(int));
            schemaTable.Columns.Add(SchemaTableColumn.ProviderType, typeof(string));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
            schemaTable.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool));

            int ordinal = 0;
            foreach (var columnMetadata in columns)
            {
                var row = schemaTable.Rows.Add();
                var attMeta = columnMetadata.AttributeMetadata;
                row[SchemaTableColumn.AllowDBNull] = !attMeta.IsPrimaryId.GetValueOrDefault();
                row[SchemaTableColumn.BaseColumnName] = attMeta.LogicalName;
                row[SchemaTableColumn.BaseSchemaName] = null;
                row[SchemaTableColumn.BaseTableName] = attMeta.EntityLogicalName;
                row[SchemaTableColumn.ColumnName] = columnMetadata.ColumnName;
                row[SchemaTableColumn.ColumnOrdinal] = ordinal;

                //  row[SchemaTableColumn.ColumnSize] = ordinal;
                row[SchemaTableColumn.DataType] = columnMetadata.GetFieldType().Name;
                row[SchemaTableColumn.IsAliased] = columnMetadata.HasAlias;
                row[SchemaTableColumn.IsExpression] = false;
                row[SchemaTableColumn.IsKey] = false; //for multi part keys // columnMetadata.AttributeMetadata.IsPrimaryId;
                row[SchemaTableColumn.IsLong] = false;
                // only id must be unique.
                row[SchemaTableColumn.IsUnique] = false;  //for timestamp columns only. //columnMetadata.AttributeMetadata.IsPrimaryId;
                row[SchemaTableColumn.NonVersionedProviderType] = columnMetadata.AttributeType().ToString();
                switch (columnMetadata.AttributeMetadata.AttributeType.GetValueOrDefault())
                {
                    case AttributeTypeCode.Decimal:
                        var decimalMeta = (DecimalAttributeMetadata)columnMetadata.AttributeMetadata;
                        var decimalPrecision = Math.Max(decimalMeta.MinValue.ToString().Length, decimalMeta.MaxValue.ToString().Length) + decimalMeta.Precision;
                        row[SchemaTableColumn.NumericPrecision] = decimalPrecision;
                        row[SchemaTableColumn.NumericScale] = decimalMeta.Precision; // dynamics uses the term precision in its metadata to refer to the number of decimal places - which is actually the "scale"!
                        break;
                    case AttributeTypeCode.Double:
                        var doubleMeta = (DoubleAttributeMetadata)columnMetadata.AttributeMetadata;
                        var doublePrecision = Math.Max(doubleMeta.MinValue.ToString().Length, doubleMeta.MaxValue.ToString().Length) + doubleMeta.Precision;
                        row[SchemaTableColumn.NumericPrecision] = doublePrecision;
                        row[SchemaTableColumn.NumericScale] = doubleMeta.Precision; // dynamics uses the term precision in its metadata to refer to the number of decimal places - which is actually the "scale"!
                        break;
                }
                row[SchemaTableColumn.ProviderType] = columnMetadata.AttributeType().ToString();

                // some other optional columns..
                row["DataTypeName"] = columnMetadata.GetDataTypeName();
                row[SchemaTableOptionalColumn.IsReadOnly] = !columnMetadata.AttributeMetadata.IsValidForUpdate.GetValueOrDefault() && !columnMetadata.AttributeMetadata.IsValidForCreate.GetValueOrDefault();
                row["IsIdentity"] = columnMetadata.AttributeMetadata.IsPrimaryId;
                row[SchemaTableOptionalColumn.IsAutoIncrement] = false;
                // could possibly add a column with correct datatype etc..

                ordinal++;
            }

            return schemaTable;

        }
    }
}