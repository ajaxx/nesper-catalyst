﻿///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Xml;
using System.Xml.Linq;
using com.espertech.esper.client;
using com.espertech.esper.client.soda;
using com.espertech.esper.compat.logging;

namespace NEsper.Catalyst
{
    using Common;

    /// <summary>
    /// Exposes service points via REST.
    /// </summary>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode =  InstanceContextMode.Single)]
    public class RestControlManager
        : IControlManager
    {
        /// <summary>
        /// Engine manager the control manager exposes.
        /// </summary>
        private EngineManager _engineManager;
        /// <summary>
        /// Service host
        /// </summary>
        private WebServiceHost _serviceHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestControlManager"/> class.
        /// </summary>
        public RestControlManager(EngineManager engineManager)
        {
            _engineManager = engineManager;
            _engineManager.InstanceCreated += RegisterInstance;
            _engineManager.InstanceDestroyed += UnregisterInstance;
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open(Uri serviceUri) 
        {
            _serviceHost = new WebServiceHost(this, serviceUri);

            var serviceBehavior = new WebHttpBehavior();
            serviceBehavior.AutomaticFormatSelectionEnabled = true;
            serviceBehavior.DefaultBodyStyle = WebMessageBodyStyle.Bare;
            serviceBehavior.HelpEnabled = true;
            serviceBehavior.FaultExceptionEnabled = true;
            serviceBehavior.DefaultOutgoingRequestFormat = WebMessageFormat.Json;
            serviceBehavior.DefaultOutgoingResponseFormat = WebMessageFormat.Json;

            var serviceBinding = new WebHttpBinding();
            var serviceEndpoint = _serviceHost.AddServiceEndpoint(typeof(IControlManager), serviceBinding, "");
            serviceEndpoint.Behaviors.Add(serviceBehavior);

            _serviceHost.BeginOpen(HandleAsyncOpen, null);
        }

        /// <summary>
        /// Handles the completion of the asynchronous begin open process.
        /// </summary>
        /// <param name="asyncResult">The async result.</param>
        private void HandleAsyncOpen(IAsyncResult asyncResult)
        {
            if (_serviceHost != null) {
                _serviceHost.EndOpen(asyncResult);
            }
        }

        /// <summary>
        /// Registers an instance.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="instanceEventArgs">The <see cref="NEsper.Catalyst.InstanceEventArgs"/> instance containing the event data.</param>
        private void RegisterInstance(Object sender, InstanceEventArgs instanceEventArgs)
        {
            var instance = instanceEventArgs.Instance;
        }

        /// <summary>
        /// Unregisters an instance.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="instanceEventArgs">The <see cref="NEsper.Catalyst.InstanceEventArgs"/> instance containing the event data.</param>
        private void UnregisterInstance(Object sender, InstanceEventArgs instanceEventArgs)
        {
            var instance = instanceEventArgs.Instance;
        }

        /// <summary>
        /// Gets the engine instance or throws a webfault exception.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <returns></returns>
        private IEngineInstance GetInstanceOrFault(string instanceId)
        {
            var instance = _engineManager.GetInstance(instanceId);
            if (instance == null)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
            return instance;
        }

        
        #region Implementation of IControlManager

        /// <summary>
        /// Gets the instance descriptor.
        /// </summary>
        /// <param name="engineInstance">The engine instance.</param>
        /// <returns></returns>
        private InstanceDescriptor GetInstanceDescriptor(IEngineInstance engineInstance)
        {
            if (engineInstance == null) {
                return null;
            }

            InstanceDescriptor instanceDescriptor = new InstanceDescriptor();
            instanceDescriptor.Id = engineInstance.Id;
            instanceDescriptor.Name = engineInstance.Name;
            return instanceDescriptor;
        }

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        /// <returns></returns>
        public InstanceDescriptor GetDefaultInstance()
        {
            var defaultInstance = _engineManager.DefaultInstance;
            if (defaultInstance == null)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            return GetInstanceDescriptor(defaultInstance);
        }

        /// <summary>
        /// Gets the descriptor for the specified instance.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public InstanceDescriptor GetInstance(string id)
        {
            var instance = GetInstanceOrFault(id);
            return GetInstanceDescriptor(instance);
        }

        /// <summary>
        /// Lists all catalyst instances.
        /// </summary>
        /// <returns></returns>
        public InstanceDescriptor[] ListInstances()
        {
            return _engineManager
                .GetInstances()
                .Select(GetInstanceDescriptor)
                .ToArray();
        }

        /// <summary>
        /// Creates a catalyst instance.
        /// </summary>
        /// <returns></returns>
        public InstanceDescriptor CreateInstance()
        {
            var instance = _engineManager.CreateInstance();
            return GetInstanceDescriptor(instance);
        }

        /// <summary>
        /// Destroys a catalyst instance.
        /// </summary>
        /// <param name="id">The id.</param>
        public void DestroyInstance(string id)
        {
            _engineManager.DestroyInstance(id);
        }

        /// <summary>
        /// Gets the statement description.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementId">The statement id.</param>
        public StatementDescriptor GetStatement(string instanceId, string statementId)
        {
            var instance = GetInstanceOrFault(instanceId);
            var statementDescriptor = instance.GetStatement(statementId);
            if (statementDescriptor != null) {
                return statementDescriptor;
            }

            throw new WebFaultException(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Gets descriptions for all statements.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <returns></returns>
        /// <value>The statements.</value>
        public StatementDescriptorCollection GetStatements(string instanceId)
        {
            var instance = GetInstanceOrFault(instanceId);
            var statementDescriptors = instance.GetStatements();
            if (statementDescriptors != null) {
                return new StatementDescriptorCollection(statementDescriptors.ToArray());
            }

            throw new WebFaultException(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementCreationArgs">The statement creation args.</param>
        /// <returns></returns>
        public StatementDescriptor CreateEPL(string instanceId, StatementCreationArgs statementCreationArgs)
        {
            try {
                var instance = GetInstanceOrFault(instanceId);
                return instance.CreateEPL(statementCreationArgs);
            } catch( EPException e ) {
                throw new WebFaultException<Exception>(e, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Creates a statement based off the pattern that is presented.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementCreationArgs">The statement creation args.</param>
        /// <returns></returns>
        public StatementDescriptor CreatePattern(string instanceId, StatementCreationArgs statementCreationArgs)
        {
            try {
                var instance = GetInstanceOrFault(instanceId);
                return instance.CreatePattern(statementCreationArgs);
            }
            catch (EPException e) {
                throw new WebFaultException<Exception>(e, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Compiles a statement.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        public EPStatementObjectModel Compile(string instanceId, StatementCreationArgs creationArgs)
        {
            try {
                var instance = GetInstanceOrFault(instanceId);
                return instance.Compile(creationArgs);
            }
            catch (EPException e) {
                throw new WebFaultException<Exception>(e, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Destroys the pattern.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementId">The statement id.</param>
        public void DestroyPattern(string instanceId, string statementId)
        {
            var instance = GetInstanceOrFault(instanceId);
            instance.DestroyPattern(statementId);
        }

        /// <summary>
        /// Sends an event into the instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="event">The @event.</param>
        public void SendXmlEvent(string instanceId, XElement @event)
        {
            try {
                var instance = GetInstanceOrFault(instanceId);
                instance.SendEvent(@event);
            }
            catch (EPException e) {
                throw new WebFaultException<Exception>(e, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Sends an event into the instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="eventArgs">The <see cref="NEsper.Catalyst.Common.JsonEventArgs"/> instance containing the event data.</param>
        public void SendJsonEvent(string instanceId, JsonEventArgs eventArgs)
        {
            try {
                var instance = GetInstanceOrFault(instanceId);
                var eventBytes = System.Text.Encoding.UTF8.GetBytes(eventArgs.EventData);
                var dictionaryReader = JsonReaderWriterFactory.CreateJsonReader(
                    eventBytes, 0, eventBytes.Length, new XmlDictionaryReaderQuotas());
                var dictionaryDocument = XDocument.Load(dictionaryReader);
                dictionaryDocument.Root.Name = eventArgs.EventType;

                instance.SendEvent(dictionaryDocument.Root);
            }
            catch (EPException e) {
                throw new WebFaultException<Exception>(e, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Gets statistics for the engine.
        /// </summary>
        /// <returns></returns>
        public EngineStatistics GetStatistics()
        {
            var statistics = new EngineStatistics();
            return statistics;
        }

        /// <summary>
        /// Gets statistics for an instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <returns></returns>
        public InstanceStatistics GetInstanceStatistics(string instanceId)
        {
            var instance = GetInstanceOrFault(instanceId);
            var statistics = new InstanceStatistics();
            return statistics;
        }

        #endregion

        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
