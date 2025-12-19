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

        [SerializeField] private Color _enabledColor = Color.white;
        [SerializeField, ColorUsage(true, true)] private Color _emissionColor = Color.white;
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

            TweenSettings<Color> colorSettings = new(_enabledColor, _disabledColor, FadeOutDuration, FadeOutEase);
            TweenSettings<float> alphaSettings = new(1f, 0f, FadeOutDuration, FadeOutEase);

            Sequence.Create()
                .Group(Tween.Custom(colorSettings, _screenMaterial.SetBaseColor))
                .Group(Tween.Custom(alphaSettings, _screenUI.SetCanvasGroupAlpha))
                .OnComplete(() => StartCoroutine(RapidScreenFlickerRoutine()));
        }

        private IEnumerator RapidScreenFlickerRoutine()
        {
            WaitForSeconds delay = new(FlickerDelay);

            _scrollStepAudioSource.SetPitch(0.5f);

            for (int i = 0; i < FlickerCycles; i++)
            {
                _scrollStepAudioSource.PlayOneShot();

                bool isEnabled = i % 2 != 0;

                Color color = isEnabled ? _disabledColor : _emissionColor;

                _screenMaterial.SetBaseColor(color);

                yield return delay;
            }

            _screenMaterial.SetBaseColor(_enabledColor);
            _screenMaterial.SetEmissionColor(_disabledColor);

            _screenUI.SetCanvasEnabledState(true);
            _screenUI.SetCanvasGroupAlpha(1f);

            _scrollStepAudioSource.SetPitch(1f);

            _isFlickering = false;

            RapidScreenGlitchEnded?.Invoke();
        }
    }
}