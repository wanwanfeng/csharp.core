using System;
using System.Collections.Generic;

namespace Library
{
    public partial class LocalEvent
    {
        public static EventObject localEvent { get; set; }

        static LocalEvent()
        {
            localEvent = new EventObject();
            Ldebug.OnActionLog += OnLogAction;
        }

        private static void OnLogAction(object message)
        {
            localEvent.TriggerListener("NewMessage", message);
        }
    }
}
