using System;
using UnityEngine;
using UnityEngine.InputSystem;
using PrimeTween;

namespace ClassicHotel
{
    public class PlayerCameraRotator : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;

        private Transform _cameraTransform;

        private LookState _desiredLookState = LookState.Forward;

        private InputAction _lookBackAction;

        private readonly TweenSettings<Vector3> _lookForwardTweenSettings = new(Vector3.zero, LookForwardDuration, LookForwardEase);

        private readonly TweenSettings<Vector3> _leftLookBackTweenSettings = new(Vector3.up * LookBackYRotation, LookBackDuration,
            LookBackEase);
        private readonly TweenSettings<Vector3> _rightLookBackTweenSettings = new(Vector3.down * LookBackYRotation, LookBackDuration,
            LookBackEase);

        private const string LookBackActionName = "LookBack";

        private const float LookForwardDuration = 0.5f;
        private const Ease LookForwardEase = Ease.OutCubic;

        private const float LookBackYRotation = 165f;
        private const float LookBackDuration = 1f;
        private const Ease LookBackEase = Ease.InOutSine;

        private void OnValidate()
        {
            _cameraTransform = transform;
        }

        private void Awake()
        {
            _lookBackAction = InputSystem.actions.FindAction(LookBackActionName);
        }

        private void OnEnable()
        {
            _lookBackAction.performed += UpdateDesiredLookState;
            _lookBackAction.canceled += UpdateDesiredLookState;
        }

        private void OnDisable()
        {
            _lookBackAction.performed -= UpdateDesiredLookState;
            _lookBackAction.canceled -= UpdateDesiredLookState;
        }

        private void UpdateDesiredLookState(InputAction.CallbackContext context)
        {
            _desiredLookState = (LookState)context.ReadValue<float>();

            LookAtDesiredState();
        }

        private void LookAtDesiredState()
        {
            switch (_desiredLookState)
            {
                case LookState.Forward:
                    LookForward();
                    break;
                case LookState.BackFromLeft:
                    LookBackFromLeft();
                    break;
                case LookState.BackFromRight:
                    LookBackFromRight();
                    break;
                default:
                    throw new InvalidOperationException(nameof(_desiredLookState));
            }
        }

        private void LookForward() => Look(_lookForwardTweenSettings, LookState.Forward);

        private void LookBackFromLeft() => Look(_leftLookBackTweenSettings, LookState.BackFromLeft);

        private void LookBackFromRight() => Look(_rightLookBackTweenSettings, LookState.BackFromRight);

        private void Look(TweenSettings<Vector3> tweenSettings, LookState finishLookState)
        {
            Tween.StopAll(_cameraTransform);
            Tween.LocalRotation(_cameraTransform, tweenSettings)
                .OnComplete(() => _currentLookState = finishLookState);
        }
    }
}