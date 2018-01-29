using System;
using System.Linq;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;

namespace Wind.iSeller.NServiceBus.Expo
{
    /// <summary>
    /// EXPO消息解析
    /// </summary>
    public sealed class ExpoCommandMessageParser
    {
        /// <summary>
        /// EXPO请求格式
        /// </summary>
        public sealed class ExpoMessageInput
        {
            /// <summary>
            /// 请求消息ID
            /// </summary>
            public string RequestMessageId { get; private set; }

            /// <summary>
            /// 服务程序集名
            /// </summary>
            public string ServiceAssemblyName { get; private set; }

            /// <summary>
            /// 服务程序集名
            /// </summary>
            public string ServiceCommandName { get; private set; }

            /// <summary>
            /// 请求消息体
            /// </summary>
            public string RequestMessageContent { get; private set; }


            public ExpoMessageInput(RpcTransportMessageRequest request)
            {
                if (request == null)
                    throw new ExpoMessageException("rpc transport message null exception!");

                if (string.IsNullOrWhiteSpace(request.MessageId))
                    throw new ExpoMessageException("rpc transport message RequestMessageId null exception!");
                if (request.ServiceUniqueName == null)
                    throw new ExpoMessageException("rpc transport message CommandUniqueName null exception!");
                if (request.MessageContent == null)
                    throw new ExpoMessageException("rpc transport message MessageContent null exception!");

                this.RequestMessageId = request.MessageId;
                this.ServiceAssemblyName = request.ServiceUniqueName.ServiceAssemblyName;
                this.ServiceCommandName = request.ServiceUniqueName.ServiceMessageName;
                this.RequestMessageContent = request.MessageContent;
            }

            public ExpoMessageInput(object[] expoMessageBody)
            {
                if (expoMessageBody == null || expoMessageBody.Length != 4)
                    throw new ExpoMessageException("expo input message empty or error format!");

                this.RequestMessageId = (string)expoMessageBody[0];
                this.ServiceAssemblyName = (string)expoMessageBody[1];
                this.ServiceCommandName = (string)expoMessageBody[2];
                this.RequestMessageContent = (string)expoMessageBody[3];

                if (string.IsNullOrWhiteSpace(this.RequestMessageId))
                    throw new ExpoMessageException("expo input message MessageId null exception!");
                if (string.IsNullOrWhiteSpace(this.ServiceAssemblyName))
                    throw new ExpoMessageException("expo input message ServiceAssemblyName null exception!");
                if (string.IsNullOrWhiteSpace(this.ServiceCommandName))
                    throw new ExpoMessageException("expo input message ServiceCommandName null exception!");
                if (this.RequestMessageContent == null)
                    throw new ExpoMessageException("expo input message RequestMessageContent null exception!");
            }

            /// <summary>
            /// 创建RPC请求消息
            /// </summary>
            public RpcTransportMessageRequest BuidRpcTransportMessage()
            {
                return new RpcTransportMessageRequest(this.RequestMessageId)
                {
                    MessageHeader = new RpcTransportMessageHeader(),
                    ServiceUniqueName = new ServiceUniqueNameInfo(this.ServiceAssemblyName, this.ServiceCommandName, ServiceUniqueNameInfo.ServiceMessageType.ServiceCommand),
                    MessageContent = this.RequestMessageContent
                };
            }

            /// <summary>
            /// 创建Expo消息体
            /// </summary>
            public object[] BuidExpoMessageBody()
            {
                return new object[] {
                    this.RequestMessageId.ToString(),
                    this.ServiceAssemblyName,
                    this.ServiceCommandName,
                    this.RequestMessageContent
                };
            }
        }


        /// <summary>
        /// EXPO响应格式
        /// </summary>
        public sealed class ExpoMessageOutput
        {
            /// <summary>
            /// 响应消息ID
            /// </summary>
            public string ResponseMessageId { get; private set; }

            /// <summary>
            /// 响应内容
            /// </summary>
            public Wind.Comm.HugeString ResponseContent { get; private set; }

            /// <summary>
            /// 响应码
            /// </summary>
            public int ResponseCode { get; set; }

            /// <summary>
            /// 异常信息
            /// </summary>
            public string ErrorInfo { get; set; }


            public ExpoMessageOutput(RpcTransportMessageResponse response)
            {
                if (response == null)
                    throw new ExpoMessageException("rpc transport message null exception!");

                if (string.IsNullOrWhiteSpace(response.MessageId))
                    throw new ExpoMessageException("rpc transport message MessageId null exception!");
                if (response.ServiceUniqueName == null)
                    throw new ExpoMessageException("rpc transport message ServiceUniqueName null exception!");
                //if (response.MessageContent == null)
                //    throw new ExpoMessageException("rpc transport message MessageContent null exception!");

                this.ResponseMessageId = response.MessageId;
                this.ResponseContent = new Comm.HugeString(response.MessageContent);
                this.ResponseCode = (int)response.ResponseCode;
                this.ErrorInfo = response.ErrorInfo;
            }

            public ExpoMessageOutput(object[] expoMessageBody)
            {
                if (expoMessageBody == null || expoMessageBody.Length != 4)
                    throw new ExpoMessageException("expo output message empty or error format!");

                this.ResponseMessageId = (string)expoMessageBody[0];
                this.ResponseContent = this.convertExpoMessageBodyContent(expoMessageBody[1]);
                this.ResponseCode = (int)expoMessageBody[2];
                this.ErrorInfo = (string)expoMessageBody[3];

                if (string.IsNullOrWhiteSpace(this.ResponseMessageId))
                    throw new ExpoMessageException("expo output message ResponseMessageId null exception!");
                if (this.ResponseContent == null)
                    throw new ExpoMessageException("expo output message MessageContent null exception!");
            }

            private Comm.HugeString convertExpoMessageBodyContent(object expoMessageBodyContent)
            {
                var expoMessageBodyContentType = expoMessageBodyContent.GetType();
                if (expoMessageBodyContentType == typeof(Comm.HugeString))
                {
                    return (Comm.HugeString)expoMessageBodyContent;
                }
                else if (expoMessageBodyContentType == typeof(string))
                {
                    return new Comm.HugeString((string)expoMessageBodyContent);
                }
                else
                {
                    throw new ExpoMessageException(string.Format("error message type:[{0}]!", expoMessageBodyContentType.FullName));
                }
            }

            /// <summary>
            /// 创建RPC响应消息
            /// </summary>
            /// <param name="requestMessageId">请求消息ID</param>
            public RpcTransportMessageResponse BuidRpcTransportMessage(RpcTransportMessageRequest requestMessage)
            {
                if (requestMessage == null)
                    throw new ArgumentNullException("requestMessage");

                return new RpcTransportMessageResponse(this.ResponseMessageId)
                {
                    MessageHeader = new RpcTransportMessageHeader(),
                    CorrelationMessageId = requestMessage.MessageId,
                    ServiceUniqueName = requestMessage.ServiceUniqueName,
                    MessageContent = this.ResponseContent.Value,
                    ResponseCode = (RpcTransportResponseCode)this.ResponseCode,
                    ErrorInfo = this.ErrorInfo
                };
            }

            /// <summary>
            /// 创建Expo消息体
            /// </summary>
            public object[] BuidExpoMessageBody()
            {
                return new object[] {
                    this.ResponseMessageId.ToString(),
                    this.ResponseContent,
                    this.ResponseCode,
                    this.ErrorInfo
                };
            }
        }
    }
}
