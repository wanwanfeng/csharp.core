using Library.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Library
{
    public interface ISimpleState<T>
    {
        Func<T, bool> IsCanEnter { get; set; }
        Action<T> OnEnterAction { get; set; }
        Action<T> OnUpdateAction { get; set; }
        Action<T> OnExitAction { get; set; }
    }

    public class StateMachine<T> : ISimpleState<T>
    {
        private float startTime;

        public float StartTime
        {
            get { return Time.time - startTime; }
            private set { startTime = value; }
        }

        public T CurState { get; private set; }

        public object Args { get; private set; }

        public void ChangeState(T targetState, object args = null)
        {
            var isCanEnter = IsCanEnter == null ? true : IsCanEnter.Invoke(targetState);
            if (isCanEnter == false) return;

            OnExitAction?.Invoke(CurState);
            StartTime = Time.time;
            Args = args;
            OnEnterAction?.Invoke(CurState = targetState);
        }

        public Action<T> OnEnterAction { get; set; }
        public Action<T> OnUpdateAction { get; set; }
        public Action<T> OnExitAction { get; set; }
        public Func<T, bool> IsCanEnter { get; set; }
    }

    public class StateMachineCache<T> : StateMachine<T>
    {
        protected static Action<T> StateAction = (state) => { };
        protected Dictionary<T, Action<T>> CacheEnter;
        protected Dictionary<T, Action<T>> CacheUpdate;
        protected Dictionary<T, Action<T>> CacheExit;

        public virtual void Initialization(object args)
        {
            CacheEnter = EnumHelper.ToIntDic<T>().ToDictionary(p => p.Key, p => StateAction);
            CacheUpdate = EnumHelper.ToIntDic<T>().ToDictionary(p => p.Key, p => StateAction);
            CacheExit = EnumHelper.ToIntDic<T>().ToDictionary(p => p.Key, p => StateAction);
            OnEnterAction += (state) => { CacheEnter[state].Invoke(state); };
            OnUpdateAction += (state) => { CacheUpdate[state].Invoke(state); };
            OnExitAction += (state) => { CacheExit[state].Invoke(state); };
        }
    }
}
