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

        private InputAction _lookAroundAction;

        private const string ControlWalkAndMusicActionName = "ControlWalkAndMusic";
        private const string LookAroundActionName = "LookAround";

        private void Awake()
        {
            _controlWalkAndMusicAction = InputSystem.actions.FindAction(ControlWalkAndMusicActionName);

            _lookAroundAction = InputSystem.actions.FindAction(LookAroundActionName);
        }

        private void Start()
        {
            _currentState = PlayerState.StandStill;

            _playerCameraRotator.EnableLook();
            _lookAroundAction.performed += HandleLookInput;
            _lookAroundAction.canceled += HandleLookInput;
        }

        private void OnEnable()
        {
            _controlWalkAndMusicAction.performed += ControlWalkAndMusic;
        }

        private void OnDisable()
        {
            _controlWalkAndMusicAction.performed -= ControlWalkAndMusic;
        }

        private void ControlWalkAndMusic(InputAction.CallbackContext context)
        {
            switch (_currentState)
            {
                case PlayerState.StandStill:
                    _currentState = PlayerState.WalkAndListenToMusic;

                    _lookAroundAction.performed -= HandleLookInput;
                    _lookAroundAction.canceled -= HandleLookInput;
                    _playerCameraRotator.DisableLook();

                    _playerMover.StartMoving();
                    _musicPlayer.Play();
                    break;
                case PlayerState.WalkAndListenToMusic:
                    _currentState = PlayerState.StandStill;

                    _playerCameraRotator.EnableLook();
                    _lookAroundAction.performed += HandleLookInput;
                    _lookAroundAction.canceled += HandleLookInput;

                    _playerMover.StopMoving();
                    _musicPlayer.Pause();
                    break;
            }
        }

        private void HandleLookInput(InputAction.CallbackContext context)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();

            _playerCameraRotator.UpdateLookInput(lookInput);
        }
    }
}