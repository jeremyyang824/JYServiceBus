using System;

namespace Wind.iSeller.NServiceBus.Core.Context
{
    /// <summary>
    /// 带响应环境上下文信息的服务返回消息
    /// </summary>
    public sealed class ServiceResponseMessageWithResponseContext
    {
        public string ResponseMessageContent { get; private set; }

        /// <summary>
        /// 服务响应远程上下文
        /// </summary>
        public RemoteServiceBusResponseContext ResponseContext { get; private set; }

        public ServiceResponseMessageWithResponseContext(
            string responseMessage, RemoteServiceBusResponseContext context)
        {
            this.ResponseMessageContent = responseMessage;
            this.ResponseContext = context;
        }
    }
}
