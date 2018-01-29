using System;
using System.Collections.Generic;

public class EventActionListManager<TK, T> : EventListManager<TK, Action<T>>
{
    //public void throwEvent(K key, T param = default(T))
    //{
    //    List<Action<T>> list = null;
    //    if (TryGetValue(key, out list))
    //    {
    //        var remove = new List<Action<T>>();
    //        foreach (var action in list)
    //        {
    //            if (action.Target.Equals(null))
    //            {
    //                remove.Add(action);
    //                continue;
    //            }
    //            action.Invoke(param);
    //        }
    //        if (remove.Count == 0) return;
    //        list.RemoveAll(p => remove.Contains(p));
    //    }
    //}

    public void throwEvent(TK key, T param = default(T))
    {
        List<Action<T>> list = null;
        if (TryGetValue(key, out list))
        {
            foreach (var action in list)
            {
                action.Invoke(param);
            }
        }
    }
}
