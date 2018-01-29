using System;
using Wind.iSeller.Framework.Core;

namespace Wind.iSeller.NServiceBus.Core.Exceptions
{
    /// <summary>
    /// ServiceBus异常
    /// </summary>
    [Serializable]
    public class WindServiceBusException : WindException
    {
        public WindServiceBusException(string message)
            : base(message)
        { }

        public WindServiceBusException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
