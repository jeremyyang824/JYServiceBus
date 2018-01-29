using System;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.ZeroService.Commands
{
    /// <summary>
    /// 取得所有服务的js代理
    /// </summary>
    [ServiceCommandName("wind.iSeller.serviceBus.zero", "getAllScriptProxyCommand")]
    public class GetAllScriptProxyCommand : IServiceCommand<GetAllScriptProxyCommandResult>
    {
    }

    public class GetAllScriptProxyCommandResult : IServiceCommandResult
    {
        /// <summary>
        /// js内容
        /// </summary>
        public string ScriptContent { get; set; }
    }
}
