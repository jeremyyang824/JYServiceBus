using System;
using System.Collections.Generic;
using System.Linq;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.MetaData;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// RPC服务管理
    /// </summary>
    public class RpcServerManager : IRpcMessageSender, ISingletonDependency
    {
        private readonly IIocResolver iocResolver;
        private readonly IRpcServer[] rpcServers = new IRpcServer[0];

        public RpcServerManager(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
            rpcServers = iocResolver.ResolveAll<IRpcServer>();
        }

        /// <summary>
        /// 根据配置启动对应的RPC服务
        /// </summary>
        public void StartServers()
        {
            foreach (IRpcServer rpcServer in this.rpcServers)
            {
                if (rpcServer.Configuration.IsServerStart)
                {
                    rpcServer.Start();
                }
            }
        }

        /// <summary>
        /// 关闭服务
        /// </summary>
        public void StopServers()
        {
            foreach (IRpcServer rpcServer in this.rpcServers)
            {
                rpcServer.Stop();
            }
        }

        /// <summary>
        /// 设为维护模式
        /// </summary>
        public void SetMaintenanceState(bool isSetMaintenanceState)
        {
            foreach (IRpcServer rpcServer in this.rpcServers)
            {
                rpcServer.SetMaintenanceState(isSetMaintenanceState);
            }
        }

        /// <summary>
        /// 远程服务发现
        /// (该方法不会发起远程调用)
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>返回服务请求上下文信息列表</returns>
        public IList<IRpcMessageSenderContext> DiscoverRemoteService(ServiceUniqueNameInfo serviceName)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            //TODO: 考虑RpcServer的优先级（路由）
            List<IRpcMessageSenderContext> contextFullList = new List<IRpcMessageSenderContext>();
            foreach (IRpcServer rpcServer in this.rpcServers)
            {
                //只从启动的RPC服务中查找服务信息
                if (rpcServer.Configuration.IsServerStart)
                {
                    //根据服务名，获取与目标容器通信的RpcServer
                    var contextList = rpcServer.ContextBuilder.BuildSenderContext(serviceName);

                    if (contextList != null && contextList.Count > 0)
                    {
                        contextFullList.AddRange(contextList);
                    }
                }
            }
            return contextFullList;
        }

        /// <summary>
        /// 发送RPC消息
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="request">请求消息</param>
        /// <returns>响应消息</returns>
        public RpcTransportMessageResponse SendMessage(RpcTransportMessageRequest request, IRpcMessageSenderContext rpcContext)
        {
            IRpcServer rpcServer = rpcServers.FirstOrDefault(server => server.RpcType == rpcContext.RpcType);
            return rpcServer.SendMessage(request, rpcContext);
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
            List<RpcTransportMessageResponse> allResultMessage = new List<RpcTransportMessageResponse>();
            List<RpcTransportErrorResponse> allErrorResponse = new List<RpcTransportErrorResponse>();

            foreach (var ctxGrp in requestContext.GroupBy(ctx => ctx.RpcType))
            {
                IRpcServer rpcServer = rpcServers.FirstOrDefault(server => server.RpcType == ctxGrp.Key);
                IList<IRpcMessageSenderContext> requestContextGroup = ctxGrp.ToList();

                ICollection<RpcTransportErrorResponse> errorMessageGroup = null;
                ICollection<RpcTransportMessageResponse> resultMessageGroup = rpcServer.BroadcastMessage(request, requestContextGroup, out errorMessageGroup);

                allResultMessage.AddRange(resultMessageGroup);
                if (errorMessageGroup != null)
                {
                    allErrorResponse.AddRange(errorMessageGroup);
                }
            }

            errorResponse = allErrorResponse;
            return allResultMessage;
        }
    }
}
