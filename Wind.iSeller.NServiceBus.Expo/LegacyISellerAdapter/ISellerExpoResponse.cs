using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wind.iSeller.NServiceBus.Core.RPC;

namespace Wind.iSeller.NServiceBus.Expo.LegacyISellerAdapter
{
    /// <summary>
    /// iSeller遗留EXPO响应
    /// </summary>
    public class ISellerExpoResponse
    {
        /// <summary>
        /// errCode:
        /// 0: success
        /// 1: failure
        /// </summary>
        public byte errCode { get; set; }

        /// <summary>
        /// errMsg
        /// </summary>
        public string errMsg { get; set; }

        /// <summary>
        /// dataJson
        /// </summary>
        public Dictionary<string, string> dataJson { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public ISellerPaginationInfo Page { get; set; }

        /// <summary>
        /// 服务端时间
        /// </summary>
        public string ServerTime { get; set; }

        public ISellerExpoResponse()
        {
            this.dataJson = new Dictionary<string, string>();
            this.ServerTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public ISellerExpoResponse(byte errCode, string errMsg)
            : this()
        {
            this.errCode = errCode;
            this.errMsg = errMsg;
        }

        public ISellerExpoResponse(RpcTransportMessageResponse response)
            : this()
        {
            if (response == null)
                throw new ExpoMessageException("rpc transport message null exception!");

            if (response.ResponseCode == RpcTransportResponseCode.Success)
            {
                this.fillContent(response.MessageContent);
            }
            else
            {
                //错误信息
                this.errCode = 1;
                this.errMsg = response.ErrorInfo;
            }
        }

        //将标准JSON 转为iSeller JSON 
        private void fillContent(string messageContent)
        {
            if (string.IsNullOrWhiteSpace(messageContent))
                return;

            JObject jObj = (JObject)JsonConvert.DeserializeObject(messageContent);
            this.dataJson["DATA"] = jObj["DATA"].ToString();
            Page = jObj["Page"].ToObject<ISellerPaginationInfo>();
        }

        public string ConvertToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static ISellerExpoResponse ConvertFromJson(string jsonResponse)
        {
            return JsonConvert.DeserializeObject<ISellerExpoResponse>(jsonResponse);
        }
    }
}
