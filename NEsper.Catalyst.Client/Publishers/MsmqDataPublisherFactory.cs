///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml.Linq;

namespace NEsper.Catalyst.Client.Publishers
{
    public class MsmqDataPublisherFactory : IDataPublisherFactory
    {
        /// <summary>
        /// Initializes the specified publisher configuration.
        /// </summary>
        /// <param name="publisherConfiguration">The publisher configuration.</param>
        public void Initialize(XElement publisherConfiguration)
        {
        }

        /// <summary>
        /// Creates an event publisher.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public IDataPublisher CreatePublisher(Uri uri)
        {
            if (uri.Scheme == "msmq")
            {
                return new MsmqDataPublisher(uri.LocalPath);
            }

            return null;
        }
    }
}
