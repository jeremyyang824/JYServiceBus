using System;
using System.Collections.Generic;
using System.Linq;
using Wind.iSeller.Framework.Core.Application.Services;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.ZeroService.Beans;
using Wind.iSeller.NServiceBus.ZeroService.Domain;

namespace Wind.iSeller.NServiceBus.ZeroService.AppServices
{
    /// <summary>
    /// 管理服务定义
    /// </summary>
    public class ServiceDefineAppService : ApplicationService
    {
        private readonly ServiceBusRegistry busRegistry;
        private readonly ServiceMsgTypeDefineBuilder typeBuilder;

        //忽略的服务程序集
        private readonly string[] filterExceptServiceAssemblies = new string[] {
            "wind.bond.serviceBus.zero"
        };

        public ServiceDefineAppService(ServiceBusRegistry busRegistry, ServiceMsgTypeDefineBuilder typeBuilder)
        {
            this.busRegistry = busRegistry;
            this.typeBuilder = typeBuilder;
        }

        /// <summary>
        /// 取得本地ServiceBusN名称
        /// </summary>
        /// <returns></returns>
        public string GetLocalServiceBusName()
        {
            return this.busRegistry.LocalBusServer.ServerName;
        }

        /// <summary>
        /// 取得本地ServiceBus中的命令定义
        /// </summary>
        /// <returns></returns>
        public List<ServiceCommandDefine> GetServiceCommandDefine()
        {
            var serviceAssemblies = busRegistry.GetAllServiceAssemblies()
                .Where(assemb => !filterExceptServiceAssemblies.Contains(assemb.ServiceAssemblyName));  //跳过忽略的服务程序集

            List<ServiceCommandDefine> commandDefineList = new List<ServiceCommandDefine>();
            foreach (var serviceAssemb in serviceAssemblies)
            {
                var serviceCommandInfos = serviceAssemb.GetAllCommands();
                foreach (var serviceCommandInfo in serviceCommandInfos)
                {
                    var inputTypeDefine = this.typeBuilder.Build(serviceCommandInfo.CommandType);
                    var outputTypeDefine = this.typeBuilder.Build(serviceCommandInfo.CommandResultType);

                    //构建DTO
                    commandDefineList.Add(new ServiceCommandDefine
                    {
                        ServiceAssemblyName = serviceCommandInfo.ServiceCommandUniqueName.ServiceAssemblyName,
                        ServiceCommandName = serviceCommandInfo.ServiceCommandUniqueName.ServiceMessageName,
                        InputTypeDefine = inputTypeDefine.GetFormatString(),
                        OutputTypeDefine = outputTypeDefine.GetFormatString()
                    });
                }
            }
            return commandDefineList;
        }
    }
}
