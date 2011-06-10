using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace NEsper.Catalyst.Common
{
    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "MapEvent")]
    public class MapEvent
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public XElement Atoms { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEvent"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="atoms">The atoms.</param>
        public MapEvent(string name, XElement atoms)
        {
            Name = name;
            Atoms = atoms;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEvent"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="atoms">The atoms.</param>
        public MapEvent(string name, IDictionary<string, object> atoms)
        {
            Name = name;
            Atoms = atoms.ToXElement();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEvent"/> class.
        /// </summary>
        public MapEvent()
        {
        }
    }
}
