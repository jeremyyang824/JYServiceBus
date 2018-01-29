using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.Factories;
using Wind.iSeller.NServiceBus.Core.Configurations;

namespace Wind.iSeller.NServiceBus.Core.MetaData
{
    /// <summary>
    /// 全局环境Windbus注册表
    /// </summary>
    public class ServiceBusRegistry : ISingletonDependency
    {
        private readonly object locker = new object();

        //ServiceBus列表
        //[servicebusName, servicebusInfo]
        private readonly ConcurrentDictionary<string, ServiceBusServerInfo> serviceBusDic
            = new ConcurrentDictionary<string, ServiceBusServerInfo>();

        //服务程序集列表
        //[serviceAssemblyName, serviceAssemblyInfo]
        private readonly ConcurrentDictionary<string, ServiceAssemblyInfo> serviceAssemblyDic
            = new ConcurrentDictionary<string, ServiceAssemblyInfo>();

        /// <summary>
        /// 当前ServiceBus
        /// </summary>
        public ServiceBusServerInfo LocalBusServer { get; private set; }

        /// <summary>
        /// 日志处理器
        /// </summary>
        public ILogger Logger { get; set; }


        /// <summary>
        /// 取得所有服务程序集
        /// </summary>
        /// <returns>服务程序集列表</returns>
        public IList<ServiceAssemblyInfo> GetAllServiceAssemblies()
        {
            return new List<ServiceAssemblyInfo>(this.serviceAssemblyDic.Values);
        }

        #region 服务发现

        /// <summary>
        /// 服务（程序集）发现
        /// </summary>
        /// <param name="serviceAssemblyName">目标服务程序集名称</param>
        /// <returns>目标ServiceBus</returns>
        public IList<ServiceBusServerInfo> DiscoverRemoteBusInfo(string serviceAssemblyName)
        {
            ServiceAssemblyInfo serviceAssemblyInfo = this.getServiceAssemblyInfo(serviceAssemblyName);
            if (serviceAssemblyInfo != null)
            {
                var busList = serviceAssemblyInfo.GetServiceBusInfo()
                    .Where(busInfo => !busInfo.Equals(LocalBusServer))  //发现的ServiceBus不能包括自身
                    .ToList();
                return busList;
            }
            return null;    //服务程序集未找到
        }

        /// <summary>
        /// 是否本地服务命令
        /// </summary>
        /// <param name="targetServiceName">服务名</param>
        /// <returns></returns>
        public bool IsLocalServiceCommand(ServiceUniqueNameInfo serviceUniqueName)
        {
            ServiceCommandTypeInfo localCommandInfo = null;
            return this.IsLocalServiceCommand(serviceUniqueName, out localCommandInfo);
        }

        /// <summary>
        /// 是否本地服务命令
        /// </summary>
        /// <param name="targetServiceName">服务名</param>
        /// <returns></returns>
        public bool IsLocalServiceCommand(ServiceUniqueNameInfo serviceUniqueName, out ServiceCommandTypeInfo localCommandInfo)
        {
            localCommandInfo = null;

            ServiceAssemblyInfo serviceAssemblyInfo = this.getServiceAssemblyInfo(serviceUniqueName.ServiceAssemblyName);
            if (serviceAssemblyInfo != null)
            {
                ////是否本地ServiceBus包括的服务程序集
                //bool isServiceAssemblyLocal = serviceAssemblyInfo.GetServiceBusInfo()
                //    .Any(busInfo => busInfo.Equals(this.LocalBusServer));

                //if (!isServiceAssemblyLocal)
                //    return false;

                localCommandInfo = serviceAssemblyInfo.TryGetCommand(serviceUniqueName);
                if (localCommandInfo != null)
                    return true;
            }
            return false;    //服务程序集不存在
        }


        private ServiceAssemblyInfo getServiceAssemblyInfo(string serviceAssemblyName)
        {
            if (string.IsNullOrWhiteSpace(serviceAssemblyName))
                throw new ArgumentNullException("serviceAssemblyName");

            ServiceAssemblyInfo serviceAssemblyInfo = null;
            bool isServiceAssemblyExists = this.serviceAssemblyDic.TryGetValue(serviceAssemblyName.Trim(), out serviceAssemblyInfo);
            return serviceAssemblyInfo;
        }

        #endregion

        #region 服务初始化/注册

