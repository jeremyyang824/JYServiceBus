using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Logging;
using Wind.Comm;
using Wind.iSeller.Framework.Core.Dependency;
using Wind.iSeller.NServiceBus.Core.Dispatchers;
using Wind.iSeller.NServiceBus.Core.Exceptions;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Expo.Configurations;
using Wind.iSeller.NServiceBus.Expo.LegacyISellerAdapter;

namespace Wind.iSeller.NServiceBus.Expo
{
    /// <summary>
    /// 接收Expo请求
    /// </summary>
    public class ExpoCommandStub : ICommandStub, ITransientDependency
    {
        private readonly IIocResolver iocResolver;

        /// <summary>
        /// 日志处理器
        /// </summary>
        public ILogger Logger { get; set; }

        public ExpoCommandStub(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
            Logger = NullLogger.Instance;
        }

        public void DoCommandStub(CommandHeader header, object[] inputArgs, out object[] outputArgs)
        {
            var commandId = header.CommandId;
            var appClass = header.AppClass;

            if (commandId == (uint)ExpoCommandName.ServiceBusCommand)
            {
                //获取请求输入
                var expoInput = new ExpoCommandMessageParser.ExpoMessageInput(inputArgs);
                try
                {
                    //处理请求命令
                    var commandProcessor = this.iocResolver.Resolve<RemoteServiceCommandProcessor>();
                    var rpcRequest = expoInput.BuidRpcTransportMessage();
                    //构建请求环境
                    this.fillRequestRemoteContext(header, rpcRequest.MessageHeader);

                    //处理并返回
                    var rpcResponse = commandProcessor.ProcessCommand(rpcRequest);

                    //返回命令响应
                    outputArgs = (new ExpoCommandMessageParser.ExpoMessageOutput(rpcResponse)).BuidExpoMessageBody();
                }
                catch (Exception ex)
                {
                    //返回异常响应
                    var serviceUniqueName = new ServiceUniqueNameInfo(expoInput.ServiceAssemblyName, expoInput.ServiceCommandName, ServiceUniqueNameInfo.ServiceMessageType.ServiceCommand);
                    var errorResp = RpcTransportMessageResponse.BuildErrorResponse(serviceUniqueName, RpcTransportResponseCode.SystemError, ex);
                    outputArgs = (new ExpoCommandMessageParser.ExpoMessageOutput(errorResp)).BuidExpoMessageBody();

                    this.Logger.Error(serviceUniqueName.ToString(), ex);
                }
            }
            else if (commandId == (uint)ExpoCommandName.ServiceBusBroadcastCommand)
            {
                //获取请求输入
                var expoInput = new ExpoCommandMessageParser.ExpoMessageInput(inputArgs);
                try
                {
                    //处理请求命令
                    var commandProcessor = this.iocResolver.Resolve<RemoteServiceCommandProcessor>();
                    var rpcRequest = expoInput.BuidRpcTransportMessage();
                    //构建请求环境
                    this.fillRequestRemoteContext(header, rpcRequest.MessageHeader);

                    //处理并返回
                    var rpcResponse = commandProcessor.ProcessBroadcastCommand(rpcRequest);

                    //返回命令响应
                    outputArgs = (new ExpoCommandMessageParser.ExpoMessageOutput(rpcResponse)).BuidExpoMessageBody();
                }
                catch (Exception ex)
                {
                    //返回异常响应
                    var serviceUniqueName = new ServiceUniqueNameInfo(expoInput.ServiceAssemblyName, expoInput.ServiceCommandName, ServiceUniqueNameInfo.ServiceMessageType.ServiceCommand);
                    var errorResp = RpcTransportMessageResponse.BuildErrorResponse(serviceUniqueName, RpcTransportResponseCode.SystemError, ex);
                    outputArgs = (new ExpoCommandMessageParser.ExpoMessageOutput(errorResp)).BuidExpoMessageBody();

                    this.Logger.Error(serviceUniqueName.ToString(), ex);
                }
            }
            else if (commandId == (uint)ExpoCommandName.ISellerLegacyCommand)
            {
                var commandProcessor = this.iocResolver.Resolve<ISellerCommandProcessor>();

                string jsonRequest = Encoding.Default.GetString(inputArgs[0] as byte[]);
                ISellerExpoRequest request = ISellerExpoRequest.ConvertFromJson(jsonRequest);
                ISellerExpoResponse response = commandProcessor.ProcessCommand(request, messageHeader =>
                {
                    //构建请求环境
                    this.fillRequestRemoteContext(header, messageHeader);
                });
                outputArgs = new object[] { Encoding.Default.GetBytes(response.ConvertToJson()) };
            }
            else
            {
                throw new WindServiceBusException(string.Format("EXPO命令:[{0}]无法识别!", commandId));
            }
        }

        private void fillRequestRemoteContext(CommandHeader header, RpcTransportMessageHeader rpcRequestHeader)
        {
            rpcRequestHeader["SourceAppClassId"] = header.AppClass.ToString();
            rpcRequestHeader["SourceCommandId"] = header.CommandId.ToString();
        }
    }
}
