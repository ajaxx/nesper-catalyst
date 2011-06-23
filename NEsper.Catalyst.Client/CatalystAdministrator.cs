///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

using com.espertech.esper.client;
using com.espertech.esper.client.deploy;
using com.espertech.esper.client.soda;
using com.espertech.esper.compat;
using com.espertech.esper.compat.logging;
using com.espertech.esper.util;
using NEsper.Catalyst.Common;

namespace NEsper.Catalyst.Client
{
    using TypeMap = IDictionary<string, object>;

    public class CatalystAdministrator
        : ICatalystAdministrator
    {
        private readonly Catalyst _adapter;
        private readonly IDictionary<string, WeakReference<CatalystStatement>> _statementTable;
        private readonly WebChannelFactory<IControlManager> _webChannelFactory;
        private readonly string _instanceId;

        private readonly HashSet<Type> _registeredTypes =
            new HashSet<Type>();

        public event EventHandler<TypeEventArgs> TypeRegistered;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystAdministrator"/> class.
        /// </summary>
        /// <param name="adapter">The catalyst adapter.</param>
        /// <param name="instanceId">The instance id.</param>
        public CatalystAdministrator(Catalyst adapter, string instanceId)
        {
            _adapter = adapter;
            _statementTable = new Dictionary<string, WeakReference<CatalystStatement>>();
            _webChannelFactory = adapter.WebChannelFactory;
            _instanceId = instanceId;
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
        /// Sanitizes the type map.
        /// </summary>
        /// <param name="typeMap">The type map.</param>
        /// <returns></returns>
        private IEnumerable<EventTypeAtom> ToEventTypeAtoms(IEnumerable<KeyValuePair<string, object>> typeMap)
        {
            foreach (var entry in typeMap) {
                var name = entry.Key;
                var type = entry.Value;
                if (type is Type) {
                    var asType = (Type) type;
                    var eventTypeAtom = new EventTypeAtom
                    {
                        Name = name, 
                        TypeName = asType.FullName
                    };

                    yield return eventTypeAtom;
                }
                else if (type is IDictionary<string, object>) {
                    var asDictionary = (IDictionary<string, object>) type;
                    var eventTypeAtom = new EventTypeAtom
                    {
                        Name = name,
                        TypeDecl = ToEventTypeAtoms(asDictionary).ToArray()
                    };
                    yield return eventTypeAtom;
                }
                else if (type is string) {
                    var eventTypeAtom = new EventTypeAtom
                    {
                        Name = name, 
                        TypeName = (string) type
                    };
                    yield return eventTypeAtom;
                }
                else {
                    throw new ArgumentException("invalid typeMap");
                }
            }
        }


        /// <summary>
        /// Declares a native type.  Not required, but encouraged.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterType<T>()
        {
            RegisterType(typeof (T));
        }

        /// <summary>
        /// Declares a native type.  Not required, but encouraged.
        /// </summary>
        /// <param name="nativeType">Type of the native.</param>
        public void RegisterType(Type nativeType)
        {
            if ((nativeType == null) || 
                (nativeType.IsBuiltinDataType()))
            {
                return;
            }
            
            lock (_registeredTypes)
            {
                if (_registeredTypes.Contains(nativeType))
                {
                    return;
                }

                var dataContractExporter = new XsdDataContractExporter();
                dataContractExporter.Export(nativeType);

                var rootElementName = dataContractExporter.GetRootElementName(nativeType);
                var rootElementSchemaSet = dataContractExporter.Schemas;

                using (var wrapper = CreateControlManager())
                {
                    WithExceptionHandling(
                        delegate
                            {
                                var controlManager = wrapper.Channel;
                                var eventTypeDefinition = new NativeTypeDefinition(
                                    rootElementName,
                                    rootElementSchemaSet);

                                controlManager.RegisterType(_instanceId, eventTypeDefinition);
                            });
                }

                _registeredTypes.Add(nativeType);

                Log.Info("RegisterType: Type '{0}' registered", nativeType.FullName);

                if ( TypeRegistered != null )
                {
                    TypeRegistered(this, new TypeEventArgs(nativeType));
                }
            }
        }

        /// <summary>
        /// Registers a type tree.
        /// </summary>
        /// <param name="typeMap">The type map.</param>
        public void RegisterTypeTree(TypeMap typeMap)
        {
            foreach(var value in typeMap.Values)
            {
                if (value is Type)
                {
                    RegisterType((Type) value);
                }
                else if ( value is string )
                {
                    RegisterType(TypeHelper.ResolveType((string) value, false));
                }
                else if ( value is IDictionary<string, object> )
                {
                    RegisterTypeTree((IDictionary<string, object>) value);
                }
            }
        }

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddEventType<T>()
        {
            AddEventType(typeof (T).Name, typeof(T));
        }

        /// <summary>
        /// Adds the type of the event with a specific name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventTypeName">The name.</param>
        public void AddEventType<T>(string eventTypeName)
        {
            AddEventType(eventTypeName, typeof (T));
        }

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="eventTypeName">Name of the event type.</param>
        /// <param name="nativeEventType">Native event type.</param>
        public void AddEventType(string eventTypeName, Type nativeEventType)
        {
            RegisterType(nativeEventType);

            using (var wrapper = CreateControlManager())
            {
                WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var eventTypeDefinition = new NativeEventTypeDefinition(
                            eventTypeName, nativeEventType.FullName);

                        controlManager.AddEventType(_instanceId, eventTypeDefinition);
                    });
            }
        }

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="eventTypeName">Name of the event type.</param>
        /// <param name="typeMap">The type map.</param>
        public void AddEventType(string eventTypeName, IDictionary<string, object> typeMap)
        {
            var eventTypeAtoms = ToEventTypeAtoms(typeMap);

            using (var wrapper = CreateControlManager())
            {
                RegisterTypeTree(typeMap);

                WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var eventTypeDefinition = new MapEventTypeDefinition(
                            eventTypeName, eventTypeAtoms.ToArray());
                        controlManager.AddEventType(_instanceId, eventTypeDefinition);
                    });
            }
        }

        /// <summary>
        /// Adds the type of the event.
        /// </summary>
        /// <param name="eventTypeName">Name of the event type.</param>
        /// <param name="typeMap">The type map.</param>
        /// <param name="superTypes">The super types.</param>
        public void AddEventType(string eventTypeName, IDictionary<string, object> typeMap, params string[] superTypes)
        {
            var eventTypeAtoms = ToEventTypeAtoms(typeMap);
            var superTypeArray = superTypes != null ? superTypes.ToArray() : null;

            using (var wrapper = CreateControlManager())
            {
                WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var eventTypeDefinition = new MapEventTypeDefinition(
                            eventTypeName, eventTypeAtoms.ToArray(), superTypeArray);
                        controlManager.AddEventType(_instanceId, eventTypeDefinition);
                    });
            }
        }

        /// <summary>
        /// Returns deployment administrative services.
        /// </summary>
        /// <returns>deployment administration</returns>
        public EPDeploymentAdmin DeploymentAdmin
        {
            get { throw new NotSupportedException("not supported on this platform"); }
        }

        /// <summary>
        /// Returns the statement names of all started and stopped statements.
        /// <para/>
        /// This excludes the name of destroyed statements.
        /// </summary>
        /// <returns>
        /// statement names
        /// </returns>
        public IList<string> StatementNames
        {
            get
            {
                using (var wrapper = CreateControlManager())
                {
                    return WithExceptionHandling(
                        delegate
                        {
                            var controlManager = wrapper.Channel;
                            var statements = controlManager.GetStatements(_instanceId);
                            return statements.Select(statementDescriptor => statementDescriptor.Id).ToList();
                        });
                }
            }
        }

        private void BindStatement(CatalystStatement statement)
        {
            lock(_statementTable) {
                _statementTable[statement.Id] = new WeakReference<CatalystStatement>(statement);
            }
        }

        #region CreatePattern

        /// <summary>
        /// Create and starts an event pattern statement for the expressing string passed.
        /// <para/>
        /// The engine assigns a unique name to the statement.
        /// </summary>
        /// <param name="onExpression">must follow the documented syntax for pattern statements</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreatePattern(string onExpression)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = onExpression;

                        var statement = controlManager.CreatePattern(_instanceId, statementArgs);
                        var statementWrapper = new CatalystStatement(_adapter, statement);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Create and starts an event pattern statement for the expressing string passed
        /// and assign the name passed.
        /// <para/>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique
        /// statement name.
        /// </summary>
        /// <param name="onExpression">must follow the documented syntax for pattern statements</param>
        /// <param name="statementName">is the name to assign to the statement for use in managing the statement</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreatePattern(string onExpression, string statementName)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;

                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = onExpression;
                        statementArgs.StatementName = statementName;

                        var statement = controlManager.CreatePattern(_instanceId, statementArgs);
                        var statementWrapper = new CatalystStatement(_adapter, statement);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Create and starts an event pattern statement for the expressing string passed
        /// and assign the name passed.
        /// <para/>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique
        /// statement name.
        /// <para/>
        /// Accepts an application defined user data object associated with the statement.
        /// The <em>user object</em> is a single, unnamed field that is stored with every
        /// statement. Applications may put arbitrary objects in this field or a null value.
        /// </summary>
        /// <param name="onExpression">must follow the documented syntax for pattern statements</param>
        /// <param name="statementName">is the name to assign to the statement for use in managing the statement</param>
        /// <param name="userObject">is the application-defined user object</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreatePattern(string onExpression, string statementName, object userObject)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;

                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = onExpression;
                        statementArgs.StatementName = statementName;

                        var statement = controlManager.CreatePattern(_instanceId, statementArgs);
                        var statementWrapper = new CatalystStatement(_adapter, statement, userObject);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Create and starts an event pattern statement for the expressing string passed
        /// and assign the name passed.
        /// <para/>
        /// Accepts an application defined user data object associated with the statement.
        /// The <em>user object</em> is a single, unnamed field that is stored with every
        /// statement. Applications may put arbitrary objects in this field or a null value.
        /// </summary>
        /// <param name="onExpression">must follow the documented syntax for pattern statements</param>
        /// <param name="userObject">is the application-defined user object</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreatePattern(string onExpression, object userObject)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;

                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = onExpression;

                        var statement = controlManager.CreatePattern(_instanceId, statementArgs);
                        var statementWrapper = new CatalystStatement(_adapter, statement, userObject);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }
        #endregion

        #region CreateEPL

        /// <summary>
        /// Creates and starts an EPL statement.
        /// <para/>
        /// The engine assigns a unique name to the statement. The returned statement is in
        /// started state.
        /// </summary>
        /// <param name="eplStatement">is the query language statement</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreateEPL(string eplStatement)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = eplStatement;

                        var statement = controlManager.CreateEPL(_instanceId, statementArgs);
                        var statementWrapper = new CatalystStatement(_adapter, statement);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Create and starts an EPL statement.
        /// <para/>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique
        /// statement name.
        /// </summary>
        /// <param name="eplStatement">is the query language statement</param>
        /// <param name="statementName">is the name to assign to the statement for use in managing the statement</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreateEPL(string eplStatement, string statementName)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = eplStatement;
                        statementArgs.StatementName = statementName;

                        var statement = controlManager.CreateEPL(_instanceId, statementArgs);
                        var statementWrapper = new CatalystStatement(_adapter, statement);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Create and starts an EPL statement.
        /// <para/>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique
        /// statement name.
        /// <para/>
        /// Accepts an application defined user data object associated with the statement.
        /// The <em>user object</em> is a single, unnamed field that is stored with every
        /// statement. Applications may put arbitrary objects in this field or a null value.
        /// </summary>
        /// <param name="eplStatement">is the query language statement</param>
        /// <param name="statementName">is the name to assign to the statement for use in managing the statement</param>
        /// <param name="userObject">is the application-defined user object</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreateEPL(string eplStatement, string statementName, object userObject)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = eplStatement;
                        statementArgs.StatementName = statementName;

                        var statement = controlManager.CreateEPL(_instanceId, statementArgs);
                        var statementWrapper = new CatalystStatement(_adapter, statement, userObject);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Create and starts an EPL statement.
        /// <para/>
        /// Accepts an application defined user data object associated with the statement.
        /// The <em>user object</em> is a single, unnamed field that is stored with every
        /// statement. Applications may put arbitrary objects in this field or a null value.
        /// </summary>
        /// <param name="eplStatement">is the query language statement</param>
        /// <param name="userObject">is the application-defined user object</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement CreateEPL(string eplStatement, object userObject)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = eplStatement;

                        var statement = controlManager.CreateEPL(_instanceId, statementArgs);
                        var statementWrapper = new CatalystStatement(_adapter, statement, userObject);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates and starts an EPL statement.
        /// <para/>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique
        /// statement name.
        /// </summary>
        /// <param name="sodaStatement">is the statement object model</param>
        /// <param name="statementName">is the name to assign to the statement for use in managing the statement</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement Create(EPStatementObjectModel sodaStatement, string statementName)
        {
            return CreateEPL(sodaStatement.ToEPL(), statementName);
        }

        /// <summary>
        /// Creates and starts an EPL statement.
        /// <para/>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique
        /// statement name.
        /// <para/>
        /// Accepts an application defined user data object associated with the statement.
        /// The <em>user object</em> is a single, unnamed field that is stored with every
        /// statement. Applications may put arbitrary objects in this field or a null value.
        /// </summary>
        /// <param name="sodaStatement">is the statement object model</param>
        /// <param name="statementName">is the name to assign to the statement for use in managing the statement</param>
        /// <param name="userObject">is the application-defined user object</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement Create(EPStatementObjectModel sodaStatement, string statementName, object userObject)
        {
            return CreateEPL(sodaStatement.ToEPL(), statementName, userObject);
        }

        /// <summary>
        /// Creates and starts an EPL statement.
        /// </summary>
        /// <param name="sodaStatement">is the statement object model</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement Create(EPStatementObjectModel sodaStatement)
        {
            return CreateEPL(sodaStatement.ToEPL());
        }

        #endregion

        #region Create - PreparedStatement

        /// <summary>
        /// Creates and starts a prepared statement.
        /// <para/>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique
        /// statement name.
        /// </summary>
        /// <param name="prepared">is the prepared statement for which all substitution values have been provided</param>
        /// <param name="statementName">is the name to assign to the statement for use in managing the statement</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the prepared statement was not valid</throws>
        public EPStatement Create(EPPreparedStatement prepared, string statementName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates and starts a prepared statement.
        /// <para/>
        /// The statement name is optimally a unique name. If a statement of the same name
        /// has already been created, the engine assigns a postfix to create a unique
        /// statement name.
        /// <para/>
        /// Accepts an application defined user data object associated with the statement.
        /// The <em>user object</em> is a single, unnamed field that is stored with every
        /// statement. Applications may put arbitrary objects in this field or a null value.
        /// </summary>
        /// <param name="prepared">is the prepared statement for which all substitution values have been provided</param>
        /// <param name="statementName">is the name to assign to the statement for use in managing the statement</param>
        /// <param name="userObject">is the application-defined user object</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the prepared statement was not valid</throws>
        public EPStatement Create(EPPreparedStatement prepared, string statementName, object userObject)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates and starts a prepared statement.
        /// </summary>
        /// <param name="prepared">is the prepared statement for which all substitution values have been provided</param>
        /// <returns>
        /// EPStatement to poll data from or to add listeners to
        /// </returns>
        /// <throws>EPException when the expression was not valid</throws>
        public EPStatement Create(EPPreparedStatement prepared)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Prepare

        /// <summary>
        /// Prepares a statement for the given EPL, which can include substitution
        /// parameters marked via question mark '?'.
        /// </summary>
        /// <param name="eplExpression">is the statement text to prepare</param>
        /// <returns>
        /// prepared statement
        /// </returns>
        /// <throws>EPException indicates compilation errors.</throws>
        public EPPreparedStatement PrepareEPL(string eplExpression)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = eplExpression;

                        var preparationId = controlManager.PrepareEPL(_instanceId, statementArgs);
                        var statementWrapper = new CatalystPreparedStatement(_adapter, _instanceId, preparationId);
                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Prepares a statement for the given pattern, which can include substitution
        /// parameters marked via question mark '?'.
        /// </summary>
        /// <param name="patternExpression">is the statement text to prepare</param>
        /// <returns>
        /// prepared statement
        /// </returns>
        /// <throws>EPException indicates compilation errors.</throws>
        public EPPreparedStatement PreparePattern(string patternExpression)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statementArgs = new StatementCreationArgs();
                        statementArgs.StatementText = patternExpression;

                        var preparationId = controlManager.PreparePattern(_instanceId, statementArgs);
                        var statementWrapper = new CatalystPreparedStatement(_adapter, _instanceId, preparationId);
                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Sets the value of the designated parameter using the given object.
        /// </summary>
        /// <param name="statementId">The statement id.</param>
        /// <param name="parameterIndex">the first parameter is 1, the second is 2, ...</param>
        /// <param name="value">the object containing the input parameter value</param>
        /// <exception name="EPException">if the substitution parameter could not be located</exception>
	    public void SetObject(string statementId, int parameterIndex, Object value)
        {
            using (var wrapper = CreateControlManager())
            {
                WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var preparedValueArgs = new PreparedValueArgs();
                        preparedValueArgs.DataType = value.GetType().FullName;
                        preparedValueArgs.Data = SerializationFabric.Serialize(value);
                        preparedValueArgs.ParameterIndex = parameterIndex;

                        controlManager.SetPreparedValue(
                            _instanceId,
                            statementId,
                            preparedValueArgs);
                    });
            }
        }

        #endregion

        /// <summary>
        /// Compiles a given EPL into an object model representation of the query.
        /// </summary>
        /// <param name="eplExpression">is the statement text to compile</param>
        /// <returns>
        /// object model of statement
        /// </returns>
        /// <throws>EPException indicates compilation errors.</throws>
        public EPStatementObjectModel CompileEPL(string eplExpression)
        {
            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statementArgs = new StatementCreationArgs();
                        return controlManager.Compile(_instanceId, statementArgs);
                    });
            }
        }

        /// <summary>
        /// Returns the statement by the given statement name. Returns null if a statement
        /// of that name has not been created, or if the statement by that name has been
        /// destroyed.
        /// </summary>
        /// <param name="name">is the statement name to return the statement for</param>
        /// <returns>
        /// statement for the given name, or null if no such started or stopped statement
        /// exists
        /// </returns>
        public EPStatement GetStatement(string name)
        {
            lock (_statementTable)
            {
                var statementWrapper = _statementTable.Get(name);
                if ((statementWrapper != null) && statementWrapper.IsAlive)
                {
                    return statementWrapper.Target;
                }
            }

            using (var wrapper = CreateControlManager())
            {
                return WithExceptionHandling(
                    delegate
                    {
                        var controlManager = wrapper.Channel;
                        var statement = controlManager.GetStatement(_instanceId, name);
                        var statementWrapper = new CatalystStatement(_adapter, statement);
                        BindStatement(statementWrapper);

                        return statementWrapper;
                    });
            }
        }

        /// <summary>
        /// Starts all statements that are in stopped state. Statements in started state are
        /// not affected by this method.
        /// </summary>
        /// <throws>EPException when an error occured starting statements.</throws>
        public void StartAllStatements()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Stops all statements that are in started state. Statements in stopped state are
        /// not affected by this method.
        /// </summary>
        /// <throws>EPException when an error occured stopping statements</throws>
        public void StopAllStatements()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Stops and destroys all statements.
        /// </summary>
        /// <throws>EPException when an error occured stopping or destroying statements</throws>
        public void DestroyAllStatements()
        {
            throw new NotSupportedException();
        }

        private static void HandleProtocolException(ProtocolException e)
        {
            var serializer = new DataContractSerializer(typeof(string));
            var webException = e.InnerException as WebException;
            if (webException != null)
            {
                var httpWebResponse = webException.Response as HttpWebResponse;
                if (httpWebResponse != null)
                {
                    switch (httpWebResponse.StatusCode)
                    {
                        case HttpStatusCode.BadRequest:
                            throw new EPException(
                                (string)serializer.ReadObject(httpWebResponse.GetResponseStream()));
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedAccessException(
                                (string)serializer.ReadObject(httpWebResponse.GetResponseStream()));
                    }
                }
            }
        }

        private static void WithExceptionHandling(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (ProtocolException e)
            {
                HandleProtocolException(e);
                throw;
            }
        }

        private static T WithExceptionHandling<T>(Func<T> action)
        {
            try
            {
                return action.Invoke();
            }
            catch (ProtocolException e)
            {
                HandleProtocolException(e);
                throw;
            }
        }

        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
