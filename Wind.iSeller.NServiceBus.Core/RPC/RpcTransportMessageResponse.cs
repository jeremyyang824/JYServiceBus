using System;
using Wind.iSeller.NServiceBus.Core.MetaData;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// 通过RPC框架传输的响应消息
    /// </summary>
    [Serializable]
    public sealed class RpcTransportMessageResponse : RpcTransportMessage
    {
        /// <summary>
        /// 关联消息ID
        /// </summary>
        public string CorrelationMessageId { get; set; }

        /// <summary>
        /// 返回消息代码
        /// </summary>
        public RpcTransportResponseCode ResponseCode { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorInfo { get; set; }


        public RpcTransportMessageResponse(string messageId)
            : base(messageId)
        {
            this.ResponseCode = RpcTransportResponseCode.Success;
        }

        public override string ToString()
        {
            return string.Format("ResponseMessage: [{0}] [{1}]", MessageId.ToString(), SendDate);
        }

        /// <summary>
        /// 构建异常返回消息
        /// </summary>
        public static RpcTransportMessageResponse BuildErrorResponse(
            ServiceUniqueNameInfo serviceUniqueName, RpcTransportResponseCode errorCode, Exception ex)
        {
            if (serviceUniqueName == null)
                throw new ArgumentNullException("serviceUniqueName");

            var errorResp = new RpcTransportMessageResponse(MessageIdGenerator.CreateMessageId())
            {
                ResponseCode = errorCode,
                ServiceUniqueName = serviceUniqueName,
                ErrorInfo = ex == null ? string.Empty : ex.Message
            };
            return errorResp;
        }
    }

    /// <summary>
    /// 返回消息代码
    /// </summary>
    public enum RpcTransportResponseCode : int
    {
        Success = 100,

        CommandError = 200,
        CommandError_Format = 201,
        CommandError_InputFormat = 202,

        BusinessError = 300,

        SystemError = 400,

        ServiceBusError = 500,
    }
}
