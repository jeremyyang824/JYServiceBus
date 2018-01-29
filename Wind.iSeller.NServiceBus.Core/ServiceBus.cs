using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core;
using Castle.Core.Logging;
using Wind.iSeller.Framework.Core;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.Framework.Core.Threading;
using Wind.iSeller.NServiceBus.Core.Commands;
using Wind.iSeller.NServiceBus.Core.Context;
using Wind.iSeller.NServiceBus.Core.Dispatchers;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.Factories;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Core.Serialization;
using Wind.iSeller.NServiceBus.Core.Services;

namespace Wind.iSeller.NServiceBus.Core
{
    /// <summary>
    /// 服务总线
    /// </summary>
    public class ServiceBus : ISingletonDependency
    {
        /// <summary>
        /// 单例服务总线
        /// </summary>
        public static ServiceBus Instance { get; private set; }

        public IIocResolver IocResolver { get; set; }

        public ISerializer Serializer { get; set; }

        public WindPerformanceCounter PerformanceCounter { get; set; }

        /// <summary>
        /// 日志处理器
        /// </summary>
        public ILogger Logger { get; set; }


        //TODO: [事件类型, 事件消费者]
        //private readonly ConcurrentDictionary<Type, List<IServiceEventHandlerFactory>> _eventHandlerFactories;

        static ServiceBus()
        {
            Instance = new ServiceBus();
        }

        private ServiceBus()
        {
            Logger = NullLogger.Instance;
        }

        #region ServiceCommand

        /// <summary>
        /// 触发命令并获取返回值
        /// </summary>
        /// <typeparam name="TServiceCommand">命令定义</typeparam>
        /// <typeparam name="TCommandResult">命令返回类型</typeparam>
        /// <param name="commandData">命令实例</param>
        /// <param name="exceptionHandler">异常处理</param>
        public TCommandResult TriggerServiceCommand<TServiceCommand, TCommandResult>(
            TServiceCommand commandData, Action<Exception> exceptionHandler = null)
            where TServiceCommand : IServiceCommand<TCommandResult>
            where TCommandResult : IServiceCommandResult
        {
            RemoteServiceBusResponseContext remoteContext = null;
            return this.TriggerServiceCommand<TServiceCommand, TCommandResult>(commandData, exceptionHandler, out remoteContext);
        }

        /// <summary>
        /// 触发命令并获取返回值
        /// </summary>
        /// <typeparam name="TServiceCommand">命令定义</typeparam>
        /// <typeparam name="TCommandResult">命令返回类型</typeparam>
        /// <param name="commandData">命令实例</param>
        /// <param name="exceptionHandler">异常处理</param>
        /// <param name="remoteContext">远程上下文（如果为本地调用，则返回null）</param>
        public TCommandResult TriggerServiceCommand<TServiceCommand, TCommandResult>(
            TServiceCommand commandData, Action<Exception> exceptionHandler, out RemoteServiceBusResponseContext remoteContext)
            where TServiceCommand : IServiceCommand<TCommandResult>
            where TCommandResult : IServiceCommandResult
        {
            remoteContext = null;
            try
            {
                var cmdResult = this.TriggerServiceCommandCore(
                    typeof(TServiceCommand), typeof(TCommandResult), commandData, out remoteContext);
                return (TCommandResult)cmdResult;
            }
            catch (Exception ex)
            {
                if (exceptionHandler != null)
                {
                    exceptionHandler(ex);
                }
            }
            return default(TCommandResult);
        }


        /// <summary>
        /// 触发命令且无返回值
        /// </summary>
        /// <typeparam name="TServiceCommand">命令定义</typeparam>
        /// <param name="commandData">命令实例</param>
        /// <param name="exceptionHandler">异常处理</param>
        public EmptyCommandResult TriggerServiceCommand<TServiceCommand>(
            TServiceCommand commandData, Action<Exception> exceptionHandler = null)
            where TServiceCommand : IServiceCommand<EmptyCommandResult>
        {
            RemoteServiceBusResponseContext remoteContext = null;
            return this.TriggerServiceCommand<TServiceCommand>(commandData, exceptionHandler, out remoteContext);
        }

