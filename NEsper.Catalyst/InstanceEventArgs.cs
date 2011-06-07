///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace NEsper.Catalyst
{
    public class InstanceEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public IEngineInstance Instance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceEventArgs"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public InstanceEventArgs(IEngineInstance instance)
        {
            Instance = instance;
        }
    }
}
