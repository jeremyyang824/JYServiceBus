using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core;
using Wind.iSeller.NServiceBus.Core.Services;
using Wind.iSeller.NServiceBus.ZeroService.AppServices;
using Wind.iSeller.NServiceBus.ZeroService.Commands;
using Wind.iSeller.NServiceBus.ZeroService.Domain;

namespace Wind.iSeller.NServiceBus.ZeroService.Services
{
    /// <summary>
    /// 服务定义管理服务
    /// </summary>
    public class ServiceDefineService : ITransientDependency,
        IServiceCommandHandler<GetAllServiceDefineCommand, GetAllServiceDefineCommandResult>,
        IServiceCommandHandler<GetAllScriptProxyCommand, GetAllScriptProxyCommandResult>
    {
        private readonly ServiceDefineAppService serviceDefineAppService;
        private readonly IScriptProxyGenerator scriptProxyGenerator;

        public ServiceDefineService(
            ServiceDefineAppService serviceDefineAppService,
            IScriptProxyGenerator scriptProxyGenerator)
        {
            this.serviceDefineAppService = serviceDefineAppService;
            this.scriptProxyGenerator = scriptProxyGenerator;
        }

        /// <summary>
        /// 取得所有服务定义
        /// </summary>
        public GetAllServiceDefineCommandResult HandlerCommand(GetAllServiceDefineCommand command)
        {
            var serviceBusName = this.serviceDefineAppService.GetLocalServiceBusName();
            var commandDefineList = this.serviceDefineAppService.GetServiceCommandDefine();

            return new GetAllServiceDefineCommandResult
            {
                ServiceBusName = serviceBusName,
                CommandDefineList = commandDefineList
            };
        }

        /// <summary>
        /// 取得所有服务的js代理
        /// </summary>
        public GetAllScriptProxyCommandResult HandlerCommand(GetAllScriptProxyCommand command)
        {
            //取得所有服务定义
            var allCommandDefines = ServiceBus.Instance.BroadcastServiceCommand<GetAllServiceDefineCommand, GetAllServiceDefineCommandResult>(
                new GetAllServiceDefineCommand(), ex =>
                {
                    throw ex;
                })
                .SelectMany(c => c.CommandDefineList);

            //生成
            string script = this.scriptProxyGenerator.Build(allCommandDefines);
            return new GetAllScriptProxyCommandResult
            {
                ScriptContent = script
            };
        }
    }
}
