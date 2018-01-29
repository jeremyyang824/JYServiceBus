using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Wind.iSeller.NServiceBus.Core.MetaData;
using Wind.iSeller.NServiceBus.Core.RPC;
using Wind.iSeller.NServiceBus.Expo.Configurations;

namespace Wind.iSeller.NServiceBus.Expo.LegacyISellerAdapter
{
    /// <summary>
    /// iSeller遗留EXPO请求
    /// </summary>
    public class ISellerExpoRequest
    {
        /// <summary>
        /// cmd
        /// </summary>
        public string cmd { get; set; }

        /// <summary>
        /// wftId
        /// </summary>
        public string wftId { get; set; }

        /// <summary>
        /// wmId
        /// </summary>
        public string wmId { get; set; }

        /// <summary>
        /// internalId
        /// </summary>
        public uint internalId { get; set; }

        /// <summary>
        /// dataJson
        /// </summary>
        public Dictionary<string, string> dataJson { get; set; }


        public static ISellerExpoRequest ConvertFromJson(string jsonRequest)
        {
            return JsonConvert.DeserializeObject<ISellerExpoRequest>(jsonRequest);
        }

        public string ConvertToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public RpcTransportMessageRequest BuidRpcTransportMessage(LegancyISellerConfiguration configuration)
        {
            string messageId = MessageIdGenerator.CreateMessageId();
            return new RpcTransportMessageRequest(messageId)
            {
                MessageHeader = new RpcTransportMessageHeader() { /*TODO: 请求上下文*/ },
                ServiceUniqueName = new ServiceUniqueNameInfo(configuration.ServiceAssemblyName, cmd, ServiceUniqueNameInfo.ServiceMessageType.ServiceCommand),
                MessageContent = this.buildMessageContentJson()
            };
        }

        //将iSeller JSON格式转为标准JSON
        private string buildMessageContentJson()
        {
            if (!dataJson.ContainsKey("DATA"))
                return string.Empty;

            var jsonContent = new
            {
                DATA = JsonConvert.DeserializeObject(dataJson["DATA"]),
            };

            return JsonConvert.SerializeObject(jsonContent);
        }
    }
}
