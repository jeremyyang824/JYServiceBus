using System;
using Wind.iSeller.Framework.Core.Dependency;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// 容器间通信服务
    /// </summary>
    public interface IRpcServer : IRpcMessageSender
    {
        /// <summary>
        /// RPC类型
        /// </summary>
        RpcType RpcType { get; }

        /// <summary>
        /// 服务配置
        /// </summary>
        IRpcServerConfiguration Configuration { get; }

        /// <summary>
        /// RPC请求上下文Builder
        /// </summary>
        IRpcMessageSenderContextBuilder ContextBuilder { get; }

        /// <summary>
        /// 服务当前状态
        /// </summary>
        RpcServerState CurrentServerState { get; }

        /// <summary>
        /// 启动RPC服务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止RPC服务
        /// </summary>
        void Stop();

        /// <summary>
        /// 设为维护模式（维护模式可发出请求，但不接收请求）
        /// </summary>
        void SetMaintenanceState(bool isSetMaintenanceState);
    }
}
