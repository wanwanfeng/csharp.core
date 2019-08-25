using UnityEngine;

namespace Library
{
    public partial class CacheMonoBehaviour : MonoBehaviour, ILoad
    {
        private GameObject _gameObject;
        private Rigidbody _rigidbody;
        private Rigidbody2D _rigidbody2D;
        private Camera _camera;
        private Light _light;
        private Animation _animation;
        private Animator _animator;
        private ConstantForce _constantForce;
        private ConstantForce2D _constantForce2D;
        private Renderer _renderer;
        private AudioSource _audio;
        private GUIText _guiText;
        private NetworkView _networkView;
        private GUIElement _guiElement;
        private GUITexture _guiTexture;
        private Collider _collider;
        private Collider2D _collider2D;
        private HingeJoint _hingeJoint;
        private HingeJoint2D _hingeJoint2D;
        private Transform _transform;
        private ParticleEmitter _particleEmitter;
        private ParticleSystem _particleSystem;

        public new GameObject gameObject { get { return _gameObject == null ? (_gameObject = base.gameObject) : _gameObject; } }
        public new Rigidbody rigidbody { get { return _rigidbody == null ? (_rigidbody = gameObject.GetComponent<Rigidbody>()) : _rigidbody; } }
        public new Rigidbody2D rigidbody2D { get { return _rigidbody2D == null ? (_rigidbody2D = gameObject.GetComponent<Rigidbody2D>()) : _rigidbody2D; } }
        public new Camera camera { get { return _camera == null ? (_camera = gameObject.GetComponent<Camera>()) : _camera; } }
        public new Light light { get { return _light == null ? (_light = gameObject.GetComponent<Light>()) : _light; } }
        public new Animation animation { get { return _animation == null ? (_animation = gameObject.GetComponent<Animation>()) : _animation; } }
        public new Animator animator { get { return _animator == null ? (_animator = gameObject.GetComponent<Animator>()) : _animator; } }
        public new ConstantForce constantForce { get { return _constantForce == null ? (_constantForce = gameObject.GetComponent<ConstantForce>()) : _constantForce; } }
        public new ConstantForce2D constantForce2D { get { return _constantForce2D == null ? (_constantForce2D = gameObject.GetComponent<ConstantForce2D>()) : _constantForce2D; } }
        public new Renderer renderer { get { return _renderer == null ? (_renderer = gameObject.GetComponent<Renderer>()) : _renderer; } }
        public new AudioSource audio { get { return _audio == null ? (_audio = gameObject.GetComponent<AudioSource>()) : _audio; } }
        public new GUIText guiText { get { return _guiText == null ? (_guiText = gameObject.GetComponent<GUIText>()) : _guiText; } }
        public new NetworkView networkView { get { return _networkView == null ? (_networkView = gameObject.GetComponent<NetworkView>()) : _networkView; } }
        public new GUIElement guiElement { get { return _guiElement == null ? (_guiElement = gameObject.GetComponent<GUIElement>()) : _guiElement; } }
        public new GUITexture guiTexture { get { return _guiTexture == null ? (_guiTexture = gameObject.GetComponent<GUITexture>()) : _guiTexture; } }
        public new Collider collider { get { return _collider == null ? (_collider = gameObject.GetComponent<Collider>()) : _collider; } }
        public new Collider2D collider2D { get { return _collider2D == null ? (_collider2D = gameObject.GetComponent<Collider2D>()) : _collider2D; } }
        public new HingeJoint hingeJoint { get { return _hingeJoint == null ? (_hingeJoint = gameObject.GetComponent<HingeJoint>()) : _hingeJoint; } }
        public new HingeJoint2D hingeJoint2D { get { return _hingeJoint2D == null ? (_hingeJoint2D = gameObject.GetComponent<HingeJoint2D>()) : _hingeJoint2D; } }
        public new Transform transform { get { return _transform == null ? (_transform = gameObject.GetComponent<Transform>()) : _transform; } }
        public new ParticleEmitter particleEmitter { get { return _particleEmitter == null ? (_particleEmitter = gameObject.GetComponent<ParticleEmitter>()) : _particleEmitter; } }
        public new ParticleSystem particleSystem { get { return _particleSystem == null ? (_particleSystem = gameObject.GetComponent<ParticleSystem>()) : _particleSystem; } }

    }
}