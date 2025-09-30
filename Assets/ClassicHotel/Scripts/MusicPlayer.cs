using UnityEngine;
using UnityEngine.InputSystem;

namespace ClassicHotel
{
    [RequireComponent(typeof(SpriteRenderer), typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private Sprite _enabledSprite;
        [SerializeField] private Sprite _disabledSprite;

        private bool _isPlaying;

        private InputAction _controlMusicAction;

        private const string ControlMusicActionName = "ControlMusic";

        private void OnValidate()
        {
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        private void Start()
        {
            _controlMusicAction = InputSystem.actions.FindAction(ControlMusicActionName);

            _controlMusicAction.performed += (_) => InvertMusicState();
        }

        private void InvertMusicState()
        {
            _isPlaying = !_isPlaying;

            if (_isPlaying)
            {
                _spriteRenderer.sprite = _enabledSprite;
                _audioSource.Play();
            }
            else
            {
                _spriteRenderer.sprite = _disabledSprite;
                _audioSource.Pause();
            }
        }
    }
}