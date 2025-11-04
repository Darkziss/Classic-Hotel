using System.Collections;
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

        [SerializeField] private AudioClip _firstMusicTrack;
        [SerializeField] private AudioClip[] _musicTracks;

        private bool _isPlaying;

        private float _currentPlaytime;
        private float _targetPlaytime;

        private readonly MutableWaitForSeconds _waitTime = new();

        private Coroutine _waitCoroutine;

        private const int FirstTrackPlaytime = 10;

        private const float RandomTrackMinPlaytime = 7f;
        private const float RandomTrackMaxPlaytime = 12f;

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

            if (_audioSource.time > 0f)
            {
                UnPauseCurrentTrack();
            }
            else
            {
                SetFirstTrackAndPlay();
            }

            _ambienceAudioSource.volume = MuffledAmbienceVolume;

            _meshRenderer.sharedMaterials[ScreenMaterialIndex].color = Color.white;
        }

        public void Pause()
        {
            if (!_isPlaying)
            {
                return;
            }

            _isPlaying = false;

            PlayRandomClickSound();

            PauseCurrentTrack();

            _ambienceAudioSource.volume = NormalAmbienceVolume;
            
            _meshRenderer.sharedMaterials[ScreenMaterialIndex].color = _screenDisabledColor;
        }

        private void PlayRandomClickSound()
        {
            int clickClipIndex = Random.Range(0, _clickAudioClips.Length);

            _clickAudioSource.clip = _clickAudioClips[clickClipIndex];
            _clickAudioSource.Play();
        }

        private void SetFirstTrackAndPlay()
        {
            SetTrackAndPlay(_firstMusicTrack, FirstTrackPlaytime);
        }

        private void SetRandomTrackAndPlay()
        {
            int index = Random.Range(0, _musicTracks.Length);
            _targetPlaytime = Random.Range(RandomTrackMinPlaytime, RandomTrackMaxPlaytime);

            SetTrackAndPlay(_musicTracks[index], _targetPlaytime);
        }

        private void SetTrackAndPlay(AudioClip track, float duration)
        {
            _audioSource.clip = track;
            _audioSource.Play();

            _waitCoroutine = StartCoroutine(WaitForEndOfCurrentTrack(duration));
        }

        private void PauseCurrentTrack()
        {
            _audioSource.Pause();

            StopCoroutine(_waitCoroutine);
            _waitCoroutine = null;
        }

        private void UnPauseCurrentTrack()
        {
            _audioSource.UnPause();

            _waitCoroutine = StartCoroutine(WaitForEndOfCurrentTrack(_targetPlaytime));
        }

        private IEnumerator WaitForEndOfCurrentTrack(float duration)
        {
            _waitTime.SetSeconds(duration);

            yield return _waitTime;

            SetRandomTrackAndPlay();
        }
    }
}