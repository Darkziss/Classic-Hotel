using System;
using System.Collections;
using UnityEngine;
using PrimeTween;
using UnityRandom = UnityEngine.Random;

namespace ClassicHotel
{
    [RequireComponent(typeof(MeshFilter), typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _clickAudioSource;
        
        [SerializeField] private AudioSource _ambienceAudioSource;

        [SerializeField] private AudioClip _scrollStepSound;

        [SerializeField] private AudioClip[] _clickAudioClips;

        [SerializeField] private AudioClip _firstMusicTrack;
        [SerializeField] private AudioClip[] _musicTracks;

        [SerializeField] private Material _monsterMaterial;

        private bool _isPlaying;

        private int _tracksPlayed;

        private bool _isScaryEventTriggered;

        private int _scaryEventTriggerTrackCount;
        private float _scaryEventTrackStartTime;

        private float _currentPlaytime;
        private float _targetPlaytime;

        private Material[] _originalMaterials;
        private Material[] _materialsWithMonster;

        private readonly MutableWaitForSeconds _waitTime = new();

        private Coroutine _waitCoroutine;

        private const int MinScrolls = 1;
        private const int MaxScrolls = 4;

        private const float ScrollStepSoundVolumeScale = 2f;

        private const int FirstTrackPlaytime = 10;
        private const float FirstTrackStartTime = 0f;

        private const int ScaryEventTriggerTrackMinCount = 2;
        private const int ScaryEventTriggerTrackMaxCount = 4;

        private const float ScaryEventTrackMinStartTime = 3;
        private const float ScaryEventTrackMaxStartTime = RandomTrackMaxPlaytime * 0.8f;

        private const float ScaryEventAudioPitch = 0.09f;
        private const float ScaryEventAudioPitchDuration = 1f;

        private const float RandomTrackMinPlaytime = 7f;
        private const float RandomTrackMaxPlaytime = 12f;

        private const float NormalAmbienceVolume = 1f;
        private const float MuffledAmbienceVolume = 0.3f;

        private const int ScreenMaterialIndex = 1;
        private const float MonsterMaterialXOffset = 1.29f;
        private const float MonsterArrivingDuration = 2f;

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

        private void Start()
        {
            _scaryEventTriggerTrackCount = UnityRandom.Range(ScaryEventTriggerTrackMinCount, ScaryEventTriggerTrackMaxCount);
            _scaryEventTrackStartTime = UnityRandom.Range(ScaryEventTrackMinStartTime, ScaryEventTrackMaxStartTime);

            int length = _meshRenderer.sharedMaterials.Length;

            _originalMaterials = new Material[length];
            _materialsWithMonster = new Material[length];

            Array.Copy(_meshRenderer.sharedMaterials, _originalMaterials, length);
            Array.Copy(_meshRenderer.sharedMaterials, _materialsWithMonster, length);

            _materialsWithMonster[ScreenMaterialIndex] = new(_monsterMaterial);
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

                _meshRenderer.materials = _materialsWithMonster;

                Sequence.Create()
                    .Group(Tween.MaterialMainTextureOffset(_materialsWithMonster[ScreenMaterialIndex], Vector2.right * MonsterMaterialXOffset, 
                        MonsterArrivingDuration))
                    .Group(Tween.AudioPitch(_audioSource, ScaryEventAudioPitch, ScaryEventAudioPitchDuration));
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

            PauseCurrentTrack();

            _ambienceAudioSource.volume = NormalAmbienceVolume;
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
            float startTime = UnityRandom.Range(0f, _musicTracks[index].length - playtime);

            StartCoroutine(PlayScrollStepSoundsAndChangeMusic(_musicTracks[index], playtime, startTime));
        }

        private void SetTrackAndPlay(AudioClip track, float duration, float startTime)
        {
            _audioSource.clip = track;
            _audioSource.time = startTime;
            _audioSource.Play();

            _currentPlaytime = 0f;
            _targetPlaytime = duration;

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

            _waitCoroutine = StartCoroutine(WaitForEndOfCurrentTrack(_targetPlaytime - _currentPlaytime));
        }

        private IEnumerator PlayScrollStepSoundsAndChangeMusic(AudioClip track, float playtime, float startTime)
        {
            int count = UnityRandom.Range(MinScrolls, MaxScrolls);
            const float AdditionalDelay = 0.1f;
            WaitForSeconds delay = new(_scrollStepSound.length + AdditionalDelay);

            _audioSource.Stop();

            for (int i = 0; i < count; i++)
            {
                _audioSource.PlayOneShot(_scrollStepSound, ScrollStepSoundVolumeScale);

                yield return delay;
            }

            SetTrackAndPlay(track, playtime, startTime);
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