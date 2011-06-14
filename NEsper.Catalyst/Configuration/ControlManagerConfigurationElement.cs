using System;
using System.Configuration;

namespace NEsper.Catalyst.Configuration
{
    public class ControlManagerConfigurationElement 
        : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        [ConfigurationProperty("uri", IsDefaultCollection = false)]
        public Uri Uri
        {
            get { return (Uri) this["uri"]; }
            set { this["uri"] = value; }
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
            if ( name == "uri" )
            {
                Uri = new Uri(value);
                return true;
            }

            return base.OnDeserializeUnrecognizedAttribute(name, value);
        }
    }
}
