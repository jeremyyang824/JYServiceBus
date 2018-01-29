using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wind.Comm;

namespace Expo.Web
{
    public class ServiceBusExpoHttpHandler : IHttpHandler
    {
        private RealProxy _proxy;

        private uint TargetAppClassId = 1365;
        private uint TargetCommandId = 3903;
        private uint TargetBroadcastCommandId = 3902;

        private const int SUCCESS_RESPONSE_CODE = 100;

        public ServiceBusExpoHttpHandler()
        {
            _proxy = RealProxy.InitInstance();
        }

        public void ProcessRequest(HttpContext context)
        {
            //获取请求内容
            string requestContent = null;
            if (context.Request.InputStream != null)
            {
                using (StreamReader reader = new StreamReader(context.Request.InputStream))
                {
                    requestContent = HttpUtility.UrlDecode(reader.ReadToEnd());
                }
            }

            //路由
            switch (context.Request.CurrentExecutionFilePath.Trim())
            {
                //触发服务调用
                case "/ServiceBusTriggerCommand.ashx":
                    this.triggerCommandHandler(context, requestContent);
                    break;
                //广播服务调用
                case "/ServiceBusBroadcastCommand.ashx":
                    this.broadcastCommandHandler(context, requestContent);
                    break;
                //取得完整的服务Js代理
                case "/ServiceBusGetScript.ashx":
                    this.getScriptHandler(context, requestContent);
                    break;
                default:
                    throw new UriFormatException(string.Format("Url [{0}] can not resolve!", context.Request.CurrentExecutionFilePath));
            }
        }

        private void triggerCommandHandler(HttpContext context, string requestContent)
        {
            string jsonResponse = this.callService(this.TargetAppClassId, this.TargetCommandId, requestContent);

            context.Response.ContentType = "application/json";
            context.Response.Charset = "UTF-8";
            context.Response.Write(jsonResponse);
        }

        private void broadcastCommandHandler(HttpContext context, string requestContent)
        {
            string jsonResponse = this.callService(this.TargetAppClassId, this.TargetBroadcastCommandId, requestContent);

            context.Response.ContentType = "application/json";
            context.Response.Charset = "UTF-8";
            context.Response.Write(jsonResponse);
        }

        private void getScriptHandler(HttpContext context, string requestContent)
        {
            var input = new ExpoMessageInput
            {
                UserId = 0,
                ServiceAssemblyName = "wind.iSeller.serviceBus.zero",
                ServiceCommandName = "getAllScriptProxyCommand",
                RequestMessageContent = "{}"
            };

            ExpoMessageOutput response = this.callService(this.TargetAppClassId, this.TargetCommandId, input);
            if (response.ResponseCode == SUCCESS_RESPONSE_CODE)
            {
                string script = (((JObject)JsonConvert.DeserializeObject(response.ResponseContent)).GetValue("ScriptContent")).ToString();

                context.Response.ContentType = "application/javascript";
                context.Response.Charset = "UTF-8";
                context.Response.Write(script);
            }
        }


        /// <summary>
        /// 调用EXPO服务
        /// </summary>
        private string callService(uint appClassId, uint commandId, string jsonRequest)
        {
            var request = JsonConvert.DeserializeObject<ExpoMessageInput>(jsonRequest);
            var response = this.callService(appClassId, commandId, request);

            if (response.ResponseCode == SUCCESS_RESPONSE_CODE)
            {
                return JsonConvert.SerializeObject(response);
            }
            else
            {
                throw new Exception(response.ErrorInfo);
            }
        }

        private ExpoMessageOutput callService(uint appClassId, uint commandId, ExpoMessageInput request)
        {
            request.RequestMessageId = Guid.NewGuid().ToString();

            var header = new CommandHeader(request.UserId, appClassId, commandId);
            object[] retValues = null;
            bool isSuccess = _proxy.DoCommandProxy(header, request.BuidExpoMessageBody(), out retValues);

            if (isSuccess)
            {
                var response = new ExpoMessageOutput(retValues);
                return response;
            }
            else
            {
                throw new Exception("unknow error!");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        #region 请求/响应数据

        sealed class ExpoMessageInput
        {
            /// <summary>
            /// 用户Id
            /// </summary>
            public uint UserId { get; set; }

            /// <summary>
            /// 请求消息ID
            /// </summary>
            public string RequestMessageId { get; set; }

            /// <summary>
            /// 服务程序集名
            /// </summary>
            public string ServiceAssemblyName { get; set; }

            /// <summary>
            /// 服务程序集名
            /// </summary>
            public string ServiceCommandName { get; set; }

            /// <summary>
            /// 请求消息体
            /// </summary>
            public string RequestMessageContent { get; set; }

            /// <summary>
            /// 创建Expo消息体
            /// </summary>
            public object[] BuidExpoMessageBody()
            {
                return new object[] {
                    this.RequestMessageId.ToString(),
                    this.ServiceAssemblyName,
                    this.ServiceCommandName,
                    this.RequestMessageContent
                };
            }
        }

        sealed class ExpoMessageOutput
        {
            /// <summary>
            /// 响应消息ID
            /// </summary>
            public string ResponseMessageId { get; set; }

            /// <summary>
            /// 响应内容
            /// </summary>
            public string ResponseContent { get; set; }

            /// <summary>
            /// 响应码
            /// </summary>
            public int ResponseCode { get; set; }

            /// <summary>
            /// 异常信息
            /// </summary>
            public string ErrorInfo { get; set; }

            public ExpoMessageOutput(object[] expoMessageBody)
            {
                if (expoMessageBody == null || expoMessageBody.Length != 4)
                    throw new Exception("expo output message empty or error format!");

                this.ResponseMessageId = (string)expoMessageBody[0];
                //this.ResponseContent = ((Wind.Comm.HugeString)expoMessageBody[1]).Value;
                this.ResponseContent = (string)expoMessageBody[1];
                this.ResponseCode = (int)expoMessageBody[2];
                this.ErrorInfo = (string)expoMessageBody[3];

                if (string.IsNullOrWhiteSpace(this.ResponseMessageId))
                    throw new Exception("expo output message ResponseMessageId null exception!");
                if (this.ResponseContent == null)
                    throw new Exception("expo output message MessageContent null exception!");
            }
        }

        #endregion
    }
}