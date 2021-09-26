using System;
using UnityEngine;

namespace Library
{
    public class TriggerMonoBehaviour : MonoBehaviour
    {
        public Action<GameObject, Collision> OnCollisionEnterAction;
        public Action<GameObject, Collision> OnCollisionStayAction;
        public Action<GameObject, Collision> OnCollisionExitAction;

        public Action<GameObject, Collider> OnTriggerEnterAction;
        public Action<GameObject, Collider> OnTriggerStayAction;
        public Action<GameObject, Collider> OnTriggerExitAction;

        private void OnCollisionEnter(Collision col) => OnCollisionEnterAction?.Invoke(gameObject, col);

        private void OnCollisionStay(Collision col) => OnCollisionStayAction?.Invoke(gameObject, col);

        private void OnCollisionExit(Collision col) => OnCollisionExitAction?.Invoke(gameObject, col);

        private void OnTriggerEnter(Collider col) => OnTriggerEnterAction?.Invoke(gameObject, col);

        private void OnTriggerStay(Collider col) => OnTriggerStayAction?.Invoke(gameObject, col);

        private void OnTriggerExit(Collider col) => OnTriggerExitAction?.Invoke(gameObject, col);
    }
}