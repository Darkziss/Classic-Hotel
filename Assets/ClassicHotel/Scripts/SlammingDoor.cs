using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    [RequireComponent(typeof(AudioSource))]
    public class SlammingDoor : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private AudioSource _audioSource;
        
        [SerializeField] private BoxTrigger _trigger;

        [SerializeField] private Vector3 _closedRotation;

        private const float SlamDuration = 0.3f;
        private const Ease SlamEase = Ease.InBack;

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

        private void OnEnable()
        {
            _trigger.TriggerEntered += Slam;
        }

        private void OnDisable()
        {
            _trigger.TriggerEntered -= Slam;
        }

        private void Slam()
        {
            TweenSettings<Vector3> settings = new(_closedRotation, SlamDuration, SlamEase);

            Tween.LocalRotation(_transform, settings);

            _audioSource.Play();
        }
    }
}