namespace NEsper.Catalyst.Client
{
    public interface ICatalyst
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <returns></returns>
        CatalystInstance GetInstance(string instanceId);

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        /// <returns></returns>
        CatalystInstance GetDefaultInstance();
    }
}