using System;
using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class PlayerCameraRotator : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;

        [SerializeField] private Transform _headTransform;

        [SerializeField] private float _sensitivity = 1f;

        private bool _canLook;

        private Vector2 _rotation;

        private Vector2 _lookInput;

        public bool CanLook => _canLook;

        private bool HasLookInput => _lookInput != Vector2.zero;

        private const float MinXRotation = -90f;
        private const float MaxXRotation = 85f;

        private const float MinYRotation = -MaxYRotation;
        private const float MaxYRotation = 140f;

        private void Update()
        {
            if (_canLook && HasLookInput)
            {
                Look();
            }
        }

        public void UpdateLookInput(Vector2 delta)
        {
            if (!_canLook)
            {
                throw new InvalidOperationException(nameof(_canLook));
            }
            
            _lookInput = delta;
        }

        public void EnableLook()
        {
            if (_canLook)
            {
                throw new InvalidOperationException(nameof(_canLook));
            }

            _canLook = true;
        }

        public void DisableLook()
        {
            if (!_canLook)
            {
                throw new InvalidOperationException(nameof(_canLook));
            }

            _canLook = false;

            _lookInput = Vector2.zero;

            TweenSettings<Vector3> settings = new(Vector3.zero, 0.3f, Ease.OutSine);
            Tween.LocalRotation(_headTransform, settings)
                .OnComplete(() => _rotation = Vector2.zero);
        }

        private void Look()
        {
            Vector2 rotationDelta = new(_lookInput.y, _lookInput.x);
            rotationDelta *= _sensitivity * Time.deltaTime;

            _rotation += rotationDelta;
            _rotation.x = Mathf.Clamp(_rotation.x, MinXRotation, MaxXRotation);
            _rotation.y = Mathf.Clamp(_rotation.y, MinYRotation, MaxYRotation);

            _headTransform.localRotation = Quaternion.Euler(_rotation);
        }
    }
}