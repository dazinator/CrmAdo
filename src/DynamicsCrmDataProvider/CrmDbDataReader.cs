using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsCrmDataProvider
{

    public class EntityResultSet
    {
        public EntityCollection Results { get; set; }
        public List<AttributeMetadata> ColumnMetadata { get; set; }
    }

    public class CrmDbDataReader : DbDataReader
    {
        private EntityCollection _Results;
        private DbConnection _DbConnection;
        private bool _IsOpen;
        private const int StartPosition = -1;
        private int _Position = StartPosition;
        private bool _HasRows;
        private List<AttributeMetadata> _Metadata;
        // private List<ColumnMetadata> _Metadata;
        private int _ResultSetCount = 0;
        // private int _TotalRecordCount = 0;

        public CrmDbDataReader(EntityResultSet results)
            : this(results, null)
        {

        }

        public CrmDbDataReader(EntityResultSet results, DbConnection dbConnection)
        {
            // TODO: Complete member initialization
            this._Results = results.Results;
            _Metadata = results.ColumnMetadata;
            this._DbConnection = dbConnection;
            _IsOpen = true;
            // TODO: Change the below - shouldnt discover metadata from the first item, should retreive the metadata from CRM in advance.
            // ALSO Extend metadata with fields for EntityReference Name, Option Set Name, and possibly some other formatted values.
            //  _Metadata = new List<ColumnMetadata>();
            if (_Results.Entities != null && _Results.Entities.Any())
            {
                _HasRows = true;
                _ResultSetCount = _Results.Entities.Count;

                // _FirstResult = results.[0];
                //foreach (var att in _FirstResult.Attributes)
                //{
                //    var columnMeta = new ColumnMetadata();
                //    columnMeta.Name = att.Key;
                //    // TODO: This needs to be set properly..
                //    columnMeta.AttributeTypeCode = AttributeTypeCode.String;
                //    // THIS IS BAD - VALUE COULD BE NULL..
                //    columnMeta.DataType = att.Value.GetType();
                //    _Metadata.Add(columnMeta);
                //}
            }
        }

        private int _Depth = 0;
        public override int Depth
        {
            get { return _Depth; }
        }

        public override bool IsClosed
        {
            get { return !_IsOpen; }
        }

        public override int RecordsAffected
        {
            get
            {
                // RecordsAffected is only applicable to batch statements
                // that include inserts/updates/deletes. This always returns -1.
                return -1;
            }
        }

        public override void Close()
        {
            //Potentially clean up other resources if data is still being obtained from the server..
            _IsOpen = false;
            if (_DbConnection != null)
            {
                _DbConnection.Close();
                _DbConnection = null;
            }
        }

        public override bool NextResult()
        {
            // This only supports sinlge result set..
            return false;
        }

        public override bool Read()
        {
            // Return true if it is possible to advance and if you are still positioned
            // on a valid row.
            if (++_Position >= _ResultSetCount)
            {
                // come to end of result set.. if more available need to load them..
                if (!_Results.MoreRecords)
                {
                    return false;
                }
                else
                {
                    //TODO: Look into loading this asynchornously and ahead of time - perhaps 100 rows in advance?
                    // Could grab the next page of results..
                    throw new NotImplementedException("The result set has many pages, and paging has not yet been supported");
                }
            }
            else
            {
                return true;
            }

        }

        public override bool HasRows
        {
            get { return _HasRows; }
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, this._DbConnection != null);
        }

        #region Metadata - Leaving a Lot to be desired here..

        //private Entity FirstResult()
        //{
        //    return _FirstResult;
        //}

        public override int FieldCount
        {
            get { return _Metadata.Count; }
        }

        public override string GetName(int ordinal)
        {
            return _Metadata[ordinal].LogicalName;
        }

        public override int GetOrdinal(string name)
        {
            int ordinal = 0;
            foreach (var m in _Metadata)
            {
                if (m.LogicalName == name)
                {
                    return ordinal;
                }
                ordinal++;
            }
            // Throw an exception if the ordinal cannot be found.
            throw new IndexOutOfRangeException("Could not find specified column in results");
        }

        public override string GetDataTypeName(int ordinal)
        {
            var columnMeta = _Metadata[ordinal];
            return columnMeta.AttributeTypeName.Value;
        }

        public override Type GetFieldType(int ordinal)
        {
            switch (_Metadata[ordinal].AttributeType)
            {
                case AttributeTypeCode.BigInt:
                    return typeof(long);
                case AttributeTypeCode.Boolean:
                    return typeof(bool);
                case AttributeTypeCode.CalendarRules:
                    return typeof(string);
                case AttributeTypeCode.Customer:
                    return typeof(Guid);
                case AttributeTypeCode.DateTime:
                    return typeof(DateTime);
                case AttributeTypeCode.Decimal:
                    return typeof(decimal);
                case AttributeTypeCode.Double:
                    return typeof(double);
                case AttributeTypeCode.EntityName:
                    return typeof(string);
                case AttributeTypeCode.Integer:
                    return typeof(int);
                case AttributeTypeCode.Lookup:
                    return typeof(Guid);
                case AttributeTypeCode.ManagedProperty:
                    return typeof(bool);
                case AttributeTypeCode.Memo:
                    return typeof(string);
                case AttributeTypeCode.Money:
                    return typeof(decimal);
                case AttributeTypeCode.Owner:
                    return typeof(Guid);
                case AttributeTypeCode.PartyList:
                    return typeof(string);
                case AttributeTypeCode.Picklist:
                    return typeof(int);
                case AttributeTypeCode.State:
                    return typeof(int);
                case AttributeTypeCode.Status:
                    return typeof(int);
                case AttributeTypeCode.String:
                    return typeof(string);
                case AttributeTypeCode.Uniqueidentifier:
                    return typeof(Guid);
                case AttributeTypeCode.Virtual:
                    return typeof(string);
                default:
                    throw new NotSupportedException();

            }
        }

        public override DataTable GetSchemaTable()
        {
            //Note to self: See here for implementation info: http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(v=vs.110).aspx
            throw new NotSupportedException();
        }

        #endregion

        #region Field Data Retrieval

        public override object this[int ordinal]
        {
            get { return GetValue(ordinal); }
        }

        public override object this[string name]
        {
            // Look up the ordinal and return 
            // the value at that position.
            get { return this[GetOrdinal(name)]; }
        }

        public override object GetValue(int ordinal)
        {
            var name = GetName(ordinal);
            var record = _Results[_Position];
            if (!record.Attributes.ContainsKey(name))
            {
                return DBNull.Value;
            }
            return record[name];
        }

        public T GetValue<T>(int ordinal)
        {
            var name = GetName(ordinal);
            try
            {
               
                var record = _Results[_Position];
                if (!record.Attributes.ContainsKey(name))
                {
                    return default(T);
                }
                return (T)record[name];
            }
            catch (InvalidCastException e)
            {
                Debug.Write("error!");
                throw;
            }
         
        }

        public override int GetValues(object[] values)
        {
            int i;
            for (i = 0; i < values.Length; i++)
            {
                values[i] = GetValue(i);
            }
            return i;
        }

        public override bool IsDBNull(int ordinal)
        {
            var value = GetValue(ordinal);
            return value == null || value == DBNull.Value;
        }

        public override bool GetBoolean(int ordinal)
        {
            var value = GetValue<bool>(ordinal);
            return value;
        }

        public override byte GetByte(int ordinal)
        {
            var value = GetValue<byte>(ordinal);
            return (byte)value;
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            var value = GetValue<char>(ordinal);
            return (char)value;
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            var value = GetValue<Guid>(ordinal);
            return (Guid)value;
        }

        public override short GetInt16(int ordinal)
        {
            var value = GetValue<Int16>(ordinal);
            return (Int16)value;
        }

        public override int GetInt32(int ordinal)
        {
            // If the value is an option set, return the value of the option.
            var value = GetValue<int>(ordinal);
            return (int)value;
        }

        public override long GetInt64(int ordinal)
        {
            var value = GetValue<long>(ordinal);
            return (long)value;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            var value = GetValue<DateTime>(ordinal);
            return (DateTime)value;
        }

        public override string GetString(int ordinal)
        {
            var value = GetValue<string>(ordinal);
            return (string)value;
        }

        public override decimal GetDecimal(int ordinal)
        {
            var value = GetValue<decimal>(ordinal);
            return (decimal)value;
        }

        public override double GetDouble(int ordinal)
        {
            var value = GetValue<double>(ordinal);
            return (double)value;
        }

        public override float GetFloat(int ordinal)
        {
            var value = GetValue<float>(ordinal);
            return (float)value;
        }

        #endregion


    }
}