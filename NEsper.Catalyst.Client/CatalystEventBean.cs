///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    class CatalystEventBean
        : EventBean
    {
        private readonly XElement _event;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="event">is the node with event property information</param>
        /// <param name="type">is the event type for this event wrapper</param>

        public CatalystEventBean(XElement @event, EventType type)
        {
            _event = @event;
            EventType = type;
        }

        /// <summary>
        /// Return the <see cref="EventType" /> instance that describes the set of properties available for this event.
        /// </summary>
        /// <value></value>
        /// <returns> event type
        /// </returns>
        public EventType EventType { get; private set; }

        /// <summary>
        /// Get the underlying data object to this event wrapper.
        /// </summary>
        /// <value></value>
        /// <returns> underlying data object, usually either a Map or a bean instance.
        /// </returns>
        public Object Underlying
        {
            get { return _event; }
        }

        /// <summary>
        /// Returns the value of an event property.
        /// </summary>
        /// <value></value>
        /// <returns> the value of a simple property with the specified name.
        /// </returns>
        /// <throws>  PropertyAccessException - if there is no property of the specified name, or the property cannot be accessed </throws>
        public Object this[String property]
        {
            get
            {
                XElement element = _event.XPathSelectElement(property);
                if (element == null)
                {
                    throw new PropertyAccessException("Property named '" + property + "' is not a valid property name for this type");
                }

                XAttribute typeAttribute = element.Attribute("type");
                if (typeAttribute == null)
                {
                    return element.Value; // aggregate type, you get to do the hard work
                }

                XmlTypeCode typeCode;
                if (!Enum.TryParse(typeAttribute.Value, true, out typeCode))
                {
                    return element.Value;
                }
                
                switch(typeCode)
                {
                    case XmlTypeCode.String:
                        return Convert.ToString(element.Value);
                    case XmlTypeCode.Integer:
                        return Convert.ToInt32(element.Value);
                    case XmlTypeCode.Long:
                        return Convert.ToInt64(element.Value);
                    case XmlTypeCode.Short:
                        return Convert.ToInt16(element.Value);
                    case XmlTypeCode.DateTime:
                        return Convert.ToDateTime(element.Value);
                    case XmlTypeCode.Float:
                        return Convert.ToSingle(element.Value);
                    case XmlTypeCode.Double:
                        return Convert.ToDouble(element.Value);
                    case XmlTypeCode.Decimal:
                        return Convert.ToDecimal(element.Value);
                    case XmlTypeCode.Boolean:
                        return Convert.ToBoolean(element.Value);
                }

                return element.Value;
            }
        }

        /// <summary>
        /// Returns the value of an event property.  This method is a proxy of the indexer.
        /// </summary>
        /// <param name="property">name of the property whose value is to be retrieved</param>
        /// <returns>
        /// the value of a simple property with the specified name.
        /// </returns>
        /// <throws>  PropertyAccessException - if there is no property of the specified name, or the property cannot be accessed </throws>
        public Object Get(String property)
        {
            return this[property];
        }

        public Object GetFragment(String propertyExpression)
        {
            XElement element = _event.XPathSelectElement(propertyExpression);
            if (element == null)
            {
                throw new PropertyAccessException(
                    "Property named '" + propertyExpression + "' is not a valid property name for this type");
            }
            return element;
        }
    }
}
