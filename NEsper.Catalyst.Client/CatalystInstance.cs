///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    public class CatalystInstance
    {
        /// <summary>
        /// Gets or sets the catalyst adapter.
        /// </summary>
        /// <value>The catalyst adapter.</value>
        public Catalyst Adapter { get; private set; }

        /// <summary>
        /// Gets the admininstrator.
        /// </summary>
        /// <value>The admininstrator.</value>
        public EPAdministrator Admininstrator { get; private set; }

        /// <summary>
        /// Gets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        public EPRuntime Runtime { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystInstance"/> class.
        /// </summary>
        /// <param name="adapter">The catalyst adapter.</param>
        /// <param name="instanceId">The instance id.</param>
        public CatalystInstance(Catalyst adapter, string instanceId)
        {
            Adapter = adapter;
            Admininstrator = new CatalystAdministrator(adapter, instanceId);
            Runtime = new CatalystRuntime(adapter, instanceId);
        }
    }
}
