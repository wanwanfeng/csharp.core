using System;

namespace Library
{
    public static class Ldebug
    {
        public static void Log(object message)
        {
            if (OnActionLog == null) return;
            message = message.ToString().Replace("\n", "");
            OnActionLog.Invoke(message);
        }

        public static void LogError(object message)
        {
            if (OnActionLogError == null) return;
            message = message.ToString().Replace("\n", "");
            OnActionLogError.Invoke(message);
        }

        public static Action<object> OnActionLogError { get; set; }
        public static Action<object> OnActionLog { get; set; }
    }
}
