using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Wind.iSeller.NServiceBus.Core.Exceptions;

namespace Wind.iSeller.NServiceBus.Core.Serialization
{
    /// <summary>
    /// XML序列化
    /// </summary>
    public class ServiceBusXmlSerializer : ISerializer
    {
        private static readonly object lockObj = new object();
        private readonly XmlSerializerNamespaces _namespaces = new XmlSerializerNamespaces();
        private readonly XmlWriterSettings _xmlSettings;

        private readonly Dictionary<Type, XmlAttributeOverrides> _overrides = new Dictionary<Type, XmlAttributeOverrides>();
        private readonly Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();

        public ServiceBusXmlSerializer()
        {
            this._xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = true,
                Indent = true
            };
            this._namespaces.Add(string.Empty, string.Empty);
        }

        public string SerializeString(object instance)
        {
            if (instance == null)
                throw new WindServiceBusException("XmlSerializer can not serialize null !", new ArgumentNullException("instance"));

            Type type = instance.GetType();
            XmlSerializer serializer = this.getSerializer(type);
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, this._xmlSettings))
            {
                serializer.Serialize(xmlWriter, instance, this._namespaces);
                xmlWriter.Flush();
            };
            return stringBuilder.ToString();
        }

        public Stream SerializeStream(object instance)
        {
            if (instance == null)
                throw new WindServiceBusException("XmlSerializer can not serialize null !", new ArgumentNullException("instance"));

            string content = this.SerializeString(instance);
            return new MemoryStream(Encoding.UTF8.GetBytes(content));
        }

        public byte[] SerializeByte(object instance)
        {
            var stream = this.SerializeStream(instance);
            return stream.ToBytes();
        }

        public object DeserializeStream(Type type, Stream stream)
        {
            object result;
            using (Stream stream2 = stream.Copy())
            {
                using (XmlDictionaryReader xmlDictionaryReader = XmlDictionaryReader.CreateTextReader(stream2, Encoding.UTF8, new XmlDictionaryReaderQuotas
                {
                    MaxArrayLength = 2147483647,
                    MaxStringContentLength = 2147483647,
                    MaxNameTableCharCount = 2147483647
                }, null))
                {
                    result = this.getSerializer(type).Deserialize(xmlDictionaryReader);
                }
            }
            return result;
        }

        public object DeserializeByte(Type type, byte[] byteStream)
        {
            using (var stream = new MemoryStream(byteStream))
            {
                var obj = this.DeserializeStream(type, stream);
                return obj;
            }
        }

        public object DeserializeString(Type type, string content)
        {
            return this.DeserializeByte(type, Encoding.UTF8.GetBytes(content));
        }

        private XmlSerializer getSerializer(Type type)
        {
            XmlSerializer result;
            lock (lockObj)
            {
                if (!this._overrides.ContainsKey(type))
                {
                    this._overrides.Add(type, new XmlAttributeOverrides());
                }
                if (!this._serializers.ContainsKey(type))
                {
                    this._serializers.Add(type, new XmlSerializer(type, this._overrides[type]));
                }
                result = this._serializers[type];
            }
            return result;
        }


        public string CombineToArray(IList<string> contentList)
        {
            throw new NotImplementedException();
        }
    }
}
