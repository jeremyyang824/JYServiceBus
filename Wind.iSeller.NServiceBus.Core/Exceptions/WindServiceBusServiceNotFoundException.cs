using System;

namespace Wind.iSeller.NServiceBus.Core.Exceptions
{
    /// <summary>
    /// 未找到指定服务
    /// </summary>
    [Serializable]
    public abstract class WindServiceBusServiceNotFoundException : WindServiceBusException
    {
        public WindServiceBusServiceNotFoundException(string serviceName)
            : base(serviceName)
        { }
    }
}
