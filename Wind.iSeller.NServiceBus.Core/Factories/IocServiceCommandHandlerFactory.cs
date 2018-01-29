using System;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Factories
{
    /// <summary>
    /// 从容器中获取服务消费者
    /// </summary>
    internal class IocServiceCommandHandlerFactory : IServiceCommandHandlerFactory
    {
        private readonly IIocResolver iocResolver;
        private readonly Type handlerType;

        public IocServiceCommandHandlerFactory(IIocResolver iocResolver, Type handlerType)
        {
            this.iocResolver = iocResolver;
            this.handlerType = handlerType;
        }

        public IServiceCommandHandler CreateHandler()
        {
            return (IServiceCommandHandler)iocResolver.Resolve(handlerType);
        }

        public void ReleaseHandler(IServiceCommandHandler handler)
        {
            //单件对象不会被释放
            iocResolver.Release(handler);
        }
    }
}
