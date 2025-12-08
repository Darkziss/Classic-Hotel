using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerStateMachine : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private PlayerCameraRotator _playerCameraRotator;
        [SerializeField] private MusicPlayer _musicPlayer;

        [SerializeField] private PlayerInput _playerInput;

        private PlayerState _currentState;

        private bool _isBlackoutMode;
        
        private void OnValidate()
        {
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }
        }

        private void Start()
        {
            _currentState = PlayerState.StandStill;
        }

        private void OnEnable()
        {
            _playerInput.ControlWalkAndMusicActionPerformed += ControlWalkAndMusic;
            
            _playerInput.LookActionTriggered += HandleLookInput;

            _playerInput.EnableLookActionPerformed += _playerCameraRotator.EnableLook;
            _playerInput.EnableLookActionCanceled += _playerCameraRotator.DisableLook;
        }

        private void OnDisable()
        {
            _playerInput.ControlWalkAndMusicActionPerformed -= ControlWalkAndMusic;

            _playerInput.LookActionTriggered -= HandleLookInput;

            _playerInput.EnableLookActionPerformed -= _playerCameraRotator.EnableLook;
            _playerInput.EnableLookActionCanceled -= _playerCameraRotator.DisableLook;
        }

        public void SwitchToBlackoutMode()
        {
            _currentState = PlayerState.StandStill;

            _isBlackoutMode = true;

            _playerInput.DisableWalk();

            _playerInput.EnableLook();

            if (_playerMover.IsMoving)
            {
                _playerMover.StopMoving();
            }

            _musicPlayer.Pause();
        }

        public void EnableWalkInBlackoutMode()
        {
            _playerMover.ChangeSpeedToBlackoutSpeed();

            _playerInput.EnableWalk();
        }

        public void ParalyzePlayer()
        {
            _currentState = PlayerState.StandStill;

            _playerInput.DisableWalk();

            _playerInput.DisableLook();

            if (_playerMover.IsMoving)
            {
                _playerMover.StopMoving();
            }
        }

        public void ForceStop()
        {
            _currentState = PlayerState.StandStill;

            if (_playerMover.IsMoving)
            {
                _playerMover.StopMoving();
            }
            _musicPlayer.Pause();

            _playerInput.EnableLook();
        }

        private void ControlWalkAndMusic()
        {
            switch (_currentState)
            {
                case PlayerState.StandStill:
                    _currentState = PlayerState.WalkAndListenToMusic;

                    _playerInput.DisableLook();

                    _playerMover.StartMoving();

                    if (!_isBlackoutMode)
                    {
                        _musicPlayer.Play();
                    }

                    break;
                case PlayerState.WalkAndListenToMusic:
                    _currentState = PlayerState.StandStill;

                    _playerInput.EnableLook();

                    _playerMover.StopMoving();
                    
                    if (!_isBlackoutMode)
                    {
                        _musicPlayer.Pause();
                    }

                    break;
            }
        }

        private void HandleLookInput(Vector2 input)
        {
            if (!_playerCameraRotator.CanLook)
            {
                return;
            }

            _playerCameraRotator.UpdateLookInput(input);
        }
    }
}