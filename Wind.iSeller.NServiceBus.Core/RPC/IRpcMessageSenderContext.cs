using System;
using Wind.iSeller.NServiceBus.Core.Context;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// RPC请求上下文
    /// </summary>
    public interface IRpcMessageSenderContext
    {
        /// <summary>
        /// RPC类型
        /// </summary>
        RpcType RpcType { get; }

        /// <summary>
        /// 填充远程上下文
        /// </summary>
        void FillRemoteContext(RemoteServiceBusResponseContext remoteContext);
    }
}
