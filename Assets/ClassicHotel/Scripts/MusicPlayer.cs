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
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _clickAudioSource;

        [SerializeField] private Canvas _screenCanvas;
        [SerializeField] private CanvasGroup _screenCanvasGroup;

        [SerializeField] private Image _playStateImage;

        [SerializeField] private Light _screenLight;

        [SerializeField] private Light _frontLight;
        [SerializeField] private Light _backLight;
        
        [SerializeField] private Ambience _ambience;

        [SerializeField] private AudioClip _scrollStepSound;

        [SerializeField] private AudioClip[] _clickAudioClips;

        [SerializeField] private TrackInfo _firstMusicTrack;
        [SerializeField] private TrackInfo[] _musicTracks;

        [SerializeField] private Sprite _playSprite;
        [SerializeField] private Sprite _pauseSprite;

        [SerializeField] private Vector3 _flashlightRotation;
        [SerializeField] private Vector3 _flashlightMove;

        [SerializeField] private float _screenLightIntensity;

        private bool _isPlaying;

        private bool _isGlitching;

        private int _tracksPlayed;

        private bool _isScaryEventTriggered;

        private int _scaryEventTriggerTrackCount;
        private float _scaryEventTrackStartTime;

        private float _currentPlaytime;
        private float _targetPlaytime;

        private readonly MutableWaitForSeconds _waitTime = new();

        private Coroutine _waitCoroutine;

        private Material _instancedScreenMaterial;

        private readonly Color EnabledEmissionColor = Color.white * EmissionIntensity;
        private readonly Color DisabledEmissionColor = Color.black * EmissionIntensity;

        public event Action RapidScreenGlitchStarted;
        public event Action RapidScreenGlitchEnded;

        private const int MinScrolls = 1;
        private const int MaxScrolls = 4;

        private const float ScrollStepSoundVolumeScale = 2f;

        private const int FirstTrackPlaytime = 10;
        private const float FirstTrackStartTime = 0f;

        private const int ScaryEventTriggerTrackMinCount = 1;
        private const int ScaryEventTriggerTrackMaxCount = 2;

        private const float ScaryEventTrackMinStartTime = 3;
        private const float ScaryEventTrackMaxStartTime = RandomTrackMaxPlaytime * 0.8f;

        private const float ScaryEventAudioPitch = 0f;
        private const float ScaryEventAudioPitchDuration = 1f;

        private const float RandomTrackMinPlaytime = 7f;
        private const float RandomTrackMaxPlaytime = 12f;

        private const int ScreenMaterialIndex = 1;
        
        private const string ColorPropertyName = "_BaseColor";
        private const string EmissionPropertyName = "_EmissionColor";

        private const float EmissionIntensity = 2.416924f;

        public event Action TrackPaused;
        public event Action<float, float> TrackResumed;

        public event Action<TrackInfo> TrackChanged;

        private void OnValidate()
        {
            if (_transform == null)
            {
                _transform = transform;
            }

            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
            }
            
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        private void Start()
        {
            _instancedScreenMaterial = _meshRenderer.materials[ScreenMaterialIndex];

            _scaryEventTriggerTrackCount = UnityRandom.Range(ScaryEventTriggerTrackMinCount, ScaryEventTriggerTrackMaxCount);
            _scaryEventTrackStartTime = UnityRandom.Range(ScaryEventTrackMinStartTime, ScaryEventTrackMaxStartTime);


        }

        private void Update()
        {
            if (_isPlaying)
            {
                _currentPlaytime += Time.deltaTime;
            }
            
            if (!_isPlaying || _isScaryEventTriggered || _tracksPlayed != _scaryEventTriggerTrackCount)
            {
                return;
            }

            const float MinDelta = 0.1f;
            if (_scaryEventTrackStartTime - _currentPlaytime <= MinDelta)
            {
                _isScaryEventTriggered = true;

                const Ease ScaryEventEase = Ease.OutCirc;

                TweenSettings<Color> screenTween = new(Color.white, Color.black, ScaryEventAudioPitchDuration, ScaryEventEase);
                TweenSettings<Color> screenEmissionTween = new(EnabledEmissionColor, DisabledEmissionColor, ScaryEventAudioPitchDuration, ScaryEventEase);
                TweenSettings<float> screenUIAlphaTween = new(1f, 0f, ScaryEventAudioPitchDuration, ScaryEventEase);

                Sequence.Create()
                    .ChainCallback(() => RapidScreenGlitchStarted?.Invoke())
                    .Chain(Tween.AudioPitch(_audioSource, ScaryEventAudioPitch, ScaryEventAudioPitchDuration, ScaryEventEase))
                    .Group(Tween.Custom(screenTween, (color) => _instancedScreenMaterial.SetColor(ColorPropertyName, color)))
                    .Group(Tween.Custom(screenEmissionTween, (color) => _instancedScreenMaterial.SetColor(EmissionPropertyName, color)))
                    .Group(Tween.Alpha(_screenCanvasGroup, screenUIAlphaTween))
                    .ChainCallback(() => _audioSource.pitch = 1f)
                    .ChainCallback(() => StartCoroutine(StartRapidScreenFlicker()));
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

            if (_currentPlaytime > 0f)
            {
                UnPauseCurrentTrack();
            }
            else
            {
                SetFirstTrackAndPlay();
            }

            _ambience.MuffleVolume();

            _playStateImage.sprite = _playSprite;

            TrackResumed?.Invoke(_currentPlaytime, _targetPlaytime);
        }

        public void Pause()
        {
            if (!_isPlaying || _isGlitching)
            {
                return;
            }

            _isPlaying = false;

            PlayRandomClickSound();

            PauseCurrentTrack();

            _ambience.NormalizeVolume();
            
            _playStateImage.sprite = _pauseSprite;

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

        private void PlayRandomClickSound()
        {
            int clickClipIndex = UnityRandom.Range(0, _clickAudioClips.Length);

            _clickAudioSource.clip = _clickAudioClips[clickClipIndex];
            _clickAudioSource.Play();
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

            TrackChanged?.Invoke(track);
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

        private IEnumerator StartRapidScreenFlicker()
        {
            _isGlitching = true;

            const int Cycles = 30;
            WaitForSeconds delay = new(0.03f);

            _audioSource.pitch = 0.5f;

            for (int i = 0; i < Cycles; i++)
            {
                _audioSource.PlayOneShot(_scrollStepSound, ScrollStepSoundVolumeScale);

                bool isDisabled = i % 2 == 0;

                Color currentColor = isDisabled ? Color.black : Color.white;
                Color currentEmissionColor = isDisabled ? DisabledEmissionColor : EnabledEmissionColor;
                _instancedScreenMaterial.SetColor(ColorPropertyName, currentColor);
                _instancedScreenMaterial.SetColor(EmissionPropertyName, currentEmissionColor);

                _screenCanvas.enabled = !isDisabled;

                yield return delay;
            }

            _audioSource.pitch = 1f;
            _screenCanvas.enabled = true;
            _screenCanvasGroup.alpha = 1f;

            _isGlitching = false;

            RapidScreenGlitchEnded?.Invoke();
        }
    }
}