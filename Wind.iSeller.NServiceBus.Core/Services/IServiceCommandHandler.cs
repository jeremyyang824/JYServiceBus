using System;
using Wind.iSeller.NServiceBus.Core.Commands;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Services
{
    /// <summary>
    /// 服务命令处理程序
    /// </summary>
    public interface IServiceCommandHandler
    { }

    /// <summary>
    /// 服务命令处理程序
    /// </summary>
    /// <typeparam name="TCommand">命令对象</typeparam>
    /// <typeparam name="TCommandResult">命令返回值</typeparam>
    public interface IServiceCommandHandler<TCommand, TCommandResult> : IServiceCommandHandler
        where TCommand : IServiceCommand<TCommandResult>
        where TCommandResult : IServiceCommandResult
    {
        /// <summary>
        /// 处理命令
        /// </summary>
        /// <param name="command">命令对象</param>
        /// <returns>命令返回值</returns>
        TCommandResult HandlerCommand(TCommand command);
    }

    /// <summary>
    /// 服务命令处理程序（无返回值）
    /// </summary>
    /// <typeparam name="TCommand">命令对象</typeparam>
    public interface IServiceCommandHandler<TCommand> : IServiceCommandHandler<TCommand, EmptyCommandResult>
        where TCommand : IServiceCommand<EmptyCommandResult>
    { }
}
