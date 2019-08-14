using System;
using System.Collections.Generic;

namespace Library
{
    public interface IEvent
    {
        Dictionary<string, Action<object, object>> CacheEvents { get; set; }
    }

    public class EventObject : IEvent
    {
        public Dictionary<string, Action<object, object>> CacheEvents { get; set; }       
    }

    /// <summary>
    /// System.Object事件扩展
    /// </summary>
    public static class EventsExtensions
    {
        /// <summary>
        /// 事件注册，用 TriggerListener 触发
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        /// <param name="callAction"></param>
        public static void AddListener(this IEvent target, string eventName, Action<object, object> callAction)
        {
            if (target == null) return;
            if (target.CacheEvents != null)
            {
                Action<object, object> list = null;
                if (target.CacheEvents.TryGetValue(eventName, out list))
                    target.CacheEvents[eventName] += callAction;
                else
                    target.CacheEvents[eventName] = callAction;
            }
            else
            {
                target.CacheEvents = new Dictionary<string, Action<object, object>>() {{eventName, callAction}};
            }
        }

        /// <summary>
        /// 移除 AddListener 注册的事件
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        /// <param name="callAction"></param>
        public static void RemoveListener(this IEvent target, string eventName, Action<object, object> callAction)
        {
            if (target == null || target.CacheEvents == null || target.CacheEvents.Count == 0) return;
            Action<object, object> list = null;
            if (!target.CacheEvents.TryGetValue(eventName, out list)) return;
            if (list.GetInvocationList().Length == 0) return;
            target.CacheEvents[eventName] -= callAction;
        }

        /// <summary>
        /// 移除 AddListener 注册的事件
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        public static void RemoveListener(this IEvent target, string eventName)
        {
            if (target == null || target.CacheEvents == null || target.CacheEvents.Count == 0) return;
            Action<object, object> list = null;
            if (!target.CacheEvents.TryGetValue(eventName, out list)) return;
            target.CacheEvents.Remove(eventName);
        }

        /// <summary>
        /// 清除 AddListener 注册的所有事件
        /// </summary>
        /// <param name="target"></param>
        public static void RemoveAllListener(this IEvent target)
        {
            if (target == null || target.CacheEvents == null || target.CacheEvents.Count == 0) return;
            target.CacheEvents.Clear();
            target.CacheEvents = null;
        }

        /// <summary>
        /// 触发用 AddListener 注册的事件
        /// </summary>
        /// <param name="target"></param>
        /// <param name="eventName"></param>
        /// <param name="param"></param>
        public static void TriggerListener(this IEvent target, string eventName, object param = default(object))
        {
            if (target == null || target.CacheEvents == null || target.CacheEvents.Count == 0) return;
            Action<object, object> list = null;
            if (!target.CacheEvents.TryGetValue(eventName, out list)) return;
            list.Invoke(list.Target, param);
        }
    }
}