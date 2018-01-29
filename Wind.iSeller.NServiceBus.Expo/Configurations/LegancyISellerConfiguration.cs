using System;
using Wind.iSeller.Framework.Core.Dependency;

namespace Wind.iSeller.NServiceBus.Expo.Configurations
{
    /// <summary>
    /// 老版iSeller配置
    /// </summary>
    public class LegancyISellerConfiguration : ISingletonDependency
    {
        /// <summary>
        /// EXPO Class ID
        /// </summary>
        public int AppClassId { get; set; }

        /// <summary>
        /// Expo命令ID
        /// </summary>
        public int CommandId { get; set; }

        /// <summary>
        /// 超时时限
        /// </summary>
        public int CommandTimeout { get; set; }

        /// <summary>
        /// 遗留程序集名称
        /// </summary>
        public string ServiceAssemblyName { get; set; }

        public LegancyISellerConfiguration()
        {
            this.AppClassId = 1363;
            this.CommandId = 3906;
            this.CommandTimeout = 3000;
            this.ServiceAssemblyName = "wind.iSeller.legacyManage";
        }
    }
}
