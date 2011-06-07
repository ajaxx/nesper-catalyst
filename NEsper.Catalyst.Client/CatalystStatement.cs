///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;

using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    using Common;

    class CatalystStatement : EPStatement
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        internal string Id
        {
            get { return StatementDescriptor.Id; }
        }

        /// <summary>
        /// Gets or sets the adapter.
        /// </summary>
        /// <value>The adapter.</value>
        internal Catalyst Adapter { get; private set; }

        /// <summary>
        /// Gets or sets the statement descriptor.
        /// </summary>
        /// <value>The statement descriptor.</value>
        internal StatementDescriptor StatementDescriptor { get; set; }

        /// <summary>
        /// Internal representation of the event handler
        /// </summary>
        private UpdateEventHandler _eventHandler;

        /// <summary>
        /// Entity that subscribes to events on our behalf
        /// </summary>
        private IDisposable _eventSubscriber;

        /// <summary>
        /// Occurs whenever new events are available or old events are removed.
        /// </summary>
        public event UpdateEventHandler Events
        {
            add
            {
                _eventHandler += value;
                if ( _eventSubscriber == null ) {
                    _eventSubscriber = Adapter.EventConsumerFactory.Subscribe(
                        StatementDescriptor.URIs, ReceiveEvents);
                }
            }
            remove
            {
                _eventHandler -= value;
                if (_eventHandler == null) {
                    _eventSubscriber.Dispose();
                    _eventSubscriber = null;
                }
            }
        }

        private void ReceiveEvents(object sender, UpdateEventArgs e)
        {
            if (_eventHandler != null) {
                _eventHandler(this, new UpdateEventArgs(null, this, e.NewEvents, e.OldEvents));
            }
        }

        /// <summary>
        /// Returns the application defined user data object associated with the statement,
        /// or null if none was supplied at time of statement creation.
        /// </summary>
        public Object UserObject { get; internal set; }

        /// <summary> Returns the statement name.</summary>
        /// <returns> statement name</returns>
        public string Name
        {
            get { return StatementDescriptor.Id; }
        }

        /// <summary> Returns the underlying expression text or XML.</summary>
        /// <returns> expression text</returns>
        public string Text
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary> Returns the type of events the iterable returns.</summary>
        /// <returns> event type of events the iterator returns
        /// </returns>
        public EventType EventType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the statement's current state
        /// </summary>
        /// <value></value>
        public EPStatementState State
        {
            get { return StatementDescriptor.State; }
        }

        /// <summary>
        /// Returns true if the statement state is started.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true for started statements, false for stopped or destroyed statements.
        /// </returns>
        public bool IsStarted
        {
            get { return StatementDescriptor.State == EPStatementState.STARTED; }
        }

        /// <summary>
        /// Returns true if the statement state is stopped.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true for stopped statements, false for started or destroyed statements.
        /// </returns>
        public bool IsStopped
        {
            get { return StatementDescriptor.State == EPStatementState.STOPPED; }
        }

        /// <summary>
        /// Returns true if the statement state is destroyed.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true for destroyed statements, false for started or stopped statements.
        /// </returns>
        public bool IsDisposed
        {
            get { return StatementDescriptor.State == EPStatementState.DESTROYED; }
        }

        /// <summary>
        /// Returns the system time in milliseconds of when the statement last change state.
        /// </summary>
        /// <value></value>
        /// <returns>time in milliseconds of last statement state change</returns>
        public long TimeLastStateChange
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets or sets the current subscriber instance that receives statement results.
        /// <para/>
        /// Only a single subscriber may be set for a statement. If this method is invoked twice
        /// any previously-set subscriber is no longer used.
        /// </summary>
        /// <value>The subscriber.</value>
        public object Subscriber
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>Returns true if statement is a pattern</summary>
        /// <returns>true if statement is a pattern</returns>
        public bool IsPattern
        {
            get { return StatementDescriptor.IsPattern; }
        }

        /// <summary>
        /// Returns EPL or pattern statement attributes provided in the statement text, if any.
        /// <para/>
        /// See the annotation <seealso cref="com.espertech.esper.client.annotation"/> namespace
        /// for additional attributes / annotations.
        /// </summary>
        public ICollection<Attribute> Attributes
        {
            get { return new Attribute[0]; }
        }

        /// <summary>
        /// Returns the name of the isolated service provided is the statement is currently
        /// isolated in terms of event visibility and scheduling, or returns null if the
        /// statement is live in the engine.
        /// </summary>
        /// <returns>
        /// isolated service name or null for statements that are not currently isolated
        /// </returns>
        public string ServiceIsolated
        {
            get { return null; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystStatement"/> class.
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="statementDescriptor">The statement descriptor.</param>
        public CatalystStatement(Catalyst adapter, StatementDescriptor statementDescriptor)
        {
            Adapter = adapter;
            StatementDescriptor = statementDescriptor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystStatement"/> class.
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="statementDescriptor">The statement descriptor.</param>
        /// <param name="userObject">The user object.</param>
        public CatalystStatement(Catalyst adapter, StatementDescriptor statementDescriptor, object userObject)
        {
            Adapter = adapter;
            StatementDescriptor = statementDescriptor;
            UserObject = userObject;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            RemoveAllEventHandlers();
        }

        /// <summary>
        /// Removes all event handlers.
        /// </summary>
        public void RemoveAllEventHandlers()
        {
            _eventHandler = null;
            if (_eventHandler == null)
            {
                _eventSubscriber.Dispose();
                _eventSubscriber = null;
            }
        }

        /// <summary> Start the statement.</summary>
        public void Start()
        {
            throw new NotSupportedException();
        }

        /// <summary> Stop the statement.</summary>
        public void Stop()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Add an event handler to the current statement and replays current statement 
        /// results to the handler.
        /// </summary>
        /// <param name="eventHandler">eventHandler that will receive events</param>
        public void AddEventHandlerWithReplay(UpdateEventHandler eventHandler)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<EventBean> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a concurrency-safe iterator that iterates over events representing statement results (pull API)
        /// in the face of concurrent event processing by further threads.
        /// </summary>
        public IEnumerator<EventBean> GetSafeEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
