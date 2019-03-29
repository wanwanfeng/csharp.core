using System;
using System.Collections.Generic;

namespace UnityEngine.Library
{

    #region 加载接口

    public interface ILoad
    {
        bool HasPath(string path);
        T Load<T>(string filePath, Action<T> callAction = null) where T : Object;
    }

    #endregion
}