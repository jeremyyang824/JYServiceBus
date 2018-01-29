using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wind.iSeller.Framework.Log4Net;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.NServiceBus.Core;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Expo;
using Wind.iSeller.NServiceBus.ZeroService;
using Wind.iSeller.Framework.NHibernate;
using Wind.iSeller.Framework.AutoMapper;

namespace Wind.iSeller.NServiceBus.Host
{
    [DependsOn(
        typeof(WindServiceBusModule),
        typeof(WindServiceBusExpoModule),
        typeof(WindCastleLog4NetModule),
        typeof(WindAutoMapperModule),
        typeof(WindNHibernateModule),
        typeof(WindServiceBusZeroServiceModule))]
    public class WindServiceBusHostModule : WindModule
    {
        private RpcServerManager serverManager = null;

        public override void PreInitialize()
        {

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            //所有模块Initialize完成

            //启动容器通信服务
            serverManager = IocManager.Resolve<RpcServerManager>();
            serverManager.StartServers();
            //设为维护模式（不接收请求）
            serverManager.SetMaintenanceState(true);
        }

        public override void PostInitialize()
        {
            //设为正常模式
            serverManager.SetMaintenanceState(false);
        }
    }
}
