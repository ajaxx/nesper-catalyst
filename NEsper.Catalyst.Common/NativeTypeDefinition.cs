using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace NEsper.Catalyst.Common
{
    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "NativeTypeDefinition")]
    public class NativeTypeDefinition
    {
        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>The name of the element.</value>
        [DataMember]
        public XmlQualifiedName SchemaTypeName { get; set; }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>The schema.</value>
        [DataMember]
        public string[] Schemas { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeEventTypeDefinition"/> class.
        /// </summary>
        /// <param name="schemaTypeName">Name of the schema type.</param>
        /// <param name="schemaSet">The schema set.</param>
        public NativeTypeDefinition(XmlQualifiedName schemaTypeName, XmlSchemaSet schemaSet)
        {
            SchemaTypeName = schemaTypeName;

            var schemaList = new List<string>();
            foreach(XmlSchema schema in schemaSet.Schemas())
            {
                var stringWriter = new StringWriter();
                schema.Write(stringWriter);
                schemaList.Add(stringWriter.ToString());
            }

            Schemas = schemaList.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeEventTypeDefinition"/> class.
        /// </summary>
        /// <param name="schemaTypeName">Name of the schema type.</param>
        /// <param name="schemas">The schemas.</param>
        public NativeTypeDefinition(XmlQualifiedName schemaTypeName, string[] schemas)
        {
            SchemaTypeName = schemaTypeName;
            Schemas = schemas;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeEventTypeDefinition"/> class.
        /// </summary>
        public NativeTypeDefinition()
        {
        }
    }

}
