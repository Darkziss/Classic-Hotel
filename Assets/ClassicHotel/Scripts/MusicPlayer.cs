using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(MeshFilter), typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _clickAudioSource;
        
        [SerializeField] private AudioSource _ambienceAudioSource;

        [SerializeField] private Color32 _screenEnabledColor = Color.white;
        [SerializeField] private Color32 _screenDisabledColor = Color.black;

        [SerializeField] private AudioClip[] _clickAudioClips;

        [SerializeField] private AudioClip[] _musicAudioClips;
        [SerializeField] private AudioClip[] _musicTracks;

        private bool _isPlaying;

        private const float MinStartAudioDelta = 0.01f;
        private const float MinEndAudioDelta = 1f;

        private const float NormalAmbienceVolume = 1f;
        private const float MuffledAmbienceVolume = 0.3f;

        private const int ScreenMaterialIndex = 1;

        private void OnValidate()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
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

            _meshRenderer.sharedMaterials[ScreenMaterialIndex].color = Color.white;

            if (_audioSource.time < MinStartAudioDelta)
            {
                _audioSource.Play();
            }
            else if (_audioSource.clip.length - _audioSource.time > MinEndAudioDelta)
            {
                _audioSource.UnPause();
            }
            else
            {
                ChangeMusicToRandom();
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

            _meshRenderer.sharedMaterials[ScreenMaterialIndex].color = _screenDisabledColor;

            _audioSource.Pause();

            _ambienceAudioSource.volume = NormalAmbienceVolume;
        }

        private void PlayRandomClickSound()
        {
            int clickClipIndex = Random.Range(0, _clickAudioClips.Length);

            _clickAudioSource.clip = _clickAudioClips[clickClipIndex];
            _clickAudioSource.Play();
        }

        private void ChangeMusicToRandom()
        {
            int index = UnityRandom.Range(0, _musicTracks.Length);
            _audioSource.clip = _musicTracks[index];
        }
    }
}