using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Wind.iSeller.Framework.Core.Dependency;

namespace Wind.iSeller.NServiceBus.Core.Dispatchers
{
    /// <summary>
    /// 远端服务定义探测
    /// </summary>
    public class RemoteServiceInfoProbe : ITransientDependency
    {
        /// <summary>
        /// 日志处理器
        /// </summary>
        public ILogger Logger { get; set; }


    }
}
