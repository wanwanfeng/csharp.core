﻿using System;
using System.Collections;
using System.Threading;

namespace Library.Thread
{
    public class ThreadAction
    {
        private float _progress = 0;
        private bool _isDone = false;
        public bool IsSuccess = false;

        public ThreadAction()
        {
            _progress = 0;
            _isDone = IsSuccess = false;
        }

        #region

        public ThreadAction(Func<bool> func,int workerThreads = 1, int completionPortThreads = 5) : this()
        {
            ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (func != null)
                    IsSuccess = func();
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

        public ThreadAction(Func<Action<float>, bool> func, int workerThreads = 1, int completionPortThreads = 5): this()
        {
            ThreadPool.SetMaxThreads(workerThreads, completionPortThreads);
            ThreadPool.QueueUserWorkItem((obj) =>
            {
                if (func != null)
                    IsSuccess = func((val) =>
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
