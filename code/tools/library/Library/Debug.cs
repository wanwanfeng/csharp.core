using System;

namespace Library
{
    public static class Debug
    {
        public static string NewMessage = "NewMessage";

        public static void Log(object message)
        {
            message = message.ToString().Replace("\n", "");
            if (LogAction != null)
                LogAction.Invoke(message);
            LocalEvent.eventManager.throwEvent(NewMessage, message);
        }

        public static void LogError(object message)
        {
            message = message.ToString().Replace("\n", "");
            if (LogErrorAction != null)
                LogErrorAction.Invoke(message);
            LocalEvent.eventManager.throwEvent(NewMessage, message);
        }

        public static Action<object> LogErrorAction;
        public static Action<object> LogAction;
    }
}
