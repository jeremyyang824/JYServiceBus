using System;
using Wind.iSeller.NServiceBus.Core.Factories;

namespace Wind.iSeller.NServiceBus.Core.MetaData
{
    /// <summary>
    /// 服务命令信息
    /// </summary>
    public sealed class ServiceCommandTypeInfo
    {
        /// <summary>
        /// 全局唯一的命令名称
        /// </summary>
        public ServiceUniqueNameInfo ServiceCommandUniqueName { get; private set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        public Type CommandType { get; private set; }

        /// <summary>
        /// 命令响应类型
        /// </summary>
        public Type CommandResultType { get; private set; }

        /// <summary>
        /// 命令消费者工厂
        /// </summary>
        public IServiceCommandHandlerFactory CommandHandlerFactory { get; private set; }


        public ServiceCommandTypeInfo(
            ServiceUniqueNameInfo serviceCommandUniqueName,
            Type commandType,
            Type commandResultType,
            IServiceCommandHandlerFactory commandHandlerFactory)
        {
            this.ServiceCommandUniqueName = serviceCommandUniqueName;
            this.CommandType = commandType;
            this.CommandResultType = commandResultType;
            this.CommandHandlerFactory = commandHandlerFactory;
        }
    }
}
