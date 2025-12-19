using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class MusicPlayerVisuals : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        [SerializeField] private Light _screenLight;
        
        [SerializeField] private Vector3 _flashlightRotation;
        [SerializeField] private Vector3 _flashlightMove;

        [SerializeField] private float _screenLightIntensity;

        private const float Duration = 0.6f;

        const Ease RotateEase = Ease.OutExpo;
        const Ease PositionEase = Ease.OutBack;

        const Ease IntensityEase = Ease.Default;

        private void OnValidate()
        {
            if (_transform == null)
            {
                _transform = transform;
            }
        }

        public void SwitchToFlashlightMode()
        {
            TweenSettings<Vector3> rotationSettings = new(_flashlightRotation, Duration, RotateEase);
            TweenSettings<Vector3> positionSettings = new(_transform.localPosition + _flashlightMove, Duration, PositionEase);

            TweenSettings<float> intensitySettings = new(_screenLightIntensity, Duration, IntensityEase);

            Sequence.Create()
                .Chain(Tween.LocalRotation(_transform, rotationSettings))
                .Group(Tween.LocalPosition(_transform, positionSettings))
                .Group(Tween.LightIntensity(_screenLight, intensitySettings));
        }
    }
}