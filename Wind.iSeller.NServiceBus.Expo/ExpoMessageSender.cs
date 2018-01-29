using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Wind.Comm;
using Wind.Comm.Expo4;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Expo.Configurations;
using Wind.iSeller.NServiceBus.Expo.LegacyISellerAdapter;
using System.Threading;

namespace Wind.iSeller.NServiceBus.Expo
{
    /// <summary>
    /// Expo请求发送
    /// </summary>
    public class ExpoMessageSender : IRpcMessageSender
    {
        private readonly SimplePassiveAppServer appServer;

        public ExpoMessageSender(SimplePassiveAppServer appServer)
        {
            this.appServer = appServer;
        }

        public RpcTransportMessageResponse SendMessage(RpcTransportMessageRequest request, IRpcMessageSenderContext requestContext)
        {
            this.validateMessage(request);
            var context = this.validateExpoSenderContext(requestContext);

            return this.sendMessageCore(request, context);
        }

        public ICollection<RpcTransportMessageResponse> BroadcastMessage(
            RpcTransportMessageRequest request, IEnumerable<IRpcMessageSenderContext> requestContext, out ICollection<RpcTransportErrorResponse> errorResponse)
        {
            this.validateMessage(request);

            /*
            List<RpcTransportMessageResponse> resultResponse = new List<RpcTransportMessageResponse>();
            errorResponse = new List<RpcTransportErrorResponse>();
            foreach (var senderContext in requestContext)
            {
                ExpoMessageSenderContext context = null;
                try
                {
                    context = this.validateExpoSenderContext(senderContext);
                    RpcTransportMessageResponse response = this.sendMessageCore(request, context);
                    resultResponse.Add(response);
                }
                catch (Exception ex)
                {
                    RpcTransportErrorResponse errorMsg = new RpcTransportErrorResponse(request.MessageId, context, ex);
                    errorResponse.Add(errorMsg);
                }
            }
            return resultResponse;
            */

            //并行版本
            var requestContextList = requestContext.ToList();
            List<RpcTransportMessageResponse> resultResponse = new List<RpcTransportMessageResponse>();
            List<RpcTransportErrorResponse> errorResponseList = new List<RpcTransportErrorResponse>();

            using (var countdownEvent = new CountdownEvent(1))
            {
                for (int i = 0; i < requestContextList.Count; i++)
                {
                    countdownEvent.AddCount();

                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        int idx = (int)state;
                        ExpoMessageSenderContext context = null;
                        try
                        {
                            var senderContext = requestContextList[idx];

                            context = this.validateExpoSenderContext(senderContext);
                            RpcTransportMessageResponse response = this.sendMessageCore(request, context);
                            resultResponse.Add(response);
                        }
                        catch (Exception ex)
                        {
                            RpcTransportErrorResponse errorMsg = new RpcTransportErrorResponse(request.MessageId, context, ex);
                            errorResponseList.Add(errorMsg);
                        }
                        finally
                        {
                            countdownEvent.Signal();
                        }
                    }, i);
                }

                countdownEvent.Signal();
                countdownEvent.Wait();
            }

