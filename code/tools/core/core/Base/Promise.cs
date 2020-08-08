using System;
using System.Collections.Generic;
using System.Linq;
using Library.Extensions;

namespace Core
{
    public interface IPromise
    {
        IPromise Process(params Action<object>[] action);
        void Notify(object obj);
        IPromise Done(params Action<object>[] action);
        void Resolve(object obj);
        IPromise Fail(params Action<object>[] action);
        void Reject(object obj);
        IPromise Then(Action<object> success = null, Action<object> error = null, Action<object> process = null);
        IPromise Always(params Action<object>[] action);
        void Destroy();
        IPromise GetProtected();
    }

    public class Promise : IPromise
    {
        internal enum State
        {
            init,
            process,
            success,
            error,
        }

        private State state;

        private Action<object> successAction, errorAction, processAction;

        private object successArg, errorArg, processArg;

        protected bool isProtected = false;

        /// <summary>
        /// 无参数由外部改变状态
        /// </summary>
        public Promise()
        {
            state = State.init;
            isProtected = false;
        }

        /// <summary>
        /// 有参数自动改变状态
        /// </summary>
        /// <param name="arg"></param>
        public Promise(object arg) : this()
        {
            this.Resolve(arg);
        }

        public IPromise Process(params Action<object>[] process)
        {
            process.ForEach(e => processAction += e);
            if (state == State.process)
                process.ForEach(e => e.Invoke(processArg));
            return this;
        }

        public void Notify(object obj)
        {
            if (isProtected) return;
            if (state == State.success || state == State.error)
                throw new Exception("this state is " + state);
            state = State.process;
            processAction.Invoke(processArg = obj);
        }

        public IPromise Done(params Action<object>[] success)
        {
            success.ForEach(e => successAction += e);
            if (state == State.success)
                success.ForEach(e => e.Invoke(successArg));
            return this;
        }

        public void Resolve(object obj)
        {
            if (isProtected) return;
            if (state == State.success) return;
            state = State.success;
            successAction.Invoke(successArg = obj);
        }

        public IPromise Fail(params Action<object>[] error)
        {
            error.ForEach(e => errorAction += e);
            if (state == State.error)
                error.ForEach(e => e.Invoke(errorArg));
            return this;
        }

        public void Reject(object obj)
        {
            if (isProtected) return;
            if (state == State.error) return;
            state = State.error;
            errorAction.Invoke(errorArg = obj);
        }

        public IPromise Then(Action<object> success = null, Action<object> error = null, Action<object> process = null)
        {
            return Done(success).Fail(error).Process(process);
        }

        public IPromise Always(params Action<object>[] action)
        {
            return Done(action).Fail(action);
        }

        public void Destroy()
        {
            processAction = null;
            successAction = null;
            errorAction = null;

            successArg = null;
            errorArg = null;
            processArg = null;
        }

        /// <summary>
        /// 不已受保护的，状态允许外部更改
        ///  允许调用
        ///  void Notify(object obj);
        ///  void Resolve(object obj);
        ///  void Reject(object obj);    
        /// </summary>
        /// <returns></returns>
        public IPromise GetPromise()
        {
            isProtected = false;   
            return this;
        }

        /// <summary>
        /// 已受保护的，状态不允许外部更改
        ///  不允许调用以下方法(调用无效)
        ///  void Notify(object obj);
        ///  void Resolve(object obj);
        ///  void Reject(object obj);   
        /// </summary>
        /// <returns></returns>
        public IPromise GetProtected()
        {
            isProtected = true;
            return this;
        }

        public static IPromise New
        {
            get { return new Promise(); }
        }
    }

    public class When : Promise
    {
        private int successCount, errorSuccess;
        private Dictionary<IPromise, object> cacheObjects;

        public IPromise Run(params IPromise[] promises)
        {
            isProtected = true;
            cacheObjects = new Dictionary<IPromise, object>();
            successCount = promises.Length;
            errorSuccess = 0;
            promises.ForEach(p =>
            {
                p.GetProtected().Done(q =>
                {
                    successCount--;
                    cacheObjects[p] = q;
                    if (successCount <= 0) this.Resolve(cacheObjects);
                }).Fail(q =>
                {
                    errorSuccess++;
                    cacheObjects[p] = q;
                    if (errorSuccess > 0) this.Reject(cacheObjects);
                }).Process(q => { this.Notify(q); });
            });
            return this;
        }

        public When(params object[] args)
        {
            Run(args.Select(p => (p is IPromise) ? (IPromise) p : new Promise(args)).ToArray());
        }

        public When(params IPromise[] promises)
        {
            Run(promises);
        }
    }
}