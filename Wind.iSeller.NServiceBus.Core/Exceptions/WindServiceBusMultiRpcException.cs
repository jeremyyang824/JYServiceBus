using System;
using System.Collections;
using System.Collections.Generic;

namespace Wind.iSeller.NServiceBus.Core.Exceptions
{
    /// <summary>
    /// 代表一组WindServiceBusRpcException
    /// </summary>
    [Serializable]
    public class WindServiceBusMultiRpcException : WindServiceBusException, IEnumerable<WindServiceBusRpcException>
    {
        private List<WindServiceBusRpcException> rpcExceptionList = new List<WindServiceBusRpcException>();

        public int Count
        {
            get { return this.rpcExceptionList.Count; }
        }

        public WindServiceBusMultiRpcException(IEnumerable<WindServiceBusRpcException> exceptions)
            : base("WindServiceBusException Collection!")
        {
            rpcExceptionList.AddRange(exceptions);
        }

        public IEnumerator<WindServiceBusRpcException> GetEnumerator()
        {
            return this.rpcExceptionList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.rpcExceptionList.GetEnumerator();
        }
    }
}
