using System.Runtime.Serialization;

namespace NEsper.Catalyst.Common
{
    [DataContract(Namespace = "http://www.espertech.com")]
    public class InstanceDescriptor
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }
    }
}
