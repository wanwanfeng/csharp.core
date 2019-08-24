using System;
using System.Collections.Generic;
using UnityEngine;

namespace Library
{
    public interface ISimpleState<T>
    {
        void OnEnter(T state);
        void OnUpdate();
        void OnExit();

        Func<T, bool> IsCanEnter { get; set; }
        Action<T> OnEnterAction { get; set; }
        Action<T> OnUpdateAction { get; set; }
        Action<T> OnExitAction { get; set; }
    }

    public class StateMachine<T> : ISimpleState<T>, ILoad
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
            bool isCanEnter = true;
            if (IsCanEnter != null)
                isCanEnter = IsCanEnter.Invoke(targetState);
            if (!isCanEnter) return;
            OnExit();
            StartTime = Time.time;
            Args = args;
            OnEnter(CurState = targetState);
        }

        public virtual void OnEnter(T state)
        {
            Debug.Log(GetType().Name + ":" + state);
            if (OnEnterAction != null)
                OnEnterAction.Invoke(state);
        }

        public virtual void OnExit()
        {
            if (OnExitAction != null)
                OnExitAction.Invoke(CurState);
        }

        public virtual void OnUpdate()
        {
            if (OnUpdateAction != null)
                OnUpdateAction.Invoke(CurState);
        }

        public Action<T> OnEnterAction { get; set; }
        public Action<T> OnUpdateAction { get; set; }
        public Action<T> OnExitAction { get; set; }
        public Func<T, bool> IsCanEnter { get; set; }
    }

    #region StateMachineInstanceBase

    /// <summary>
    /// 状态机管理器管理状态实例（状态实例也是一个状态机）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TInstance"></typeparam>
    public class StateMachineInstance<T, TInstance> : StateMachine<T> where TInstance : StateMachine<T>
    {
        public Dictionary<T, TInstance> CacheInstances;
        public TInstance CurInstance { get; set; }

        public virtual void Init()
        {
            CacheInstances = new Dictionary<T, TInstance>();
        }

        public override void OnEnter(T state)
        {
            base.OnEnter(state);
            if (CacheInstances[state] == null) return;
            CurInstance = CacheInstances[state];
            CurInstance.OnEnter(state);
        }

        public override void OnExit()
        {
            if (CurInstance == null) return;
            CurInstance.OnExit();
        }

        public override void OnUpdate()
        {
            if (CurInstance == null) return;
            CurInstance.OnUpdate();
        }
    }

    #endregion
}
