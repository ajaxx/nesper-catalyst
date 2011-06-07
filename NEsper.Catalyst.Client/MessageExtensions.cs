///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Linq;
using System.Xml.Linq;

using com.espertech.esper.client;
using com.espertech.esper.events.xml;

namespace NEsper.Catalyst.Client
{
    public static class MessageExtensions
    {
        public static UpdateEventArgs ToUpdateEventArgs(this string elementXml)
        {
            // process the message
            var eventElement = XElement.Parse(elementXml);
            // find the xelements that represent the old and new event beans
            // and convert them back into an eventBean form
            var oldEvents = eventElement
                .Element("old")
                .Elements()
                .Select(element => new XEventBean(element, null))
                .ToArray();
            var newEvents = eventElement
                .Element("new")
                .Elements()
                .Select(element => new XEventBean(element, null))
                .ToArray();

            // construct event args
            var updateEventArgs = new UpdateEventArgs(
                null,
                null,
                newEvents,
                oldEvents);

            // send the event(s) along
            return updateEventArgs;
        }
    }
}
