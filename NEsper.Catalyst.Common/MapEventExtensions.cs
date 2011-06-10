using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using com.espertech.esper.util;

namespace NEsper.Catalyst.Common
{
    public static class MapEventExtensions
    {
        /// <summary>
        /// Converts the dictionary into an xml element tree.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public static XElement ToXElement(this IDictionary<string, object> dictionary)
        {
            return new XElement(
                "dictionary",
                dictionary.Select(ToXElement));
        }

        /// <summary>
        /// Converts the entry into an xelement.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public static XElement ToXElement(this KeyValuePair<string, object> entry)
        {
            var element = new XElement("entry");
            element.SetAttributeValue("key", entry.Key);
            if (entry.Value is IDictionary<string, object>) {
                var asDictionary = (IDictionary<string, object>) entry.Value;
                element.AddFirst(ToXElement(asDictionary));
            } else if (entry.Value != null) {
                element.SetAttributeValue("type", entry.Value.GetType().FullName);
                element.SetValue(entry.Value);
            }
            return element;
        }

        /// <summary>
        /// Converts the element into a dictionary.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static IDictionary<string, object> ToDictionary(this XElement element)
        {
            if (element == null) {
                return null;
            }

            if (element.Name != XName.Get("dictionary")) {
                return null;
            }

            var dictionary = new Dictionary<string, object>();
            foreach(var entry in element.Elements("entry").Select(ToKeyValuePair)) {
                dictionary[entry.Key] = entry.Value;
            }

            return dictionary;
        }

        /// <summary>
        /// Converts the element into a key value pair.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static KeyValuePair<string, object> ToKeyValuePair(this XElement element)
        {
            var key = element.Attribute("key");
            if (key == null) {
                throw new ArgumentException("element missing required attribute \"key\"");
            }

            var type = element.Attribute("type");
            if (type != null) {
                var trueType = TypeHelper.ResolveType(type.Value);
                var value = Convert.ChangeType(element.Value, trueType);
                return new KeyValuePair<string, object>(key.Value, value);
            }

            var dictionary = ToDictionary(element.Element("dictionary"));
            return new KeyValuePair<string, object>(key.Value, dictionary);
        }
    }
}
