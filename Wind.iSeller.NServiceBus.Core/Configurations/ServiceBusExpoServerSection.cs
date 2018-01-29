using System;
using System.Collections.Generic;
using System.Configuration;

namespace Wind.iSeller.NServiceBus.Core.Configurations
{
    public class ServiceBusExpoServerSection : ConfigurationElement
    {
        [ConfigurationProperty("appClassId", IsRequired = true)]
        public int AppClassId
        {
            get { return (int)this["appClassId"]; }
        }

        [ConfigurationProperty("commandId", IsRequired = true)]
        public int CommandId
        {
            get { return (int)this["commandId"]; }
        }

        [ConfigurationProperty("commandTimeout", IsRequired = false, DefaultValue = 5000)]
        public int CommandTimeout
        {
            get { return (int)this["commandTimeout"]; }
        }

        [ConfigurationProperty("isStart", IsRequired = true)]
        public bool IsStart
        {
            get { return (bool)this["isStart"]; }
        }
    }
}
