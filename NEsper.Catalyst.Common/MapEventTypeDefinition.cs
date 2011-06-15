///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Runtime.Serialization;

namespace NEsper.Catalyst.Common
{
    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "MapEventTypeDefinition")]
    public class MapEventTypeDefinition
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type map.
        /// </summary>
        /// <value>The type map.</value>
        [DataMember]
        public EventTypeAtom[] TypeMap { get; set; }

        /// <summary>
        /// Gets or sets the super types.
        /// </summary>
        /// <value>The super types.</value>
        [DataMember]
        public string[] SuperTypes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEventTypeDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeMap">The type map.</param>
        public MapEventTypeDefinition(string name, EventTypeAtom[] typeMap)
        {
            Name = name;
            TypeMap = typeMap;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEventTypeDefinition"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeMap">The type map.</param>
        /// <param name="superTypes">The super types.</param>
        public MapEventTypeDefinition(string name, EventTypeAtom[] typeMap, string[] superTypes)
        {
            Name = name;
            TypeMap = typeMap;
            SuperTypes = superTypes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEventTypeDefinition"/> class.
        /// </summary>
        public MapEventTypeDefinition()
        {
        }
    }

    [DataContract(
        Namespace = "http://www.patchwork-consulting.org",
        Name = "EventTypeAtom")]
    public class EventTypeAtom
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        [DataMember]
        public string TypeName { get; set; }
        /// <summary>
        /// Gets or sets the type declaration.
        /// </summary>
        /// <value>The type decl.</value>
        [DataMember]
        public EventTypeAtom[] TypeDecl { get; set; }
    }


}
