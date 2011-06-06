using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

using com.espertech.esper.client;

namespace NEsper.Catalyst.Common
{
    [DataContract(Namespace = "http://www.espertech.com")]
    public class StatementDescriptor
    {
        /// <summary>
        /// Gets or sets the unique identifier for the statement.
        /// </summary>
        /// <value>The id.</value>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the URI that defines the location of the event stream.
        /// </summary>
        /// <value>The URI.</value>
        [DataMember]
        public string URI { get; set; }

        /// <summary>Returns true if statement is a pattern</summary>
        /// <returns>true if statement is a pattern</returns>
        [DataMember]
        public bool IsPattern { get; set; }

        /// <summary>Gets the statement's current state</summary>
        [DataMember]
        public EPStatementState State { get; set; }
    }

    [CollectionDataContract(
        Name = "StatementDescriptorCollection",
        Namespace = "http://www.espertech.com",
        ItemName = "Descriptor")]
    public class StatementDescriptorCollection : Collection<StatementDescriptor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatementDescriptorCollection"/> class.
        /// </summary>
        public StatementDescriptorCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatementDescriptorCollection"/> class.
        /// </summary>
        /// <param name="list">The list.</param>
        public StatementDescriptorCollection(IList<StatementDescriptor> list) : base(list)
        {
        }
    }
}
