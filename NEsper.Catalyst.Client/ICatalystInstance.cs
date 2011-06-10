using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    public interface ICatalystInstance
    {
        /// <summary>
        /// Gets or sets the catalyst adapter.
        /// </summary>
        /// <value>The catalyst adapter.</value>
        ICatalyst Adapter { get; }

        /// <summary>
        /// Gets the administrator.
        /// </summary>
        /// <value>The administrator.</value>
        ICatalystAdministrator Administrator { get; }

        /// <summary>
        /// Gets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        EPRuntime Runtime { get; }
    }
}