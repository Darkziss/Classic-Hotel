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

        private LookState _currentLookState = LookState.Forward;
        private LookState _desiredLookState = LookState.Forward;

        private readonly TweenSettings<Vector3> _lookForwardTweenSettings = new(Vector3.zero, LookForwardDuration, LookForwardEase);

        private readonly TweenSettings<Vector3> _leftLookBackTweenSettings = new(Vector3.up * LookBackYRotation, LookBackDuration,
            LookBackEase);
        private readonly TweenSettings<Vector3> _rightLookBackTweenSettings = new(Vector3.down * LookBackYRotation, LookBackDuration,
            LookBackEase);

        public LookState CurrentLookState => _currentLookState;

        private const float LookForwardDuration = 0.5f;
        private const Ease LookForwardEase = Ease.OutCubic;

        private const float LookBackYRotation = 135f;
        private const float LookBackDuration = 1f;
        private const Ease LookBackEase = Ease.InOutSine;

        private void Update()
        {
            if (_canLook && _lookInput != Vector2.zero)
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
        }

        private void Look()
        {
            Vector2 rotationDelta = new(_lookInput.y, _lookInput.x);
            rotationDelta *= _sensitivity * Time.deltaTime;
            _rotation += rotationDelta;

            _headTransform.localRotation = Quaternion.Euler(_rotation);
        }
    }
}