using System;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// 通过RPC框架传输的请求消息
    /// </summary>
    [Serializable]
    public sealed class RpcTransportMessageRequest : RpcTransportMessage
    {

        public RpcTransportMessageRequest(string messageId)
            : base(messageId)
        { }

        public override string ToString()
        {
            return string.Format("RequestMessage: [{0}] [{1}]", MessageId.ToString(), SendDate);
        }
    }
}