        /// <summary>
        /// 触发命令且无返回值
        /// </summary>
        /// <typeparam name="TServiceCommand">命令定义</typeparam>
        /// <param name="commandData">命令实例</param>
        /// <param name="exceptionHandler">异常处理</param>
        /// <param name="remoteContext">远程上下文（如果为本地调用，则返回null）</param>
        public EmptyCommandResult TriggerServiceCommand<TServiceCommand>(
            TServiceCommand commandData, Action<Exception> exceptionHandler, out RemoteServiceBusResponseContext remoteContext)
            where TServiceCommand : IServiceCommand<EmptyCommandResult>
        {
            remoteContext = null;
            try
            {
                var cmdResult = this.TriggerServiceCommandCore(
                    typeof(TServiceCommand), typeof(EmptyCommandResult), commandData, out remoteContext);
                return (EmptyCommandResult)cmdResult;
            }
            catch (Exception ex)
            {
                if (exceptionHandler != null)
                {
                    exceptionHandler(ex);
                }
            }
            return null;
        }

        /// <summary>
        /// 广播命令
        /// </summary>
        /// <typeparam name="TServiceCommand">命令服务类型</typeparam>
        /// <typeparam name="TCommandResult">命令服务返回结果类型</typeparam>
        /// <param name="commandData">命令服务数据</param>
        /// <param name="exceptionHandler">异常处理</param>
        /// <returns></returns>
        public IEnumerable<TCommandResult> BroadcastServiceCommand<TServiceCommand, TCommandResult>(
            TServiceCommand commandData, Action<Exception> exceptionHandler = null)
            where TServiceCommand : IServiceCommand<TCommandResult>
            where TCommandResult : IServiceCommandResult
        {
            try
            {
                var result = this.BroadcastServiceCommandCore(typeof(TServiceCommand), typeof(TCommandResult), commandData);
                return result.Cast<TCommandResult>();
            }
            catch (Exception ex)
            {

                if (exceptionHandler != null)
                {
                    exceptionHandler(ex);
                }
            }
            return default(IEnumerable<TCommandResult>);
        }

        /*
        /// <summary>
        /// 调用本地服务命令
        /// </summary>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandResultType">命令响应类型</param>
        /// <param name="commandData">命令实例</param>
        /// <returns>是否执行成功</returns>
        private bool TriggerServiceCommandLocal(
            Type commandType, Type commandResultType, IServiceCommand commandData,
            out ServiceCommandResultWithResponseContext commandResultData)
        {
            //TODO: 创建服务响应环境的上下文信息
            IServiceBusResponseContext responseContext = new BasicServiceBusResponseContext();

            //从本地容器找
            IServiceCommandHandlerFactory handlerFactory = null;
            bool isLocalExists = this._commandHandlerFactories.TryGetValue(commandType, out handlerFactory);
            if (isLocalExists)
            {
                responseContext.IsFromLocalService = true;
                IServiceCommandHandler commandHandler = handlerFactory.CreateHandler(); //服务实例（消费者）

                try
                {
                    Type commandHandlerType = typeof(IServiceCommandHandler<,>).MakeGenericType(commandType, commandResultType);
                    MethodInfo method = commandHandlerType.GetMethod("HandlerCommand", new[] { commandType });
                    var result = (IServiceCommandResult)method.Invoke(commandHandler, new object[] { commandData });
                    result.CommandId = commandData.CommandId;    //设置命令ID

                    commandResultData = new ServiceCommandResultWithResponseContext(result, responseContext);
                    return true;
                }
                catch (Exception ex)
                {
                    //TODO: 异常处理
                    throw ex;
                }
                finally
                {
                    //清理服务
                    handlerFactory.ReleaseHandler(commandHandler);
                }
            }
            commandResultData = null;
            return false;   //本地调用失败
        }
        */

