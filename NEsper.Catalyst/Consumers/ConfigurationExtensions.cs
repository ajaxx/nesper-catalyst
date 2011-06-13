using System;
using System.Configuration;
using System.Xml.Linq;

namespace NEsper.Catalyst.Consumers
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the required attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string RequiredAttribute(this XElement element, string name)
        {
            var attribute = element.Attribute(name);
            if (attribute == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format("missing required attribute: \"{0}\"", name));
            }

            return attribute.Value;
        }

        /// <summary>
        /// Gets the optional attribute.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string OptionalAttribute(this XElement element, string name)
        {
            var attribute = element.Attribute(name);
            return (attribute != null) ? attribute.Value : null;
        }

        public static void OnOptionalAttribute(this XElement element, string name, Action<string> action)
        {
            var attribute = element.Attribute(name);
            if (attribute != null)
            {
                action.Invoke(attribute.Value);
            }
        }
    }
}
