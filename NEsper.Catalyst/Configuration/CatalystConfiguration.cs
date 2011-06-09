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
    }
}
