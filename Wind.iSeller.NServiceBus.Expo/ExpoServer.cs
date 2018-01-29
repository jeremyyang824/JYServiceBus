using System;
using Castle.Core.Logging;
using Wind.Comm;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Expo.Configurations;
using System.Collections.Generic;

namespace Wind.iSeller.NServiceBus.Expo
{
    /// <summary>
    /// 基于EXPO的通信服务
    /// </summary>
    public class ExpoServer : IRpcServer
    {
        private readonly IIocResolver iocResolver;
        private ServiceBusRegistry windbusRegistry;

        /// <summary>
        /// EXPO服务
        /// </summary>
        private SimplePassiveAppServer appServer;

        /// <summary>
        /// 服务当前状态
        /// </summary>
        public RpcServerState CurrentServerState { get; private set; }

        /// <summary>
        /// 服务配置
        /// </summary>
        public IRpcServerConfiguration Configuration { get; private set; }

        /// <summary>
        /// RPC请求上下文Builder
        /// </summary>
        public IRpcMessageSenderContextBuilder ContextBuilder { get; private set; }

        /// <summary>
        /// EXPO消息发送器
        /// </summary>
        public ExpoMessageSender MessageSender { get; private set; }

        /// <summary>
        /// 日志处理器
        /// </summary>
        public ILogger Logger { get; set; }

        public RpcType RpcType { get { return RpcType.Expo; } }

        public ExpoServer(IIocResolver iocResolver, ServiceBusRegistry windbusRegistry)
        {
            this.iocResolver = iocResolver;
            this.windbusRegistry = windbusRegistry;

            this.CurrentServerState = RpcServerState.Closed;
            Logger = NullLogger.Instance;

            this.init();
        }

        private void init()
        {
            this.appServer = new SimplePassiveAppServer();
            this.appServer.LogHandler = this.ExpoLogger;

            CommandManager cm = new CommandManager(this.iocResolver.Resolve<ExpoCommandStub>());
            this.appServer.CommandHandler = cm.DoCommand;
            this.appServer.MulticastCommandHandler = cm.DoMulticastCommand;
            this.appServer.BroadcastCommandHandler = cm.DoBroadcastCommand;

            this.MessageSender = new ExpoMessageSender(this.appServer);
            this.Configuration = new ExpoServerConfiguration(this.windbusRegistry.LocalBusServer.ExpoConfig);

            //建立contextBuilder
            this.ContextBuilder = new ExpoMessageSenderContextBuilder((ExpoServerConfiguration)Configuration, this.windbusRegistry);
        }

        public void Start()
        {
            try
            {
                if (this.CurrentServerState == RpcServerState.Started)
                    return;

                this.Logger.Info("开始启动Expo服务......");

                this.appServer.Start();

                this.CurrentServerState = RpcServerState.Started;
                this.Logger.Info("Expo服务已启动......");
            }
            catch (Exception ex)
            {
                this.CurrentServerState = RpcServerState.Error;
                this.Logger.Error("Expo服务启动失败......", ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (this.CurrentServerState == RpcServerState.Closed)
                    return;

                this.Logger.Info("开关闭Expo服务......");

                this.appServer.Stop();
                this.CurrentServerState = RpcServerState.Closed;

                this.Logger.Info("Expo已成功关闭......");
            }
            catch (Exception ex)
            {
                this.CurrentServerState = RpcServerState.Error;
                this.Logger.Error("Expo服务关闭失败......", ex);
            }
        }

        public void SetMaintenanceState(bool isSetMaintenanceState)
        {
            this.appServer.SetMaintenanceState(isSetMaintenanceState);
        }

        /// <summary>
        /// 发送RPC请求并获取响应
        /// </summary>
        public RpcTransportMessageResponse SendMessage(RpcTransportMessageRequest request, IRpcMessageSenderContext requestContext)
        {
            return this.MessageSender.SendMessage(request, requestContext);
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <param name="requestContext">请求目标集合</param>
        /// <returns>响应消息集合</returns>
        public ICollection<RpcTransportMessageResponse> BroadcastMessage(
            RpcTransportMessageRequest request, IEnumerable<IRpcMessageSenderContext> requestContext, out ICollection<RpcTransportErrorResponse> errorResponse)
        {
            return this.MessageSender.BroadcastMessage(request, requestContext, out errorResponse);
        }

        private void ExpoLogger(object sender, string message)
        {
            if (sender == null)
                return;

            this.Logger.Info(message);
        }
    }
}
