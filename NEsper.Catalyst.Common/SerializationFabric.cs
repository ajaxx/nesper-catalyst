using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using com.espertech.esper.compat;
using com.espertech.esper.util;

namespace NEsper.Catalyst.Common
{
    public class SerializationFabric
    {
        private readonly static IDictionary<Type, DataContractJsonSerializer> SerializerTable =
            new Dictionary<Type, DataContractJsonSerializer>();

        /// <summary>
        /// Gets the serializer associated with the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static DataContractJsonSerializer GetSerializer(Type type)
        {
            lock (SerializerTable)
            {
                var serializer = SerializerTable.Get(type);
                if (serializer != null)
                {
                    return serializer;
                }

                if (type.IsSerializable)
                {
                    serializer = new DataContractJsonSerializer(type);
                    SerializerTable[type] = serializer;
                    return serializer;
                }

                // look for a data contract in the hierarchy of types
                var dataContractType = type.FindAttributeInTypeTree(typeof(DataContractAttribute));
                if (dataContractType != null)
                {
                    serializer = new DataContractJsonSerializer(dataContractType);
                    SerializerTable[type] = serializer;
                    return serializer;
                }

                throw new ArgumentException("type is not annotated as DataContract and is not Serializable");
            }
        }

        /// <summary>
        /// Serializes an object ... at least to the degree that it can.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var dictionaryWriter = JsonReaderWriterFactory.CreateJsonWriter(memoryStream, Encoding.UTF8, false))
                {
                    var serializer = GetSerializer(obj.GetType());
                    serializer.WriteObject(dictionaryWriter, obj);
                    dictionaryWriter.Close();
                    return Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Deserializes an object of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="objectData">The obj data.</param>
        /// <returns></returns>
        public static Object Deserialize(Type type, string objectData)
        {
            var bytes = Encoding.UTF8.GetBytes(objectData);
            var dictionaryReader = JsonReaderWriterFactory.CreateJsonReader(
                    bytes, 0, bytes.Length, new XmlDictionaryReaderQuotas());
            var serializer = GetSerializer(type);
            return serializer.ReadObject(dictionaryReader);
        }
    }
}
