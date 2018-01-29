using System;

namespace Wind.iSeller.NServiceBus.Core.Exceptions
{
    /// <summary>
    /// 本地服务（命令）未能找到的异常
    /// </summary>
    [Serializable]
    public class WindServiceBusLocalServiceNotFoundException : WindServiceBusServiceNotFoundException
    {
        public WindServiceBusLocalServiceNotFoundException(Type commandType)
            : base(string.Format("Local service: [{0}] not found !", commandType.FullName))
        { }

        public WindServiceBusLocalServiceNotFoundException(string commandName)
            : base(string.Format("Local service command: [{0}] not found !", commandName))
        { }
    }
}
