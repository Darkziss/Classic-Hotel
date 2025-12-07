using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class SlammingDoor : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        
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
        }
    }
}