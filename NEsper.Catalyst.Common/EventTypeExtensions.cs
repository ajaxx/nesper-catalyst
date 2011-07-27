using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.util;

namespace NEsper.Catalyst.Common
{
    public static class EventTypeExtensions
    {
        public static T RequireAttribute<T>(XElement element, string attributeName, Func<string, T> select)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute == null)
            {
                throw new ArgumentException("invalid element, missing required attribute: " + attributeName, "element");
            }

            return select.Invoke(attribute.Value);
        }

        public static string RequireAttribute(XElement element, string attributeName)
        {
            return RequireAttribute(element, attributeName, s => s);
        }

        public static EventPropertyDescriptor ToEventPropertyDescriptor(this XElement element)
        {
            var attrName           = RequireAttribute(element, "name");
            var attrIsFragment     = RequireAttribute(element, "isFragment", Boolean.Parse);
            var attrIsIndexed      = RequireAttribute(element, "isIndexed", Boolean.Parse);
            var attrIsMapped       = RequireAttribute(element, "isMapped", Boolean.Parse);
            var attrRequiresIndex  = RequireAttribute(element, "requiresIndex", Boolean.Parse);
            var attrRequiresMapKey = RequireAttribute(element, "requiresMapKey", Boolean.Parse);

            Type propertyType = null;
            Type propertyElementType = null;

            var attrPropertyType = element.Attribute("propertyType");
            if (attrPropertyType != null)
            {
                propertyType = TypeHelper.ResolveType(attrPropertyType.Value, false);
            }

            var attrPropertyComponentType = element.Attribute("propertyComponentType");
            if (attrPropertyComponentType != null)
            {
                propertyElementType = TypeHelper.ResolveType(attrPropertyComponentType.Value, false);
            }

            return new EventPropertyDescriptor(
                attrName,
                propertyType,
                propertyElementType,
                attrRequiresIndex,
                attrRequiresMapKey,
                attrIsIndexed,
                attrIsMapped,
                attrIsFragment);
        }

        /// <summary>
        /// Converts the property descriptor to an xelement.
        /// </summary>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns></returns>
        public static XElement ToXElement(this EventPropertyDescriptor propertyDescriptor)
        {
            var propertyType = propertyDescriptor.PropertyType;
            var propertyComponentType = propertyDescriptor.PropertyComponentType;

            var element = new XElement(
                "propertyDescriptor",
                new XAttribute("name",           propertyDescriptor.PropertyName),
                new XAttribute("isFragment",     propertyDescriptor.IsFragment),
                new XAttribute("isIndexed",      propertyDescriptor.IsIndexed),
                new XAttribute("isMapped",       propertyDescriptor.IsMapped),
                new XAttribute("requiresIndex",  propertyDescriptor.RequiresIndex),
                new XAttribute("requiresMapKey", propertyDescriptor.RequiresMapKey)
                );

            if (propertyType != null)
            {
                element.Add(ToXElement(propertyType, "propertyType"));
            }

            if (propertyComponentType != null)
            {
                element.Add(ToXElement(propertyComponentType, "propertyComponentType"));
            }

            return element;
        }

        private static XElement ToXElement(this Type type, string name)
        {
            var nillable = false;
                
            if (type.IsNullable())
            {
                nillable = true;
                type = Nullable.GetUnderlyingType(type);
            }

            return new XElement(name,
                                new XAttribute("type", type.FullName),
                                new XAttribute("nillable", nillable));
        }

        /// <summary>
        /// Converts the event tyep into an xelement.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <returns></returns>
        public static XElement ToXElement(this EventType eventType)
        {
            var eventTypeName = new XAttribute("name", eventType.Name);
            var propertyDescriptors = eventType.PropertyDescriptors.Select(ToXElement);

            var itemList = new List<XObject>();
            itemList.Add(eventTypeName);
            itemList.AddRange(propertyDescriptors);

            return new XElement("eventType", itemList.ToArray());
        }
    }
}
