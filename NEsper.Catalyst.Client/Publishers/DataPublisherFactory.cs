///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace NEsper.Catalyst.Client.Publishers
{
    public class DataPublisherFactory : IDataPublisherFactory
    {
        /// <summary>
        /// Gets the factory list.
        /// </summary>
        /// <value>The factory list.</value>
        public IList<IDataPublisherFactory> Factories { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPublisherFactory"/> class.
        /// </summary>
        public DataPublisherFactory()
        {
            Factories = new List<IDataPublisherFactory>();
        }

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
            foreach(var factory in Factories)
            {
                var publisher = factory.CreatePublisher(uri);
                if (publisher != null)
                {
                    return publisher;
                }
            }

            throw new ArgumentException("unable to handle URI of form " + uri, "uri");
        }
    }
}
