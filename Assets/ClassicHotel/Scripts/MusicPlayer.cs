using UnityEngine;

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

        public void Play()
        {
            if (_isPlaying)
            {
                return;
            }

            _isPlaying = true;

            _spriteRenderer.sprite = _enabledSprite;
            
            if (_audioSource.time > 0f)
            {
                _audioSource.UnPause();
            }
            else
            {
                _audioSource.Play();
            }
        }

        public void Pause()
        {
            if (!_isPlaying)
            {
                return;
            }

            _isPlaying = false;

            _spriteRenderer.sprite = _disabledSprite;
            _audioSource.Pause();
        }
    }
}