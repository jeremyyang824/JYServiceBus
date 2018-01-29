using System;

namespace Wind.iSeller.NServiceBus.Core.MetaData
{
    /// <summary>
    /// 服务命令名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceCommandNameAttribute : Attribute
    {
        /// <summary>
        /// 服务程序集名
        /// </summary>
        public string ServiceAssemblyName { get; set; }

        /// <summary>
        /// 服务命令标识名
        /// </summary>
        public string ServiceCommandName { get; set; }

        public ServiceCommandNameAttribute(string serviceAssemblyName, string serviceCommandName)
        {
            if (string.IsNullOrWhiteSpace(serviceAssemblyName))
                throw new ArgumentNullException("serviceAssemblyName");
            if (string.IsNullOrWhiteSpace(serviceCommandName))
                throw new ArgumentNullException("serviceCommandName");

            this.ServiceAssemblyName = serviceAssemblyName;
            this.ServiceCommandName = serviceCommandName;
        }
    }
}
