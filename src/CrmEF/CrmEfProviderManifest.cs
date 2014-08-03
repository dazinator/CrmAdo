using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CrmEF
{
    internal sealed class CrmEfProviderManifest : DbXmlEnabledProviderManifest
    {
        /// <param name="manifestToken">A token used to infer the capabilities of the store</param>
        public CrmEfProviderManifest(string manifestToken)
            : base(CrmEfProviderManifest.GetProviderManifest())
        {
            // GetStoreVersion will throw ArgumentException if manifestToken is null, empty, or not recognized.
            // _version = StoreVersionUtils.GetStoreVersion(manifestToken);
            // _token = manifestToken;
        }

        private static XmlReader GetXmlResource(string resourceName)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            Stream stream = executingAssembly.GetManifestResourceStream(resourceName);
            return XmlReader.Create(stream);
        }

        private static XmlReader GetProviderManifest()
        {
            return CrmEfProviderManifest.GetXmlResource("CrmEfProviderServices.ProviderManifest.xml");
        }

        private XmlReader GetStoreSchemaMapping()
        {
            return CrmEfProviderManifest.GetXmlResource("CrmEfProviderServices.StoreSchemaMapping.msl");
        }

        private XmlReader GetStoreSchemaDescription()
        {
            return CrmEfProviderManifest.GetXmlResource("CrmEfProviderServices.StoreSchemaDefinition.ssdl");
        }
       
        /// <param name="informationType">The name of the information to be retrieved.</param>
        /// <returns>An XmlReader at the begining of the information requested.</returns>
        protected override XmlReader GetDbInformation(string informationType)
        {
            if (informationType == DbProviderManifest.StoreSchemaDefinitionVersion3)
            {
                return GetStoreSchemaDescription();
            }

            if (informationType == DbProviderManifest.StoreSchemaMappingVersion3)
            {
                return GetStoreSchemaMapping();
            }

            throw new ProviderIncompatibleException(String.Format("The provider returned null for the informationType '{0}'.", informationType));
        }

        public override string NamespaceName
        {
            get
            {
                return "CrmEf";
            }
        }

        /// <summary>
        /// This method takes a type and a set of facets and returns the best mapped equivalent type 
        /// in EDM.
        /// </summary>
        /// <param name="storeType">A TypeUsage encapsulating a store type and a set of facets</param>
        /// <returns>A TypeUsage encapsulating an EDM type and a set of facets</returns>
        public override TypeUsage GetEdmType(TypeUsage storeType)
        {
            if (storeType == null)
            {
                throw new ArgumentNullException("storeType");
            }

            string storeTypeName = storeType.EdmType.Name.ToLowerInvariant();
            if (!base.StoreTypeNameToEdmPrimitiveType.ContainsKey(storeTypeName))
            {
                throw new ArgumentException(String.Format("The underlying provider does not support the type '{0}'.", storeTypeName));
            }

            PrimitiveType edmPrimitiveType = base.StoreTypeNameToEdmPrimitiveType[storeTypeName];

            switch (storeTypeName)
            {
                // for some types we just go with simple type usage with no facets
                case "int":
                case "integer":
                case "uint":
                case "long":
                case "ulong":
                case "float":
                case "double":
                case "boolean":
                case "binary":
                case "string":
                    return TypeUsage.CreateDefaultTypeUsage(edmPrimitiveType);

                case "decimal":
                    return TypeUsage.CreateDecimalTypeUsage(edmPrimitiveType);

                case "date":
                case "datetime":
                    return TypeUsage.CreateDateTimeTypeUsage(edmPrimitiveType, null);

                default:
                    throw new NotSupportedException(String.Format("The underlying provider does not support the type '{0}'.", storeTypeName));
            }

        }

        /// <summary>
        /// This method takes a type and a set of facets and returns the best mapped equivalent type 
        /// in Dynamics Crm.
        /// </summary>
        /// <param name="storeType">A TypeUsage encapsulating an EDM type and a set of facets</param>
        /// <returns>A TypeUsage encapsulating a store type and a set of facets</returns>
        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            if (edmType == null)
            {
                throw new ArgumentNullException("edmType");
            }
            System.Diagnostics.Debug.Assert(edmType.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType);

            PrimitiveType primitiveType = edmType.EdmType as PrimitiveType;
            if (primitiveType == null)
            {
                throw new ArgumentException(String.Format("The underlying provider does not support the type '{0}'.", edmType));
            }

            ReadOnlyMetadataCollection<Facet> facets = edmType.Facets;

            switch (primitiveType.PrimitiveTypeKind)
            {
                case PrimitiveTypeKind.Binary:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["binary"]);
                    }
                case PrimitiveTypeKind.Boolean:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["boolean"]);
                    }
                case PrimitiveTypeKind.Byte:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["int"]);
                    }
                case PrimitiveTypeKind.DateTime:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["datetime"]);
                    }
                case PrimitiveTypeKind.Decimal:
                    {
                        return TypeUsage.CreateDecimalTypeUsage(base.StoreTypeNameToStorePrimitiveType["decimal"], 18, 0);
                    }
                case PrimitiveTypeKind.Double:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["float"]);
                    }
                case PrimitiveTypeKind.Single:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["double"]);

                    }
                case PrimitiveTypeKind.Int16:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["int"]);

                    }
                case PrimitiveTypeKind.Int32:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["int"]);

                    }
                case PrimitiveTypeKind.Int64:
                    {
                        return TypeUsage.CreateDefaultTypeUsage(base.StoreTypeNameToStorePrimitiveType["long"]);

                    }
                case PrimitiveTypeKind.String:
                    {
                        return TypeUsage.CreateStringTypeUsage(base.StoreTypeNameToStorePrimitiveType["string"], true, false);
                    }
                default:
                    {
                        throw new NotSupportedException(string.Format("There is no store type corresponding to the EDM type '{0}' of primitive type '{1}'.", edmType, primitiveType.PrimitiveTypeKind));
                    }
            }
        }

    }
}
