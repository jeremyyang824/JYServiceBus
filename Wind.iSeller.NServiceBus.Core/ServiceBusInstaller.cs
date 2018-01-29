using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Modules;
using Wind.iSeller.Framework.Core.Reflection;
using Wind.iSeller.NServiceBus.Core.Configurations;
using Wind.iSeller.NServiceBus.Core.Factories;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core
{
    /// <summary>
    /// 服务总线自安装程序
    /// </summary>
    public class ServiceBusInstaller : IWindsorInstaller
    {
        private readonly IIocResolver iocResolver;
        private ServiceBus serviceBus;
        private ServiceBusRegistry serviceBusRegistry;

        //程序集模块 [serviceAssemblyName, Assembly]
        private Dictionary<string, Assembly> serviceAssemblyDic = new Dictionary<string, Assembly>();

        public ILogger Logger { get; set; }

        public ServiceBusInstaller(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
            this.Logger = iocResolver.Resolve<ILogger>();
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //全局服务注册表
            ServiceBusSection serviceBusSection = (ServiceBusSection)ConfigurationManager.GetSection("serviceBus");
            container.Register(Component.For<ServiceBusRegistry>().LifestyleSingleton());
            var busRegistry = container.Resolve<ServiceBusRegistry>();
            this.serviceBusRegistry = busRegistry.Initital(serviceBusSection);

            //ServiceBus初始化
            container.Register(
                Component.For<ServiceBus>().UsingFactoryMethod(() =>
                {
                    ServiceBus.Instance.IocResolver = iocResolver;
                    ServiceBus.Instance.Serializer = container.Resolve<Serialization.ISerializer>();
                    ServiceBus.Instance.Logger = iocResolver.Resolve<ILogger>();
                    ServiceBus.Instance.PerformanceCounter = iocResolver.Resolve<WindPerformanceCounter>();
                    return ServiceBus.Instance;
                }).LifestyleSingleton());
            this.serviceBus = container.Resolve<ServiceBus>();

            //获取服务程序集模块
            var moduleManager = iocResolver.Resolve<IWindModuleManager>();
            foreach (var windModuleInfo in moduleManager.Modules)
            {
                var serviceAssembAttr = ReflectionHelper.GetSingleAttributeOrDefault<ServiceAssemblyNameAttribute>(windModuleInfo.Type);
                if (serviceAssembAttr != null)
                {
                    string serviceAssemblyName = serviceAssembAttr.ServiceAssemblyName;
                    serviceAssemblyDic.Add(serviceAssemblyName, windModuleInfo.Assembly);

                    // 尝试注册未在配置文件中配置的本地程序集!!!
                    busRegistry.RegisterServiceAssembly(serviceAssemblyName);
                }
            }

            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        private void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            Type targetServiceType = handler.ComponentModel.Implementation;

            //注册命令服务
            if (!typeof(IServiceCommandHandler).IsAssignableFrom(targetServiceType))
            {
                return;
            }

            //对于一个服务类，可能实现了多个IServiceCommandHandler, 用以处理多种命令
            var interfaces = targetServiceType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (!typeof(IServiceCommandHandler).IsAssignableFrom(@interface))
                {
                    continue;
                }

                var genericArgs = @interface.GetGenericArguments(); //IServiceCommandHandler<TCommand, TCommandResult> 
                if (genericArgs.Length == 2)
                {
                    Type commandType = genericArgs[0];
                    Type commandResultType = genericArgs[1];

                    var serviceCommandName = new ServiceUniqueNameInfo(commandType);

                    //验证IServiceCommandHandler只能在对应的服务程序集模块中实现
                    Assembly shouldBeAssembly;
                    if (!this.serviceAssemblyDic.TryGetValue(serviceCommandName.ServiceAssemblyName, out shouldBeAssembly))
                    {
                        this.Logger.WarnFormat("ServiceAssembly [{0}] not found! Please check ServiceAssemblyNameAttribute in WindModule.", serviceCommandName.ServiceAssemblyName);
                        continue;
                    }
                    if (shouldBeAssembly != targetServiceType.Assembly)
                    {
                        this.Logger.WarnFormat("Service [{0}] CommandHandler should not be implemented in WindModule [{1}] !", serviceCommandName.FullServiceUniqueName, @interface.Assembly.FullName);
                        continue;
                    }

                    //（严重通过的 IServiceCommandHandler）注册到ServiceBusRegistry
                    this.serviceBusRegistry.RegisterCommand(
                        new ServiceCommandTypeInfo(
                            serviceCommandName,
                            commandType,
                            commandResultType,
                            new IocServiceCommandHandlerFactory(this.iocResolver, targetServiceType))
                    );

                }
            }

            //TODO: 注册事件服务
        }
    }
}
