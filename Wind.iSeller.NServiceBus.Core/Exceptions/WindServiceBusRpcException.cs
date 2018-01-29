using System;
using Wind.iSeller.NServiceBus.Core.Context;

namespace Wind.iSeller.NServiceBus.Core.Exceptions
{
    [Serializable]
    public class WindServiceBusRpcException : WindServiceBusException
    {
        /// <summary>
        /// 远程上下文
        /// </summary>
        public RemoteServiceBusResponseContext RemoteContext { get; private set; }

        public WindServiceBusRpcException(RemoteServiceBusResponseContext remoteContext, Exception innerException)
            : base(innerException.Message, innerException)
        {
            this.RemoteContext = remoteContext;
        }
    }
}
