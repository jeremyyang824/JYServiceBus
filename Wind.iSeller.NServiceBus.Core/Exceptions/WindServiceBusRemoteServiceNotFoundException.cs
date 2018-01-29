using System;

namespace Wind.iSeller.NServiceBus.Core.Exceptions
{
    /// <summary>
    /// 远程服务（命令）未找到异常
    /// </summary>
    [Serializable]
    public class WindServiceBusRemoteServiceNotFoundException : WindServiceBusServiceNotFoundException
    {
        public WindServiceBusRemoteServiceNotFoundException(string serviceName)
            : base(string.Format("Remote service: [{0}] not found !", serviceName))
        { }

        public WindServiceBusRemoteServiceNotFoundException(string serviceName, string commandName)
            : base(string.Format("Remote service: [{0}] command: [{1}] not found !", serviceName, commandName))
        { }
    }
}
