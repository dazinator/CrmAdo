using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsCrmDataProvider
{
    public class CrmDbDataReader : DbDataReader
    {
        //private EntityCollection _Results;
        private DbConnection _DbConnection;
        private bool _IsOpen;
        private const int StartPosition = -1;
        private int _Position = StartPosition;
        private EntityResultSet _Results;

        public CrmDbDataReader(EntityResultSet results)
            : this(results, null)
        {

        }

        public CrmDbDataReader(EntityResultSet results, DbConnection dbConnection)
        {
            // TODO: Complete member initialization
            if (results == null)
            {
                throw new ArgumentNullException("results");
            }
            this._Results = results;
            this._DbConnection = dbConnection;
            _IsOpen = true;
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
            if (++_Position >= _Results.ResultCount())
            {
                // come to end of result set.. if more available need to load them..
                if (!_Results.Results.MoreRecords)
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
            get { return _Results.HasResults(); }
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, this._DbConnection != null);
        }

        #region Metadata 

        public override int FieldCount
        {
            get { return _Results.ColumnMetadata.Count; }
        }

        public override string GetName(int ordinal)
        {
            return _Results.ColumnMetadata[ordinal].ColumnName;
        }

        public override int GetOrdinal(string name)
        {
            int ordinal = 0;
            foreach (var m in _Results.ColumnMetadata)
            {
                if (m.ColumnName == name)
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
            return _Results.ColumnMetadata[ordinal].ColumnDataType();
        }

        public override Type GetFieldType(int ordinal)
        {
            switch (_Results.ColumnMetadata[ordinal].AttributeType())
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

        #region Field Value Retrieval

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
            var record = _Results.Results[_Position];
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

                var record = _Results.Results[_Position];
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