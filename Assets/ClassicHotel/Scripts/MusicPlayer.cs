using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(SpriteRenderer), typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _clickAudioSource;
        
        [SerializeField] private AudioSource _ambienceAudioSource;

        [SerializeField] private Sprite _enabledSprite;
        [SerializeField] private Sprite _disabledSprite;

        [SerializeField] private AudioClip[] _clickAudioClips;

        private bool _isPlaying;

        private const float NormalAmbienceVolume = 1f;
        private const float MuffledAmbienceVolume = 0.3f;

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

            PlayRandomClickSound();

            _spriteRenderer.sprite = _enabledSprite;

            if (_audioSource.time > 0f)
            {
                _audioSource.UnPause();
            }
            else
            {
                _audioSource.Play();
            }

            _ambienceAudioSource.volume = MuffledAmbienceVolume;
        }

        public void Pause()
        {
            if (!_isPlaying)
            {
                return;
            }

            _isPlaying = false;

            PlayRandomClickSound();

            _spriteRenderer.sprite = _disabledSprite;
            _audioSource.Pause();

            _ambienceAudioSource.volume = NormalAmbienceVolume;
        }

        private void PlayRandomClickSound()
        {
            int clickClipIndex = Random.Range(0, _clickAudioClips.Length);

            _clickAudioSource.clip = _clickAudioClips[clickClipIndex];
            _clickAudioSource.Play();
        }
    }
}