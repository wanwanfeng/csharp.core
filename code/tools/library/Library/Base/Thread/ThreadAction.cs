using System;
using System.Collections;
using System.Threading;

namespace Library.Thread
{
    public class ThreadAction
    {
        private float _progress = 0;
        private bool _isDone = false;
        public bool isSuccess = false;

        public ThreadAction()
        {
            _progress = 0;
            _isDone = isSuccess = false;
        }

        #region

        public ThreadAction(Func<bool> func) : this()
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (func != null)
                    isSuccess = func();
                _isDone = true;
            });
        }

        public IEnumerator WaitForComplete()
        {
            while (!_isDone)
                yield return null;
        }

        #endregion

        #region

        public ThreadAction(Func<Action<float>, bool> func) : this()
        {
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (func != null)
                    isSuccess = func((val) =>
                    {
                        _progress = val;
                    });
                _isDone = true;
            });
        }

        public IEnumerator WaitForComplete(Action<float> action, bool difFrame = true)
        {
            while (!_isDone)
            {
                if (difFrame)
                {
                    var cur = _progress;
                    yield return null;
                    action(_progress - cur);
                }
                else
                {
                    yield return null;
                    action(_progress);
                }
            }
        }

        #endregion
    }
}
