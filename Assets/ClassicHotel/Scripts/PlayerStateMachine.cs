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

        private void OnEnable()
        {
            _controlWalkAndMusicAction.performed += ControlWalkAndMusic;

            _lookAroundAction.performed += _lookAroundAction_performed;
            _lookAroundAction.canceled += _lookAroundAction_performed;
        }

        private void OnDisable()
        {
            _controlWalkAndMusicAction.performed -= ControlWalkAndMusic;

            _lookAroundAction.performed -= _lookAroundAction_performed;
            _lookAroundAction.canceled -= _lookAroundAction_performed;
        }

        private void ControlWalkAndMusic(InputAction.CallbackContext context)
        {
            switch (_currentState)
            {
                case PlayerState.StandStill when _playerCameraRotator.CurrentLookState == LookState.Forward:
                    _currentState = PlayerState.WalkAndListenToMusic;
                    _playerMover.StartMoving();
                    _musicPlayer.Play();
                    break;
                case PlayerState.WalkAndListenToMusic:
                    _currentState = PlayerState.StandStill;
                    _playerMover.StopMoving();
                    _musicPlayer.Pause();
                    break;
            }
        }

        private void _lookAroundAction_performed(InputAction.CallbackContext context)
        {
            int lookInput = (int)context.ReadValue<float>();

            if (_currentState != PlayerState.StandStill)
            {
                return;
            }

            _playerCameraRotator.UpdateDesiredLookStateAndLookAtIt(lookInput);
        }
    }
}