using System;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;

namespace Wind.iSeller.NServiceBus.Expo.Configurations
{
    /// <summary>
    /// Expo服务配置
    /// </summary>
    public class ExpoServerConfiguration : IRpcServerConfiguration
    {
        /// <summary>
        /// 是否要启动该RPC服务
        /// </summary>
        public bool IsServerStart { get; set; }

        /// <summary>
        /// EXPO Class ID
        /// </summary>
        public int AppClassId { get; set; }

        /// <summary>
        /// Expo命令ID
        /// </summary>
        public int CommandId { get; set; }

        /// <summary>
        /// 超时时间(毫秒)
        /// </summary>
        public int CommandTimeout { get; set; }

        public ExpoServerConfiguration()
        {
            this.IsServerStart = true;
        }

        public ExpoServerConfiguration(ServiceBusServerInfo.ExpoServer localExpoConfig)
        {
            this.IsServerStart = localExpoConfig.IsStart;
            this.AppClassId = localExpoConfig.AppClassId;
            this.CommandId = localExpoConfig.CommandId;
            this.CommandTimeout = localExpoConfig.Timeout;
        }
    }
}
