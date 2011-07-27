///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Xml.Linq;

namespace NEsper.Catalyst.Client
{
    public interface IDataPublisher
    {
        /// <summary>
        /// Sends an event into the instance.
        /// </summary>
        /// <param name="event">The @event.</param>
        void SendEvent(XElement @event);
    }
}
