using System;
using System.IO;
using System.Linq;
using ICSharpCode;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;
using Library.Extensions;

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
        public static string CompressFile(string inFile, string outFile, Action<string, float> runAction)
        {
            return ICSharpGzipCode.CompressFileGzip(inFile, outFile, runAction);
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="runAction"></param>
        public static string CompressFile(string inFile, Action<string, float> runAction)
        {
            string outFile = Path.GetTempFileName();
            var ret = CompressFile(inFile, outFile, runAction);
            if (string.IsNullOrEmpty(ret))
            {
                try
                {
                    File.Delete(inFile);
                    File.Move(outFile, inFile);
                }
                catch (Exception e)
                {
                    ret = e.Message;
                }
            }
            return ret;
        }

        #endregion

        #region 解压文件

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        public static string DecompressFile(string inFile, string outFile, Action<string, float> runAction)
        {
            return ICSharpGzipCode.DecompressFileGzip(inFile, outFile, runAction);
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="runAction"></param>
        public static string DecompressFile(string inFile, Action<string, float> runAction)
        {
            string outFile = inFile + "temp";
            if (File.Exists(outFile))
                File.Delete(outFile);
            var ret = DecompressFile(inFile, outFile, runAction);
            if (string.IsNullOrEmpty(ret))
            {
                try
                {
                    File.Delete(inFile);
                    File.Move(outFile, inFile);
                }
                catch (Exception e)
                {
                    ret = e.Message;
                }
            }
            return ret;
        }

        #endregion

        #region 解压byte[]

        /// <summary>
        /// 解压byte[]
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        public static string DecompressMemory(byte[] bs, string outFile, Action<string, float> runAction)
        {
            return ICSharpGzipCode.DecompressMemoryGzip(bs, outFile, runAction);
        }

        #endregion

        #region 压缩文件夹

        /// <summary>
        /// 实现压缩功能(压缩指定目录到文件)
        /// </summary>
        /// <param name="zipDir">要压缩文件夹(绝对路径)</param>
        /// <param name="compressionLevel">压缩比</param>
        /// <param name="password">加密密码</param>
        /// <param name="comment">压缩文件描述</param>
        /// <returns>异常信息</returns>
        public static string MakeZipFile(string zipDir, int compressionLevel = 0, string password = "",
            string comment = "")
        {
            var list = Directory.GetFiles(zipDir, "*.*", SearchOption.AllDirectories)
                .Select(p => p.Replace(zipDir + "\\", ""))
                .ToArray();
            return ICSharpZipCode.MakeZipFile(zipDir, list, zipDir + ".zip", compressionLevel, password, comment);
        }

        /// <summary>
        /// 实现压缩功能(压缩指定文件列表到文件)
        /// </summary>
        /// <param name="filenameToZip">要压缩文件路径(绝对文件路径)</param>
        /// <param name="zipedfiledname">压缩(绝对文件路径)</param>
        /// <param name="compressionLevel">压缩比</param>
        /// <param name="password">加密密码</param>
        /// <param name="comment">压缩文件描述</param>
        /// <returns>异常信息</returns>
        public static string MakeZipFile(string[] filenameToZip, string zipedfiledname, int compressionLevel = 0,
            string password = "", string comment = "")
        {
            return ICSharpZipCode.MakeZipFile(null, filenameToZip, zipedfiledname, compressionLevel, password, comment);
        }


        #endregion

        #region 解压文件夹

        /// <summary>
        /// 实现解压操作(解压到指定目录)
        /// </summary>
        /// <param name="zipfilename">要解压文件Zip(物理路径)</param>
        /// <param name="unZipDir">解压目的路径(物理路径)</param>
        /// <param name="password">解压密码</param>
        /// <returns>异常信息</returns>
        public static string UnMakeZipFileTo(string zipfilename, string unZipDir, string password = "")
        {
            return ICSharpZipCode.UnMakeZipFile(zipfilename, unZipDir, password);
        }

        /// <summary>
        /// 实现解压操作(解压到压缩文件所在目录)
        /// </summary>
        /// <param name="zipfilename">要解压文件Zip(物理路径)</param>
        /// <param name="password">解压密码</param>
        /// <param name="isHaveSelf">自身名称作为一级目录</param>
        /// <returns>异常信息</returns>
        public static string UnMakeZipFile(string zipfilename, string password = "", bool isHaveSelf = true)
        {
            return ICSharpZipCode.UnMakeZipFile(zipfilename,
                Path.GetDirectoryName(zipfilename) +
                (isHaveSelf ? "\\" + Path.GetFileNameWithoutExtension(zipfilename) : ""),
                password);
        }

        #endregion
    }
}