        /*
        /// <summary>
        /// 调用远程服务命令
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandResultType">命令响应类型</param>
        /// <param name="commandData">命令实例</param>
        /// <returns>命令响应实例及响应上下文</returns>
        private ServiceCommandResultWithResponseContext TriggerServiceCommandRemote(
            string serviceName, Type commandType, Type commandResultType, IServiceCommand commandData)
        {
            //TODO: 创建服务响应环境的上下文信息
            IServiceBusResponseContext responseContext = new RemoteServiceBusResponseContext();

            //1. TODO: 处理请求消息体
            byte[] requestMessageContent = null;
            try
            {
                requestMessageContent = this.Serializer.Serialize(commandData).ToBytes();
            }
            catch (Exception ex)
            {
                //TODO: 消息内容序列化失败
                throw new WindServiceBusException("Serialize command data failed !", ex);
            }

            //2. 通过RPC框架执行远程调用
            byte[] responseMessageContent = null;
            try
            {
                var rpcServerManager = this.IocResolver.Resolve<RpcServerManager>();
                RpcTransportMessage requestMessage = new RpcTransportMessage(commandData.CommandId)
                {
                    MessageType = commandType.FullName,
                    MessageContent = requestMessageContent,
                };
                RpcTransportMessage responseMessage = rpcServerManager.SendMessage(serviceName, requestMessage);

                //响应消息验证
                if (responseMessage == null || responseMessage.CorrelationMessageId == null
                    || responseMessage.CorrelationMessageId != requestMessage.MessageId
                    || responseMessage.MessageContent == null)
                {
                    throw new WindServiceBusException(string.Format("request [{0}] get error response !", requestMessage.MessageId));
                }
                responseMessageContent = responseMessage.MessageContent;
            }
            catch (WindServiceBusServiceNotFoundException notFoundEx)
            {
                //TODO: 未找到符合的远程服务
                throw notFoundEx;
            }
            catch (Exception ex)
            {
                //TODO: 其他异常处理
                throw ex;
            }

            //3. 处理响应消息体
            IServiceCommandResult commandResult = null;
            using (var responseStream = new MemoryStream(responseMessageContent))
            {
                var obj = this.Serializer.Deserialize(commandResultType, responseStream);
                commandResult = (IServiceCommandResult)obj;
            }

            return new ServiceCommandResultWithResponseContext(commandResult, responseContext);
        }
        */

        /// <summary>
        /// 执行命令服务核心
        /// </summary>
        /// <typeparam name="TServiceCommand">命令服务类型</typeparam>
        /// <typeparam name="TCommandResult">命令服务返回结果类型</typeparam>
        /// <param name="commandData">命令服务数据</param>
        /// <param name="remoteContext">远端上下文（如果是本地调用，返回null）</param>
        /// <returns>命令服务返回结果数据</returns>
        internal IServiceCommandResult TriggerServiceCommandCore(Type commandType, Type commandResultType,
            IServiceCommand commandData, out RemoteServiceBusResponseContext remoteContext)
        {
            using (PerformanceCounter.BeginStopwatch(string.Format("localCall: {0}", commandType.FullName)))
            {
                //1. 本地调用
                IServiceCommandResult commandResult = null;
                bool isTriggeredLocal = this.triggerLocalCommand(commandType, commandResultType, commandData, out commandResult);
                if (isTriggeredLocal)
                {
                    remoteContext = null;
                    return commandResult;
                }
            }

            //2. 从远程容器找
            try
            {
                using (PerformanceCounter.BeginStopwatch(string.Format("remoteCall: {0}", commandType.FullName)))
                {
                    var resultWithContext = this.triggerRemoteCommand(commandType, commandResultType, commandData);

                    remoteContext = resultWithContext.ResponseContext;
                    return resultWithContext.ServiceCommandResult;
                }
            }
            catch (WindServiceBusRpcException rpcException)
            {
                remoteContext = rpcException.RemoteContext;
                throw rpcException.InnerException;
            }
            catch (WindServiceBusMultiRpcException multiRpcException)
            {
                //处理多次调度异常信息
                if (multiRpcException.Count == 1)
                {
                    var rpcException = multiRpcException.First();
                    remoteContext = rpcException.RemoteContext;
                    throw rpcException.InnerException;
                }
                else
                {
                    throw multiRpcException;
                }
            }
        }

