using System;
using System.Collections.Generic;
using System.Configuration;

namespace Wind.iSeller.NServiceBus.Core.Configurations
{
    [ConfigurationCollection(typeof(ServiceCollectionSection), AddItemName = "busServer",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ServiceGroupSection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceCollectionSection();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ServiceCollectionSection).Name;
        }
    }
}
