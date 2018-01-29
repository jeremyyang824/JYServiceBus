using System;
using System.Collections.Generic;
using System.Linq;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Expo.Configurations;

namespace Wind.iSeller.NServiceBus.Expo
{
    /// <summary>
    /// 构建Expo请求上下文
    /// </summary>
    public class ExpoMessageSenderContextBuilder : IRpcMessageSenderContextBuilder
    {
        private readonly ExpoServerConfiguration expoServerConfiguration;
        private readonly ServiceBusRegistry serviceBusRegistry;

        public ExpoMessageSenderContextBuilder(
            ExpoServerConfiguration expoServerConfiguration, ServiceBusRegistry serviceBusRegistry)
        {
            this.expoServerConfiguration = expoServerConfiguration;
            this.serviceBusRegistry = serviceBusRegistry;
        }

        /// <summary>
        /// 根据目标服务名称，建立请求上下文
        /// </summary>
        /// <param name="targetServiceName">服务名称</param>
        /// <returns>请求上下文列表</returns>
        public IList<IRpcMessageSenderContext> BuildSenderContext(ServiceUniqueNameInfo targetServiceName)
        {
            IList<ServiceBusServerInfo> busInfoList = this.serviceBusRegistry.DiscoverRemoteBusInfo(targetServiceName.ServiceAssemblyName);
            if (busInfoList == null || busInfoList.Count < 1)
                return null;    //未发现服务，则返回null

            //TODO: 考虑路由优先级

            var contextList = busInfoList
                .Where(busInfo => busInfo.ExpoConfig != null && busInfo.ExpoConfig.IsStart)
                .Select(buildContext)
                .ToList();
            return contextList;
        }

        private IRpcMessageSenderContext buildContext(ServiceBusServerInfo busInfo)
        {
            return new ExpoMessageSenderContext
            {
                TargetAppClassId = busInfo.ExpoConfig.AppClassId,
                TargetCommandId = busInfo.ExpoConfig.CommandId,
                CommandTimeout = busInfo.ExpoConfig.Timeout,
            };
        }
    }
}
