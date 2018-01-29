using System;
using System.Collections.Generic;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.Services;
using Wind.iSeller.NServiceBus.ZeroService.Beans;

namespace Wind.iSeller.NServiceBus.ZeroService.Commands
{
    /// <summary>
    /// 取得所有服务定义
    /// </summary>
    [ServiceCommandName("wind.iSeller.serviceBus.zero", "getAllServiceDefineCommand")]
    public class GetAllServiceDefineCommand : IServiceCommand<GetAllServiceDefineCommandResult>
    {

    }

    /// <summary>
    /// 取得所有服务定义返回
    /// </summary>
    public class GetAllServiceDefineCommandResult : IServiceCommandResult
    {
        /// <summary>
        /// ServiceBus名称
        /// </summary>
        public string ServiceBusName { get; set; }

        /// <summary>
        /// 命令定义集合
        /// </summary>
        public IList<ServiceCommandDefine> CommandDefineList { get; set; }
    }
}