namespace ICSharpCode
{
    public class ICSharpGzipCode
    {
        #region GzipFile

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="inFile"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        public static string CompressFileGzip(string inFile, string outFile, Action<string, float> runAction)
        {
            try
            {
                using (var fw = new FileStream(outFile, FileMode.OpenOrCreate))
                {
                    using (var gzip = new GZipOutputStream(fw))
                    {
                        using (var fr = new FileStream(inFile, FileMode.Open))
                        {
                            fr.CopyTo(gzip, p => { runAction(outFile, p); });
                        }
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                return "CompressFileGzip：" + e.Message + "\n" + outFile;
            }
        }

        /// <summary>
        /// 先解压到内存在统一写入（可能出现内存不足情况）
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        /// <returns></returns>
        private static string DecompressMemoryGzip2(Stream input, string outFile, Action<string, float> runAction)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var gzip = new GZipInputStream(input))
                    {
                        gzip.CopyTo(ms, p => { runAction(outFile, p); });
                        File.WriteAllBytes(outFile, ms.ToArray());
                        return "";
                    }
                }
            }
            catch (Exception e)
            {
                return ("DecompressMemoryGzip：" + e.Message + "\n" + outFile);
            }
            finally
            {
                runAction(outFile, 1);
            }
        }

        /// <summary>
        /// 边读边写
        /// </summary>
        /// <param name="input"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        /// <returns></returns>
        public static string DecompressMemoryGzip(Stream input, string outFile, Action<string, float> runAction)
        {
            runAction(outFile, 0);
            try
            {
                using (var fw = new FileStream(outFile, FileMode.Create))
                {
                    using (var gzip = new GZipInputStream(input))
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
        public static string DecompressFileGzip(string inFile, string outFile, Action<string, float> runAction)
        {
            using (FileStream input = new FileStream(inFile, FileMode.Open))
            {
                return DecompressMemoryGzip(input, outFile, runAction);
            }
        }

        /// <summary>
        /// 解压内存byte[]文件
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="outFile"></param>
        /// <param name="runAction"></param>
        public static string DecompressMemoryGzip(byte[] bs, string outFile, Action<string, float> runAction)
        {
            using (Stream input = new MemoryStream(bs))
            {
                return DecompressMemoryGzip(input, outFile, runAction);
            }
        }

        #endregion
    }

    /// <summary>
    /// 使用ICSharpZipCode.Dll实现解压缩
    /// Author:chenkai  Time:2009年7月13日22:03:27
    /// Version:Beta1.0.0-(测试版)
    /// </summary>
    public class ICSharpZipCode
    {
        private static readonly System.Text.RegularExpressions.Regex NewRegex =
            //new System.Text.RegularExpressions.Regex(@"^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w   ]*.*))");
            new System.Text.RegularExpressions.Regex(@"^(([0-9a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w   ]*.*))");

        public enum CodePage : int
        {
            GB2312 = 936,
            UTF16 = 1200,
            UTF7 = 65000,
            UTF8 = 65001,
            UTF32 = 65005,
        }

        static ICSharpZipCode()
        {
            //http://www.cnblogs.com/angestudy/archive/2011/04/22/2024986.html
            ZipConstants.DefaultCodePage = (int) CodePage.GB2312;
        }

        /// <summary>
        /// 实现压缩功能
        /// </summary>
        /// <param name="fileNameRoot">要压缩文件根目录</param>
        /// <param name="filenameToZip">要压缩文件相对路径(相对于fileNameRoot)</param>
        /// <param name="zipedfiledname">压缩(绝对文件路径)</param>
        /// <param name="compressionLevel">压缩比</param>
        /// <param name="password">加密密码</param>
        /// <param name="comment">压缩文件描述</param>
        /// <returns>异常信息</returns>
        public static string MakeZipFile(string fileNameRoot, string[] filenameToZip, string zipedfiledname,
            int compressionLevel,
            string password, string comment)
        {
            try
            {
                //使用正则表达式-判断压缩文件路径
                //if (!string.IsNullOrEmpty(fileNameRoot) && !NewRegex.Match(fileNameRoot).Success)
                //{
                //    File.Delete(fileNameRoot);
                //    return "压缩文件夹的路径有误!";
                //}

                ////使用正则表达式-判断压缩文件路径
                //if (!NewRegex.Match(zipedfiledname).Success)
                //{
                //    File.Delete(zipedfiledname);
                //    return "压缩文件的路径有误!";
                //}

                //创建ZipFileOutPutStream

                var tempPath = Path.GetTempFileName();
                using (var zs = new ZipOutputStream(File.Open(tempPath, FileMode.OpenOrCreate)))
                {
                    //判断Password
                    if (!string.IsNullOrEmpty(password))
                        zs.Password = password;
                    if (!string.IsNullOrEmpty(comment))
                        zs.SetComment(comment);
                    //设置CompressionLevel
                    zs.SetLevel(compressionLevel); //-查看0 - means store only to 9 - means best compression 
                    //执行压缩
                    foreach (string filename in filenameToZip)
                    {
                        //打开预压缩文件
                        using (FileStream fw = string.IsNullOrEmpty(fileNameRoot)
                            ? File.OpenRead(filename)
                            : File.OpenRead(fileNameRoot + "\\" + filename))
                        {
                            //新建ZipEntrity//设置时间-长度
                            ZipEntry newEntry = new ZipEntry(filename)
                            {
                                DateTime = DateTime.Now,
                                Size = fw.Length
                            };
                            zs.PutNextEntry(newEntry); //压入
                            fw.CopyTo(zs);
                        }
                    }
                }
                if (File.Exists(zipedfiledname)) File.Delete(zipedfiledname);
                if (File.Exists(tempPath)) File.Move(tempPath, zipedfiledname);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "";
        }

        /// <summary>
        /// 实现解压操作
        /// </summary>
        /// <param name="zipfilename">要解压文件Zip(物理路径)</param>
        /// <param name="unZipDir">解压目的路径(物理路径)</param>
        /// <param name="password">解压密码</param>
        /// <returns>异常信息</returns>
        public static string UnMakeZipFile(string zipfilename, string unZipDir, string password = "")
        {
            //判断待解压文件路径
            if (!File.Exists(zipfilename))
            {
                return "待解压文件不存在!";
            }

            //创建ZipInputStream
            var targetDir = unZipDir;
            using (ZipInputStream zs = new ZipInputStream(File.OpenRead(zipfilename)))
            {
                //判断Password
                if (!string.IsNullOrEmpty(password))
                    zs.Password = password;

                //获取Zip中单个File
                ZipEntry theEntry;

                while ((theEntry = zs.GetNextEntry()) != null)
                {
                    string filename = Path.GetFileName(theEntry.Name); //获得子集文件名
                    if (filename == string.Empty) continue;

                    string pathname = Path.GetDirectoryName(theEntry.Name); //获得子级目录
                    pathname = pathname.Replace(":", "$"); //处理当前压缩出现盘符问题                  
                    string driectoryname = targetDir + "\\" + pathname; //获得目的目录信息           
                    if (!Directory.Exists(driectoryname))
                        Directory.CreateDirectory(driectoryname); //创建

                    //解压指定子目录
                    using (var fw = File.Create(driectoryname + "\\" + filename))
                    {
                        fw.CopyTo(zs);
                    }
                }
            }
            return "";
        }
    }
}