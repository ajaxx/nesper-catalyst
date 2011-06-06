using System.Runtime.Serialization;

namespace NEsper.Catalyst.Common
{
    [DataContract(Namespace = "http://www.espertech.com")]
    public class StatementCreationArgs
    {
        /// <summary>
        /// Gets or sets the statement text.
        /// </summary>
        /// <value>The statement text.</value>
        [DataMember]
        public string StatementText { get; set; }

        /// <summary>
        /// Gets or sets the name of the statement.
        /// </summary>
        /// <value>The name of the statement.</value>
        [DataMember]
        public string StatementName { get; set; }
    }
}
