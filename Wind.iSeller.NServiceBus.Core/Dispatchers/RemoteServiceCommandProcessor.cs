using System;
using System.Collections.Generic;
using System.Linq;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core.Context;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Core.Serialization;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Dispatchers
{
    /// <summary>
    /// 远程服务请求响应处理
    /// </summary>
    public class RemoteServiceCommandProcessor : ITransientDependency
    {
        private readonly ISerializer serializer;
        private readonly ServiceBusRegistry serviceBusRegistry;
        private readonly ServiceBus serviceBus;
        private readonly WindPerformanceCounter performanceCounter;

        //public ILogger Logger { get; set; }

        public RemoteServiceCommandProcessor(
            ServiceBusRegistry serviceBusRegistry, ServiceBus serviceBus, WindPerformanceCounter performanceCounter, ISerializer serializer)
        {
            this.serviceBusRegistry = serviceBusRegistry;
            this.serviceBus = serviceBus;
            this.performanceCounter = performanceCounter;
            this.serializer = serializer;
        }

        /// <summary>
        /// 响应远程请求
        /// </summary>
        public RpcTransportMessageResponse ProcessCommand(RpcTransportMessageRequest rpcRequest /*TODO: 请求上下文信息*/)
        {
            //本地服务发现
            ServiceCommandTypeInfo localCommandInfo = null;
            bool isLocalCommand = this.serviceBusRegistry.IsLocalServiceCommand(rpcRequest.ServiceUniqueName, out localCommandInfo);

            string responseMessageContent = null;
            if (isLocalCommand)
            {
                using (performanceCounter.BeginStopwatch(string.Format("localInvoke: {0}", rpcRequest.ServiceUniqueName.FullServiceUniqueName)))
                {
                    //本地服务
                    IServiceCommandResult commandResult = null;
                    var commandData = (IServiceCommand)this.serializer.DeserializeString(localCommandInfo.CommandType, rpcRequest.MessageContent);
                    bool triggeredLocal = serviceBus.triggerLocalCommand(localCommandInfo.CommandType, localCommandInfo.CommandResultType, commandData, out commandResult);
                    if (!triggeredLocal)
                    {
                        throw new WindServiceBusLocalServiceNotFoundException(rpcRequest.ServiceUniqueName.FullServiceUniqueName);
                    }

                    // 去除null判断，允许数据内容为null
                    //if (commandResult == null)
                    //{
                    //    throw new WindServiceBusException(string.Format("service command [{0}] process error!", rpcRequest.ServiceUniqueName.FullServiceUniqueName));
                    //}

                    responseMessageContent = this.serializer.SerializeString(commandResult);
                }
            }
            else
            {
                using (performanceCounter.BeginStopwatch(string.Format("remoteInvoke: {0}", rpcRequest.ServiceUniqueName.FullServiceUniqueName)))
                {
                    //远程服务
                    var resultWithContext = serviceBus.triggerRemoteCommand(rpcRequest.ServiceUniqueName, rpcRequest.MessageContent);
                    responseMessageContent = resultWithContext.ResponseMessageContent;
                }
            }

            //构建响应输出
            var messageHeader = new RpcTransportMessageHeader();
            var rpcResponse = new RpcTransportMessageResponse(MessageIdGenerator.CreateMessageId())
            {
                MessageHeader = messageHeader,
                CorrelationMessageId = rpcRequest.MessageId,
                ServiceUniqueName = rpcRequest.ServiceUniqueName,
                MessageContent = responseMessageContent
            };
            return rpcResponse;
        }

        /// <summary>
        /// 响应远程广播请求
        /// </summary>
        /// <param name="rpcRequest"></param>
        /// <returns></returns>
        public RpcTransportMessageResponse ProcessBroadcastCommand(RpcTransportMessageRequest rpcRequest)
        {
            List<string> resultList = new List<string>();

            //1. 本地调用
            using (performanceCounter.BeginStopwatch(string.Format("localBroadcastInvoke: {0}", rpcRequest.ServiceUniqueName.FullServiceUniqueName)))
            {
                ServiceCommandTypeInfo localCommandInfo = null;
                bool isLocalCommand = this.serviceBusRegistry.IsLocalServiceCommand(rpcRequest.ServiceUniqueName, out localCommandInfo);

                if (isLocalCommand)
                {
                    IServiceCommandResult commandResult = null;
                    var commandData = (IServiceCommand)this.serializer.DeserializeString(localCommandInfo.CommandType, rpcRequest.MessageContent);
                    bool triggeredLocal = serviceBus.triggerLocalCommand(localCommandInfo.CommandType, localCommandInfo.CommandResultType, commandData, out commandResult);

                    if (triggeredLocal)
                    {
                        string responseMessageContent = this.serializer.SerializeString(commandResult);
                        resultList.Add(responseMessageContent);
                    }
                }
            }

            using (performanceCounter.BeginStopwatch(string.Format("remoteBroadcastInvoke: {0}", rpcRequest.ServiceUniqueName.FullServiceUniqueName)))
            {
                //2. 远程调用
                var resultWithContext = serviceBus.broadcastRemoteCommand(rpcRequest.ServiceUniqueName, rpcRequest.MessageContent);
                resultList.AddRange(resultWithContext.Select(r => r.ResponseMessageContent));
            }

            //3. 构建响应输出
            var contentResult = this.serializer.CombineToArray(resultList);

            var messageHeader = new RpcTransportMessageHeader();
            var rpcResponse = new RpcTransportMessageResponse(MessageIdGenerator.CreateMessageId())
            {
                MessageHeader = messageHeader,
                CorrelationMessageId = rpcRequest.MessageId,
                ServiceUniqueName = rpcRequest.ServiceUniqueName,
                MessageContent = contentResult
            };
            return rpcResponse;
        }

    }
}
