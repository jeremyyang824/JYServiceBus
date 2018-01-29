using System;
using System.Collections.Generic;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// 通过RpcServer发送消息
    /// </summary>
    public interface IRpcMessageSender
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <param name="requestContext">请求目标</param>
        /// <returns>响应消息</returns>
        RpcTransportMessageResponse SendMessage(RpcTransportMessageRequest request, IRpcMessageSenderContext requestContext);

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <param name="requestContext">请求目标集合</param>
        /// <param name="errorResponse">异常响应</param>
        /// <returns>响应消息集合</returns>
        ICollection<RpcTransportMessageResponse> BroadcastMessage(
            RpcTransportMessageRequest request, IEnumerable<IRpcMessageSenderContext> requestContext, out ICollection<RpcTransportErrorResponse> errorResponse);
    }
}
