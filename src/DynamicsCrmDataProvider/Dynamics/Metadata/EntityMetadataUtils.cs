using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace DynamicsCrmDataProvider.Dynamics
{
    public static class EntityMetadataUtils
    {
        /// <summary>Serialize metadata</summary>
        /// <param name="metaData">Metadata to serialize</param>
        /// <param name="formatting">Formatting, determines if indentation and line feeds are used in the file</param>
        public static string SerializeMetaData(this EntityMetadata metaData, Formatting formatting)
        {

            using (var stringWriter = new StringWriter())
            {

                var serializer = new DataContractSerializer(typeof(EntityMetadata), null, int.MaxValue, false, false, null, new KnownTypesResolver());
                var writer = new XmlTextWriter(stringWriter)
                    {
                        Formatting = formatting
                    };
                serializer.WriteObject(writer, metaData);

                writer.Close();

                return stringWriter.ToString();
            }

        }

        /// <summary>
        /// Deserialises the xml into the EntityMetadata object.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static EntityMetadata DeserializeMetaData(XmlReader reader)
        {
            var serializer = new DataContractSerializer(typeof(EntityMetadata), null, int.MaxValue, false, false, null, new KnownTypesResolver());
            var entity = (EntityMetadata)serializer.ReadObject(reader);
            return entity;
        }
    }
}