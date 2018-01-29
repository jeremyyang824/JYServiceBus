using System;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// 通过RPC框架传输的响应异常封装
    /// </summary>
    public sealed class RpcTransportErrorResponse
    {
        /// <summary>
        /// 关联消息ID
        /// </summary>
        public string CorrelationMessageId { get; set; }

        /// <summary>
        /// 请求上下文
        /// </summary>
        public IRpcMessageSenderContext SenderContext { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception ErrorInfo { get; set; }


        public RpcTransportErrorResponse(
            string correlationMessageId, IRpcMessageSenderContext senderContext, Exception errorInfo)
        {
            this.CorrelationMessageId = correlationMessageId;
            this.SenderContext = senderContext;
            this.ErrorInfo = errorInfo;
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", this.SenderContext.RpcType.ToString(), ErrorInfo.Message);
        }
    }
}