        internal bool triggerLocalCommand(Type commandType, Type commandResultType, IServiceCommand commandData, out IServiceCommandResult commandResult)
        {
            //命令参数验证规则
            if (commandData == null)
                throw new WindServiceBusException("command data empty error!", new ArgumentNullException("commandData"));

            //从本地容器调用
            var registry = this.IocResolver.Resolve<ServiceBusRegistry>();
            bool isLocalCommand = registry.IsLocalServiceCommand(new ServiceUniqueNameInfo(commandType));
            if (!isLocalCommand)
            {
                commandResult = null;
                return false;
            }

            var localCommandDispatcher = this.createLocalDispatcher(registry);
            commandResult = localCommandDispatcher.DispatchCommand(commandType, commandResultType, commandData);
            return true;
        }

        internal ServiceCommandResultWithResponseContext triggerRemoteCommand(Type commandType, Type commandResultType, IServiceCommand commandData)
        {
            var remoteCommandDispatcher = this.createRemoteDispatcher();
            var resultWithContext = remoteCommandDispatcher.DispatchCommand(commandType, commandResultType, commandData);
            return resultWithContext;
        }

        internal ServiceResponseMessageWithResponseContext triggerRemoteCommand(ServiceUniqueNameInfo serviceName, string requestMessageContent)
        {
            var remoteCommandDispatcher = this.createRemoteDispatcher();
            var resultWithContext = remoteCommandDispatcher.DispatchCommand(serviceName, requestMessageContent);
            return resultWithContext;
        }


        internal IEnumerable<IServiceCommandResult> BroadcastServiceCommandCore(Type commandType, Type commandResultType, IServiceCommand commandData)
        {
            using (PerformanceCounter.BeginStopwatch(string.Format("localBroadcast: {0}", commandType.FullName)))
            {
                //1. 本地调用
                IServiceCommandResult commandResult = null;
                bool isTriggeredLocal = this.triggerLocalCommand(commandType, commandResultType, commandData, out commandResult);
                if (isTriggeredLocal)
                {
                    yield return commandResult;
                }
            }

            using (PerformanceCounter.BeginStopwatch(string.Format("remoteBroadcast: {0}", commandType.FullName)))
            {
                //2. 远程调用
                var remoteCommandDispatcher = this.createRemoteDispatcher();
                var remoteResultList = remoteCommandDispatcher.BroadcastCommand(commandType, commandResultType, commandData);
                foreach (var remoteResult in remoteResultList)
                {
                    yield return remoteResult.ServiceCommandResult;
                }
            }
        }

        internal ICollection<ServiceResponseMessageWithResponseContext> broadcastRemoteCommand(ServiceUniqueNameInfo serviceName, string requestMessageContent)
        {
            var remoteCommandDispatcher = this.createRemoteDispatcher();
            return remoteCommandDispatcher.BroadcastCommand(serviceName, requestMessageContent);
        }


        private RemoteServiceCommandDispatcher createRemoteDispatcher()
        {
            var rpcManager = this.IocResolver.Resolve<RpcServerManager>();
            RemoteServiceCommandDispatcher remoteCommandDispatcher = new RemoteServiceCommandDispatcher(rpcManager, this.Serializer, this.Logger);
            return remoteCommandDispatcher;
        }

        private LocalServiceCommandDispatcher createLocalDispatcher(ServiceBusRegistry registry)
        {
            LocalServiceCommandDispatcher localCommandDispatcher = new LocalServiceCommandDispatcher(registry, this.Logger);
            return localCommandDispatcher;
        }

        #endregion
    }
}
