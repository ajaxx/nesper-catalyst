using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Xml;

using com.espertech.esper.client;
using com.espertech.esper.util;

namespace NEsper.Catalyst.Common
{
    public static class SchemaFabricatorExtensions
    {
        /// <summary>
        /// Types that are deemed "safe" to instantiate outside of the
        /// fabrication assembly.
        /// </summary>
        private static readonly ICollection<string> WhiteList =
            new HashSet<string>();

        /// <summary>
        /// Initializes the <see cref="SchemaFabricatorExtensions"/> class.
        /// </summary>
        static SchemaFabricatorExtensions()
        {
            WhiteList.Add(typeof(string).FullName);
            WhiteList.Add(typeof(short).FullName);
            WhiteList.Add(typeof(int).FullName);
            WhiteList.Add(typeof(long).FullName);
            WhiteList.Add(typeof(ushort).FullName);
            WhiteList.Add(typeof(uint).FullName);
            WhiteList.Add(typeof(ulong).FullName);
            WhiteList.Add(typeof(char).FullName);
            WhiteList.Add(typeof(float).FullName);
            WhiteList.Add(typeof(double).FullName);
            WhiteList.Add(typeof(decimal).FullName);
            WhiteList.Add(typeof(sbyte).FullName);
            WhiteList.Add(typeof(byte).FullName);
            WhiteList.Add(typeof(Guid).FullName);
            WhiteList.Add(typeof(DateTime).FullName);
        }

        /// <summary>
        /// Fabricates the entity.
        /// </summary>
        /// <param name="fabricator">The fabricator.</param>
        /// <param name="data">The data.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns></returns>
        public static object Fabricate(this SchemaFabricator fabricator, string data, string dataType)
        {
            return Fabricate(fabricator, data, dataType, true);
        }

        /// <summary>
        /// Fabricates the entity.
        /// </summary>
        /// <param name="fabricator">The fabricator.</param>
        /// <param name="data">The data.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="throwError">if set to <c>true</c> [throw error].</param>
        /// <returns></returns>
        public static object Fabricate(this SchemaFabricator fabricator, string data, string dataType, bool throwError)
        {
            var eventBytes = System.Text.Encoding.UTF8.GetBytes(data);
            var dictionaryReader = JsonReaderWriterFactory.CreateJsonReader(
                    eventBytes, 0, eventBytes.Length, new XmlDictionaryReaderQuotas());

            var fabricatorType = fabricator.GetType(dataType);
            if (fabricatorType == null)
            {
                // We should consider the possibility that form of type instantiation could
                // be used to instantiate types that the host doesn't want instantiated.  There
                // needs to be a way to blacklist or whitelist types.
                if (WhiteList.Contains(dataType))
                {
                    fabricatorType = TypeHelper.ResolveType(dataType, false);
                }
            }

            if (fabricatorType != null)
            {
                var serializer = new DataContractJsonSerializer(fabricatorType);
                var trueEntity = serializer.ReadObject(dictionaryReader);
                return trueEntity;
            }

            if (throwError)
            {
                throw new EPException(string.Format("Unable to fabricate object of type {0}", dataType));
            }

            return null;
        }
    }
}
