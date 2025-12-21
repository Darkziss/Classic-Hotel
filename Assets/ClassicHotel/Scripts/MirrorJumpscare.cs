using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class MirrorJumpscare : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _trigger;

        [SerializeField] private Transform _mirrorSphereTransform;
        [SerializeField] private Transform _jumpscarePointTransform;

        [SerializeField] private Transform _sphereStartTransform;

        [SerializeField] float _sphereTargetUniformScale;

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
            const float ScaleDuration = 1f;
            const Ease ScaleEase = Ease.OutQuad;

            const float StartPositionDuration = 0.5f;

            const float JumpscareDelay = 1f;

            const float JumpscareDuration = 0.25f;
            const Ease JumpscareEase = Ease.InBack;

            TweenSettings<float> scaleSettings = new(0f, _sphereTargetUniformScale, ScaleDuration, ScaleEase);
            TweenSettings<Vector3> startPositionSettings = new(_sphereStartTransform.position, StartPositionDuration);

            TweenSettings<Vector3> jumpscareSettings = new(_jumpscarePointTransform.position, JumpscareDuration, JumpscareEase);

            _mirrorSphereTransform.gameObject.SetActive(true);

            Sequence.Create()
                .Group(Tween.Scale(_mirrorSphereTransform, scaleSettings))
                .Group(Tween.Position(_mirrorSphereTransform, startPositionSettings))
                .ChainDelay(JumpscareDelay)
                .Chain(Tween.Position(_mirrorSphereTransform, jumpscareSettings))
                .OnComplete(() => _mirrorSphereTransform.gameObject.SetActive(false));
        }
    }
}