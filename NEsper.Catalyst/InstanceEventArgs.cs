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
