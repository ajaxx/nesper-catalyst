using System.Configuration;

namespace NEsper.Catalyst.Configuration
{
    public class PublisherCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherCollection"/> class.
        /// </summary>
        public PublisherCollection()
        {
            AddElementName = "publisher";
        }

        /// <summary>
        /// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new PublisherElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"/>.
        /// </returns>
        /// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement"/> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PublisherElement) element).Name;
        }
    }
}
