using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace NEsper.Catalyst.Common
{
    public class SchemaFabricator
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _moduleBuilder;
        private readonly IDictionary<XmlQualifiedName, Type> _typeTable;

        /// <summary>
        /// Gets or sets the default instance.
        /// </summary>
        /// <value>The default instance.</value>
        public static SchemaFabricator DefaultInstance { get; private set; }

        /// <summary>
        /// Initializes the <see cref="SchemaFabricator"/> class.
        /// </summary>
        static SchemaFabricator()
        {
            var assemblyName = new AssemblyName("__Fabrication");
            DefaultInstance = new SchemaFabricator(assemblyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaFabricator"/> class.
        /// </summary>
        public SchemaFabricator(AssemblyName assemblyName)
        {
            var appDomain = Thread.GetDomain();

            _assemblyBuilder = appDomain.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.RunAndSave);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(
                assemblyName.Name,
                string.Format("{0}.dll", assemblyName.Name),
                true);
            _typeTable = new Dictionary<XmlQualifiedName, Type>();
        }

        /// <summary>
        /// Gets a fabricated type.
        /// </summary>
        /// <param name="fullTypeName">Full name of the type.</param>
        /// <returns></returns>
        public Type GetType(string fullTypeName)
        {
            return _assemblyBuilder.GetType(fullTypeName, false, true);
        }

        /// <summary>
        /// Gets a unique signature for a complex type.
        /// </summary>
        /// <param name="complexType">Type of the complex.</param>
        /// <returns></returns>
        private string GetTypeSignature(XmlSchemaComplexType complexType)
        {
            var name = complexType.QualifiedName;
            var particle = complexType.Particle as XmlSchemaSequence;
            Debug.Assert(particle != null);

            var fields = particle.Items
                .OfType<XmlSchemaElement>()
                .Select(GetNativeElement);

            var @namespace = name.Namespace;
            @namespace = @namespace.Substring(@namespace.LastIndexOf('/') + 1);

            var sha256 = SHA256.Create();
            var memoryStream = new MemoryStream();
            var cryptoStream = new CryptoStream(
                memoryStream, sha256, CryptoStreamMode.Write);
            var cryptoWriter = new StreamWriter(cryptoStream, Encoding.Unicode);

            cryptoWriter.Write(string.Format("{0}.{1}", @namespace, name.Name));

            foreach (var field in fields)
            {
                cryptoWriter.Write(field.Name);
                cryptoWriter.Write(field.Type.FullName);
            }

            cryptoWriter.Flush();
            cryptoStream.FlushFinalBlock();

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        /// Builds a native type from a complex type.
        /// </summary>
        /// <param name="complexType">Type of the complex.</param>
        /// <param name="signature">The signature.</param>
        /// <returns></returns>
        private Type BuildNativeType(XmlSchemaComplexType complexType, string signature)
        {
            var name = complexType.QualifiedName;
            var particle = complexType.Particle as XmlSchemaSequence;
            Debug.Assert(particle != null);

            var fields = particle.Items
                .OfType<XmlSchemaElement>()
                .Select(GetNativeElement)
                .ToArray();

            // special case for generic arrays
            if (fields.Length == 1)
            {
                var singleField = fields[0];
                if (singleField.Type.IsGenericType &&
                    singleField.Type.GetGenericTypeDefinition() == typeof(IList<>) &&
                    singleField.TypeReduced)
                {
                    return singleField.Type;
                }
            }

            var @namespace = name.Namespace;
            @namespace = @namespace.Substring(@namespace.LastIndexOf('/') + 1);

            var typename = string.Format("{0}.{1}", @namespace, name.Name);
            var typeBuilder = _moduleBuilder.DefineType(
                typename,
                TypeAttributes.Public | TypeAttributes.Serializable | TypeAttributes.Sealed,
                typeof(object));

            var dataContractAttributeType = typeof(DataContractAttribute);
            var dataContractAttributeConstructor = dataContractAttributeType.GetConstructor(Type.EmptyTypes);
            var dataContractCustomAttributeBuilder = new CustomAttributeBuilder(
                dataContractAttributeConstructor, new object[0]);

            typeBuilder.SetCustomAttribute(dataContractCustomAttributeBuilder);

            var dataMemberAttributeType = typeof(DataMemberAttribute);
            var dataMemberAttributeConstructor = dataMemberAttributeType.GetConstructor(Type.EmptyTypes);

            var signatureBuilder = typeBuilder.DefineField(
                "__signature",
                typeof (string),
                FieldAttributes.Public | FieldAttributes.Static | FieldAttributes.Literal);
            signatureBuilder.SetConstant(signature);

            foreach (var field in fields)
            {
                var propName = field.Name;
                var propType = field.Type;

                var fieldName = string.Format("_mField{0}", propName);
                
                var fieldBuilder = typeBuilder.DefineField(
                    fieldName,
                    propType,
                    FieldAttributes.Private);

                var propertyBuilder = typeBuilder.DefineProperty(
                    propName,
                    PropertyAttributes.HasDefault,
                    propType,
                    null);

                propertyBuilder.SetCustomAttribute(
                    new CustomAttributeBuilder(dataMemberAttributeConstructor, Type.EmptyTypes));

                var getMethodBuilder = typeBuilder.DefineMethod(
                    string.Format("get_{0}", propName),
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    propType,
                    Type.EmptyTypes);

                var getMethodIL = getMethodBuilder.GetILGenerator();
                getMethodIL.Emit(OpCodes.Ldarg_0);
                getMethodIL.Emit(OpCodes.Ldfld, fieldBuilder);
                getMethodIL.Emit(OpCodes.Ret);

                var setMethodBuilder = typeBuilder.DefineMethod(
                    string.Format("set_{0}", propName),
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    typeof(void),
                    new[] { propType });

                var setMethodIL = setMethodBuilder.GetILGenerator();
                setMethodIL.Emit(OpCodes.Ldarg_0);
                setMethodIL.Emit(OpCodes.Ldarg_1);
                setMethodIL.Emit(OpCodes.Stfld, fieldBuilder);
                setMethodIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getMethodBuilder);
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Gets the native type associated with this complex type.
        /// </summary>
        /// <param name="complexType">Type of the complex.</param>
        /// <returns></returns>
        private Type GetNativeType(XmlSchemaComplexType complexType)
        {
            Type nativeType;

            var name = complexType.QualifiedName;
            var signature = GetTypeSignature(complexType);
            if (!_typeTable.TryGetValue(name, out nativeType))
            {
                _typeTable[name] = nativeType = BuildNativeType(complexType, signature);
            }
            else
            {
                var signatureField = nativeType.GetField("__signature");
                if (signatureField == null)
                {
                    return nativeType;
                }

                var signatureValue = (string) signatureField.GetValue(null);
                if (signatureValue == signature)
                {
                    return nativeType;
                }

                throw new ArgumentException("incompatible type declared that overwrites previous type");
            }

            return nativeType;
        }

        /// <summary>
        /// Gets the native type associated with this simple type.
        /// </summary>
        /// <param name="schemaType">Type of the schema.</param>
        /// <returns></returns>
        private static Type GetNativeType(XmlSchemaSimpleType schemaType)
        {
            switch (schemaType.TypeCode)
            {
                case XmlTypeCode.String:
                case XmlTypeCode.NormalizedString:
                    return typeof(string);
                case XmlTypeCode.Int:
                    return typeof(int);
                case XmlTypeCode.Long:
                    return typeof(long);
                case XmlTypeCode.Double:
                    return typeof(double);
                case XmlTypeCode.Float:
                    return typeof(float);
                case XmlTypeCode.Decimal:
                    return typeof(decimal);
                case XmlTypeCode.Short:
                    return typeof(short);
                case XmlTypeCode.Boolean:
                    return typeof(bool);
                case XmlTypeCode.DateTime:
                    return typeof(DateTime);
            }

            throw new ArgumentException("unsupported schema type", "schemaType");
        }

        /// <summary>
        /// Gets the native type associated with this schema type.
        /// </summary>
        /// <param name="schemaType">Type of the schema.</param>
        /// <returns></returns>
        private Type GetNativeType(XmlSchemaType schemaType)
        {
            return schemaType is XmlSchemaComplexType
                       ? GetNativeType((XmlSchemaComplexType)schemaType)
                       : GetNativeType((XmlSchemaSimpleType)schemaType);
        }

        /// <summary>
        /// Gets a native element representation for the schema element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private Element GetNativeElement(XmlSchemaElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            var elementName = element.QualifiedName.Name;
            var elementType = GetNativeType(element.ElementSchemaType);
            var elementTypeReduced = false;

            if (element.MaxOccurs > 1)
            {
                elementType = typeof (IList<>).MakeGenericType(elementType);
                elementTypeReduced = true;
            }

            var elementPair = new Element(
                elementName,
                elementType,
                elementTypeReduced);

            return elementPair;
        }

        /// <summary>
        /// Gets the native element representation of the named element.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public Element GetNativeElement(XmlSchema schema, XmlQualifiedName elementName)
        {
            if ((elementName == null) ||
                (elementName == XmlQualifiedName.Empty))
            {
                throw new ArgumentException("invalid root element", "elementName");
            }

            var rootElement = (XmlSchemaElement)schema.Elements[elementName];
            var rootNative = GetNativeElement(rootElement);

            _assemblyBuilder.Save(
                string.Format("{0}.dll", _assemblyBuilder.GetName().Name));

            return rootNative;
        }
        
        /// <summary>
        /// Gets the native element representation of the named element.
        /// </summary>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public Element GetNativeElement(XmlSchemaSet schemaSet, XmlQualifiedName elementName )
        {
            if ((elementName == null) ||
                (elementName == XmlQualifiedName.Empty))
            {
                throw new ArgumentException("invalid root element", "elementName");
            }

            var rootElementNamespace = elementName.Namespace;
            var rootElementSchema = schemaSet.Schemas(rootElementNamespace).OfType<XmlSchema>().FirstOrDefault();

            return GetNativeElement(rootElementSchema, elementName);
        }
        
        /// <summary>
        /// Imports a schema.
        /// </summary>
        /// <param name="schema">The schema.</param>
        public void ImportSchema(XmlSchema schema)
        {
            foreach(XmlSchemaElement schemaElement in schema.Elements)
            {
                GetNativeElement(schemaElement);
            }
        }

        /// <summary>
        /// Imports a collection of schemas.
        /// </summary>
        /// <param name="schemaSet">The schema set.</param>
        public void ImportSchemas(XmlSchemaSet schemaSet)
        {
            foreach(XmlSchema schema in schemaSet.Schemas())
            {
                ImportSchema(schema);
            }
        }

        public class Element
        {
            /// <summary>
            /// Gets or sets the name of the element.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }
            /// <summary>
            /// Gets or sets the type associated with this element.
            /// </summary>
            /// <value>The type.</value>
            public Type Type { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether [type reduced].
            /// </summary>
            /// <value><c>true</c> if [type reduced]; otherwise, <c>false</c>.</value>
            public bool TypeReduced { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Element"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="type">The type.</param>
            /// <param name="typeReduced">if set to <c>true</c> [type reduced].</param>
            public Element(string name, Type type, bool typeReduced)
            {
                Name = name;
                Type = type;
                TypeReduced = typeReduced;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Element"/> class.
            /// </summary>
            public Element()
            {
            }
        }
    }
}