            errorResponse = errorResponseList;
            return resultResponse;
        }

        /// <summary>
        /// 向老版本iSeller发出EXPO请求，获取EXPO响应
        /// </summary>
        /// <param name="request">EXPO请求</param>
        /// <param name="configuration">EXPO响应</param>
        /// <returns></returns>
        public ISellerExpoResponse SendMessageToLegencyISeller(ISellerExpoRequest request, LegancyISellerConfiguration configuration)
        {
            //构建消息体
            var requestMsg = new Message();
            requestMsg.SetCommand((ushort)configuration.AppClassId, (uint)configuration.CommandId);

            byte[] jsonByte = Encoding.Default.GetBytes(request.ConvertToJson());
            requestMsg.FillBody(new object[] { jsonByte }, true);

            var expoSyncMessage = new WindMessageBus.SyncUserMessage(requestMsg);
            int resultVal = this.appServer.sendMessage(expoSyncMessage, configuration.CommandTimeout);

            //处理（抛出）异常
            if (expoSyncMessage.ErrInfo != null)
            {
                throw new WindServiceBusException(expoSyncMessage.ErrInfo);
            }

            var responseMsg = expoSyncMessage.Response;
            if (responseMsg.Header.CommandClass != configuration.AppClassId
                || responseMsg.Header.CommandValue != configuration.CommandId)
            {
                throw new WindServiceBusException("expo response not match request!");
            }

            if (responseMsg.isErrMsg())
            {
                string errorInfo = "unknow expo response exception";
                responseMsg.GetErrInfo(out errorInfo);
                throw new WindServiceBusException(errorInfo);
            }

            // 构建消息返回
            var expoResponse = responseMsg.ToAnswerObj();
            string jsonResult = Encoding.Default.GetString((expoResponse[0] as byte[]));
            return ISellerExpoResponse.ConvertFromJson(jsonResult);
        }


        private void validateMessage(RpcTransportMessageRequest request)
        {
            //验证消息
            if (string.IsNullOrWhiteSpace(request.MessageId))
                throw new WindServiceBusException("request message id empty error!");
        }

        private ExpoMessageSenderContext validateExpoSenderContext(IRpcMessageSenderContext requestContext)
        {
            //获取请求目标上下文
            var context = requestContext as ExpoMessageSenderContext;
            if (context == null)
                throw new WindServiceBusException("request context type error!");
            if (context.TargetAppClassId <= 0)
                throw new WindServiceBusException("target expo AppClassId shouled not be empty!");
            if (context.TargetCommandId <= 0)
                throw new WindServiceBusException("target expo CommandId shouled not be empty!");
            return context;
        }

        private RpcTransportMessageResponse sendMessageCore(RpcTransportMessageRequest request, ExpoMessageSenderContext context)
        {
            /*
            var proxy = RealProxy.InitInstanceByConnection(this.appServer);
            CommandHeader header = new CommandHeader((ushort)context.TargetAppClassId, (uint)context.TargetCommandId);
            var reqObj = (new ExpoCommandMessageParser.ExpoMessageInput(request)).BuidExpoMessageBody();
            object[] resObj = null;
            proxy.DoCommandProxy(header, reqObj, out resObj);
            var resultRpcMessage = (new ExpoCommandMessageParser.ExpoMessageOutput(resObj)).BuidRpcTransportMessage(request);
            return resultRpcMessage;
            */


            //构建消息体
            var requestMsg = new Message();
            requestMsg.SetCommand((ushort)context.TargetAppClassId, (uint)context.TargetCommandId);
            requestMsg.FillBody((new ExpoCommandMessageParser.ExpoMessageInput(request)).BuidExpoMessageBody(), true);

            //调用RPC
            var expoSyncMessage = new WindMessageBus.SyncUserMessage(requestMsg);
            int resultVal = this.appServer.sendMessage(expoSyncMessage, context.CommandTimeout);

            //处理（抛出）异常
            if (expoSyncMessage.ErrInfo != null)
            {
                throw new WindServiceBusException(expoSyncMessage.ErrInfo);
            }

            var responseMsg = expoSyncMessage.Response;
            if (responseMsg.Header.CommandClass != context.TargetAppClassId
                || responseMsg.Header.CommandValue != context.TargetCommandId)
            {
                throw new WindServiceBusException("expo response not match request!");
            }

            if (responseMsg.isErrMsg())
            {
                string errorInfo = "unknow expo response exception";
                responseMsg.GetErrInfo(out errorInfo);
                throw new WindServiceBusException(errorInfo);
            }

            //构建消息返回
            var expoResponse = responseMsg.ToAnswerObj();
            var resultRpcMessage = (new ExpoCommandMessageParser.ExpoMessageOutput(expoResponse)).BuidRpcTransportMessage(request);
            //resultRpcMessage.MessageHeader... //TODO: 其他响应消息头
            return resultRpcMessage;
        }
    }
}
