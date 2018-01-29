using System;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// 通信服务状态
    /// </summary>
    public enum RpcServerState
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Closed = 0,

        /// <summary>
        /// 启动
        /// </summary>
        Started = 1,

        /// <summary>
        /// 错误
        /// </summary>
        Error = 2,
    }
}
