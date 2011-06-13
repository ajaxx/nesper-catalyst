using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;

using NEsper.Catalyst.Common;

namespace NEsper.Catalyst.Consumers
{
    public abstract class BaseEventConsumer
        : IEventConsumer
    {
        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri { get; protected set; }

        /// <summary>
        /// Event handler for xml events.
        /// </summary>
        public event Action<XElement> XmlEvent;

        /// <summary>
        /// Event handler for dictionary events.
        /// </summary>
        public event Action<string, IDictionary<string, object>> DictionaryEvent;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Decodes and routes an event.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="content">The content.</param>
        protected void DecodeAndRouteEvent(string contentType, string content)
        {
            switch (contentType)
            {
                case "application/dictionary+xml":
                    if (DictionaryEvent != null)
                    {
                        var element = XElement.Parse(content);
                        var dataContractSerializer = new DataContractSerializer(typeof (MapEvent));
                        var mapEvent = (MapEvent) dataContractSerializer.ReadObject(element.CreateReader());
                        DictionaryEvent(
                            mapEvent.Name,
                            mapEvent.Atoms.ToDictionary());
                    }
                    break;
                case "application/xml":
                    if (XmlEvent != null)
                    {
                        XmlEvent.Invoke(XElement.Parse(content));
                    }
                    break;
                default:
                    throw new ArgumentException(string.Format("Content type \"{0}\" is not recognized", contentType));
            }
        }
    }
}
