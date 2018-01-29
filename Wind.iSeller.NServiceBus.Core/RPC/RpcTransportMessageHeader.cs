using System;
using System.Collections.Generic;
using Wind.iSeller.NServiceBus.Core.Context;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// RPC消息头
    /// </summary>
    [Serializable]
    public sealed class RpcTransportMessageHeader : Dictionary<string, string>
    {

    }
}
