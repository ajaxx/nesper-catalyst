///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Linq;
using System.Xml.Linq;

namespace NEsper.Catalyst.Common
{
    using com.espertech.esper.client;
    using com.espertech.esper.client.util;
    using com.espertech.esper.events.util;

    public static class EventExtensions
    {
        /// <summary>
        /// Converts the event bean into a contract event.
        /// </summary>
        /// <param name="eventBean">The event bean.</param>
        /// <returns></returns>
        public static XElement ToXElement(this EventBean eventBean)
        {
            if (eventBean.Underlying is XElement) {
                return (XElement) eventBean.Underlying;
            }

            var renderingOptions = new XMLRenderingOptions();
            renderingOptions.IsDefaultAsAttribute = true;
            renderingOptions.PreventLooping = true;

            var elementRendererImpl = new XElementRendererImpl(eventBean.EventType, renderingOptions);

            return elementRendererImpl.Render(
                "eventBean", eventBean);
        }

        /// <summary>
        /// Converts the array of even beans into an XElement.
        /// </summary>
        /// <param name="eventBeans">The event beans.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static XElement ToXElement(this EventBean[] eventBeans, string elementName)
        {
            return new XElement(
                elementName,
                eventBeans.Select(ToXElement).Cast<object>().ToArray());
        }

        /// <summary>
        /// Converts the update events args into an XElement.
        /// </summary>
        /// <param name="updateEventArgs">The <see cref="com.espertech.esper.client.UpdateEventArgs"/> instance containing the event data.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public static XElement ToXElement(this UpdateEventArgs updateEventArgs, string elementName)
        {
            return new XElement(
                elementName,
                new XElement("statement", updateEventArgs.Statement.Name),
                ToXElement(updateEventArgs.NewEvents, "new"),
                ToXElement(updateEventArgs.NewEvents, "old"));
        }

        /// <summary>
        /// Converts the update events args into an XElement.
        /// </summary>
        /// <param name="updateEventArgs">The <see cref="com.espertech.esper.client.UpdateEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public static XElement ToXElement(this UpdateEventArgs updateEventArgs)
        {
            return ToXElement(updateEventArgs, "update");
        }
    }
}
