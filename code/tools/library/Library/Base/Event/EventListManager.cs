using System.Collections.Generic;

public abstract class EventListManager<K, T> : Dictionary<K, List<T>>
{
    public bool addEvent(K key, T callAction)
    {
        List<T> list = null;
        if (TryGetValue(key, out list))
        {
            list.Add(callAction);
        }
        else
        {
            list = new List<T>() {callAction};
            this[key] = list;
        }
        return true;
    }

    public bool removeEvent(K key, T callAction)
    {
        List<T> list = null;
        if (TryGetValue(key, out list))
        {
            list.Remove(callAction);
            if (list.Count == 0)
            {
                Remove(key);
            }
            return true;
        }
        return false;
    }

    public bool removeEvent(K key)
    {
        List<T> list = null;
        if (TryGetValue(key, out list))
        {
            Remove(key);
            return true;
        }
        return false;
    }
}
