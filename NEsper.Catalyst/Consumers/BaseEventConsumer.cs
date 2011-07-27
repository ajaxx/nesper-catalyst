using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Linq;
using com.espertech.esper.client;
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
        /// Gets or sets the schema fabricator.
        /// </summary>
        /// <value>The schema fabricator.</value>
        public SchemaFabricator SchemaFabricator { get; set; }

        /// <summary>
        /// Event handler for data events.
        /// </summary>
        public event Action<Object> DataEvent;

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
        /// <param name="element">The element.</param>
        protected void DecodeAndRouteEvent(XElement element)
        {
            if (element.Name == XName.Get("json"))
            {
                try
                {
                    var eventTypeElement = element.Element("type");
                    if (eventTypeElement == null)
                    {
                        return;
                    }

                    var eventDataElement = element.Element("data");
                    if (eventDataElement == null)
                    {
                        return;
                    }

                    var eventType = eventTypeElement
                        .Nodes()
                        .OfType<XCData>()
                        .FirstOrDefault()
                        .Value;
                    var eventData = eventDataElement
                        .Nodes()
                        .OfType<XCData>()
                        .FirstOrDefault()
                        .Value;

                    var eventBytes = System.Text.Encoding.UTF8.GetBytes(eventData);
                    var dictionaryReader = JsonReaderWriterFactory.CreateJsonReader(
                            eventBytes, 0, eventBytes.Length, new XmlDictionaryReaderQuotas());

                    var fabricator = SchemaFabricator;
                    if (fabricator != null)
                    {
                        var fabricatorType = fabricator.GetType(eventType);
                        if (fabricatorType != null)
                        {
                            if (DataEvent != null)
                            {
                                var serializer = new DataContractJsonSerializer(fabricatorType);
                                var trueEntity = serializer.ReadObject(dictionaryReader);
                                DataEvent.Invoke(trueEntity);
                            }

                            return;
                        }
                    }

                    if (XmlEvent != null)
                    {
                        var dictionaryDocument = XDocument.Load(dictionaryReader);
                        dictionaryDocument.Root.Name = eventType;
                        XmlEvent.Invoke(dictionaryDocument.Root);
                    }
                }
                catch (EPException e)
                {
                }
            }
            else if (element.Name == XName.Get("xml"))
            {
                if (XmlEvent != null)
                {
                    XmlEvent.Invoke(element.Elements().FirstOrDefault());
                }
            }
            else if (element.Name == XName.Get("map"))
            {
                DictionaryEvent(
                    element.Attribute("name").Value,
                    element.Elements().FirstOrDefault().ToDictionary());
            }
            else
            {
                throw new ArgumentException(string.Format("Content type \"{0}\" is not recognized", element.Name));
            }
        }
    }
}
