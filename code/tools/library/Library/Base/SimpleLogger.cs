using System;
using System.Diagnostics;
using System.IO;

namespace Library
{
    public class SimpleLogger
    {
        public static string Path { get; private set; }
        private static TextWriterTraceListener TraceListener { get; set; }

        private static string GetTime
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public static void Init(string logFile)
        {
            if (!File.Exists(logFile))
                File.Create(logFile);

            Trace.AutoFlush = true;
            TraceListener = new TextWriterTraceListener(Path = logFile);
            Trace.Listeners.Add(TraceListener);
        }

        public static void Log(string msg)
        {
            Trace.WriteLine(string.Format("[{0}] [{1}] {2} ", GetTime, "log", msg));
        }
    }
}