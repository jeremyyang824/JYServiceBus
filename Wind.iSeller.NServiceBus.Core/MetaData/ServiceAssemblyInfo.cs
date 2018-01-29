using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Wind.iSeller.NServiceBus.Core.MetaData
{
    /// <summary>
    /// 服务程序集
    /// </summary>
    public sealed class ServiceAssemblyInfo
    {
        //（命令）服务信息注册表
        //[serviceFullName, serviceInfo]
        private readonly ConcurrentDictionary<string, ServiceCommandTypeInfo> serviceCommandDic
            = new ConcurrentDictionary<string, ServiceCommandTypeInfo>();

        //TODO: （事件）服务信息注册表

        //所属ServiceBus
        private readonly HashSet<ServiceBusServerInfo> serviceBusSet
            = new HashSet<ServiceBusServerInfo>();

        /// <summary>
        /// 服务程序集名称
        /// </summary>
        public string ServiceAssemblyName { get; private set; }


        public ServiceAssemblyInfo(string serviceAssemblyName)
        {
            this.ServiceAssemblyName = serviceAssemblyName;
        }


        #region 命令服务相关

        /// <summary>
        /// 注册命令元数据
        /// </summary>
        /// <param name="cmdInfo">命令元数据</param>
        /// <returns>是否注册成功</returns>
        public bool RegisterCommand(ServiceCommandTypeInfo cmdInfo)
        {
            if (!this.isCmdInfoValidated(cmdInfo))
            {
                return false;
            }

            bool isSuccessed = this.serviceCommandDic.TryAdd(cmdInfo.ServiceCommandUniqueName.FullServiceUniqueName, cmdInfo);
            return isSuccessed;
        }

        /// <summary>
        /// 取得所有注册命令
        /// </summary>
        /// <returns>命令信息列表</returns>
        public IList<ServiceCommandTypeInfo> GetAllCommands()
        {
            return new List<ServiceCommandTypeInfo>(serviceCommandDic.Values);
        }

        /// <summary>
        /// 本地已注册命令数
        /// </summary>
        /// <returns>命令数</returns>
        public int GetLocalCommandCount()
        {
            return serviceCommandDic.Count;
        }

        /// <summary>
        /// 获取已注册的命令元数据
        /// 获取失败返回null
        /// </summary>
        /// <param name="commandUniqueName">命令全局名</param>
        /// <returns>命令元数据</returns>
        public ServiceCommandTypeInfo TryGetCommand(string commandUniqueName)
        {
            ServiceCommandTypeInfo commandInfo = null;
            this.serviceCommandDic.TryGetValue(commandUniqueName, out commandInfo);
            return commandInfo;
        }

        /// <summary>
        /// 获取已注册的命令元数据
        /// 获取失败返回null
        /// </summary>
        /// <param name="commandUniqueName">命令全局名</param>
        /// <returns>命令元数据</returns>
        public ServiceCommandTypeInfo TryGetCommand(ServiceUniqueNameInfo commandUniqueName)
        {
            return this.TryGetCommand(commandUniqueName.FullServiceUniqueName);
        }

        private bool isCmdInfoValidated(ServiceCommandTypeInfo cmdInfo)
        {
            if (cmdInfo == null)
                return false;
            if (cmdInfo.ServiceCommandUniqueName == null || string.IsNullOrWhiteSpace(cmdInfo.ServiceCommandUniqueName.FullServiceUniqueName))
                return false;

            if (cmdInfo.CommandType == null || cmdInfo.CommandResultType == null)
                return false;

            if (cmdInfo.CommandHandlerFactory == null)
                return false;

            return true;
        }

        #endregion

        #region ServiceBus相关

        /// <summary>
        /// 添加ServiceBus注册信息
        /// </summary>
        public bool RegisterToServiceBus(ServiceBusServerInfo serviceBus)
        {
            if (!serviceBusSet.Contains(serviceBus))
            {
                return this.serviceBusSet.Add(serviceBus);
            }
            return false;
        }

        /// <summary>
        /// 取得ServiceBus信息
        /// </summary>
        /// <returns></returns>
        public IList<ServiceBusServerInfo> GetServiceBusInfo()
        {
            List<ServiceBusServerInfo> list = new List<ServiceBusServerInfo>();
            list.AddRange(this.serviceBusSet);
            return list;
        }

        #endregion

        #region 其他

        public override string ToString()
        {
            return this.ServiceAssemblyName;
        }

        public override int GetHashCode()
        {
            return (string.Format("[{0}]{1}", this.GetType().FullName, this.ServiceAssemblyName)).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ServiceAssemblyInfo))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return this.ServiceAssemblyName.Equals(((ServiceAssemblyInfo)obj).ServiceAssemblyName);
        }

        #endregion
    }
}
