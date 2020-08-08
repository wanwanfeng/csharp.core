using System;
using System.Collections.Generic;

namespace Library.Extensions
{
    /// <summary>
    /// Delegate扩展
    /// </summary>
    public static partial class DelegateExtensions
    {
        //----------------------------------------

        public static void Clear<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Action<T1, T2, T3, T4>;
            }
        }

        public static void Clear<T1, T2, T3>(this Action<T1, T2, T3> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Action<T1, T2, T3>;
            }
        }

        public static void Clear<T1, T2>(this Action<T1, T2> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Action<T1, T2>;
            }
        }

        public static void Clear<T1>(this Action<T1> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Action<T1>;
            }
        }

        public static void Clear(this Action target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Action;
            }
        }

        //----------------------------------------

        public static void Call<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> target, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (target == null) return;
            target.Invoke(t1, t2, t3, t4);
        }

        public static void Call<T1, T2, T3>(this Action<T1, T2, T3> target, T1 t1, T2 t2, T3 t3)
        {
            if (target == null) return;
            target.Invoke(t1, t2, t3);
        }

        public static void Call<T1, T2>(this Action<T1, T2> target, T1 t1, T2 t2)
        {
            if (target == null) return;
            target.Invoke(t1, t2);
        }

        public static void Call<T>(this Action<T> target, T t)
        {
            if (target == null) return;
            target.Invoke(t);
        }

        public static void Call(this Action target)
        {
            if (target == null) return;
            target.Invoke();
        }

        //----------------------------------------

        public static void Clear<T1, T2, T3, T4, T>(this Func<T1, T2, T3, T4, T> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Func<T1, T2, T3, T4, T>;
            }
        }

        public static void Clear<T1, T2, T3, T>(this Func<T1, T2, T3, T> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Func<T1, T2, T3, T>;
            }
        }

        public static void Clear<T1, T2, T>(this Func<T1, T2, T> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Func<T1, T2, T>;
            }
        }

        public static void Clear<T1, T>(this Func<T1, T> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Func<T1, T>;
            }
        }

        public static void Clear<T>(this Func<T> target)
        {
            if (target == null) return;
            Delegate[] dels = target.GetInvocationList();
            for (int i = 0; i < dels.Length; i++)
            {
                target -= dels[i] as Func<T>;
            }
        }

        //----------------------------------------

        public static T Call<T1, T2, T3, T4, T>(this Func<T1, T2, T3, T4, T> target, T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (target == null) return default(T);
            return target.Invoke(t1, t2, t3, t4);
        }

        public static T Call<T1, T2, T3, T>(this Func<T1, T2, T3, T> target, T1 t1, T2 t2, T3 t3)
        {
            if (target == null) return default(T);
            return target.Invoke(t1, t2, t3);
        }

        public static T Call<T1, T2, T>(this Func<T1, T2, T> target, T1 t1, T2 t2)
        {
            if (target == null) return default(T);
            return target.Invoke(t1, t2);
        }

        public static T Call<T1, T>(this Func<T1, T> target, T1 t1)
        {
            if (target == null) return default(T);
            return target.Invoke(t1);
        }

        public static T Call<T>(this Func<T> target)
        {
            if (target == null) return default(T);
            return target.Invoke();
        }
    }
}