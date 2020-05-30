using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace bilibilidan
{
    class NetConverter
    {
        public static int toInt(byte[] buff, int start)
        {
            return (buff[start] << 24) | (buff[start + 1] << 16) | (buff[start + 2] << 8) | (buff[start + 3]);
        }
        public static short toShort(byte[] buff, int start)
        {
            return (short)((buff[start] << 8) | (buff[start + 1]));
        }
        /// <summary>
        /// 解压字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecompressString(byte[] str, int start, int length)
        {
            return Encoding.UTF8.GetString(DecompressBytes(str,start,length));
        }
        /// <summary>
        /// 解压二进制
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] DecompressBytes(byte[] str,int start,int length)
        {
            var ms = new MemoryStream(str,start,length);
            var outms = new MemoryStream();
            using (var deflateStream = new DeflateStream(ms, CompressionMode.Decompress, true))
            {
                var buf = new byte[4096];
                int len;
                while ((len = deflateStream.Read(buf, 0, buf.Length)) > 0)
                    outms.Write(buf, 0, len);
            }
            return outms.ToArray();
        }
    }
}
