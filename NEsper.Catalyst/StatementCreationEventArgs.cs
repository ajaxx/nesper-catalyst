///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using com.espertech.esper.client;

namespace NEsper.Catalyst
{
    public class StatementCreationEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public IEngineInstance Instance { get; private set; }

        /// <summary>
        /// Gets or sets the statement.
        /// </summary>
        /// <value>The statement.</value>
        public EPStatement Statement { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatementCreationEventArgs"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="statement">The statement.</param>
        public StatementCreationEventArgs(IEngineInstance instance, EPStatement statement)
        {
            Instance = instance;
            Statement = statement;
        }
    }
}
