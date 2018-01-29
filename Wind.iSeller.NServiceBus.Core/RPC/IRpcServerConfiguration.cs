using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wind.iSeller.NServiceBus.Core.RPC
{
    /// <summary>
    /// RPC服务配置
    /// </summary>
    public interface IRpcServerConfiguration
    {
        /// <summary>
        /// 是否启动服务
        /// </summary>
        bool IsServerStart { get; set; }
    }
}
