using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace NEsper.Catalyst.Common
{
    public class DataContractExporter
    {
        private const string XML_SCHEMA_NAMESPACE = "http://www.w3.org/2001/XMLSchema";

        private static readonly IDictionary<Type, XName> PrimitiveTypeMap;

        /// <summary>
        /// Initializes the <see cref="DataContractExporter"/> class.
        /// </summary>
        static DataContractExporter()
        {
            PrimitiveTypeMap = new Dictionary<Type, XName>();
            PrimitiveTypeMap[typeof(DateTime)] = XName.Get("dateTime", XML_SCHEMA_NAMESPACE);
            PrimitiveTypeMap[typeof(string)] = XName.Get("string", XML_SCHEMA_NAMESPACE);
            PrimitiveTypeMap[typeof(bool)] = XName.Get("boolean", XML_SCHEMA_NAMESPACE);
            PrimitiveTypeMap[typeof(float)] = XName.Get("float", XML_SCHEMA_NAMESPACE);
            PrimitiveTypeMap[typeof(double)] = XName.Get("double", XML_SCHEMA_NAMESPACE);
            PrimitiveTypeMap[typeof(decimal)] = XName.Get("decimal", XML_SCHEMA_NAMESPACE);
            PrimitiveTypeMap[typeof(short)] = XName.Get("short", XML_SCHEMA_NAMESPACE);
            PrimitiveTypeMap[typeof(int)] = XName.Get("integer", XML_SCHEMA_NAMESPACE);
            PrimitiveTypeMap[typeof(long)] = XName.Get("long", XML_SCHEMA_NAMESPACE);
        }

        private readonly XDocument _schemaDocument;
        private readonly XElement _schemaRootElement;

        private IDictionary<Type, XName> _schemaTypeTable =
            new Dictionary<Type, XName>();
        private IDictionary<Type, XElement> _schemaComplexTypes =
            new Dictionary<Type, XElement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractExporter"/> class.
        /// </summary>
        public DataContractExporter()
        {
            _schemaDocument = new XDocument();

            _schemaRootElement = new XElement(
                XName.Get("schema", XML_SCHEMA_NAMESPACE),
                new XAttribute(XNamespace.Xmlns + "xs", XML_SCHEMA_NAMESPACE));

            _schemaRootElement.SetAttributeValue("elementFormDefault", "qualified");
            var defaultnamespace = _schemaRootElement.GetDefaultNamespace();
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private XName GetTypeName(Type type)
        {
            XName name;

            // check the primitive table
            if (PrimitiveTypeMap.TryGetValue(type, out name))
            {
                return name;
            }

            // check types that we have mapped
            if (_schemaTypeTable.TryGetValue(type, out name))
            {
                return name;
            }

            // get the namespace for the type
            var dataContractAttribute = type
                .GetCustomAttributes(typeof (DataContractAttribute), false)
                .Cast<DataContractAttribute>()
                .FirstOrDefault();

            // define a new type and enter it into the type table
            name = XName.Get(
                type.FullName,
                dataContractAttribute.Namespace);
            _schemaTypeTable[type] = name;

            GetTypeElement(type);

            return name;
        }

        /// <summary>
        /// Produces a schema type from the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private XElement GetTypeSchema(Type type)
        {
            XElement complexType;

            if (!_schemaComplexTypes.TryGetValue(type, out complexType))
            {
                var sequence = new XElement(
                    XName.Get("sequence", XML_SCHEMA_NAMESPACE));
                complexType = new XElement(
                    XName.Get("complexType", XML_SCHEMA_NAMESPACE),
                    sequence);
                complexType.SetAttributeValue(
                    XName.Get("name", XML_SCHEMA_NAMESPACE), type.FullName);

                _schemaComplexTypes[type] = complexType;

                foreach (var property in type.GetProperties())
                {
                    var dataMemberAttributes = property.GetCustomAttributes(typeof (DataMemberAttribute), false);
                    if (dataMemberAttributes.Length > 0)
                    {
                        var dataMemberType = property.PropertyType;
                        var dataMemberTypeName = GetTypeName(dataMemberType);
                        // define an element for this item
                        var propertyElement = new XElement(
                            XName.Get("element", XML_SCHEMA_NAMESPACE));
                        propertyElement.SetAttributeValue("minOccurs", 0);
                        propertyElement.SetAttributeValue("name", property.Name);
                        propertyElement.SetAttributeValue("nillable", true);
                        propertyElement.SetAttributeValue("type", dataMemberTypeName);
                        sequence.Add(propertyElement);
                    }
                }

                _schemaRootElement.Add(
                    complexType);
            }

            return complexType;
        }

        private IDictionary<Type, XElement> _schemaComplexElements =
            new Dictionary<Type, XElement>();

        private XElement GetTypeElement(Type type)
        {
            XElement complexElement;

            if (!_schemaComplexTypes.TryGetValue(type, out complexElement))
            {
                XElement schemaElement = GetTypeSchema(type);
                XAttribute schemaElementName = schemaElement.Attribute(
                    XName.Get("name", XML_SCHEMA_NAMESPACE));

                complexElement = new XElement(
                    XName.Get("element", XML_SCHEMA_NAMESPACE));
                complexElement.SetAttributeValue("name", type.FullName);
                complexElement.SetAttributeValue("nillable", true);
                complexElement.SetAttributeValue("type", schemaElementName.Name);

                _schemaRootElement.Add(complexElement);
            }

            return complexElement;
        }

        /// <summary>
        /// Exports the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void Export(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof (DataContractAttribute), false);
            if (attributes.Length == 0)
            {
                throw new InvalidDataContractException(
                    string.Format("type {0} missing DataContractAttribute", type));
            }

            GetTypeElement(type);
        }

        /// <summary>
        /// Exports this type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Export<T>()
        {
            Export(typeof (T));
        }
    }
}
