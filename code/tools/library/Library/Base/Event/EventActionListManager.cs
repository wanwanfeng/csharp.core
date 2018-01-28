using System;
using System.Collections.Generic;

public class EventActionListManager<K, T> : EventListManager<K, Action<T>>
{
    public void throwEvent(K key, T param = default(T))
    {
        List<Action<T>> list = null;
        if (eventDic.TryGetValue(key, out list))
        {
            var remove = new List<Action<T>>();
            foreach (var action in list)
            {
                if (action.Target.Equals(null))
                {
                    remove.Add(action);
                    continue;
                }
                action.Invoke(param);
            }
            if (remove.Count == 0) return;
            list.RemoveAll(p => remove.Contains(p));
        }
    }
}
