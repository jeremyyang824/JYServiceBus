using System;
using System.Collections.Generic;
using Wind.iSeller.NServiceBus.Core.MetaData;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// 建立用于特定RPC请求的Context
    /// </summary>
    public interface IRpcMessageSenderContextBuilder
    {
        /// <summary>
        /// 根据目标服务名称，建立请求上下文
        /// </summary>
        /// <param name="targetServiceName">服务名称</param>
        /// <returns>请求上下文列表</returns>
        IList<IRpcMessageSenderContext> BuildSenderContext(ServiceUniqueNameInfo targetServiceName);
    }
}
