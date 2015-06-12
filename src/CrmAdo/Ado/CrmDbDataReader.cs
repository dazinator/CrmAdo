using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using CrmAdo.Core;
using CrmAdo.Operations;

namespace CrmAdo
{
    public class CrmDbDataReader : DbDataReader
    {      
        private DbConnection _DbConnection;
        private bool _IsOpen;
        private const int StartPosition = -1;
        private int _Position = StartPosition;

        private ICrmOperationResult _Result;

        public ResultSet CurrentResultSet { get { return _Result.ResultSet; } }       

        private ISchemaTableProvider _SchemaTableProvider;

        public CrmDbDataReader(ResultSet resultSet)
            : this(new CrmOperationResult(null, resultSet, true))
        {
        }

        public CrmDbDataReader(ResultSet resultSet, DbConnection connection)
            : this(new CrmOperationResult(null, resultSet, true), connection)
        {
        }

        public CrmDbDataReader(ICrmOperationResult results)
            : this(results, null)
        {
        }

        public CrmDbDataReader(ICrmOperationResult result, DbConnection dbConnection)
            : this(result, dbConnection, new SchemaTableProvider())
        {
        }

        public CrmDbDataReader(ICrmOperationResult result, DbConnection dbConnection, ISchemaTableProvider schemaTableProvider)
        {
            // TODO: Complete member initialization
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }
            if (result.ResultSet == null)
            {
                throw new ArgumentNullException("resultset");
            }

            if (schemaTableProvider == null)
            {
                throw new ArgumentNullException("schemaTableProvider");
            }

            this._Result = result;
            this._SchemaTableProvider = schemaTableProvider;
            this._DbConnection = dbConnection; // optional.

            _IsOpen = true;          
        }

        public override bool IsClosed
        {
            get { return !_IsOpen; }
        }

        private int _Depth = 0;
        public override int Depth
        {
            get { return _Depth; }
        }

        public override int RecordsAffected
        {
            get
            {
                // RecordsAffected is only applicable to batch statements
                // that include inserts/updates/deletes. This always returns -1 for select statements,
                // and also doesn't return a value until the reader is closed.
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
            if (_Result.HasMoreResults)
            {
                _Result.NextOperationResult();              
                _Position = StartPosition;
                return true;
            }
            return false;
        }

        public override bool Read()
        {
            // Return true if it is possible to advance and if you are still positioned
            // on a valid row.
            if (++_Position >= CurrentResultSet.ResultCount())
            {
                // if the result set supports paging and has more records, load the next page.              
                if (!CurrentResultSet.HasMoreRecords())
                {
                    return false;
                }
                else
                {
                    CurrentResultSet.LoadNextPage();
                    _Position = StartPosition;
                    return true;
                    //TODO: Look into loading this asynchornously and ahead of time - perhaps 100 rows in advance?
                    // Could grab the next page of results..                      
                }
            }
            else
            {
                return true;
            }
        }

        public override bool HasRows
        {
            get { return CurrentResultSet.HasResults(); }
        }

        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this, this._DbConnection != null);
        }

        #region Metadata

        public override int FieldCount
        {
            get
            {
                return CurrentResultSet.ColumnMetadata.Count;
            }
        }

        public override string GetName(int ordinal)
        {
            return CurrentResultSet.ColumnMetadata[ordinal].ColumnName;
        }

        public override int GetOrdinal(string name)
        {
            int ordinal = 0;
            foreach (var m in CurrentResultSet.ColumnMetadata)
            {
                if (m.ColumnName.ToLower() == name.ToLower())
                {
                    return ordinal;
                }
                ordinal++;
            }
            // ok be nice.. but look for a better way to do this in future..
            // in case they are using an alias for the default entity (crm doesn't support this when using QueryExpression) then check for that..
            ordinal = 0;
            foreach (var m in CurrentResultSet.ColumnMetadata)
            {
                if (m.IsSameLogicalName(name))
                {
                    return ordinal;
                }
                ordinal++;
            }

            // Throw an exception if the ordinal cannot be found.
            var availableColumns = string.Join(",", (from c in CurrentResultSet.ColumnMetadata select c.ColumnName));
            throw new IndexOutOfRangeException("The column named " + name + " was not found in the available columns: " + availableColumns);
        }

        /// <summary>
        /// Retruns the name of the data type for the field, e.g 'varchar'
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override string GetDataTypeName(int ordinal)
        {
            // retrun the data type name e.g 'varchar'
            return CurrentResultSet.ColumnMetadata[ordinal].AttributeMetadata.GetSqlDataTypeName();
        }

        /// <summary>
        /// Returns the .NET type of the field.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override Type GetFieldType(int ordinal)
        {
            return CurrentResultSet.ColumnMetadata[ordinal].AttributeMetadata.GetFieldType();
        }

        /// <summary>
        /// Returns schema information.
        /// </summary>
        /// <returns></returns>
        public override DataTable GetSchemaTable()
        {
            return _SchemaTableProvider.GetSchemaTable(_DbConnection, CurrentResultSet.ColumnMetadata);
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
            var value = CurrentResultSet.GetValue(ordinal, _Position);
            return value;
        }

        public T GetValue<T>(int ordinal)
        {
            try
            {
                var val = GetValue(ordinal);
                if (val == DBNull.Value)
                {
                    return default(T);
                }
                return (T)val;
            }
            catch (InvalidCastException e)
            {
                Debug.Write("error casting value to T! " + e.Message);
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