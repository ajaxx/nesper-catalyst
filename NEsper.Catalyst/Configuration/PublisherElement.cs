using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace NEsper.Catalyst.Configuration
{
    public class PublisherElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        /// <summary>
        /// Gets or sets the publisher configuration.
        /// </summary>
        /// <value>The publisher configuration.</value>
        public XElement PublisherConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public IDictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherElement"/> class.
        /// </summary>
        public PublisherElement()
        {
            Attributes = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets a value indicating whether an unknown element is encountered during deserialization.
        /// </summary>
        /// <param name="elementName">The name of the unknown subelement.</param>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> being used for deserialization.</param>
        /// <returns>
        /// true when an unknown element is encountered while deserializing; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.Configuration.ConfigurationErrorsException">The element identified by <paramref name="elementName"/> is locked.- or -One or more of the element's attributes is locked.- or -<paramref name="elementName"/> is unrecognized, or the element has an unrecognized attribute.- or -The element has a Boolean attribute with an invalid value.- or -An attempt was made to deserialize a property more than once.- or -An attempt was made to deserialize a property that is not a valid member of the element.- or -The element cannot contain a CDATA or text element.</exception>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            PublisherConfiguration = XElement.Parse(reader.ReadOuterXml(), LoadOptions.None);
            return true;
        }

        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// </summary>
        /// <param name="name">The name of the unrecognized attribute.</param>
        /// <param name="value">The value of the unrecognized attribute.</param>
        /// <returns>
        /// true when an unknown attribute is encountered while deserializing; otherwise, false.
        /// </returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            Attributes[name] = value;
            return true;
        }
    }
}
