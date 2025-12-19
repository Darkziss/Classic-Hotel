using System;
using System.Collections;
using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    [RequireComponent(typeof(InstancedMaterial))]
    public class MusicPlayerScreen : MonoBehaviour
    {
        [SerializeField] private InstancedMaterial _screenMaterial;
        [SerializeField] private SingleOneShotAudioSource _scrollStepAudioSource;
        
        [SerializeField] private MusicPlayerScreenUI _screenUI;

        [SerializeField, ColorUsage(true, true)] private Color _enabledColor = Color.white;
        [SerializeField] private Color _disabledColor = Color.black;

        private bool _isFlickering;

        public bool IsFlickering => _isFlickering;

        private const int FlickerCycles = 30;
        private const float FlickerDelay = 0.03f;

        private const float FadeOutDuration = 1f;
        private const Ease FadeOutEase = Ease.OutCirc;

        public event Action RapidScreenGlitchStarted;
        public event Action RapidScreenGlitchEnded;

        private void OnValidate()
        {
            if (_screenMaterial == null)
            {
                _screenMaterial = GetComponent<InstancedMaterial>();
            }
        }

        public void TriggerRapidScreenFlicker()
        {
            _isFlickering = true;

            RapidScreenGlitchStarted?.Invoke();

            TweenSettings<Color> settings = new(_enabledColor, _disabledColor, FadeOutDuration, FadeOutEase);

            Tween.Custom(settings, (color) =>
            {
                _screenMaterial.SetBaseColor(color);
                _screenMaterial.SetEmissionColor(color);

                _screenUI.SetCanvasGroupAlpha(color.a);
            }).OnComplete(() => StartCoroutine(RapidScreenFlickerRoutine()));
        }

        private IEnumerator RapidScreenFlickerRoutine()
        {
            WaitForSeconds delay = new(FlickerDelay);

            _scrollStepAudioSource.SetPitch(0.5f);

            for (int i = 0; i < FlickerCycles; i++)
            {
                _scrollStepAudioSource.PlayOneShot();

                bool isDisabled = i % 2 == 0;

                Color color = isDisabled ? _disabledColor : _enabledColor;

                _screenMaterial.SetBaseColor(color);
                _screenMaterial.SetEmissionColor(color);

                _screenUI.SetCanvasEnabledState(!isDisabled);

                yield return delay;
            }

            _scrollStepAudioSource.SetPitch(1f);

            _screenUI.SetCanvasEnabledState(true);
            _screenUI.SetCanvasGroupAlpha(1f);

            _isFlickering = false;

            RapidScreenGlitchEnded?.Invoke();
        }
    }
}