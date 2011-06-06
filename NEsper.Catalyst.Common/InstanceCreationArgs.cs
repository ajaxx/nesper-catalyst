using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace NEsper.Catalyst.Common
{
    [DataContract(Namespace = "http://www.espertech.com")]
    public class InstanceCreationArgs
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }
}
