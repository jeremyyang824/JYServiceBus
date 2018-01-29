using System;
using System.Collections.Generic;
using System.Configuration;

namespace Wind.iSeller.NServiceBus.Core.Configurations
{
    public class ServiceCollectionSection : ConfigurationElementCollection
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceItemSection();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ServiceItemSection).Name;
        }
    }
}
