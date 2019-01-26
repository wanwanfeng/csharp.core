using System;
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

        public Promise()
        {
            state = State.init;
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
            if (state == State.success || state == State.error)
                throw new Exception("this state is " + state);
            state = State.process;
            processArg = obj;
            processAction.Invoke(obj);
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
            if (state == State.success) return;
            state = State.success;
            successArg = obj;
            successAction.Invoke(obj);
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
            if (state == State.error) return;
            state = State.error;
            errorArg = obj;
            errorAction.Invoke(obj);
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
        }

        public IPromise GetProtected()
        {
            return this;
        }

        public static IPromise New
        {
            get { return new Promise(); }
        }

        public static IPromise When(params IPromise[] promises)
        {
            return Promise.New;
        }
    }
}