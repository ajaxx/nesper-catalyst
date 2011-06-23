using System.Runtime.Serialization;

namespace NEsper.Catalyst.Common
{
    [DataContract(
        Namespace = "http://www.patchwork-consulting.org")]
    public class PreparedValueArgs
    {
        /// <summary>
        /// Gets or sets the index of the parameter.
        /// </summary>
        /// <value>The index of the parameter.</value>
        [DataMember]
        public int ParameterIndex { get; set; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        [DataMember]
        public string DataType { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        [DataMember]
        public string Data { get; set; }
    }
}
