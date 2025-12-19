using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using UnityRandom = UnityEngine.Random;

namespace ClassicHotel
{
    [RequireComponent(typeof(MeshFilter), typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        [SerializeField] private MusicPlayerScreen _musicPlayerScreen;

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private RandomOneShotAudioSource _randomClickAudioSource;

        [SerializeField] private Image _playStateImage;

        [SerializeField] private Light _screenLight;

        [SerializeField] private Light _frontLight;
        [SerializeField] private Light _backLight;
        
        [SerializeField] private Ambience _ambience;

        [SerializeField] private AudioClip _scrollStepSound;

        [SerializeField] private TrackInfo _firstMusicTrack;
        [SerializeField] private TrackInfo[] _musicTracks;

        [SerializeField] private Vector3 _flashlightRotation;
        [SerializeField] private Vector3 _flashlightMove;

        [SerializeField] private float _screenLightIntensity;

        private bool _isPlaying;

        private int _tracksPlayed;

        private bool _isRapidGlitchTriggered;

        private int _rapidGlitchTriggerTrackCount;
        private float _rapidGlitchTrackStartTime;

        private float _currentPlaytime;
        private float _targetPlaytime;

        private readonly MutableWaitForSeconds _waitTime = new();

        private Coroutine _waitCoroutine;

        private const int MinScrolls = 1;
        private const int MaxScrolls = 4;

        private const float ScrollStepSoundVolumeScale = 2f;

        private const int FirstTrackPlaytime = 10;
        private const float FirstTrackStartTime = 0f;

        private const int RapidGlitchTriggerTrackMinCount = 1;
        private const int RapidGlitchTriggerTrackMaxCount = 2;

        private const float RapidGlitchTrackMinStartTime = 3;
        private const float RapidTrackMaxStartTime = RandomTrackMaxPlaytime * 0.8f;

        private const float RandomTrackMinPlaytime = 7f;
        private const float RandomTrackMaxPlaytime = 12f;

        public event Action TrackPaused;
        public event Action<float, float> TrackResumed;

        public event Action<TrackInfo, int, int> TrackChanged;

        private void OnValidate()
        {
            if (_transform == null)
            {
                _transform = transform;
            }
            
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        private void Start()
        {
            _rapidGlitchTriggerTrackCount = UnityRandom.Range(RapidGlitchTriggerTrackMinCount, RapidGlitchTriggerTrackMaxCount);
            _rapidGlitchTrackStartTime = UnityRandom.Range(RapidGlitchTrackMinStartTime, RapidTrackMaxStartTime);

            TrackChanged?.Invoke(_firstMusicTrack, 1, _musicTracks.Length);
        }

        private void Update()
        {
            if (_isPlaying)
            {
                _currentPlaytime += Time.deltaTime;
            }
            
            if (!_isPlaying || _isRapidGlitchTriggered || _tracksPlayed != _rapidGlitchTriggerTrackCount)
            {
                return;
            }

            const float MinDelta = 0.1f;
            if (_rapidGlitchTrackStartTime - _currentPlaytime <= MinDelta)
            {
                _isRapidGlitchTriggered = true;
                
                const float TargetAudioPitch = 0f;

                const float AudioPitchDuration = 1f;
                const Ease AudioPitchEase = Ease.OutCirc;

                Tween.AudioPitch(_audioSource, TargetAudioPitch, AudioPitchDuration, AudioPitchEase);
                _musicPlayerScreen.TriggerRapidScreenFlicker();
            }
        }

        public void Play()
        {
            if (_isPlaying)
            {
                return;
            }

            _isPlaying = true;

            _randomClickAudioSource.PlayOneShot();

            if (_currentPlaytime > 0f)
            {
                UnPauseCurrentTrack();
            }
            else
            {
                SetFirstTrackAndPlay();
            }

            _ambience.MuffleVolume();

            TrackResumed?.Invoke(_currentPlaytime, _targetPlaytime);
        }

        public void Pause()
        {
            if (!_isPlaying || _musicPlayerScreen.IsFlickering)
            {
                return;
            }

            _isPlaying = false;

            _randomClickAudioSource.PlayOneShot();

            PauseCurrentTrack();

            _ambience.NormalizeVolume();
            
            TrackPaused?.Invoke();
        }

        public void SwitchToFlashlightMode()
        {
            const float Duration = 0.6f;

            const Ease RotateEase = Ease.OutExpo;
            const Ease PositionEase = Ease.OutBack;

            const Ease IntensityEase = Ease.Default;

            TweenSettings<Vector3> rotationSettings = new(_flashlightRotation, Duration, RotateEase);
            TweenSettings<Vector3> positionSettings = new(_transform.localPosition + _flashlightMove, Duration, PositionEase);

            TweenSettings<float> intensitySettings = new(_screenLightIntensity, Duration, IntensityEase);

            Sequence.Create()
                .Chain(Tween.LocalRotation(_transform, rotationSettings))
                .Group(Tween.LocalPosition(_transform, positionSettings))
                .Group(Tween.LightIntensity(_screenLight, intensitySettings));
        }

        public void EnableRimLights()
        {
            _frontLight.enabled = true;
            _backLight.enabled = true;
        }

        private void SetFirstTrackAndPlay()
        {
            SetTrackAndPlay(_firstMusicTrack, FirstTrackPlaytime, FirstTrackStartTime);
        }

        private void SetRandomTrackAndPlay()
        {
            int index = UnityRandom.Range(0, _musicTracks.Length);
            float playtime = UnityRandom.Range(RandomTrackMinPlaytime, RandomTrackMaxPlaytime);
            float startTime = UnityRandom.Range(0f, _musicTracks[index].Clip.length - playtime);

            StartCoroutine(PlayScrollStepSoundsAndChangeMusic(_musicTracks[index], playtime, startTime));
        }

        private void SetTrackAndPlay(TrackInfo track, float duration, float startTime)
        {
            _audioSource.clip = track.Clip;
            _audioSource.time = startTime;
            _audioSource.Play();

            _currentPlaytime = 0f;
            _targetPlaytime = duration;

            _waitCoroutine = StartCoroutine(WaitForEndOfCurrentTrack(duration));

            int index = Array.IndexOf(_musicTracks, track);

            if (track == _firstMusicTrack)
            {
                TrackChanged?.Invoke(track, 1, _musicTracks.Length);

            }
            else
            {
                TrackChanged?.Invoke(track, index + 1, _musicTracks.Length);
            }
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

            _waitCoroutine = StartCoroutine(WaitForEndOfCurrentTrack(_targetPlaytime - _currentPlaytime));
        }

        private IEnumerator PlayScrollStepSoundsAndChangeMusic(TrackInfo track, float playtime, float startTime)
        {
            int count = UnityRandom.Range(MinScrolls, MaxScrolls);
            const float ScrollDelay = 0.15f;
            WaitForSeconds delay = new(ScrollDelay);

            _audioSource.Stop();

            for (int i = 0; i < count; i++)
            {
                _audioSource.PlayOneShot(_scrollStepSound, ScrollStepSoundVolumeScale);

                yield return delay;
            }

            SetTrackAndPlay(track, playtime, startTime);
            TrackResumed?.Invoke(_currentPlaytime, _targetPlaytime);
        }

        private IEnumerator WaitForEndOfCurrentTrack(float duration)
        {
            _waitTime.SetSeconds(duration);

            yield return _waitTime;

            _tracksPlayed++;
            SetRandomTrackAndPlay();
        }
    }
}