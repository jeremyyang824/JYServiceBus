using System;
using System.Configuration;

namespace Wind.iSeller.NServiceBus.Core.Configurations
{
    public class ServiceItemSection : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }
    }
}
