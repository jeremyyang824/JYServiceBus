using System;
using System.Collections.Generic;
using System.Configuration;

namespace Wind.iSeller.NServiceBus.Core.Configurations
{
    /// <summary>
    /// ServiceBus配置根节点
    /// </summary>
    public class ServiceBusSection : ConfigurationSection
    {
        /// <summary>
        /// WindBus组定义
        /// </summary>
        [ConfigurationProperty("busGroup", IsRequired = true, IsDefaultCollection = true)]
        public ServiceBusGroupSection BusGroup
        {
            get { return (ServiceBusGroupSection)this["busGroup"]; }
        }

        /// <summary>
        /// WindService组定义
        /// </summary>
        [ConfigurationProperty("serviceGroup", IsRequired = true, IsDefaultCollection = true)]
        public ServiceGroupSection ServiceGroup
        {
            get { return (ServiceGroupSection)this["serviceGroup"]; }
        }
    }
}
