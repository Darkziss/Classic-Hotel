using System;
using System.Collections;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace ClassicHotel
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private PlayerCameraRotator _playerCameraRotator;

        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _endPointTransform;

        [SerializeField] private AudioSource _footstepAudioSource;

        [SerializeField] private AudioClip[] _footstepsAudioClips;

        [SerializeField] private float _speed = 1f;

        private bool _isMoving;

        private Coroutine _footstepSoundCoroutine;

        private readonly WaitForSeconds _footstepDelay = new(1f);

        public bool IsMoving => _isMoving;

        private bool IsAtEndPoint => _transform.position.z >= _endPointTransform.position.z;

        private const Space MoveSpace = Space.World;

        private void OnValidate()
        {
            if (_transform == null)
            {
                _transform = transform;
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
        }

        public void StopMoving()
        {
            if (!_isMoving)
            {
                throw new InvalidOperationException(nameof(_isMoving)); 
            }

            _isMoving = false;

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