///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using com.espertech.esper.client;
using com.espertech.esper.client.soda;
using com.espertech.esper.compat.logging;
using com.espertech.esper.util;

namespace NEsper.Catalyst
{
    using Common;
    using Configuration;

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
        private readonly EngineManager _engineManager;
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
        public void Open()
        {
            var catalystConfiguration = CatalystConfiguration.GetDefaultInstance();
            if ((catalystConfiguration != null) &&
                (catalystConfiguration.ControlManager != null) &&
                (catalystConfiguration.ControlManager.Uri != null))
            {
                Open(catalystConfiguration.ControlManager.Uri);
            }
            else
            {
                Open(new Uri("http://localhost/catalyst/engine"));
            }
        }

        /// <summary>
        /// Opens this instance.
        /// </summary>
        public void Open(Uri serviceUri)
        {
            Log.Info("Open: URI => {0}", serviceUri);

            _serviceHost = new WebServiceHost(this, serviceUri);

            var serviceBehavior = new WebHttpBehavior();
            serviceBehavior.AutomaticFormatSelectionEnabled = true;
            serviceBehavior.DefaultBodyStyle = WebMessageBodyStyle.Bare;
            serviceBehavior.HelpEnabled = true;
            serviceBehavior.FaultExceptionEnabled = false;
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

            throw new WebFaultException<string>(String.Empty, HttpStatusCode.NotFound);
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
        /// Creates a statement from a prepared statement.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementArgs">The statement args.</param>
        /// <returns></returns>
        public StatementDescriptor CreatePrepared(string instanceId, StatementCreationArgs statementArgs)
        {
            try
            {
                var instance = GetInstanceOrFault(instanceId);
                return instance.CreatePrepared(statementArgs);
            }
            catch (EPException e)
            {
                Log.Warn("CreateEPL: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }
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
                Log.Warn("CreateEPL: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
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
                Log.Warn("CreatePattern: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
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
                Log.Warn("Compile: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Creates a prepared statement based off the pattern that is presented.  The value
        /// that is returned is a unique identifier to the representation of the prepared
        /// statement on the server.  It is the prepared statement id.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        public string PrepareEPL(string instanceId, StatementCreationArgs creationArgs)
        {
            try
            {
                var instance = GetInstanceOrFault(instanceId);
                return instance.PrepareEPL(creationArgs.StatementText);
            }
            catch (EPException e)
            {
                Log.Warn("PrepareEPL: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Creates a prepared statement based off the pattern that is presented.  The value
        /// that is returned is a unique identifier to the representation of the prepared
        /// statement on the server.  It is the prepared statement id.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="creationArgs">The creation args.</param>
        /// <returns></returns>
        public string PreparePattern(string instanceId, StatementCreationArgs creationArgs)
        {
            try
            {
                var instance = GetInstanceOrFault(instanceId);
                return instance.PreparePattern(creationArgs.StatementText);
            }
            catch (EPException e)
            {
                Log.Warn("PreparePattern: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            } 
        }

        /// <summary>
        /// Sets a value within the prepared statement.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="statementId">The statement id.</param>
        /// <param name="preparedValueArgs">The prepared value args.</param>
        public void SetPreparedValue(string instanceId, string statementId, PreparedValueArgs preparedValueArgs)
        {
            try
            {
                var instance = GetInstanceOrFault(instanceId);
                var fabricator = instance.SchemaFabricator;
                var fabricationValue = fabricator.Fabricate(
                    preparedValueArgs.Data,
                    preparedValueArgs.DataType);

                instance.SetPreparedValue(
                    statementId,
                    preparedValueArgs.ParameterIndex,
                    fabricationValue);
            }
            catch (EPException e)
            {
                Log.Warn("Compile: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
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
        public void SendMapEvent(string instanceId, MapEvent @event)
        {
            try {
                var instance = GetInstanceOrFault(instanceId);
                var dictionary = @event.Atoms.ToDictionary();
                instance.SendEvent(dictionary, @event.Name);
            }
            catch (EPException e) {
                Log.Warn("SendMapEvent: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }
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
                Log.Warn("SendXmlEvent: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Sends an event into the instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="event">The <see cref="JsonEvent"/> instance containing the event data.</param>
        public void SendJsonEvent(string instanceId, JsonEvent @event)
        {
            try {
                var instance = GetInstanceOrFault(instanceId);
                var fabricator = instance.SchemaFabricator;
                var eventBytes = System.Text.Encoding.UTF8.GetBytes(@event.EventData);
                var dictionaryReader = JsonReaderWriterFactory.CreateJsonReader(
                        eventBytes, 0, eventBytes.Length, new XmlDictionaryReaderQuotas());
    
                var fabricatorType = fabricator.GetType(@event.EventType);
                if (fabricatorType != null)
                {
                    var serializer = new DataContractJsonSerializer(fabricatorType);
                    var trueEntity = serializer.ReadObject(dictionaryReader);
                    instance.SendEvent(trueEntity);
                }
                else
                {
                    var dictionaryDocument = XDocument.Load(dictionaryReader);
                    dictionaryDocument.Root.Name = @event.EventType;
                    instance.SendEvent(dictionaryDocument.Root);
                }
            }
            catch (EPException e) {
                Log.Warn("SendJsonEvent: BadRequest returned: {0}", e.Message);
                throw new WebFaultException<string>(e.Message, HttpStatusCode.BadRequest);
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

        /// <summary>
        /// Converts the atoms into a type map.
        /// </summary>
        /// <param name="atoms">The atoms.</param>
        /// <returns></returns>
        private IDictionary<string, object> ToTypeMap(IEnumerable<EventTypeAtom> atoms)
        {
            var typeMap = new Dictionary<string, object>();
            foreach(var atom in atoms) {
                if (atom.TypeName != null) {
                    typeMap[atom.Name] = TypeHelper.ResolveType(atom.TypeName);
                } else if (atom.TypeDecl != null) {
                    typeMap[atom.Name] = ToTypeMap(atom.TypeDecl);
                } else {
                    throw new ArgumentException("invalid event type atoms");
                }
            }

            return typeMap;
        }

        /// <summary>
        /// Declares a type.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="typeDefinition">The event type definition.</param>
        public void RegisterType(string instanceId, NativeTypeDefinition typeDefinition)
        {
            Log.Info("RegisterType: instanceId = {0}, schemaTypeName = {1}",
                     instanceId,
                     typeDefinition.SchemaTypeName);

            try
            {
                var instance = GetInstanceOrFault(instanceId);
                var fabricator = instance.SchemaFabricator;
                var eventTypeSchemaSet = new XmlSchemaSet();

                foreach (var schemaText in typeDefinition.Schemas)
                {
                    var reader = XmlReader.Create(new StringReader(schemaText));
                    var schema = XmlSchema.Read(reader, HandleSchemaValidation);
                    eventTypeSchemaSet.Add(schema);
                }

                eventTypeSchemaSet.Compile();

                fabricator.GetNativeElement(
                    eventTypeSchemaSet,
                    typeDefinition.SchemaTypeName);
            }
            catch (WebFaultException)
            {
                throw;
            }
            catch (Exception e)
            {
                Log.Error("RegisterType: failure due to exception", e);
                throw;
            }
        }

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="eventTypeDefinition">The event type definition.</param>
        public void AddEventType(string instanceId, MapEventTypeDefinition eventTypeDefinition)
        {
            var instance = GetInstanceOrFault(instanceId);
            var typeMap = ToTypeMap(eventTypeDefinition.TypeMap);
            instance.ServiceProvider.EPAdministrator
                .GetConfiguration()
                .AddEventType(eventTypeDefinition.Name, typeMap);
        }

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="eventTypeDefinition">The event type definition.</param>
        public void AddEventType(string instanceId, NativeEventTypeDefinition eventTypeDefinition)
        {
            Log.Info("AddEventType: instanceId = {0}, name = {1}, schemaTypeName = {2}",
                     instanceId,
                     eventTypeDefinition.Name,
                     eventTypeDefinition.TypeName);

            try
            {
                var instance = GetInstanceOrFault(instanceId);
                var eventType = TypeHelper.ResolveType(eventTypeDefinition.TypeName, false);
                if (eventType == null)
                {
                    throw new WebFaultException<string>(
                        string.Format("Unable to resolve type '{0}'", eventTypeDefinition.TypeName),
                        HttpStatusCode.BadRequest);
                }

                instance.ServiceProvider.EPAdministrator
                    .GetConfiguration()
                    .AddEventType(eventTypeDefinition.Name, eventType);
            } catch( WebFaultException )
            {
                throw;
            }
            catch( Exception e )
            {
                Log.Error("AddEventType: failure due to exception", e);
                throw;
            }
        }

        private void HandleSchemaValidation(object sender, ValidationEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
