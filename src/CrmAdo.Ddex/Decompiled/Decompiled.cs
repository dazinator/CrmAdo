using System.Collections;
using Microsoft.VisualStudio.Data.Framework;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using System.Diagnostics.CodeAnalysis;

namespace CrmAdo.DdexProvider
{
    internal static class AdoDotNetProvider
    {
        public static DbProviderFactory GetProviderFactory(string providerInvariantName)
        {
            return DbProviderFactories.GetFactory(providerInvariantName);
        }

        public static void ApplyMappings(DataTable dataTable, IDictionary<string, object> mappings)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if (mappings == null)
            {
                return;
            }
            foreach (KeyValuePair<string, object> mapping in mappings)
            {
                DataColumn item = dataTable.Columns[mapping.Key];
                if (item != null)
                {
                    continue;
                }
                string value = mapping.Value as string;
                if (value == null)
                {
                    int num = (int)mapping.Value;
                    if (num >= 0 && num < dataTable.Columns.Count)
                    {
                        item = dataTable.Columns[num];
                    }
                }
                else
                {
                    item = dataTable.Columns[value];
                }
                if (item == null)
                {
                    continue;
                }
                dataTable.Columns.Add(new DataColumn(mapping.Key, item.DataType, string.Concat("[", item.ColumnName.Replace("]", "]]"), "]")));
            }
        }

