///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Runtime.Serialization;

namespace NEsper.Catalyst.Common
{
    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "JsonEvent")]
    public class JsonEvent
    {
        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        /// <value>The type of the event.</value>
        [DataMember]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the event data.
        /// </summary>
        /// <value>The event data.</value>
        [DataMember]
        public string EventData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEvent"/> class.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="eventData">The event data.</param>
        public JsonEvent(string eventType, string eventData)
        {
            EventType = eventType;
            EventData = eventData;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEvent"/> class.
        /// </summary>
        public JsonEvent()
        {
        }
    }
}
