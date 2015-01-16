using CrmAdo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CrmAdo.EntityFramework
{
    public class CrmEfProviderServices : DbProviderServices
    {
        private static readonly CrmEfProviderServices _Instance = new CrmEfProviderServices();

        public static CrmEfProviderServices Instance
        {
            get { return _Instance; }
        }

        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            return connection.ServerVersion;
        }

        protected override DbProviderManifest GetDbProviderManifest(string versionHint)
        {
            //if (string.IsNullOrEmpty(versionHint))
            //{
            //    throw new ArgumentException("Could not determine store version; a valid store connection or a version hint is required.");
            //}

            return new CrmEfProviderManifest(versionHint);
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest manifest, DbCommandTree commandTree)
        {
            DbCommand prototype = CreateCommand(manifest, commandTree);
            DbCommandDefinition result = this.CreateCommandDefinition(prototype);
            return result;
        }

        /// <summary>
        /// Create a SampleCommand object, given the provider manifest and command tree
        /// </summary>
        private DbCommand CreateCommand(DbProviderManifest manifest, DbCommandTree commandTree)
        {
            if (manifest == null)
                throw new ArgumentNullException("manifest");

            if (commandTree == null)
                throw new ArgumentNullException("commandTree");

            CrmEfProviderManifest sampleManifest = (manifest as CrmEfProviderManifest);
            if (sampleManifest == null)
            {
                throw new ArgumentException("The provider manifest given is not of type 'CrmEfProviderManifest'.");
            }

            //StoreVersion version = sampleManifest.Version;

            CrmDbCommand command = new CrmDbCommand();

            // List<DbParameter> parameters;
            //CommandType commandType;


            throw new NotImplementedException();


            //command.CommandText = SqlGenerator.GenerateSql(commandTree, out parameters, out commandType);
            //command.CommandType = commandType;

            //if (command.CommandType == CommandType.Text)
            //{
            //    //command.CommandText += Environment.NewLine + Environment.NewLine + "-- provider: " + this.GetType().Assembly.FullName;
            //}

            //// Get the function (if any) implemented by the command tree since this influences our interpretation of parameters
            //EdmFunction function = null;
            //if (commandTree is DbFunctionCommandTree)
            //{
            //    function = ((DbFunctionCommandTree)commandTree).EdmFunction;
            //}

            //// Now make sure we populate the command's parameters from the CQT's parameters:
            //foreach (KeyValuePair<string, TypeUsage> queryParameter in commandTree.Parameters)
            //{
            //    CrmParameter parameter;

            //    // Use the corresponding function parameter TypeUsage where available (currently, the SSDL facets and 
            //    // type trump user-defined facets and type in the EntityCommand).
            //    FunctionParameter functionParameter;
            //    if (null != function && function.Parameters.TryGetValue(queryParameter.Key, false, out functionParameter))
            //    {
            //        parameter = CreateParameter(functionParameter.Name, functionParameter.TypeUsage, functionParameter.Mode, DBNull.Value);
            //    }
            //    else
            //    {
            //        parameter = CreateParameter(queryParameter.Key, queryParameter.Value, ParameterMode.In, DBNull.Value);
            //    }

            //    command.Parameters.Add(parameter);
            //}

            //// Now add parameters added as part of SQL gen (note: this feature is only safe for DML SQL gen which
            //// does not support user parameters, where there is no risk of name collision)
            //if (null != parameters && 0 < parameters.Count)
            //{
            //    if (!(commandTree is DbInsertCommandTree) &&
            //        !(commandTree is DbUpdateCommandTree) &&
            //        !(commandTree is DbDeleteCommandTree))
            //    {
            //        throw new InvalidOperationException("SqlGenParametersNotPermitted");
            //    }

            //    foreach (DbParameter parameter in parameters)
            //    {
            //        command.Parameters.Add(parameter);
            //    }
            //}

            //return command;
        }

        protected override void SetDbParameterValue(DbParameter parameter, TypeUsage parameterType, object value)
        {
            // Ensure a value that can be used with SqlParameter
            parameter.Value = EnsureParameterValue(value);
        }             

        protected override string DbCreateDatabaseScript(string providerManifestToken, StoreItemCollection storeItemCollection)
        {
            if (providerManifestToken == null)
                throw new ArgumentNullException("providerManifestToken must not be null");

            if (storeItemCollection == null)
                throw new ArgumentNullException("storeItemCollection must not be null");

            throw new NotImplementedException();

            //return DdlBuilder.CreateObjectsScript(storeItemCollection);
        }

        protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection must not be null");

            if (storeItemCollection == null)
                throw new ArgumentNullException("storeItemCollection must not be null");

            CrmDbConnection sampleConnection = connection as CrmDbConnection;
            if (sampleConnection == null)
            {
                throw new ArgumentException("The connection is not of type 'SampleConnection'.");
            }

            string databaseName = GetDatabaseName(sampleConnection);
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new InvalidOperationException("Initial Catalog is missing from the connection string");
            }



            //TODO: Need to create CRM organisation, and then create all necessary objects.
            // Need to skip over system entities... only creating custom attributes and custom entities.
            // Could put entire EF schema into a single managed crm solution and do it that way??

            throw new NotImplementedException();

            //string dataFileName, logFileName;
            //GetDatabaseFileNames(sampleConnection, out dataFileName, out logFileName);

            //string createDatabaseScript = DdlBuilder.CreateDatabaseScript(databaseName, dataFileName, logFileName);
            //string createObjectsScript = DdlBuilder.CreateObjectsScript(storeItemCollection);

            //UsingMasterConnection(sampleConnection, conn =>
            //{
            //    // create database
            //    CreateCommand(conn, createDatabaseScript, commandTimeout).ExecuteNonQuery();
            //});

            //// Clear connection pool for the database connection since after the 'create database' call, a previously
            //// invalid connection may now be valid.                
            //sampleConnection.ClearPool();

            //UsingConnection(sampleConnection, conn =>
            //{
            //    // create database objects
            //    CreateCommand(conn, createObjectsScript, commandTimeout).ExecuteNonQuery();
            //});

        }

        private static string GetDatabaseName(CrmDbConnection sampleConnection)
        {
            string databaseName = sampleConnection.Database;
            if (string.IsNullOrEmpty(databaseName))
                throw new InvalidOperationException("Connection String did not specify an Initial Catalog");

            return databaseName;

        }

        protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection must not be null");

            if (storeItemCollection == null)
                throw new ArgumentNullException("storeItemCollection must not be null");

            CrmDbConnection crmConnection = connection as CrmDbConnection;
            if (crmConnection == null)
                throw new ArgumentException("connection must be a valid SampleConnection");

            string databaseName = GetDatabaseName(crmConnection);

            // bool exists = false;

            throw new NotImplementedException();

            //UsingMasterConnection(crmConnection, conn =>
            //{
            //  throw new NotImplementedException();

            //StoreVersion storeVersion = StoreVersionUtils.GetStoreVersion(conn);
            //string databaseExistsScript = DdlBuilder.CreateDatabaseExistsScript(databaseName);

            //int result = (int)CreateCommand(conn, databaseExistsScript, commandTimeout).ExecuteScalar();
            //exists = (result == 1);
            // });

            //  return exists;
        }

        protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection must not be null");

            if (storeItemCollection == null)
                throw new ArgumentNullException("storeItemCollection must not be null");

            CrmDbConnection sampleConnection = connection as CrmDbConnection;
            if (sampleConnection == null)
                throw new ArgumentException("connection must be a valid SampleConnection");

            throw new NotImplementedException();

            //string databaseName = GetDatabaseName(sampleConnection);
            //string dropDatabaseScript = DdlBuilder.DropDatabaseScript(databaseName);

            //// clear the connection pool in case someone is holding on to the database
            //sampleConnection.ClearPool();

            //UsingMasterConnection(sampleConnection, (conn) =>
            //{
            //    CreateCommand(conn, dropDatabaseScript, commandTimeout).ExecuteNonQuery();
            //});
        }

        private static DbCommand CreateCommand(CrmDbConnection connection, string commandText, int? commandTimeout)
        {
            Debug.Assert(connection != null);
            //if (string.IsNullOrEmpty(commandText))
            //{
            //    // SqlCommand will complain if the command text is empty
            //    //commandText = Environment.NewLine;
            //}

            // throw new NotImplementedException();

            var command = connection.CreateCommand();
            command.CommandText = commandText;

            // var command = new CrmDbConnection(commandText, connection);
            if (commandTimeout.HasValue)
            {
                command.CommandTimeout = commandTimeout.Value;
            }
            return command;
        }

        private static void UsingConnection(CrmDbConnection connection, Action<CrmDbConnection> act)
        {
            // remember the connection string so that we can reset it if credentials are wiped
            string holdConnectionString = connection.ConnectionString;
            bool openingConnection = connection.State == ConnectionState.Closed;
            if (openingConnection)
            {
                connection.Open();
            }
            try
            {
                act(connection);
            }
            finally
            {
                if (openingConnection && connection.State == ConnectionState.Open)
                {
                    // if we opened the connection, we should close it
                    connection.Close();
                }
                if (connection.ConnectionString != holdConnectionString)
                {
                    connection.ConnectionString = holdConnectionString;
                }
            }
        }

        /// <summary>
        /// Creates a SqlParameter given a name, type, and direction
        /// </summary>
        internal static CrmParameter CreateParameter(string name, TypeUsage type, ParameterMode mode, object value)
        {
            // int? size;

            value = EnsureParameterValue(value);

            CrmParameter result = new CrmParameter(name, value);

            throw new NotImplementedException();

            //// .Direction
            //result.Direction = MetadataHelpers.ParameterModeToParameterDirection(mode);

            //// .Size and .SqlDbType
            //// output parameters are handled differently (we need to ensure there is space for return
            //// values where the user has not given a specific Size/MaxLength)
            //bool isOutParam = mode != ParameterMode.In;

            //string udtTypeName;
            //result.DbType = GetSqlDbType(type, isOutParam, out size, out udtTypeName);
            //result.UdtTypeName = udtTypeName;

            //// Note that we overwrite 'facet' parameters where either the value is different or
            //// there is an output parameter.
            //if (size.HasValue && (isOutParam || result.Size != size.Value))
            //{
            //    result.Size = size.Value;
            //}

            //// .IsNullable
            //bool isNullable = MetadataHelpers.IsNullable(type);
            //if (isOutParam || isNullable != result.IsNullable)
            //{
            //    result.IsNullable = isNullable;
            //}

            //return result;
        }

        /// <summary>
        /// Converts DbGeography/DbGeometry values to corresponding Sql Server spatial values.
        /// </summary>
        /// <param name="value">value to convert</param>
        /// <returns>Sql Server spatial value for DbGeometry/DbGeography or <paramref name="value"/>.</returns>
        internal static object EnsureParameterValue(object value)
        {
            if (value != null &&
                value != DBNull.Value &&
                Type.GetTypeCode(value.GetType()) == TypeCode.Object)
            {
                // If the parameter is being created based on an actual value (typically for constants found in DML expressions) then a DbGeography/DbGeometry
                // value must be replaced by an an appropriate Microsoft.SqlServer.Types.SqlGeography/SqlGeometry instance. Since the DbGeography/DbGeometry
                // value may not have been originally created by this SqlClient provider services implementation, just using the ProviderValue is not sufficient.
                //DbGeography geographyValue = value as DbGeography;
                //if (geographyValue != null)
                //{
                //    value = SqlTypes.ConvertToSqlTypesGeography(geographyValue);
                //}
                //else
                //{
                //    DbGeometry geometryValue = value as DbGeometry;
                //    if (geometryValue != null)
                //    {
                //        value = SqlTypes.ConvertToSqlTypesGeometry(geometryValue);
                //    }
                //}
            }

            return value;
        }

        /// <summary>
        /// Determines DbType for the given primitive type. Extracts facet
        /// information as well.
        /// </summary>
        private static DbType GetDbType(TypeUsage type, bool isOutParam, out int? size, out string udtName)
        {
            // only supported for primitive type
            throw new NotImplementedException();

            //PrimitiveTypeKind primitiveTypeKind = MetadataHelpers.GetPrimitiveTypeKind(type);

            //size = default(int?);
            //udtName = null;

            //// TODO add logic for Xml here
            //switch (primitiveTypeKind)
            //{
            //    case PrimitiveTypeKind.Binary:
            //        // for output parameters, ensure there is space...
            //        size = GetParameterSize(type, isOutParam);
            //        return GetBinaryDbType(type);

            //    case PrimitiveTypeKind.Boolean:
            //        return SqlDbType.Bit;

            //    case PrimitiveTypeKind.Byte:
            //        return SqlDbType.TinyInt;

            //    case PrimitiveTypeKind.Time:
            //        return SqlDbType.Time;

            //    case PrimitiveTypeKind.DateTimeOffset:
            //        return SqlDbType.DateTimeOffset;

            //    case PrimitiveTypeKind.DateTime:
            //        return SqlDbType.DateTime;

            //    case PrimitiveTypeKind.Decimal:
            //        return SqlDbType.Decimal;

            //    case PrimitiveTypeKind.Double:
            //        return SqlDbType.Float;

            //    case PrimitiveTypeKind.Guid:
            //        return SqlDbType.UniqueIdentifier;

            //    case PrimitiveTypeKind.Int16:
            //        return SqlDbType.SmallInt;

            //    case PrimitiveTypeKind.Int32:
            //        return SqlDbType.Int;

            //    case PrimitiveTypeKind.Int64:
            //        return SqlDbType.BigInt;

            //    case PrimitiveTypeKind.SByte:
            //        return SqlDbType.SmallInt;

            //    case PrimitiveTypeKind.Single:
            //        return SqlDbType.Real;

            //    case PrimitiveTypeKind.String:
            //        size = GetParameterSize(type, isOutParam);
            //        return GetStringDbType(type);

            //    case PrimitiveTypeKind.Geography:
            //        {
            //            udtName = "geography";
            //            return SqlDbType.Udt;
            //        }

            //    case PrimitiveTypeKind.Geometry:
            //        {
            //            udtName = "geometry";
            //            return SqlDbType.Udt;
            //        }

            //    default:
            //        Debug.Fail("unknown PrimitiveTypeKind " + primitiveTypeKind);
            //        return SqlDbType.Variant;
            //}
        }

        /// <summary>
        /// Determines preferred value for SqlParameter.Size. Returns null
        /// where there is no preference.
        /// </summary>
        private static int? GetParameterSize(TypeUsage type, bool isOutParam)
        {
            throw new NotImplementedException();

            //int maxLength;
            //if (MetadataHelpers.TryGetMaxLength(type, out maxLength))
            //{
            //    // if the MaxLength facet has a specific value use it
            //    return maxLength;
            //}
            //else if (isOutParam)
            //{
            //    // if the parameter is a return/out/inout parameter, ensure there 
            //    // is space for any value
            //    return int.MaxValue;
            //}
            //else
            //{
            //    // no value
            //    return default(int?);
            //}
        }

        /// <summary>
        /// Chooses the appropriate SqlDbType for the given string type.
        /// </summary>
        private static DbType GetStringDbType(TypeUsage type)
        {
            Debug.Assert(type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType &&
                PrimitiveTypeKind.String == ((PrimitiveType)type.EdmType).PrimitiveTypeKind, "only valid for string type");

            DbType dbType;
            if (type.EdmType.Name.ToLowerInvariant() == "xml")
            {
                dbType = DbType.Xml;
            }
            else
            {
                // Specific type depends on whether the string is a unicode string and whether it is a fixed length string.
                // By default, assume widest type (unicode) and most common type (variable length)

                throw new NotImplementedException();


                //bool unicode;
                //bool fixedLength;
                //if (!MetadataHelpers.TryGetIsFixedLength(type, out fixedLength))
                //{
                //    fixedLength = false;
                //}

                //if (!MetadataHelpers.TryGetIsUnicode(type, out unicode))
                //{
                //    unicode = true;
                //}

                //if (fixedLength)
                //{
                //    dbType = (unicode ? SqlDbType.NChar : SqlDbType.Char);
                //}
                //else
                //{
                //    dbType = (unicode ? SqlDbType.NVarChar : SqlDbType.VarChar);
                //}
            }
            return dbType;
        }

    }
}
