using System;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Commands
{
    /// <summary>
    /// 无返回值命令的返回结果定义
    /// </summary>
    public class EmptyCommandResult : IServiceCommandResult
    {
        private EmptyCommandResult() { }

        /// <summary>
        /// 返回一个新的命令结果实例
        /// </summary>
        public static EmptyCommandResult Create()
        {
            return new EmptyCommandResult();
        }
    }
}
