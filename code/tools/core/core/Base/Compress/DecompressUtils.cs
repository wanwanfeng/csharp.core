using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Library.Extensions;
using Library.Helper;

namespace Library.Compress
{
    public class DecompressUtils
    {

        #region byte[], Stream相互转换

        /// <summary>
        /// 将 Stream 转成byte[] 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary>
        /// 将 byte[] 转成 Stream
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static Stream BytesToStream(byte[] bytes)
        {
            return new MemoryStream(bytes);
        }

        #endregion

        #region 压缩文件

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        public static string Compress(string inFile, string outFile, Action<string, float> runAction)
        {
            try
            {
                using (FileStream fr = File.Create(inFile))
                using (FileStream fileStream = File.Create(outFile))
                using (GZipStream compressedzipStream = new GZipStream(fileStream, CompressionMode.Compress, true))
                {
                    fr.CopyTo(fr, p => { runAction(outFile, p); });
                }
                return "";
            }
            catch (Exception e)
            {
                return "CompressFileGzip：" + e.Message + "\n" + outFile;
            }
        }

        #endregion

        #region 解压文件

        /// <summary>
        /// 边读边写
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        /// <returns></returns>
        private static string Decompress(Stream input, string outFile, Action<string, float> runAction)
        {
            runAction(outFile, 0);
            try
            {
                using (var fw = new FileStream(outFile, FileMode.Create))
                {
                    using (GZipStream gzip = new GZipStream(input, CompressionMode.Decompress))
                    {
                        gzip.CopyTo(fw, p => { runAction(outFile, p); });
                        return "";
                    }
                }
            }
            catch (Exception e)
            {
                return "DecompressMemoryGzip：" + e.Message + "\n" + outFile;
            }
            finally
            {
                runAction(outFile, 1);
            }
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        public static string Decompress(string inFile, string outFile, Action<string, float> runAction)
        {
            using (FileStream input = new FileStream(inFile, FileMode.Open))
            {
                return Decompress(input, outFile, runAction);
            }
        }

        /// <summary>
        /// 解压byte[]
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        public static string Decompress(byte[] bs, string outFile, Action<string, float> runAction)
        {
            using (Stream input = new MemoryStream(bs))
            {
                return Decompress(input, outFile, runAction);
            }
        }

        #endregion

        #region 压缩文件夹

        public static string MakeZipFile(string zipDir, string keyStr = "sfnsgis09itnmpos8r74ef[/;,dfs")
        {
            var config = Directory.GetFiles(zipDir, "*.*", SearchOption.AllDirectories).ToDictionary(p => p.Replace(zipDir, "").Replace("\\", "/"), p => new FileInfo(p));
            FileHelper.GZIP.Serialize(config, zipDir + ".gz", keyStr);
            return string.Empty;
        }

        #endregion

        #region 解压文件夹

        public static Dictionary<string, byte[]> UnMakeZipFile(string zipfilename, string keyStr = "sfnsgis09itnmpos8r74ef[/;,dfs")
        {
            Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();
            IEnumerator enumerator = FileHelper.GZIP.Deserialize(zipfilename, result, keyStr);
            while (enumerator.MoveNext()) { }
            return result;
        }

        #endregion
    }
}