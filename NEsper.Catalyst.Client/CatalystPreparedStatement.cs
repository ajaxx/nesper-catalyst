///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    class CatalystPreparedStatement : EPPreparedStatement
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        internal string Id { get; set; }

        /// <summary>
        /// Gets or sets the instance id.
        /// </summary>
        /// <value>The instance id.</value>
        internal string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the adapter.
        /// </summary>
        /// <value>The adapter.</value>
        internal Catalyst Adapter { get; private set; }

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        internal CatalystInstance Instance { get; private set; }

	    /// <summary>Sets the value of the designated parameter using the given object.</summary>
	    /// <param name="parameterIndex">the first parameter is 1, the second is 2, ...</param>
	    /// <param name="value">the object containing the input parameter value</param>
	    /// <exception name="EPException">if the substitution parameter could not be located</exception>
	    public void SetObject(int parameterIndex, Object value)
	    {
	        Instance.Administrator.SetObject(Id, parameterIndex, value);
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystPreparedStatement"/> class.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="id">The id.</param>
        public CatalystPreparedStatement(Catalyst adapter, string instanceId, string id)
        {
            Adapter = adapter;
            Instance = adapter.GetInstance(instanceId);
            InstanceId = instanceId;
            Id = id;
        }
    }
}
