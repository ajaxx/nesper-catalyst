///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Configuration;

namespace NEsper.Catalyst.Configuration
{
    public class CatalystConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("publishers", IsDefaultCollection = false)]
        public PublisherCollection Publishers
        {
            get { return (PublisherCollection)this["publishers"]; }
            set { this["publishers"] = value; }
        }

        [ConfigurationProperty("consumers", IsDefaultCollection = false)]
        public ConsumerCollection Consumers
        {
            get { return (ConsumerCollection)this["consumers"]; }
            set { this["consumers"] = value; }
        }
    }
}
