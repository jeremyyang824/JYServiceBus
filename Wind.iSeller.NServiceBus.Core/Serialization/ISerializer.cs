using System;
using System.Collections.Generic;
using System.IO;

namespace Wind.iSeller.NServiceBus.Core.Serialization
{
    /// <summary>
    /// 对象序列化
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="stream">数据流</param>
        /// <returns>对象实例</returns>
        object DeserializeStream(Type type, Stream stream);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="stream">字节流</param>
        /// <returns>对象实例</returns>
        object DeserializeByte(Type type, byte[] byteStream);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="content">字符串</param>
        /// <returns>对象实例</returns>
        object DeserializeString(Type type, string content);


        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <returns>数据流</returns>
        Stream SerializeStream(object instance);

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <returns>字节流</returns>
        byte[] SerializeByte(object instance);

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="instance">对象实例</param>
        /// <returns>字符串</returns>
        string SerializeString(object instance);


        /// <summary>
        /// 将序列化后的字符串合并为一个数组
        /// </summary>
        /// <param name="contentList"></param>
        /// <returns></returns>
        string CombineToArray(IList<string> contentList);
    }
}
