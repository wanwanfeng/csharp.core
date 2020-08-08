using System;
using System.Diagnostics;
using System.IO;

namespace Library
{
    public class SimpleLogger
    {
        private static string LogFile { get; set; }
        private static TextWriterTraceListener TraceListener { get; set; }

        private static string GetTime
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
        }


        /// <summary>
        /// 初始化文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultName"></param>
        public static void Init(string path, string defaultName = "log.txt")
        {
            if (Path.HasExtension(path))
            {
                LogFile = path;
            }
            else
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                LogFile = Path.Combine(path, defaultName);
            }
            if (!File.Exists(LogFile))
                File.WriteAllText(LogFile, "");

            Trace.AutoFlush = true;
            TraceListener = new TextWriterTraceListener(LogFile);
            Trace.Listeners.Add(TraceListener);
        }

        public static void Log(string msg)
        {
            Trace.WriteLine(string.Format("[{0}] [{1}] {2} ", GetTime, "log", msg));
        }
    }
}