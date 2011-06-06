using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace NEsper.Catalyst.Client
{
    using Common;

    public class Catalyst
    {
        private readonly WebChannelFactory<IControlManager> _webChannelFactory;
        private readonly IDictionary<string, CatalystInstance> _instanceTable;
        private readonly IEventConsumerFactory _masterConsumerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="Catalyst"/> class.
        /// </summary>
        /// <param name="managerUri">The instance URI.</param>
        public Catalyst(Uri managerUri)
        {
            var webChannelBinding = new WebHttpBinding();
            _webChannelFactory = new WebChannelFactory<IControlManager>(webChannelBinding, managerUri);
            _instanceTable = new Dictionary<string, CatalystInstance>();

            var dispatchConsumerFactory = new DispatchEventConsumerFactory();
            dispatchConsumerFactory.Factories.Add(new MsmqEventConsumerFactory());

            _masterConsumerFactory = dispatchConsumerFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Catalyst"/> class.
        /// </summary>
        /// <param name="managerUri">The manager URI.</param>
        public Catalyst(string managerUri)
            : this(new Uri(managerUri))
        {
        }

        /// <summary>
        /// Gets the master consumer factory.
        /// </summary>
        /// <value>The master consumer factory.</value>
        internal IEventConsumerFactory EventConsumerFactory
        {
            get { return _masterConsumerFactory; }
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
            lock(_instanceTable) {
                CatalystInstance instance;
                if (!_instanceTable.TryGetValue(instanceId, out instance)) {
                    _instanceTable[instanceId] = instance = new CatalystInstance(this, instanceId);
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
