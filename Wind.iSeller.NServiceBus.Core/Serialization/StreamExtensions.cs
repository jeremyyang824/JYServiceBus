using System;
using System.IO;

namespace Wind.iSeller.NServiceBus.Core.Serialization
{
    /// <summary>
    /// 字节流扩展
    /// </summary>
    public static class StreamExtensions
    {
        public static byte[] ToBytesOnly(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] array = new byte[4096];
            int num = 0;
            int num2;
            while ((num2 = stream.Read(array, num, array.Length - num)) > 0)
            {
                num += num2;
                if (num == array.Length)
                {
                    int num3 = stream.ReadByte();
                    if (num3 != -1)
                    {
                        byte[] array2 = new byte[array.Length * 2];
                        Buffer.BlockCopy(array, 0, array2, 0, array.Length);
                        Buffer.SetByte(array2, num, (byte)num3);
                        array = array2;
                        num++;
                    }
                }
            }
            byte[] array3 = array;
            if (array.Length != num)
            {
                array3 = new byte[num];
                Buffer.BlockCopy(array, 0, array3, 0, num);
            }
            return array3;
        }

        public static byte[] ToBytes(this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            long position = stream.Position;
            try
            {
                stream.Position = 0L;
            }
            catch (Exception innerException)
            {
                throw new InvalidOperationException("StreamCannotSeek", innerException);
            }
            byte[] result;
            try
            {
                result = stream.ToBytesOnly();
            }
            finally
            {
                stream.Position = position;
            }
            return result;
        }

        public static Stream Copy(this Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            if (!stream.CanSeek)
            {
                throw new InvalidOperationException("StreamCannotSeek");
            }
            memoryStream.Capacity = (int)stream.Length;
            long position = stream.Position;
            try
            {
                stream.Seek(0L, SeekOrigin.Begin);
                stream.CopyTo(memoryStream);
                memoryStream.Seek(0L, SeekOrigin.Begin);
            }
            finally
            {
                stream.Seek(position, SeekOrigin.Begin);
            }
            return memoryStream;
        }
    }
}
