using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wind.iSeller.Framework.AutoMapper;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.NServiceBus.Core;
using Wind.iSeller.NServiceBus.Core.MetaData;

namespace Wind.iSeller.NServiceBus.ZeroService
{
    /// <summary>
    /// ServiceBus内部提供的服务
    /// </summary>
    [ServiceAssemblyName("wind.iSeller.serviceBus.zero")]
    [DependsOn(typeof(WindServiceBusModule), typeof(WindAutoMapperModule))]
    public class WindServiceBusZeroServiceModule : WindModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.WindAutoMapper().Configurators.Add(mapper =>
            {
                //Add your custom AutoMapper mappings here...
                //mapper.CreateMap<,>()
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            //var result = ServiceBus.Instance.TriggerServiceCommand<GetAllScriptProxyCommand, GetAllScriptProxyCommandResult>(new GetAllScriptProxyCommand());
        }
    }
}
