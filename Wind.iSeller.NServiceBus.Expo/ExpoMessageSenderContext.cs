using System;
using Wind.iSeller.NServiceBus.Core.Context;
using Wind.iSeller.NServiceBus.Core.RPC;

namespace Wind.iSeller.NServiceBus.Expo
{
    /// <summary>
    /// Expo请求上下文
    /// </summary>
    public sealed class ExpoMessageSenderContext : IRpcMessageSenderContext
    {
        /// <summary>
        /// 目标App Class ID
        /// </summary>
        public int TargetAppClassId { get; set; }

        /// <summary>
        /// 目标命令ID
        /// </summary>
        public int TargetCommandId { get; set; }

        /// <summary>
        /// 超时时间(毫秒)
        /// </summary>
        public int CommandTimeout { get; set; }

        public RpcType RpcType { get { return RpcType.Expo; } }

        public ExpoMessageSenderContext()
        { }

        public void FillRemoteContext(RemoteServiceBusResponseContext remoteContext)
        {
            remoteContext["TargetAppClassId"] = this.TargetAppClassId.ToString();
            remoteContext["TargetCommandId"] = this.TargetCommandId.ToString();
            remoteContext["CommandTimeout"] = this.CommandTimeout.ToString();
        }
    }
}
