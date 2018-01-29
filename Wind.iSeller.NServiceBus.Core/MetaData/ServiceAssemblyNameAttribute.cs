using System;

namespace Wind.iSeller.NServiceBus.Core.MetaData
{
    /// <summary>
    /// 标记模块为服务程序集
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceAssemblyNameAttribute : Attribute
    {
        /// <summary>
        /// 服务程序集名
        /// </summary>
        public string ServiceAssemblyName { get; set; }

        public ServiceAssemblyNameAttribute(string serviceAssemblyName)
        {
            this.ServiceAssemblyName = serviceAssemblyName;
        }
    }
}
