using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class MirrorJumpscare : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _trigger;

        [SerializeField] private Transform _mirrorSphereTransform;
        [SerializeField] private Transform _jumpscarePointTransform;

        [SerializeField] private TweenSettings _jumpscareTweenSettings;

        private void OnEnable()
        {
            _trigger.TriggerEntered += TriggerJumpscare;
        }

        private void OnDisable()
        {
            _trigger.TriggerEntered -= TriggerJumpscare;
        }

        private void TriggerJumpscare()
        {
            _mirrorSphereTransform.gameObject.SetActive(true);

            TweenSettings<Vector3> tweenSettings = new(_jumpscarePointTransform.position, _jumpscareTweenSettings);

            Tween.Position(_mirrorSphereTransform, tweenSettings)
                .OnComplete(() => _mirrorSphereTransform.gameObject.SetActive(false));
        }
    }
}