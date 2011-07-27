///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2011 Patchwork Consulting. All rights reserved.                      /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Configuration;
using System.Linq;
using com.espertech.esper.compat.logging;

namespace NEsper.Catalyst.Client.Configuration
{
    public class CatalystConfigurationSection : ConfigurationSection
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
        public Uri ControlManager
        {
            get { return (Uri)this["control-manager"]; }
            set { this["control-manager"] = value; }
        }

        public CatalystConfiguration AsConfiguration()
        {
            return new CatalystConfiguration(
                ControlManager,
                Consumers.Cast<ConsumerElement>().Select(
                    item => item.GetFactory()).ToList(),
                Publishers.Cast<PublisherElement>().Select(
                    item => item.GetFactory()).ToList()
                );
        }

        public static CatalystConfiguration GetDefaultConfiguration()
        {
            var defaultInstance = GetDefaultInstance();
            return defaultInstance != null
                       ? defaultInstance.AsConfiguration()
                       : null;
        }

        public static CatalystConfigurationSection GetDefaultInstance()
        {
            var appConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var catConfiguration = appConfiguration.Sections.OfType<CatalystConfigurationSection>().FirstOrDefault();
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
