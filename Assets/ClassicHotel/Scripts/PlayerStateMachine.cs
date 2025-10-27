using UnityEngine;
using UnityEngine.InputSystem;

namespace ClassicHotel
{
    public class PlayerStateMachine : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private PlayerCameraRotator _playerCameraRotator;
        [SerializeField] private MusicPlayer _musicPlayer;

        private PlayerState _currentState;
        
        private InputAction _controlWalkAndMusicAction;

        private InputAction _lookAction;
        private InputAction _enableLookAction;

        private const string ControlWalkAndMusicActionName = "ControlWalkAndMusic";

        private const string LookActionName = "Look";
        private const string EnableLookActionName = "EnableLook";

        private void Awake()
        {
            _controlWalkAndMusicAction = InputSystem.actions.FindAction(ControlWalkAndMusicActionName);

            _lookAction = InputSystem.actions.FindAction(LookActionName);
            _enableLookAction = InputSystem.actions.FindAction(EnableLookActionName);
        }

        private void Start()
        {
            _currentState = PlayerState.StandStill;

            _enableLookAction.performed += OnEnableLookPerformed;
            _enableLookAction.canceled += OnEnableLookCanceled;

            _lookAction.performed += HandleLookInput;
            _lookAction.canceled += HandleLookInput;
        }

        private void OnEnable()
        {
            _controlWalkAndMusicAction.performed += ControlWalkAndMusic;
        }

        private void OnDisable()
        {
            _controlWalkAndMusicAction.performed -= ControlWalkAndMusic;
        }

        private void OnEnableLookPerformed(InputAction.CallbackContext obj)
        {
            _playerCameraRotator.EnableLook();
        }

        private void OnEnableLookCanceled(InputAction.CallbackContext obj)
        {
            _playerCameraRotator.DisableLook();
        }

        private void ControlWalkAndMusic(InputAction.CallbackContext context)
        {
            switch (_currentState)
            {
                case PlayerState.StandStill:
                    _currentState = PlayerState.WalkAndListenToMusic;

                    _enableLookAction.performed -= OnEnableLookPerformed;
                    _enableLookAction.canceled -= OnEnableLookCanceled;

                    _lookAction.performed -= HandleLookInput;
                    _lookAction.canceled -= HandleLookInput;

                    _playerMover.StartMoving();
                    _musicPlayer.Play();
                    break;
                case PlayerState.WalkAndListenToMusic:
                    _currentState = PlayerState.StandStill;

                    _enableLookAction.performed += OnEnableLookPerformed;
                    _enableLookAction.canceled += OnEnableLookCanceled;

                    _lookAction.performed += HandleLookInput;
                    _lookAction.canceled += HandleLookInput;

                    _playerMover.StopMoving();
                    _musicPlayer.Pause();
                    break;
            }
        }

        private void HandleLookInput(InputAction.CallbackContext context)
        {
            if (!_playerCameraRotator.CanLook)
            {
                return;
            }

            Vector2 lookInput = _lookAction.ReadValue<Vector2>();

            _playerCameraRotator.UpdateLookInput(lookInput);
        }
    }
}