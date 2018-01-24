namespace Library
{
    public static class Debug
    {
        public static string NewMessage = "NewMessage";

        public static void Log(object message)
        {
            message = message.ToString().Replace("\n", "");
            LocalEvent.eventManager.throwEvent(Debug.NewMessage,message);
        }

        internal static void LogError(object message)
        {
            message = message.ToString().Replace("\n", "");
            LocalEvent.eventManager.throwEvent(Debug.NewMessage, message);
        }
    }
}
