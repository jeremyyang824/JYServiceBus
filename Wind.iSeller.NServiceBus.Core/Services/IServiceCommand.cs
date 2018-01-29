using System;

namespace Wind.iSeller.NServiceBus.Core.Services
{
    /// <summary>
    /// 服务命令
    /// </summary>
    /// <typeparam name="TCommandResult">命令返回值类型</typeparam>
    public interface IServiceCommand<TCommandResult> : IServiceCommand
        where TCommandResult : IServiceCommandResult
    {

    }

    /// <summary>
    /// 服务命令
    /// </summary>
    public interface IServiceCommand
    {

    }
}
