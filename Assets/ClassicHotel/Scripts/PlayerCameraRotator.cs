using System;
using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class PlayerCameraRotator : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;

        [SerializeField] private Transform _cameraTransform;
        private bool _canLook;

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

        private void OnValidate()
        {
            _cameraTransform = transform;
        }

        public void UpdateDesiredLookStateAndLookAtIt(int lookInput)
        {
            _desiredLookState = (LookState)lookInput;

            if (_currentLookState != _desiredLookState)
            {
                LookAtDesiredState();
            }
        }

        public void EnableLook()
        {
            if (_canLook)
            {
                throw new InvalidOperationException(nameof(_canLook));
            }

        private void LookForward()
        {
            Look(_lookForwardTweenSettings);
            _canLook = true;
        }

        public void DisableLook()
        {
            if (!_canLook)
            {
                throw new InvalidOperationException(nameof(_canLook));
            }

        private void LookBackFromRight()
        {
            Look(_rightLookBackTweenSettings);
            _canLook = false;
        }

        private void Look(TweenSettings<Vector3> tweenSettings)
        {
            if (_currentLookState == LookState.Turning)
            {
                Tween.StopAll(_cameraTransform);
            }

            _currentLookState = LookState.Turning;
            
            Tween.LocalRotation(_cameraTransform, tweenSettings)
                .OnComplete(() => _currentLookState = _desiredLookState);
        }
    }
}