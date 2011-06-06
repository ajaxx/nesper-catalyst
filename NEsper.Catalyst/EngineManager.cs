using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.espertech.esper.compat;

namespace NEsper.Catalyst
{
    /// <summary>
    /// EngineManager oversees all <seealso cref="EngineInstance" /> running in a given application 
    /// domain.  It represents the first entry point that an administrative application would see.
    /// </summary>
    public class EngineManager
    {
        /// <summary>
        /// Dictionary of all engines, indexed by their unique identifier.
        /// </summary>
        private readonly IDictionary<string, EngineInstance> _engineInstanceTable =
            new Dictionary<string, EngineInstance>();

        public event EventHandler<InstanceEventArgs> InstanceCreated;
        public event EventHandler<InstanceEventArgs> InstanceDestroyed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineManager"/> class.
        /// </summary>
        public EngineManager()
        {
        }

        /// <summary>
        /// Gets or sets the default instance.
        /// </summary>
        /// <value>The default instance.</value>
        public IEngineInstance DefaultInstance { get; internal set; }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public IEngineInstance GetInstance(string id)
        {
            lock (_engineInstanceTable) {
                return _engineInstanceTable.Get(id);
            }
        }

        /// <summary>
        /// Gets the instances associated with this manager.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEngineInstance> GetInstances()
        {
            lock(_engineInstanceTable) {
                return _engineInstanceTable.Values.ToArray();
            }
        }

        /// <summary>
        /// Creates an engine.
        /// </summary>
        /// <returns></returns>
        public IEngineInstance CreateInstance()
        {
            return CreateInstance(null);
        }

        /// <summary>
        /// Creates an engine instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public IEngineInstance CreateInstance(string name)
        {
            EngineInstance engineInstance = new EngineInstance{ Name = name };
            lock (_engineInstanceTable)
            {
                _engineInstanceTable[engineInstance.Id] = engineInstance;
            }

            if (InstanceCreated != null)
            {
                InstanceCreated(this, new InstanceEventArgs(engineInstance));
            }

            return engineInstance;
        }

        /// <summary>
        /// Destroys the intsance that is referred to by the id.
        /// </summary>
        /// <param name="instanceID">The engine ID.</param>
        public void DestroyInstance(string instanceID)
        {
            EngineInstance engineInstance;
            lock (_engineInstanceTable) {
                engineInstance = _engineInstanceTable.RemoveAndReturn(instanceID);
            }

            if (engineInstance != null) {
                engineInstance.Dispose();

                if (InstanceDestroyed != null) {
                    InstanceDestroyed(this, new InstanceEventArgs(engineInstance));
                }
            }
        }
    }
}
