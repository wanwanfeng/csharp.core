using System;
using System.Collections.Generic;

namespace Library
{
    public interface IEvent
    {
        Dictionary<string, List<Action<object>>> CacheEvents { get; set; }
    }

    public class EventObject : IEvent
    {
        public Dictionary<string, List<Action<object>>> CacheEvents { get; set; }
    }

    /// <summary>
    /// System.Object事件扩展
    /// </summary>
    public static class ExtensionsForEvents
    {
        /// <summary>
        /// 事件注册，用 TriggerListener 触发
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        /// <param name="callAction"></param>
        public static void AddListener(this IEvent target, string eventName, Action<object> callAction)
        {
            if (target == null) return;
            List<Action<object>> list = null;
            if (target.CacheEvents.TryGetValue(eventName, out list))
            {
                list.Add(callAction);
            }
            else
            {
                list = new List<Action<object>> {callAction};
                target.CacheEvents[eventName] = list;
            }
        }

        /// <summary>
        /// 移除 AddListener 注册的事件
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        /// <param name="callAction"></param>
        public static void RemoveListener(this IEvent target, string eventName, Action<object> callAction)
        {
            if (target == null) return;
            List<Action<object>> list = null;
            if (target.CacheEvents.TryGetValue(eventName, out list))
            {
                list.Remove(callAction);
                if (list.Count == 0)
                    target.CacheEvents.Remove(eventName);
            }
        }

        /// <summary>
        /// 移除 AddListener 注册的事件
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        public static void RemoveListener(this IEvent target, string eventName)
        {
            if (target == null) return;
            List<Action<object>> list = null;
            if (target.CacheEvents.TryGetValue(eventName, out list))
                target.CacheEvents.Remove(eventName);
        }

        /// <summary>
        /// 清除 AddListener 注册的所有事件
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveAllListener(this IEvent target)
        {
            if (target == null) return;
            target.CacheEvents.Clear();
        }

        /// <summary>
        /// 触发用 AddListener 注册的事件
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        /// <param name="param"></param>
        public static void TriggerListener(this IEvent target, string eventName, object param = default(object))
        {
            if (target == null) return;
            List<Action<object>> list = null;
            if (!target.CacheEvents.TryGetValue(eventName, out list)) return;
            foreach (var action in list)
            {
                action.Invoke(param);
            }
        }
    }
}