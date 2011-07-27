///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Text;
using System.Xml.Linq;

using com.espertech.esper.compat.logging;

using RabbitMQ.Client;

namespace NEsper.Catalyst.Client.Publishers
{
    class RabbitMqDataPublisher : IDataPublisher
    {
        private readonly IModel _model;
        private readonly PublicationAddress _address;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqDataPublisher"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="address">The address.</param>
        public RabbitMqDataPublisher(IModel model, PublicationAddress address)
        {
            _model = model;
            _address = address;
        }

        /// <summary>
        /// Sends the event.
        /// </summary>
        /// <param name="event">The @event.</param>
        public void SendEvent(XElement @event)
        {
            var eventElement = @event;
            var eventContent = Encoding.Unicode.GetBytes(eventElement.ToString(SaveOptions.None));

            var basicProperties = _model.CreateBasicProperties();
            basicProperties.DeliveryMode = 1;

            Log.Info("SendEvent: {0}", _address);

            _model.BasicPublish(_address, basicProperties, eventContent);
        }

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
