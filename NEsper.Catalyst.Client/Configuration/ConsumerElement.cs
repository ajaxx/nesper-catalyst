///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Configuration;
using System.Xml.Linq;
using com.espertech.esper.util;

namespace NEsper.Catalyst.Client.Configuration
{
    public class ConsumerElement : ConfigurationElement
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
        /// Gets or sets the consumer configuration.
        /// </summary>
        public XElement ConsumerConfiguration { get; set; }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <returns></returns>
        public IEventConsumerFactory GetFactory()
        {
            var factoryType =
                TypeHelper.ResolveType(Type, false) ??
                TypeHelper.ResolveType("NEsper.Catalyst.Client.Consumers." + Type, false);
            if (!factoryType.IsImplementsInterface(typeof(IEventConsumerFactory)))
            {
                throw new ConfigurationErrorsException(
                    string.Format("specified type \"{0}\" does not implement \"{1}\"", factoryType,
                                  typeof (IEventConsumerFactory)));
            }

            var eventConsumerFactory = (IEventConsumerFactory) Activator.CreateInstance(factoryType);
            eventConsumerFactory.Initialize(ConsumerConfiguration);
            return eventConsumerFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerElement"/> class.
        /// </summary>
        public ConsumerElement()
        {
            ConsumerConfiguration = null;
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
        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            ConsumerConfiguration = XNode.ReadFrom(reader) as XElement;
            return true;
        }
    }
}
