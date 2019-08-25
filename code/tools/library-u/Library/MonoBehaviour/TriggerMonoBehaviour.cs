using Library.Extensions;
using System;
using UnityEngine;

namespace Library
{
    public class TriggerMonoBehaviour : CacheMonoBehaviour
    {
        public Action<GameObject, Collision> OnCollisionEnterAction;
        public Action<GameObject, Collision> OnCollisionStayAction;
        public Action<GameObject, Collision> OnCollisionExitAction;

        public Action<GameObject, Collider> OnTriggerEnterAction;
        public Action<GameObject, Collider> OnTriggerStayAction;
        public Action<GameObject, Collider> OnTriggerExitAction;

        private void OnCollisionEnter(Collision col)
        {
            OnCollisionEnterAction.Call(gameObject, col);
        }

        private void OnCollisionStay(Collision col)
        {
            OnCollisionStayAction.Call(gameObject, col);
        }

        private void OnCollisionExit(Collision col)
        {
            OnCollisionExitAction.Call(gameObject, col);
        }

        private void OnTriggerEnter(Collider col)
        {
            OnTriggerEnterAction.Call(gameObject, col);
        }

        private void OnTriggerStay(Collider col)
        {
            OnTriggerStayAction.Call(gameObject, col);
        }

        private void OnTriggerExit(Collider col)
        {
            OnTriggerExitAction.Call(gameObject, col);
        }
    }
}