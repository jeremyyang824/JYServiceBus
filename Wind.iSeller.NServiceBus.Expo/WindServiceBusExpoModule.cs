using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.NServiceBus.Core;
using Wind.iSeller.NServiceBus.Core.RPC;

namespace Wind.iSeller.NServiceBus.Expo
{
    [DependsOn(typeof(WindServiceBusModule))]
    public class WindServiceBusExpoModule : WindModule
    {
        public override void PreInitialize()
        {
            
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            IocManager.Register<IRpcServer, ExpoServer>(Wind.iSeller.Framework.Core.Dependency.DependencyLifeStyle.Singleton);
            IocManager.Register<IRpcMessageSender, ExpoMessageSender>(Wind.iSeller.Framework.Core.Dependency.DependencyLifeStyle.Transient);
        }
    }
}