        public static T CreateObject<T>(string invariantName)
        where T : class
        {
            if (invariantName == null)
            {
                throw new DataException("");
            }
            DbProviderFactory providerFactory = GetProviderFactory(invariantName);
            if (providerFactory == null)
            {
                string adoDotNetProviderNotRegistered = "";
                object[] objArray = new object[] { invariantName };
                throw new DataException(string.Format((IFormatProvider)null, adoDotNetProviderNotRegistered, objArray));
            }
            T t = default(T);
            if (typeof(T) == typeof(DbCommandBuilder))
            {
                t = (T)(providerFactory.CreateCommandBuilder() as T);
            }
            if (typeof(T) == typeof(DbConnection))
            {
                t = (T)(providerFactory.CreateConnection() as T);
            }
            if (typeof(T) == typeof(DbConnectionStringBuilder))
            {
                t = (T)(providerFactory.CreateConnectionStringBuilder() as T);
            }
            if (typeof(T) == typeof(DbParameter))
            {
                t = (T)(providerFactory.CreateParameter() as T);
            }
            if (t == null)
            {
                string adoDotNetProviderObjectNotImplemented = "";
                object[] objArray1 = new object[] { invariantName, typeof(T).Name };
                throw new DataException(string.Format((IFormatProvider)null, adoDotNetProviderObjectNotImplemented, objArray1));
            }
            return t;
        }
    }

    /// <summary>Provides an implementation of the <see cref="T:Microsoft.VisualStudio.Data.Services.SupportEntities.IVsDataObjectSelector" /> interface using the ADO.NET <see cref="M:System.Data.Common.DbConnection.GetSchema" /> method.</summary>
    public class AdoDotNetObjectSelector : DataObjectSelector
    {
        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.VisualStudio.Data.Framework.AdoDotNet.AdoDotNetObjectSelector" /> class.</summary>
        public AdoDotNetObjectSelector()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.VisualStudio.Data.Framework.AdoDotNet.AdoDotNetObjectSelector" /> class with the data connection object.</summary>
        /// <param name="connection">An <see cref="T:Microsoft.VisualStudio.Data.Services.IVsDataConnection" /> object representing the communication to the data source.</param>
        public AdoDotNetObjectSelector(IVsDataConnection connection)
            : base(connection)
        {
        }

        /// <summary>Applies the selector mappings.</summary>
        /// <param name="dataTable">The schema returned by the call to the <see cref="M:System.Data.Common.DbConnection.GetSchema" /> method.</param>
        /// <param name="mappings">Key/value pairs containing the selector mappings.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="schema" /> parameter is null.</exception>
        protected static void ApplyMappings(DataTable dataTable, IDictionary<string, object> mappings)
        {

            AdoDotNetProvider.ApplyMappings(dataTable, mappings);
        }

        /// <summary>Returns a data reader for the data objects retrieved from the object store, which are filtered by the specified restrictions, properties, and parameters.</summary>
        /// <returns>An <see cref="T:Microsoft.VisualStudio.Data.Services.SupportEntities.IVsDataReader" /> object representing a data reader for the selected data objects.</returns>
        /// <param name="typeName">The data source–specific name of the specified type to retrieve data objects for.</param>
        /// <param name="restrictions">The restrictions for filtering the data objects returned.</param>
        /// <param name="properties">Specifies the property values of the requested data objects. This is not supported in the current version of DDEX.</param>
        /// <param name="parameters">An array containing the parameters for the specified type.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="typeName" /> parameter is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="parameters" /> parameter is not valid. Either it is null, or the number of elements contained in it is not 1 or 2, or the first element is not a string.</exception>
        /// <exception cref="T:System.InvalidOperationException">The site is null.</exception>
        /// <exception cref="T:System.NotImplementedException">The provider cannot be obtained.</exception>
        protected override IVsDataReader SelectObjects(string typeName, object[] restrictions, string[] properties, object[] parameters)
        {
            IVsDataReader adoDotNetTableReader;
            string str;
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (parameters == null || (int)parameters.Length < 1 || (int)parameters.Length > 2 || !(parameters[0] is string))
            {
                throw new ArgumentException();
            }
            if (base.Site == null)
            {
                throw new InvalidOperationException();
            }
            object lockedProviderObject = base.Site.GetLockedProviderObject();
            if (lockedProviderObject == null)
            {
                throw new NotImplementedException();
            }
            try
            {
                DbConnection dbConnection = lockedProviderObject as DbConnection;
                if (dbConnection == null)
                {
                    throw new NotImplementedException();
                }
                string[] strArrays = null;
                if (restrictions != null)
                {
                    strArrays = new string[(int)restrictions.Length];
                    for (int i = 0; i < (int)strArrays.Length; i++)
                    {
                        string[] strArrays1 = strArrays;
                        int num = i;
                        if (restrictions[i] != null)
                        {
                            str = restrictions[i].ToString();
                        }
                        else
                        {
                            str = null;
                        }
                        strArrays1[num] = str;
                    }
                }
                base.Site.EnsureConnected();
                DataTable schema = dbConnection.GetSchema(parameters[0].ToString(), strArrays);
                if ((int)parameters.Length == 2 && parameters[1] is DictionaryEntry)
                {
                    object[] value = ((DictionaryEntry)parameters[1]).Value as object[];
                    if (value != null)
                    {
                        AdoDotNetObjectSelector.ApplyMappings(schema, DataObjectSelector.GetMappings(value));
                    }
                }
                adoDotNetTableReader = new AdoDotNetTableReader(schema);
            }
            finally
            {
                base.Site.UnlockProviderObject();
            }
            return adoDotNetTableReader;
        }
    }

    public class AdoDotNetMappedObjectConverter : DataMappedObjectConverter
    {
        private bool _gotDataTypes;

        private DataTable _dataTypes;

        protected DataTable DataTypes
        {
            [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
            get
            {
                if (base.Site != null && !this._gotDataTypes)
                {
                    object lockedProviderObject = base.Site.GetLockedProviderObject();
                    if (lockedProviderObject != null)
                    {
                        try
                        {
                            DbConnection dbConnection = lockedProviderObject as DbConnection;
                            if (dbConnection != null)
                            {
                                base.Site.EnsureConnected();
                                try
                                {
                                    this._dataTypes = dbConnection.GetSchema(DbMetaDataCollectionNames.DataTypes);
                                }
                                catch
                                {
                                }
                            }
                        }
                        finally
                        {
                            base.Site.UnlockProviderObject();
                        }
                    }
                    this._gotDataTypes = true;
                }
                return this._dataTypes;
            }
        }

        public AdoDotNetMappedObjectConverter()
        {
        }

        public AdoDotNetMappedObjectConverter(IVsDataConnection connection)
            : base(connection)
        {
        }

        protected override object ConvertToMappedMember(string typeName, string mappedMemberName, object[] underlyingValues, object[] parameters)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException("typeName");
            }
            if (mappedMemberName == null)
            {
                throw new ArgumentNullException("mappedMemberName");
            }
            if (underlyingValues == null || (int)underlyingValues.Length != 1 || !(underlyingValues[0] is string))
            {
                throw new ArgumentException("invalid underlying values", "underlyingValues");
            }
            string str = underlyingValues[0] as string;
            if (mappedMemberName.Equals("AdoDotNetDataType", StringComparison.OrdinalIgnoreCase))
            {
                return this.GetProviderTypeFromNativeType(str);
            }
            if (mappedMemberName.Equals("AdoDotNetDbType", StringComparison.OrdinalIgnoreCase))
            {
                return this.GetDbTypeFromNativeType(str);
            }
            if (!mappedMemberName.Equals("FrameworkDataType", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotImplementedException();
            }
            return this.GetFrameworkTypeFromNativeType(str).FullName;
        }

        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db")]
        protected virtual DbType GetDbTypeFromNativeType(string nativeType)
        {
            Type frameworkTypeFromNativeType = this.GetFrameworkTypeFromNativeType(nativeType);
            if (frameworkTypeFromNativeType == typeof(bool))
            {
                return DbType.Boolean;
            }
            if (frameworkTypeFromNativeType == typeof(byte))
            {
                return DbType.Byte;
            }
            if (frameworkTypeFromNativeType == typeof(short))
            {
                return DbType.Int16;
            }
            if (frameworkTypeFromNativeType == typeof(int))
            {
                return DbType.Int32;
            }
            if (frameworkTypeFromNativeType == typeof(long))
            {
                return DbType.Int64;
            }
            if (frameworkTypeFromNativeType == typeof(float))
            {
                return DbType.Single;
            }
            if (frameworkTypeFromNativeType == typeof(double))
            {
                return DbType.Double;
            }
            if (frameworkTypeFromNativeType == typeof(decimal))
            {
                return DbType.Decimal;
            }
            if (frameworkTypeFromNativeType != typeof(string))
            {
                if (frameworkTypeFromNativeType == typeof(DateTime))
                {
                    return DbType.DateTime;
                }
                if (frameworkTypeFromNativeType == typeof(Guid))
                {
                    return DbType.Guid;
                }
                if (frameworkTypeFromNativeType == typeof(byte[]))
                {
                    return DbType.Binary;
                }
                return DbType.Object;
            }
            bool flag = false;
            if (this.DataTypes != null)
            {
                DataRow[] dataRowArray = this.DataTypes.Select(string.Concat("TypeName='", nativeType.Replace("'", "''"), "'"));
                DataRow[] dataRowArray1 = dataRowArray;
                for (int i = 0; i < (int)dataRowArray1.Length; i++)
                {
                    DataRow dataRow = dataRowArray1[i];
                    object item = dataRow["DataType"];
                    var obj = dataRow["IsFixedLength"];
                    if ((item is Type || item is string) && obj is bool)
                    {
                        flag = (bool)obj;
                    }
                }
            }
            if (!flag)
            {
                return DbType.String;
            }
            return DbType.StringFixedLength;
        }

        protected virtual Type GetFrameworkTypeFromNativeType(string nativeType)
        {
            Type type;
            if (this.DataTypes != null)
            {
                DataRow[] dataRowArray = this.DataTypes.Select(string.Concat("TypeName='", nativeType.Replace("'", "''"), "'"));
                DataRow[] dataRowArray1 = dataRowArray;
                int num = 0;
                while (true)
                {
                    if (num >= (int)dataRowArray1.Length)
                    {
                        return typeof(object);
                    }
                    object item = dataRowArray1[num]["DataType"];
                    Type type1 = item as Type;
                    if (type1 == null)
                    {
                        string str = item as string;
                        if (str == null)
                        {
                            num++;
                        }
                        else
                        {
                            type = Type.GetType(str);
                            break;
                        }
                    }
                    else
                    {
                        type = type1;
                        break;
                    }
                }
                return type;
            }
            return typeof(object);
        }

        protected virtual int GetProviderTypeFromNativeType(string nativeType)
        {
            if (nativeType == null)
            {
                throw new ArgumentNullException("nativeType");
            }
            if (this.DataTypes != null)
            {
                DataRow[] dataRowArray = this.DataTypes.Select(string.Concat("TypeName='", nativeType.Replace("'", "''"), "'"));
                DataRow[] dataRowArray1 = dataRowArray;
                for (int i = 0; i < (int)dataRowArray1.Length; i++)
                {
                    object item = dataRowArray1[i]["ProviderDbType"];
                    if (item is int)
                    {
                        return (int)item;
                    }
                }
            }
            return 0;
        }

        protected override void OnSiteChanged(EventArgs e)
        {
            base.OnSiteChanged(e);
            if (base.Site == null)
            {
                this._gotDataTypes = false;
                if (this._dataTypes != null)
                {
                    this._dataTypes.Dispose();
                    this._dataTypes = null;
                }
            }
        }
    }
}
