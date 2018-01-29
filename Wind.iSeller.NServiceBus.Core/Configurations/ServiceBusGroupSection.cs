using System;
using System.Collections.Generic;
using System.Configuration;

namespace Wind.iSeller.NServiceBus.Core.Configurations
{
    [ConfigurationCollection(typeof(ServiceBusItemSection), AddItemName = "busServer",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ServiceBusGroupSection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceBusItemSection();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ServiceBusItemSection).Name;
        }
    }
}
