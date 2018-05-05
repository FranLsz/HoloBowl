using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(HandDraggable), typeof(Rigidbody))]
    public class BallManager : MonoBehaviour
    {
        public float Multiplier = 1;

        public Action OnStartDragg;
        public Action OnStopDragg;
        public bool IsDraggable = true;

        private HandDraggable _handDraggable;
        private Rigidbody _rigidbody;
        private Vector3 _initialPosition;

        private Vector3 _lastPosition;
        private Vector3 _direction;

        private void Awake()
        {
            _initialPosition = transform.position;
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _handDraggable = GetComponent<HandDraggable>();

            _handDraggable.StartedDragging += OnStartDragging;
            _handDraggable.StoppedDragging += OnStopDragging;
        }

        private void Update()
        {
            _handDraggable.IsDraggingEnabled = IsDraggable;
        }

        private void LateUpdate()
        {
            _direction = transform.position - _lastPosition;
            _lastPosition = transform.position;
        }

        void OnStartDragging()
        {
            _lastPosition = transform.position;
            _rigidbody.useGravity = false;

            if (OnStartDragg != null)
                OnStartDragg();
        }

        void OnStopDragging()
        {
            _rigidbody.useGravity = true;
            _rigidbody.velocity = _direction.normalized * Multiplier;

            if (OnStopDragg != null)
                OnStopDragg();
        }

        public void Reset()
        {
            _rigidbody.velocity = Vector3.zero;
            transform.position = _initialPosition;
        }
    }
}
