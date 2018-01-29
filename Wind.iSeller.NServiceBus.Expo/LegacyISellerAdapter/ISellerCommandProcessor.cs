using System;
using Castle.Core.Logging;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core.Dispatchers;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Expo.Configurations;

namespace Wind.iSeller.NServiceBus.Expo.LegacyISellerAdapter
{
    /// <summary>
    /// 处理遗留ISeller命令
    /// </summary>
    public class ISellerCommandProcessor : ITransientDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly ExpoServer expoServer;
        private readonly LegancyISellerConfiguration legancyISellerConfiguration;

        /// <summary>
        /// 日志处理器
        /// </summary>
        public ILogger Logger { get; set; }

        public ISellerCommandProcessor(
            IIocResolver iocResolver, ExpoServer expoServer, LegancyISellerConfiguration legancyISellerConfiguration)
        {
            this.iocResolver = iocResolver;
            this.expoServer = expoServer;
            this.legancyISellerConfiguration = legancyISellerConfiguration;
        }

        public ISellerExpoResponse ProcessCommand(ISellerExpoRequest request, Action<RpcTransportMessageHeader> rpcMessageHeaderProcess)
        {
            string expoCommand = request.cmd;
            if (string.IsNullOrWhiteSpace(expoCommand))
                throw new WindServiceBusException("Expo Command Name Empty!");

            try
            {
                //1. 尝试在ServiceBus中找到可以响应的服务
                var commandProcessor = this.iocResolver.Resolve<RemoteServiceCommandProcessor>();
                var rpcRequest = request.BuidRpcTransportMessage(this.legancyISellerConfiguration);

                //构建请求环境
                if (rpcMessageHeaderProcess != null)
                {
                    rpcMessageHeaderProcess(rpcRequest.MessageHeader);
                }

                var rpcResponse = commandProcessor.ProcessCommand(rpcRequest);
                return new ISellerExpoResponse(rpcResponse);
            }
            catch (WindServiceBusServiceNotFoundException ex)
            {
                //服务未实现，记录日志
                this.Logger.WarnFormat("外部Expo请求:[{0}]未找到对应的服务实现！", expoCommand);

                //2. 转发给老版本iSeller系统处理
                var response = expoServer.MessageSender.SendMessageToLegencyISeller(request, this.legancyISellerConfiguration);
                return response;
            }
        }
    }
}
