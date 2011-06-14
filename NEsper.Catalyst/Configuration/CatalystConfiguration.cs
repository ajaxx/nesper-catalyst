///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Configuration;
using System.Linq;
using com.espertech.esper.compat.logging;

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

        [ConfigurationProperty("control-manager", IsRequired = true)]
        public ControlManagerConfigurationElement ControlManager
        {
            get { return (ControlManagerConfigurationElement)this["control-manager"]; }
            set { this["control-manager"] = value; }
        }

        public static CatalystConfiguration GetDefaultInstance()
        {
            var appConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var catConfiguration = appConfiguration.Sections.OfType<CatalystConfiguration>().FirstOrDefault();
            if (catConfiguration == null)
            {
                Log.Warn("catalyst configuration section was not found");
            }

            return catConfiguration;
        }

        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}
