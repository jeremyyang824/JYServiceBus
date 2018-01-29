using System;
using Wind.iSeller.NServiceBus.Core.Context;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Dispatchers
{
    /// <summary>
    /// 服务派发
    /// </summary>
    public interface IServiceCommandDispatcher
    {
        /// <summary>
        /// 命令是否已注册
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <returns></returns>
        bool IsCommandRegistered(Type commandType);

        /// <summary>
        /// 派发服务命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandResultType">命令响应类型</param>
        /// <param name="commandData">命令实例</param>
        /// <returns>命令响应</returns>
        IServiceCommandResult DispatchCommand(Type commandType, Type commandResultType, IServiceCommand commandData);
    }
}
