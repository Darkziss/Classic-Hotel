using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClassicHotel
{
    public class PlayerInput : MonoBehaviour
    {
        private InputAction _controlWalkAndMusicAction;

        private InputAction _lookAction;
        private InputAction _enableLookAction;

        private const string ControlWalkAndMusicActionName = "ControlWalkAndMusic";

        private const string LookActionName = "Look";
        private const string EnableLookActionName = "EnableLook";

        public event Action ControlWalkAndMusicActionPerformed;

        public event Action<Vector2> LookActionTriggered;

        public event Action EnableLookActionPerformed;
        public event Action EnableLookActionCanceled;

        private void Awake()
        {
            _controlWalkAndMusicAction = InputSystem.actions.FindAction(ControlWalkAndMusicActionName);

            _lookAction = InputSystem.actions.FindAction(LookActionName);
            _enableLookAction = InputSystem.actions.FindAction(EnableLookActionName);
        }

        private void OnEnable()
        {
            _controlWalkAndMusicAction.performed += OnControlWalkAndMusicActionPerformed;

            _lookAction.performed += OnLookActionTriggered;
            _lookAction.canceled += OnLookActionTriggered;

            _enableLookAction.performed += OnEnableLookActionPerformed;
            _enableLookAction.canceled += OnEnableLookActionCanceled;
        }

        private void OnDisable()
        {
            _controlWalkAndMusicAction.performed -= OnControlWalkAndMusicActionPerformed;

            _lookAction.performed -= OnLookActionTriggered;

            _enableLookAction.performed -= OnEnableLookActionPerformed;
            _enableLookAction.canceled -= OnEnableLookActionCanceled;
        }

        public void EnableWalk()
        {
            _controlWalkAndMusicAction.Enable();
        }

        public void DisableWalk()
        {
            _controlWalkAndMusicAction.Disable();
        }

        public void EnableLook()
        {
            _lookAction.Enable();
            _enableLookAction.Enable();
        }

        public void DisableLook()
        {
            _lookAction.Disable();
            _enableLookAction.Disable();
        }

        private void OnControlWalkAndMusicActionPerformed(InputAction.CallbackContext context)
        {
            ControlWalkAndMusicActionPerformed?.Invoke();
        }

        private void OnLookActionTriggered(InputAction.CallbackContext context)
        {
            Vector2 input = _lookAction.ReadValue<Vector2>();

            LookActionTriggered?.Invoke(input);
        }

        private void OnEnableLookActionPerformed(InputAction.CallbackContext context)
        {
            EnableLookActionPerformed?.Invoke();
        }

        private void OnEnableLookActionCanceled(InputAction.CallbackContext context)
        {
            EnableLookActionCanceled?.Invoke();
        }
    }
}