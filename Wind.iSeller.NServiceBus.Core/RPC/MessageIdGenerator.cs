using System;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    public static class MessageIdGenerator
    {
        /// <summary>
        /// RPC消息ID创建
        /// </summary>
        public static string CreateMessageId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
