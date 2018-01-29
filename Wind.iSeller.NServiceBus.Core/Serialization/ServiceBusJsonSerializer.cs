using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Wind.iSeller.NServiceBus.Core.Exceptions;

namespace Wind.iSeller.NServiceBus.Core.Serialization
{
    /// <summary>
    /// Json序列化
    /// </summary>
    public class ServiceBusJsonSerializer : ISerializer
    {
        public object DeserializeByte(Type type, byte[] byteStream)
        {
            using (var stream = new MemoryStream(byteStream))
            {
                var obj = this.DeserializeStream(type, stream);
                return obj;
            }
        }

        public object DeserializeStream(Type type, Stream stream)
        {
            if (stream == null)
                throw new WindServiceBusException("JsonSerializer can not serialize null !", new ArgumentNullException("stream"));

            using (StreamReader reader = new StreamReader(stream))
            {
                string text = reader.ReadToEnd();
                return this.DeserializeString(type, text);
            }
        }

        public object DeserializeString(Type type, string content)
        {
            return JsonConvert.DeserializeObject(content, type);
        }


        public byte[] SerializeByte(object instance)
        {
            return this.SerializeStream(instance).ToBytes();
        }

        public Stream SerializeStream(object instance)
        {
            string content = this.SerializeString(instance);
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        public string SerializeString(object instance)
        {
            //if (instance == null)
            //    throw new WindServiceBusException("JsonSerializer can not serialize null !", new ArgumentNullException("instance"));

            return JsonConvert.SerializeObject(instance);
        }

        public string CombineToArray(IList<string> contentList)
        {
            List<object> result = new List<object>();
            foreach (var content in contentList)
            {
                var obj = JsonConvert.DeserializeObject(content);
                result.Add(obj);
            }
            return JsonConvert.SerializeObject(result);
        }
    }
}
