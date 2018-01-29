using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Castle.Core.Logging;
using Wind.iSeller.NServiceBus.Core.Context;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Core.Serialization;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Dispatchers
{
    /// <summary>
    /// 远程服务派发
    /// </summary>
    public class RemoteServiceCommandDispatcher
    {
        private readonly RpcServerManager rpcServerManager;
        private readonly ISerializer serializer;
        private readonly ILogger _logger;

        public RemoteServiceCommandDispatcher(RpcServerManager rpcServerManager, ISerializer serializer, ILogger logger)
        {
            this.rpcServerManager = rpcServerManager;
            this.serializer = serializer;
            this._logger = logger;
        }

        public ServiceCommandResultWithResponseContext DispatchCommand(Type commandType, Type commandResultType, IServiceCommand commandData)
        {
            var commandUniqueName = new ServiceUniqueNameInfo(commandType);

            //1. 处理请求消息体
            string requestMessageContent = this.serializer.SerializeString(commandData);

            //2. 服务调用
            var response = this.DispatchCommand(commandUniqueName, requestMessageContent);

            //3. 处理响应消息体
            var commandResult = (IServiceCommandResult)this.serializer.DeserializeString(commandResultType, response.ResponseMessageContent);
            return new ServiceCommandResultWithResponseContext(commandResult, response.ResponseContext);
        }

        internal ServiceResponseMessageWithResponseContext DispatchCommand(ServiceUniqueNameInfo commandUniqueName, string requestMessageContent)
        {
            //1. 服务发现（找到所有可以请求的目标）
            var remoteContextList = this.rpcServerManager.DiscoverRemoteService(commandUniqueName);
            if (remoteContextList == null || remoteContextList.Count < 1)
            {
                throw new WindServiceBusRemoteServiceNotFoundException(commandUniqueName.FullServiceUniqueName);
            }

            //2. 依次尝试发现的目标服务程序集
            bool isDispatchSuccess = false;
            RemoteServiceBusResponseContext responseContext = null;
            string responseMessageContent = null;
            List<WindServiceBusRpcException> servicebusExceptionCollection = new List<WindServiceBusRpcException>();

            var contextQueue = new Queue<IRpcMessageSenderContext>(remoteContextList);
            while (!isDispatchSuccess && contextQueue.Count > 0)
            {
                var context = contextQueue.Dequeue();

                WindServiceBusRpcException busException;
                isDispatchSuccess = this.dispatchCommand(commandUniqueName, context, requestMessageContent,
                    out responseContext, out responseMessageContent, out busException);

                if (!isDispatchSuccess && busException != null)
                {
                    servicebusExceptionCollection.Add(busException);
                }
            }

            if (!isDispatchSuccess)
            {
                throw new WindServiceBusMultiRpcException(servicebusExceptionCollection);
            }

            return new ServiceResponseMessageWithResponseContext(responseMessageContent, responseContext);
        }

        private bool dispatchCommand(
            ServiceUniqueNameInfo commandUniqueName, IRpcMessageSenderContext context, string requestMessageContent,
            out RemoteServiceBusResponseContext responseContext, out string responseMessageContent, out WindServiceBusRpcException rpcException)
        {
            // 创建响应上下文
            responseMessageContent = null;
            rpcException = null;
            responseContext = new RemoteServiceBusResponseContext();
            context.FillRemoteContext(responseContext);

            // 通过RPC框架执行远程调用
            try
            {
                RpcTransportMessageRequest requestMessage = new RpcTransportMessageRequest(MessageIdGenerator.CreateMessageId())
                {
                    ServiceUniqueName = commandUniqueName,
                    MessageHeader = new RpcTransportMessageHeader
                    {
                        //TODO: 其他消息头
                    },
                    MessageContent = requestMessageContent,
                };
                RpcTransportMessageResponse responseMessage = this.rpcServerManager.SendMessage(requestMessage, context);

                //响应上下文
                if (responseMessage != null)
                {
                    responseContext.FillRemoteContext(responseMessage.MessageHeader);   //服务响应信息填入RemoteContext
                }

                //响应消息验证
                if (responseMessage == null || responseMessage.CorrelationMessageId == null
                    || responseMessage.CorrelationMessageId != requestMessage.MessageId
                    || responseMessage.MessageContent == null)
                {
                    throw new WindServiceBusException(string.Format("request [{0}] get error response !", requestMessage.MessageId));
                }

                if (responseMessage.ResponseCode != RpcTransportResponseCode.Success)
                {
                    throw new WindServiceBusException(string.Format("request [{0}] get error response: {1}", requestMessage.MessageId, responseMessage.ErrorInfo));
                }

                responseMessageContent = responseMessage.MessageContent;
                return true;    //请求成功
            }
            catch (Exception serviceBusException)
            {
                this._logger.Error("WindServiceBusException: " + commandUniqueName, serviceBusException);
                rpcException = new WindServiceBusRpcException(responseContext, serviceBusException);

                return false;   //请求失败
            }
        }



        public ICollection<ServiceCommandResultWithResponseContext> BroadcastCommand(Type commandType, Type commandResultType, IServiceCommand commandData)
        {
            var commandUniqueName = new ServiceUniqueNameInfo(commandType);

            //1. 处理请求消息体
            string requestMessageContent = this.serializer.SerializeString(commandData);

            //2. 服务调用
            var responseCollection = this.BroadcastCommand(commandUniqueName, requestMessageContent);

            //3. 处理响应消息体
            var commandResultCollection = responseCollection.Select(response =>
            {
                var commandResult = (IServiceCommandResult)this.serializer.DeserializeString(commandResultType, response.ResponseMessageContent);
                return new ServiceCommandResultWithResponseContext(commandResult, response.ResponseContext);
            }).ToList();

            return commandResultCollection;
        }

        /// <summary>
        /// 广播命令
        /// </summary>
        internal ICollection<ServiceResponseMessageWithResponseContext> BroadcastCommand(ServiceUniqueNameInfo commandUniqueName, string requestMessageContent)
        {
            //1. 服务发现（找到所有可以请求的目标）
            var remoteContextList = this.rpcServerManager.DiscoverRemoteService(commandUniqueName);
            if (remoteContextList == null || remoteContextList.Count < 1)
            {
                return new ServiceResponseMessageWithResponseContext[0];
                //throw new WindServiceBusRemoteServiceNotFoundException(commandUniqueName.FullServiceUniqueName);
            }

            //2. 广播请求
            RpcTransportMessageRequest requestMessage = new RpcTransportMessageRequest(MessageIdGenerator.CreateMessageId())
            {
                ServiceUniqueName = commandUniqueName,
                MessageHeader = new RpcTransportMessageHeader
                {
                    //TODO: 其他消息头
                },
                MessageContent = requestMessageContent,
            };

            ICollection<RpcTransportErrorResponse> errorCollection;
            var responseCollection = this.rpcServerManager.BroadcastMessage(requestMessage, remoteContextList, out errorCollection);

            var resultList = responseCollection.Select(resp =>
                new ServiceResponseMessageWithResponseContext(resp.MessageContent, new RemoteServiceBusResponseContext(resp.MessageHeader))).ToList();

            if (errorCollection != null && errorCollection.Count > 0)
            {
                //TODO: 异常列表返回处理
            }

            return resultList;
        }
    }
}
