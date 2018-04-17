namespace Library
{
    public partial class LocalEvent
    {
        public static EventActionListManager<string, object> eventManager = new EventActionListManager<string, object>();

        public static string NewMessage = "NewMessage";

        static LocalEvent()
        {
            Ldebug.OnActionLog += OnLogAction;
        }

        private static void OnLogAction(object message)
        {
            eventManager.throwEvent(NewMessage, message);
        }
    }
}
