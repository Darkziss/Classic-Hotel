using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace ClassicHotel
{
    [RequireComponent(typeof(Image))]
    public class BlinkScreen : MonoBehaviour
    {
        [SerializeField] private Image _image;

        private void OnValidate()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            } 
        }

        public void Blink()
        {
            const float Duration = 0.1f;
            const Ease BlinkEase = Ease.Linear;

            const float Delay = 0.3f;

            TweenSettings<float> fadeInSettings = new(1f, Duration, BlinkEase);
            TweenSettings<float> fadeOutSettings = new(0f, Duration, BlinkEase);

            Sequence.Create()
                .Chain(Tween.Alpha(_image, fadeInSettings))
                .ChainDelay(Delay)
                .Chain(Tween.Alpha(_image, fadeOutSettings));
        }
    }
}