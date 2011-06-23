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
        Namespace = "http://www.patchwork-consulting.org")]
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

        /// <summary>
        /// Gets or sets the prepared statement id.
        /// </summary>
        /// <value>The prepared statement id.</value>
        [DataMember]
        public string PreparedStatementId { get; set; }
    }
}
