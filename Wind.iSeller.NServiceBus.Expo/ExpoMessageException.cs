using System;
using Wind.iSeller.NServiceBus.Core.Exceptions;

namespace Wind.iSeller.NServiceBus.Expo
{
    /// <summary>
    /// EXPO消息解析异常
    /// </summary>
    [Serializable]
    public class ExpoMessageException : WindServiceBusException
    {
        public ExpoMessageException(string message)
            : base(message)
        { }
    }
}
