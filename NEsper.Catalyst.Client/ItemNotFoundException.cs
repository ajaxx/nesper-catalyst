using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.espertech.esper.client;

namespace NEsper.Catalyst.Client
{
    public class ItemNotFoundException : EPException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotFoundException"/> class.
        /// </summary>
        public ItemNotFoundException()
            : base("item not found")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotFoundException"/> class.
        /// </summary>
        /// <param name="message">error message</param>
        public ItemNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cause">The cause.</param>
        public ItemNotFoundException(string message, Exception cause) : base(message, cause)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemNotFoundException"/> class.
        /// </summary>
        /// <param name="cause">The cause.</param>
        public ItemNotFoundException(Exception cause) : base(cause)
        {
        }
    }
}
