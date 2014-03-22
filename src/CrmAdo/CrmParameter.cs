using System;
using System.Data;
using System.Data.Common;

namespace CrmAdo
{
    public class CrmParameter : DbParameter
    {
        DbType _DbType = DbType.Object;
        ParameterDirection _Direction = ParameterDirection.Input;
        bool _Nullable = false;
        string _ParamName;
        string _SourceColumn;
        DataRowVersion _SourceVersion = DataRowVersion.Current;
        object _Value;

        public CrmParameter()
        {
        }

        public CrmParameter(string parameterName, DbType type)
        {
            _ParamName = parameterName;
            _DbType = type;
        }

        public CrmParameter(string parameterName, object value)
        {
            _ParamName = parameterName;
            this.Value = value;
            // Setting the value also infers the type.
        }

        public CrmParameter(string parameterName, DbType dbType, string sourceColumn)
        {
            _ParamName = parameterName;
            _DbType = dbType;
            _SourceColumn = sourceColumn;
        }

        public override void ResetDbType()
        {
            this.DbType = DbType.AnsiString;
        }

        public override DbType DbType
        {
            get { return _DbType; }
            set { _DbType = value; }
        }

        public override ParameterDirection Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public override Boolean IsNullable
        {
            get { return _Nullable; }
            set { _Nullable = value; }
        }

        public override String ParameterName
        {
            get { return _ParamName; }
            set { _ParamName = value; }
        }

        public override int Size { get; set; }

        public override String SourceColumn
        {
            get { return _SourceColumn; }
            set { _SourceColumn = value; }
        }

        public override bool SourceColumnNullMapping { get; set; }

        public override DataRowVersion SourceVersion
        {
            get { return _SourceVersion; }
            set { _SourceVersion = value; }
        }

        public override sealed object Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                _DbType = _inferType(value);
            }
        }

        private DbType _inferType(Object value)
        {
            var type = value.GetType();
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Empty:
                    throw new SystemException("Invalid data type");

                case TypeCode.Object:

                    if (type == typeof(Guid))
                    {
                        return DbType.Guid;
                    }

                    return DbType.Object;

                case TypeCode.Boolean:
                    return DbType.Boolean;

                case TypeCode.Byte:
                    return DbType.Byte;

                case TypeCode.Int16:
                    return DbType.Int16;

                case TypeCode.Int32:
                    return DbType.Int32;

                case TypeCode.Int64:
                    return DbType.Int64;

                case TypeCode.Single:
                    return DbType.Single;

                case TypeCode.Double:
                    return DbType.Double;

                case TypeCode.Decimal:
                    return DbType.Decimal;

                case TypeCode.DateTime:
                    return DbType.DateTime;

                case TypeCode.String:
                    return DbType.String;

                case TypeCode.DBNull:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    // Throw a SystemException for unsupported data types.
                    throw new SystemException("Invalid data type");

                default:
                    throw new SystemException("Value is of unknown data type");
            }
        }


        public bool CanAddToCommand()
        {
            if (string.IsNullOrWhiteSpace(this.ParameterName))
            {
                return false;
            }
            return true;
        }

    }

}
