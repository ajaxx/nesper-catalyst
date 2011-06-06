using System;
using System.Xml.Linq;
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

                return element;
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
