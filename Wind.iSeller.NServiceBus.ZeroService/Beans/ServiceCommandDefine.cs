using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wind.iSeller.NServiceBus.ZeroService.Beans
{
    /// <summary>
    /// 命令定义
    /// </summary>
    public class ServiceCommandDefine : IComparable<ServiceCommandDefine>, IEquatable<ServiceCommandDefine>
    {
        /// <summary>
        /// 服务程序集名
        /// </summary>
        public string ServiceAssemblyName { get; set; }

        /// <summary>
        /// 服务命令名
        /// </summary>
        public string ServiceCommandName { get; set; }

        /// <summary>
        /// 请求类型
        /// </summary>
        public string InputTypeDefine { get; set; }

        /// <summary>
        /// 响应类型
        /// </summary>
        public string OutputTypeDefine { get; set; }


        public int CompareTo(ServiceCommandDefine other)
        {
            if (other == null)
                throw new ArgumentNullException("other");

            int cpr1 = string.Compare(this.ServiceAssemblyName, other.ServiceAssemblyName, true);
            if (cpr1 == 0)
            {
                int cpr2 = string.Compare(this.ServiceCommandName, other.ServiceCommandName, true);
                return cpr2;
            }
            else
            {
                return cpr1;
            }
        }

        public bool Equals(ServiceCommandDefine other)
        {
            if (other == null)
                return false;

            return this.CompareTo(other) == 0;
        }

        public override string ToString()
        {
            return this.ServiceAssemblyName + '.' + this.ServiceCommandName;
        }
    }
}
