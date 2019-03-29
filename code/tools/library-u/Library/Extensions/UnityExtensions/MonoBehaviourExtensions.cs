using System;
using System.Collections;

namespace UnityEngine.Library
{
    /// <summary>
    /// Component扩展
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        #region 延迟

        /// <summary>
        /// 秒延迟
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="seconds"></param>
        /// <param name="endAction"></param>
        /// <param name="perSecondAction"></param>
        public static void DelayTo(this MonoBehaviour monoBehaviour, float seconds, Action endAction,
            Action perSecondAction = null)
        {
            monoBehaviour.StartCoroutine(DelayToIEnumerator(seconds, endAction, perSecondAction));
        }

        private static IEnumerator DelayToIEnumerator(float seconds, Action endAction, Action perSecondAction,
            bool ignoreTimeScale = true)
        {
            if (seconds <= 1)
            {
                yield return new WaitForSeconds(ignoreTimeScale ? seconds * Time.timeScale : seconds);
            }
            else
            {
                for (int i = 0; i < seconds; i++)
                {
                    yield return new WaitForSeconds(ignoreTimeScale ? 1 * Time.timeScale : 1);
                    if (perSecondAction != null)
                        perSecondAction();
                }
            }

            if (endAction != null)
                endAction();
        }

        /// <summary>
        /// 帧延迟
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="frameCount"></param>
        /// <param name="endAction"></param>
        /// <param name="perframeAction"></param>
        public static void DelayTo(this MonoBehaviour monoBehaviour, int frameCount, Action endAction,
            Action perframeAction = null)
        {
            monoBehaviour.StartCoroutine(DelayToIEnumerator(frameCount, endAction, perframeAction));
        }

        private static IEnumerator DelayToIEnumerator(int frameCount, Action endAction, Action perframeAction)
        {
            for (int i = 0; i < frameCount; i++)
            {
                yield return new WaitForEndOfFrame();
                if (perframeAction != null)
                    perframeAction();
            }

            if (endAction != null)
                endAction();
        }

        #endregion
    }
}