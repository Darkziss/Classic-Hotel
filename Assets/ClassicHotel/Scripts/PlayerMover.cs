using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityRandom = UnityEngine.Random;
using PrimeTween;

namespace ClassicHotel
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _playerVirtualCamera;
        [SerializeField] private CinemachineBasicMultiChannelPerlin _noiseChannel;

        [SerializeField] private NoiseSettings _breathingNoiseProfile;
        [SerializeField] private NoiseSettings _headBobNoiseProfile;

        [SerializeField] private PlayerCameraRotator _playerCameraRotator;

        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _endPointTransform;

        [SerializeField] private AudioSource _footstepAudioSource;

        [SerializeField] private AudioClip[] _footstepsAudioClips;

        [SerializeField] private float _speed = 1f;

        private bool _isMoving;

        private Coroutine _footstepSoundCoroutine;

        private readonly WaitForSeconds _footstepDelay = new(1f);

        private readonly TweenSettings _fadeInSettings = new(0.3f, Ease.Linear);
        private readonly TweenSettings _fadeOutSettings = new(0.5f, Ease.InOutSine);

        public bool IsMoving => _isMoving;

        private bool IsAtEndPoint => _transform.position.z >= _endPointTransform.position.z;

        private const Space MoveSpace = Space.World;

        private void OnValidate()
        {
            if (_transform == null)
            {
                _transform = transform;
            }

            if (_playerVirtualCamera != null && _noiseChannel == null)
            {
                _noiseChannel = _playerVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }
        }

        private void Update()
        {
            if (_isMoving && !IsAtEndPoint)
            {
                Vector3 translation = _speed * Time.deltaTime * Vector3.forward;

                _transform.Translate(translation, MoveSpace);
            }
        }

        public void StartMoving()
        {
            if (_isMoving)
            {
                throw new InvalidOperationException(nameof(_isMoving));
            }

            _isMoving = true;

            _footstepSoundCoroutine = StartCoroutine(PlayRandomlyFootstepSound());

            Sequence.Create(Tween.Custom(1f, 0f, _fadeOutSettings, (value) => _noiseChannel.m_AmplitudeGain = value))
                .ChainCallback(() => _noiseChannel.m_NoiseProfile = _headBobNoiseProfile)
                .Chain(Tween.Custom(0f, 1f, _fadeInSettings, (value) => _noiseChannel.m_AmplitudeGain = value));
        }

        public void StopMoving()
        {
            if (!_isMoving)
            {
                throw new InvalidOperationException(nameof(_isMoving)); 
            }

            _isMoving = false;

            Sequence.Create(Tween.Custom(1f, 0f, _fadeOutSettings, (value) => _noiseChannel.m_AmplitudeGain = value))
                .ChainCallback(() => _noiseChannel.m_NoiseProfile = _breathingNoiseProfile)
                .Chain(Tween.Custom(0f, 1f, _fadeInSettings, (value) => _noiseChannel.m_AmplitudeGain = value));

            StopCoroutine(_footstepSoundCoroutine);
            _footstepSoundCoroutine = null;
        }

        private IEnumerator PlayRandomlyFootstepSound()
        {
            while (true)
            {
                int index = UnityRandom.Range(0, _footstepsAudioClips.Length);

                _footstepAudioSource.PlayOneShot(_footstepsAudioClips[index]);

                yield return _footstepDelay;
            }
        }
    }
}