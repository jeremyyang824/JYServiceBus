using System;
using System.Collections.Generic;
using System.Configuration;

namespace Wind.iSeller.NServiceBus.Core.Configurations
{
    public class ServiceBusItemSection : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("isCurrent", IsRequired = false, DefaultValue = false)]
        public bool IsCurrent
        {
            get { return (bool)this["isCurrent"]; }
        }

        [ConfigurationProperty("expoServer", IsRequired = true)]
        public ServiceBusExpoServerSection ExpoServer
        {
            get { return (ServiceBusExpoServerSection)this["expoServer"]; }
        }

        //TODO: rabbitMqServer
    }
}
