using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsCrmDataProvider
{

    public class ColumnMetadata
    {
        public string Name { get; set; }
        public string DataTypeName
        {
            get { return this.AttributeTypeCode.ToString(); }
        }
        public AttributeTypeCode AttributeTypeCode { get; set; }
        public Type DataType { get; set; }
    }

    public class CrmDbDataReader : DbDataReader
    {
        private EntityCollection _Results;
        private DbConnection _DbConnection;
        private bool _IsOpen;
        private const int StartPosition = -1;
        private int _Position = StartPosition;
        private bool _HasRows;
        private Entity _FirstResult;
        private List<ColumnMetadata> _Metadata;
        private int _ResultSetCount = 0;
        // private int _TotalRecordCount = 0;

        public CrmDbDataReader(EntityCollection results)
            : this(results, null)
        {

        }

        public CrmDbDataReader(EntityCollection results, DbConnection dbConnection)
        {
            // TODO: Complete member initialization
            this._Results = results;
            this._DbConnection = dbConnection;
            _IsOpen = true;
            // TODO: Change the below - shouldnt discover metadata from the first item, should retreive the metadata from CRM in advance.
            // ALSO Extend metadata with fields for EntityReference Name, Option Set Name, and possibly some other formatted values.
            _Metadata = new List<ColumnMetadata>();
            if (results.Entities != null && _Results.Entities.Any())
            {
                _HasRows = true;
                _ResultSetCount = _Results.Entities.Count;
                _FirstResult = results.Entities[0];
                foreach (var att in _FirstResult.Attributes)
                {
                    var columnMeta = new ColumnMetadata();
                    columnMeta.Name = att.Key;
                    // TODO: This needs to be set properly..
                    columnMeta.AttributeTypeCode = AttributeTypeCode.String;
                    // THIS IS BAD - VALUE COULD BE NULL..
                    columnMeta.DataType = att.Value.GetType();
                    _Metadata.Add(columnMeta);
                }
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

        private Entity FirstResult()
        {
            return _FirstResult;
        }

        public override int FieldCount
        {
            get { return _Metadata.Count; }
        }

        public override string GetName(int ordinal)
        {
            return _Metadata[ordinal].Name;
        }

        public override int GetOrdinal(string name)
        {
            int ordinal = 0;
            foreach (var m in _Metadata)
            {
                if (m.Name == name)
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
            return _Metadata[ordinal].DataTypeName;
        }

        public override Type GetFieldType(int ordinal)
        {
            return _Metadata[ordinal].DataType;
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
            return _Results[_Position][name];
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
            return value == null;
        }

        public override bool GetBoolean(int ordinal)
        {
            var value = GetValue(ordinal);
            return (bool)value;
        }

        public override byte GetByte(int ordinal)
        {
            var value = GetValue(ordinal);
            return (byte)value;
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            var value = GetValue(ordinal);
            return (char)value;
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal)
        {
            var value = GetValue(ordinal);
            return (Guid)value;
        }

        public override short GetInt16(int ordinal)
        {
            var value = GetValue(ordinal);
            return (Int16)value;
        }

        public override int GetInt32(int ordinal)
        {
            // If the value is an option set, return the value of the option.
            var value = GetValue(ordinal);
            return (int)value;
        }

        public override long GetInt64(int ordinal)
        {
            var value = GetValue(ordinal);
            return (long)value;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            var value = GetValue(ordinal);
            return (DateTime)value;
        }

        public override string GetString(int ordinal)
        {
            var value = GetValue(ordinal);
            return (string)value;
        }

        public override decimal GetDecimal(int ordinal)
        {
            var value = GetValue(ordinal);
            return (decimal)value;
        }

        public override double GetDouble(int ordinal)
        {
            var value = GetValue(ordinal);
            return (double)value;
        }

        public override float GetFloat(int ordinal)
        {
            var value = GetValue(ordinal);
            return (float)value;
        }

        #endregion


    }
}