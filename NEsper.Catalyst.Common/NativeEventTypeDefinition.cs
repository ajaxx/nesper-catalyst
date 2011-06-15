///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace NEsper.Catalyst.Common
{
    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "NativeEventTypeDefinition")]
    public class NativeEventTypeDefinition
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

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
        /// <param name="name">The name.</param>
        /// <param name="schemaTypeName">Name of the schema type.</param>
        /// <param name="schemaSet">The schema set.</param>
        public NativeEventTypeDefinition(string name, XmlQualifiedName schemaTypeName, XmlSchemaSet schemaSet)
        {
            Name = name;
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
        /// <param name="name">The name.</param>
        /// <param name="schemaTypeName">Name of the schema type.</param>
        /// <param name="schemas">The schemas.</param>
        public NativeEventTypeDefinition(string name, XmlQualifiedName schemaTypeName, string[] schemas)
        {
            Name = name;
            SchemaTypeName = schemaTypeName;
            Schemas = schemas;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeEventTypeDefinition"/> class.
        /// </summary>
        public NativeEventTypeDefinition()
        {
        }
    }
}
