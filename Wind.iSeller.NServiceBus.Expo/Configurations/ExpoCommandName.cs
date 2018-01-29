using System;

namespace Wind.iSeller.NServiceBus.Expo.Configurations
{
    /// <summary>
    /// EXPO命令
    /// </summary>
    public enum ExpoCommandName
    {
        /// <summary>
        /// ServcieBus标准接口
        /// </summary>
        ServiceBusCommand = 3903,

        /// <summary>
        /// ServiceBus广播接口
        /// </summary>
        ServiceBusBroadcastCommand = 3902,

        /// <summary>
        /// iSeller旧版接入接口
        /// </summary>
        ISellerLegacyCommand = 3906
    }
}
