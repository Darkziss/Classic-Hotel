using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class MirrorJumpscare : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _trigger;

        [SerializeField] private Transform _mirrorSphereTransform;
        [SerializeField] private AudioSource _mirrorSphereAudioSource;
        [SerializeField] private Transform _jumpscarePointTransform;

        [SerializeField] private Transform _sphereStartTransform;

        [SerializeField] private BlinkScreen _blinkScreen;

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

            float startPositionDuration = _mirrorSphereAudioSource.clip.length;
            const Ease StartPositionEase = Ease.OutCirc;

            const float JumpscareDelay = 1f;

            const float JumpscareDuration = 0.25f;
            const Ease JumpscareEase = Ease.InExpo;

            TweenSettings<float> scaleSettings = new(0f, _sphereTargetUniformScale, ScaleDuration, ScaleEase);
            TweenSettings<Vector3> startPositionSettings = new(_sphereStartTransform.position, startPositionDuration, StartPositionEase);

            TweenSettings<Vector3> jumpscareSettings = new(_jumpscarePointTransform.position, JumpscareDuration, JumpscareEase);

            _mirrorSphereTransform.gameObject.SetActive(true);
            _mirrorSphereAudioSource.Play();

            Sequence.Create()
                .Group(Tween.Scale(_mirrorSphereTransform, scaleSettings))
                .Group(Tween.Position(_mirrorSphereTransform, startPositionSettings))
                .ChainDelay(JumpscareDelay)
                .Chain(Tween.Position(_mirrorSphereTransform, jumpscareSettings))
                .OnComplete(() =>
                {
                    _blinkScreen.Blink();

                    _mirrorSphereTransform.gameObject.SetActive(false);
                });
        }
    }
}