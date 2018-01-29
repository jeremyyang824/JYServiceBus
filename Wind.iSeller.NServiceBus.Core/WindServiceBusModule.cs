using System;
using System.Configuration;
using System.Reflection;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.NServiceBus.Core.Configurations;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Core.Serialization;

namespace Wind.iSeller.NServiceBus.Core
{
    [DependsOn(typeof(WindKernelModule))]
    public class WindServiceBusModule : WindModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            IocManager.Register<ISerializer, ServiceBusJsonSerializer>();    //序列化类型

            IocManager.IocContainer.Install(new ServiceBusInstaller(IocManager));

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(), new ConventionalRegistrationConfig
            {
                InstallInstallers = false
            });

        }

        public override void PostInitialize()
        {

        }

        public override void Shutdown()
        {
            var serverManager = IocManager.Resolve<RpcServerManager>();
            serverManager.StopServers();
        }
    }
}
