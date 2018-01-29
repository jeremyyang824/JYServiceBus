using System;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Context
{
    /// <summary>
    /// 带响应环境上下文信息的服务返回值
    /// </summary>
    public sealed class ServiceCommandResultWithResponseContext
    {
        /// <summary>
        /// 服务响应值
        /// </summary>
        public IServiceCommandResult ServiceCommandResult { get; private set; }

        /// <summary>
        /// 服务响应远程上下文
        /// </summary>
        public RemoteServiceBusResponseContext ResponseContext { get; private set; }


        public ServiceCommandResultWithResponseContext(
            IServiceCommandResult result, RemoteServiceBusResponseContext context)
        {
            this.ServiceCommandResult = result;
            this.ResponseContext = context;
        }
    }
}
