using UnityEngine;

namespace Library
{
    public interface IMonoBehaviour
    {
        void Awake();
        void OnDestroy();
        void Start();
        void Update();
        void FixedUpdate();
        void LateUpdate();
        void OnEnable();
        void OnDisable();
    }

    public interface ICollision
    {
        void OnCollisionEnter(Collision other);
        void OnCollisionStay(Collision other);
        void OnCollisionExit(Collision other);
    }

    public interface ICollider
    {
        void OnTriggerEnter(Collider other);
        void OnTriggerStay(Collider other);
        void OnTriggerExit(Collider other);
    }

    public interface ICollision2D
    {
        void OnCollisionEnter2D(Collision2D other);
        void OnCollisionStay2D(Collision2D other);
        void OnCollisionExit2D(Collision2D other);
    }

    public interface ICollider2D
    {
        void OnTriggerEnter2D(Collider2D other);
        void OnTriggerStay2D(Collider2D other);
        void OnTriggerExit2D(Collider2D other);
    }
}