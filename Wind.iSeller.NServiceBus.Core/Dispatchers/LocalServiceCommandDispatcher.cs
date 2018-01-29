using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Logging;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core.Context;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.Factories;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core.Dispatchers
{
    /// <summary>
    /// 本地服务派发
    /// </summary>
    public class LocalServiceCommandDispatcher
    {
        private readonly ServiceBusRegistry registry;
        private readonly ILogger logger;

        public LocalServiceCommandDispatcher(ServiceBusRegistry registry, ILogger logger)
        {
            this.registry = registry;
            this.logger = logger;
        }

        /// <summary>
        /// 派发服务命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandResultType">命令响应类型</param>
        /// <param name="commandData">命令实例</param>
        /// <returns>命令响应实例</returns>
        public IServiceCommandResult DispatchCommand(Type commandType, Type commandResultType, IServiceCommand commandData)
        {
            var commandName = new ServiceUniqueNameInfo(commandType);
            ServiceCommandTypeInfo commandTypeInfo;
            bool isLocalCommandFound = this.registry.IsLocalServiceCommand(commandName, out commandTypeInfo);
            if (!isLocalCommandFound)
            {
                throw new WindServiceBusLocalServiceNotFoundException(commandName.FullServiceUniqueName);
            }

            //从本地容器找
            IServiceCommandHandlerFactory handlerFactory = commandTypeInfo.CommandHandlerFactory;
            IServiceCommandHandler commandHandler = handlerFactory.CreateHandler(); //服务实例（命令处理者）

            try
            {
                Type commandHandlerType = typeof(IServiceCommandHandler<,>).MakeGenericType(commandType, commandResultType);
                MethodInfo method = commandHandlerType.GetMethod("HandlerCommand", new[] { commandType });
                var result = (IServiceCommandResult)method.Invoke(commandHandler, new object[] { commandData });

                return result;
            }
            catch (Exception ex)
            {
                //TODO: 异常处理
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw ex;
            }
            finally
            {
                //清理服务
                handlerFactory.ReleaseHandler(commandHandler);
            }
        }
    }
}
