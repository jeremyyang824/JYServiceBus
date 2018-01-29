using System;
using Wind.iSeller.Framework.Core.Reflection;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.MetaData
{
    /// <summary>
    /// 服务消息（命令/事件）全局唯一名称
    /// </summary>
    [Serializable]
    public sealed class ServiceUniqueNameInfo
    {
        /// <summary>
        /// 服务程序集名
        /// </summary>
        public string ServiceAssemblyName { get; private set; }

        /// <summary>
        /// 服务命令/事件名
        /// </summary>
        public string ServiceMessageName { get; private set; }

        /// <summary>
        /// 服务消息类型
        /// </summary>
        public ServiceMessageType MessageType { get; private set; }

        /// <summary>
        /// 服务命令全局唯一名
        /// </summary>
        public string FullServiceUniqueName
        {
            get { return string.Format("{0}.{1}", ServiceAssemblyName, ServiceMessageName); }
        }


        public ServiceUniqueNameInfo(Type commandType)
        {
            if (typeof(IServiceCommand).IsAssignableFrom(commandType))
            {
                var serviceCommandAttr = ReflectionHelper.GetSingleAttributeOrDefault<ServiceCommandNameAttribute>(commandType);
                if (serviceCommandAttr == null)
                    throw new WindServiceBusException(
                        string.Format("[{0}] should has a unique command name. please add 'ServiceCommandNameAttribute' to command define !", commandType.FullName));

                if (string.IsNullOrWhiteSpace(serviceCommandAttr.ServiceAssemblyName))
                    throw new WindServiceBusException(string.Format("[{0}] ServiceAssemblyName empty!", commandType.FullName));
                this.ServiceAssemblyName = serviceCommandAttr.ServiceAssemblyName;

                if (string.IsNullOrWhiteSpace(serviceCommandAttr.ServiceCommandName))
                    throw new WindServiceBusException(string.Format("[{0}] ServiceAssemblyName empty!", commandType.FullName));
                this.ServiceMessageName = serviceCommandAttr.ServiceCommandName;

                this.MessageType = ServiceMessageType.ServiceCommand;
            }
            //TODO: event name builder
            else
            {
                throw new WindServiceBusException(
                    string.Format("[{0}] should implement IServiceCommand/IServiceEvent interface !", commandType.FullName));
            }
        }

        public ServiceUniqueNameInfo(string serviceAssemblyName, string serviceMessageName, ServiceMessageType messageType)
        {
            if (string.IsNullOrWhiteSpace(serviceAssemblyName))
                throw new WindServiceBusException("serviceAssemblyName empty!", new ArgumentNullException("serviceAssemblyName"));
            this.ServiceAssemblyName = serviceAssemblyName;

            if (string.IsNullOrWhiteSpace(serviceMessageName))
                throw new WindServiceBusException("serviceMessageName empty!", new ArgumentNullException("serviceMessageName"));
            this.ServiceMessageName = serviceMessageName;

            this.MessageType = messageType;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ServiceUniqueNameInfo))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return string.Equals(this.FullServiceUniqueName, ((ServiceUniqueNameInfo)obj).FullServiceUniqueName, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return this.FullServiceUniqueName.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", this.MessageType.ToString(), this.FullServiceUniqueName);
        }

        /// <summary>
        /// 服务消息类型
        /// </summary>
        public enum ServiceMessageType
        {
            ServiceCommand = 1,
            ServiceEvent = 2
        }
    }
}
