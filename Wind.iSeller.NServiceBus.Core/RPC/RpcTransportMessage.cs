using System;
using Wind.iSeller.NServiceBus.Core.MetaData;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// 通过RPC框架传输的消息
    /// </summary>
    [Serializable]
    public abstract class RpcTransportMessage
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public string MessageId { get; private set; }

        /// <summary>
        /// 消息发送时间
        /// </summary>
        public DateTime SendDate { get; private set; }

        /// <summary>
        /// 目标消息名称
        /// </summary>
        public ServiceUniqueNameInfo ServiceUniqueName { get; set; }

        /// <summary>
        /// 消息头
        /// </summary>
        public RpcTransportMessageHeader MessageHeader { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MessageContent { get; set; }


        public RpcTransportMessage(string messageId)
        {
            this.MessageId = messageId;
            this.SendDate = DateTime.Now;
        }

        public override string ToString()
        {
            return string.Format("[{0}] [{1}]", MessageId.ToString(), SendDate);
        }
    }
}