        /// <summary>
        /// 根据配置文件初始化注册表
        /// </summary>
        /// <param name="serviceBusSection">配置节</param>
        public ServiceBusRegistry Initital(ServiceBusSection serviceBusSection)
        {
            if (serviceBusSection == null)
                throw new ArgumentNullException("serviceBusSection");

            lock (locker)
            {
                serviceBusDic.Clear();
                serviceAssemblyDic.Clear();

                //初始化serviceBus列表
                foreach (ServiceBusItemSection busSection in serviceBusSection.BusGroup)
                {
                    string serviceBusName = busSection.Name.Trim();
                    ServiceBusServerInfo busInfo = new ServiceBusServerInfo(serviceBusName,
                        //expo config
                        new ServiceBusServerInfo.ExpoServer(
                            busSection.ExpoServer.AppClassId,
                            busSection.ExpoServer.CommandId,
                            busSection.ExpoServer.CommandTimeout,
                            busSection.ExpoServer.IsStart)
                    //TODO: rabbitmq config...
                    );
                    bool isSuccess = serviceBusDic.TryAdd(serviceBusName, busInfo);
                    if (isSuccess)
                    {
                        //当前ServiceBus
                        if (busSection.IsCurrent)
                        {
                            this.LocalBusServer = busInfo;
                        }
                    }
                    else
                    {
                        //TODO: serviceBus名称冲突处理
                        this.Logger.Error(string.Format("ServiceBus:[{0}] duplicated in configuration!", serviceBusName));
                    }
                }

                if (this.LocalBusServer == null)
                {
                    throw new WindServiceBusException("Can not located current busServer! Please set isCurrent attribute on busServer section.");
                }

                //初始化windService列表
                //遍历每个servicebus
                foreach (ServiceCollectionSection busServiceCollection in serviceBusSection.ServiceGroup)
                {
                    string serviceBusName = busServiceCollection.Name.Trim();

                    ServiceBusServerInfo busInfo = null;
                    bool isServiceBusExists = this.serviceBusDic.TryGetValue(serviceBusName, out busInfo);
                    if (isServiceBusExists)
                    {
                        //遍历每个服务程序集申明
                        foreach (ServiceItemSection serviceAssemblyItem in busServiceCollection)
                        {
                            string serviceAssemblyName = serviceAssemblyItem.Name.Trim();
                            this.RegisterServiceAssembly(serviceAssemblyName, busInfo);
                        }
                    }
                    else
                    {
                        //TODO: 未找到serviceBus名称
                        this.Logger.Error(string.Format("ServiceBus:[{0}] not found in configuration!", serviceBusName));
                    }
                }
            }
            return this;
        }

        /// <summary>
        /// 注册服务程序集
        /// </summary>
        /// <param name="serviceAssemblyName">服务程序集名</param>
        /// <param name="busInfo">servicebus信息</param>
        /// <returns></returns>
        public void RegisterServiceAssembly(string serviceAssemblyName, ServiceBusServerInfo busInfo)
        {
            if (serviceAssemblyName == null)
                throw new ArgumentNullException("serviceAssemblyName");
            if (busInfo == null)
                throw new ArgumentNullException("busInfo");

            ServiceAssemblyInfo serviceAssembly = null;
            bool isServiceAssemblyExists = serviceAssemblyDic.TryGetValue(serviceAssemblyName, out serviceAssembly);
            if (!isServiceAssemblyExists)
            {
                serviceAssembly = new ServiceAssemblyInfo(serviceAssemblyName);
                serviceAssemblyDic.TryAdd(serviceAssemblyName, serviceAssembly);
            }

            //serviceAssembly挂载到busServer上
            serviceAssembly.RegisterToServiceBus(busInfo);
        }

        /// <summary>
        /// 注册服务程序集
        /// </summary>
        /// <param name="serviceAssemblyName"></param>
        /// <returns></returns>
        public void RegisterServiceAssembly(string serviceAssemblyName)
        {
            this.RegisterServiceAssembly(serviceAssemblyName, this.LocalBusServer);
        }


        /// <summary>
        /// 注册服务命令
        /// </summary>
        /// <param name="cmdInfo">服务命令信息</param>
        /// <returns>是否注册成功</returns>
        public bool RegisterCommand(ServiceCommandTypeInfo cmdInfo)
        {
            lock (locker)
            {
                ServiceAssemblyInfo serviceAssembly = this.getServiceAssemblyInfo(cmdInfo.ServiceCommandUniqueName.ServiceAssemblyName);
                if (serviceAssembly != null)
                {
                    return serviceAssembly.RegisterCommand(cmdInfo);
                }
                return false;
            }
        }

        #endregion
    }

}
