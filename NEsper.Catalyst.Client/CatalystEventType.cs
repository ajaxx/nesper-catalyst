///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using com.espertech.esper.client;

using NEsper.Catalyst.Common;

namespace NEsper.Catalyst.Client
{
    class CatalystEventType
        : EventType
    {
        private readonly XElement _eventTypeData;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="eventTypeData">The event type data.</param>

        public CatalystEventType(XElement eventTypeData)
        {
            _eventTypeData = eventTypeData;

            Name = EventTypeExtensions.RequireAttribute(_eventTypeData, "name");
            PropertyDescriptors = _eventTypeData
                .Elements("propertyDescriptor")
                .Select(element => element.ToEventPropertyDescriptor())
                .ToList();

            PropertyNames = PropertyDescriptors
                .Select(propertyDescriptor => propertyDescriptor.PropertyName)
                .ToList();
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the underlying.
        /// </summary>
        /// <value>The type of the underlying.</value>
        public Type UnderlyingType
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets the property names.
        /// </summary>
        /// <value>The property names.</value>
        public ICollection<string> PropertyNames { get; private set; }

        /// <summary>
        /// Gets or sets the property descriptors.
        /// </summary>
        /// <value>The property descriptors.</value>
        public IList<EventPropertyDescriptor> PropertyDescriptors { get; private set; }

        /// <summary>
        /// Gets or sets the super types.
        /// </summary>
        /// <value>The super types.</value>
        public EventType[] SuperTypes
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets the deep super types.
        /// </summary>
        /// <value>The deep super types.</value>
        public EventType[] DeepSuperTypes
        {
            get { return null; }
        }

        /// <summary>
        /// Determines whether the specified property expression is property.
        /// </summary>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>
        /// 	<c>true</c> if the specified property expression is property; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProperty(string propertyExpression)
        {
            var propertyType = GetPropertyType(propertyExpression);
            return propertyType != null;
        }

        public Type GetPropertyType(string propertyExpression)
        {
            var propertyDesc = PropertyDescriptors.FirstOrDefault(
                propertyDescriptor => propertyDescriptor.PropertyName == propertyExpression);
            return propertyDesc != null ? propertyDesc.PropertyType : null;
        }

        public EventPropertyGetter GetGetter(string propertyExpression)
        {
            throw new NotSupportedException();
        }

        public FragmentEventType GetFragmentType(string propertyExpression)
        {
            throw new NotSupportedException();
        }

        public EventPropertyDescriptor GetPropertyDescriptor(string propertyName)
        {
            return PropertyDescriptors.FirstOrDefault(
                propertyDescriptor => propertyDescriptor.PropertyName == propertyName);
        }
    }
}
