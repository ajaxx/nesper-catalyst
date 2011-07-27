///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using com.espertech.esper.client;
using NEsper.Catalyst.Common;

namespace NEsper.Catalyst.Client
{
    public class CatalystInstance 
        : ICatalystInstance
    {
        /// <summary>
        /// Gets or sets the catalyst adapter.
        /// </summary>
        /// <value>The catalyst adapter.</value>
        public ICatalyst Adapter { get; private set; }

        /// <summary>
        /// Gets the administrator.
        /// </summary>
        /// <value>The administrator.</value>
        public ICatalystAdministrator Administrator { get; private set; }

        /// <summary>
        /// Gets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        public EPRuntime Runtime { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalystInstance"/> class.
        /// </summary>
        /// <param name="adapter">The catalyst adapter.</param>
        /// <param name="instanceDescriptor">The instance descriptor.</param>
        internal CatalystInstance(Catalyst adapter, InstanceDescriptor instanceDescriptor)
        {
            Adapter = adapter;
            Administrator = new CatalystAdministrator(adapter, instanceDescriptor.Id);
            Runtime = new CatalystRuntime(adapter, (CatalystAdministrator) Administrator, instanceDescriptor);
        }
    }
}
