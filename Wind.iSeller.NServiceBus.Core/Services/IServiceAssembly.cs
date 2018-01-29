using System;

namespace Wind.iSeller.NServiceBus.Core.Services
{
    /// <summary>
    /// 服务程序集
    /// </summary>
    public interface IServiceAssembly
    {
        /// <summary>
        /// 服务程序集名
        /// </summary>
        string ServiceAssemblyName { get; }
    }
}
