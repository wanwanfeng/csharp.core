using System.Collections.Generic;

public abstract class EventListManager<K, T>
{
    protected Dictionary<K, List<T>> eventDic { get; set; }

    public EventListManager()
    {
        eventDic = new Dictionary<K, List<T>>();
    }

    public bool addEvent(K key, T callAction)
    {
        List<T> list = null;
        if (eventDic.TryGetValue(key, out list))
        {
            list.Add(callAction);
        }
        else
        {
            list = new List<T>() { callAction };
            eventDic[key] = list;
        }
        return true;
    }

    public bool removeEvent(K key, T callAction)
    {
        List<T> list = null;
        if (eventDic.TryGetValue(key, out list))
        {
            list.Remove(callAction);
            if (list.Count == 0)
            {
                eventDic.Remove(key);
            }
            return true;
        }
        return false;
    }

    public bool removeEvent(K key)
    {
        List<T> list = null;
        if (eventDic.TryGetValue(key, out list))
        {
            eventDic.Remove(key);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        eventDic.Clear();
    }

    public bool TryGetValue(K key, out List<T> instacne)
    {
        return eventDic.TryGetValue(key, out instacne);
    }
}
