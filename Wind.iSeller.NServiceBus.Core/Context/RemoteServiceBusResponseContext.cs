using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Wind.iSeller.NServiceBus.Core.RPC;

namespace Wind.iSeller.NServiceBus.Core.Context
{
    /// <summary>
    /// 远程服务响应上下文信息
    /// </summary>
    public sealed class RemoteServiceBusResponseContext : Dictionary<string, string>
    {
        public RemoteServiceBusResponseContext() { }

        /// <summary>
        /// 使用MessageHeader填充
        /// </summary>
        /// <param name="messageHeader"></param>
        public RemoteServiceBusResponseContext(RpcTransportMessageHeader messageHeader)
        {
            this.FillRemoteContext(messageHeader);
        }

        public void FillRemoteContext(RpcTransportMessageHeader messageHeader)
        {
            if (messageHeader != null)
            {
                foreach (var pair in messageHeader)
                {
                    this[pair.Key] = pair.Value;
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
