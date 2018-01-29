using System;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Factories
{
    /// <summary>
    /// IServiceCommandHandler工厂
    /// </summary>
    public interface IServiceCommandHandlerFactory
    {
        /// <summary>
        /// 创建命令处理程序
        /// </summary>
        /// <returns></returns>
        IServiceCommandHandler CreateHandler();

        /// <summary>
        /// 注销命令处理程序
        /// </summary>
        /// <param name="handler">命令处理程序</param>
        void ReleaseHandler(IServiceCommandHandler handler);
    }
}
