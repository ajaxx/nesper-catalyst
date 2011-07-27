///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using NEsper.Catalyst.Client.Configuration;
using NEsper.Catalyst.Client.Consumers;
using NEsper.Catalyst.Client.Publishers;

namespace NEsper.Catalyst.Client
{
    using Common;

    public class Catalyst : ICatalyst
    {
        private readonly WebChannelFactory<IControlManager> _webChannelFactory;
        private readonly IDictionary<string, CatalystInstance> _instanceTable;
        private readonly DispatchEventConsumerFactory _masterConsumerFactory;
        private readonly DataPublisherFactory _dataPublisherFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Catalyst"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Catalyst(CatalystConfiguration configuration)
        {
            var webChannelBinding = new WebHttpBinding();
            _webChannelFactory = new WebChannelFactory<IControlManager>(webChannelBinding, configuration.ManagerUri);
            _instanceTable = new Dictionary<string, CatalystInstance>();

            _masterConsumerFactory = new DispatchEventConsumerFactory();
            foreach (var consumerFactory in configuration.ConsumerFactories) {
                _masterConsumerFactory.Factories.Add(consumerFactory);
            }

            _dataPublisherFactory = new DataPublisherFactory();
            foreach (var publisherFactory in configuration.PublisherFactories) {
                _dataPublisherFactory.Factories.Add(publisherFactory);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catalyst"/> class.
        /// </summary>
        public Catalyst()
            : this(CatalystConfigurationSection.GetDefaultConfiguration())
        {
        }

        /// <summary>
        /// Gets the master consumer factory.
        /// </summary>
        /// <value>The master consumer factory.</value>
        internal DispatchEventConsumerFactory EventConsumerFactory
        {
            get { return _masterConsumerFactory; }
        }

        /// <summary>
        /// Gets the master data publisher factory.
        /// </summary>
        /// <value>The master data publisher factory.</value>
        internal DataPublisherFactory DataPublisherFactory
        {
            get { return _dataPublisherFactory; }
        }

        /// <summary>
        /// Gets the web channel factory.
        /// </summary>
        /// <value>The web channel factory.</value>
        internal WebChannelFactory<IControlManager> WebChannelFactory
        {
            get { return _webChannelFactory; }
        }

        /// <summary>
        /// Creates a control manager within a disposable wrapper.
        /// </summary>
        /// <returns></returns>
        private ChannelWrapper<IControlManager> CreateControlManager()
        {
            return new ChannelWrapper<IControlManager>(_webChannelFactory.CreateChannel());
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <returns></returns>
        public CatalystInstance GetInstance(string instanceId)
        {
            using (var wrapper = CreateControlManager())
            {
                var controlManager = wrapper.Channel;
                var instanceDescriptor = controlManager.GetInstance(instanceId);
                if (instanceDescriptor != null)
                {
                    return GetInstance(instanceDescriptor);
                }

                throw new ItemNotFoundException();
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="instanceDescriptor">The instance descriptor.</param>
        /// <returns></returns>
        private CatalystInstance GetInstance(InstanceDescriptor instanceDescriptor)
        {
            var instanceId = instanceDescriptor.Id;

            lock (_instanceTable)
            {
                CatalystInstance instance;
                if (!_instanceTable.TryGetValue(instanceId, out instance)) 
                {
                    _instanceTable[instanceId] = instance = new CatalystInstance(this, instanceDescriptor);
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        /// <returns></returns>
        public CatalystInstance GetDefaultInstance()
        {
            using (var wrapper = CreateControlManager()) {
                var controlManager = wrapper.Channel;
                var defaultInstance = controlManager.GetDefaultInstance();
                return GetInstance(defaultInstance.Id);
            }
        }
    }
}
