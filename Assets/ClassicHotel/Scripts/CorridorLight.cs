using System.Collections;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace ClassicHotel
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CorridorLight : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Light _light;
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip[] _flickerSounds;

        private Material _instancedMaterial;

        private float _originalIntensity;
        private Color _originalEmissionColor;

        private const int MinFlickerCount = 3;
        private const int MaxFlickerCount = 8;

        private const float MinFlickerStartDelay = 0.05f;
        private const float MaxFlickerStartDelay = 0.3f;

        private const float EnabledMinDelay = 0.02f;
        private const float EnabledMaxDelay = 0.08f;

        private const float DisabledMinDelay = 0.05f;
        private const float DisabledMaxDelay = 0.2f;

        private const float FlickerMinPitch = 0.8f;
        private const float FlickerMaxPitch = 1.2f;

        private const int LightMaterialIndex = 0;
        private const string EmissionPropertyName = "_EmissionColor";

        private void Start()
        {
            _instancedMaterial = _renderer.materials[LightMaterialIndex];

            _originalIntensity = _light.intensity;
            _originalEmissionColor = _instancedMaterial.GetColor(EmissionPropertyName);
        }

        public void TriggerFlicker()
        {
            StartCoroutine(FlickerAnimation());
        }

        private IEnumerator FlickerAnimation()
        {
            int flickerCount = UnityRandom.Range(MinFlickerCount, MaxFlickerCount);
            float startDelay = UnityRandom.Range(MinFlickerStartDelay, MaxFlickerStartDelay);

            MutableWaitForSeconds delay = new();

            delay.SetSeconds(startDelay);
            yield return delay;

            for (int i = 0; i < flickerCount; i++)
            {
                DisableLamp();

                PlayRandomFlickerSound();

                float flickerDelay = UnityRandom.Range(DisabledMinDelay, DisabledMaxDelay);

                delay.SetSeconds(flickerDelay);
                yield return delay;

                EnableLamp();

                PlayRandomFlickerSound();

                flickerDelay = UnityRandom.Range(EnabledMinDelay, EnabledMaxDelay);

                delay.SetSeconds(flickerDelay);
                yield return delay;
            }

            EnableLamp();
        }

        private void PlayRandomFlickerSound()
        {
            int soundIndex = UnityRandom.Range(0, _flickerSounds.Length);

            _audioSource.pitch = UnityRandom.Range(FlickerMinPitch, FlickerMaxPitch);
            _audioSource.PlayOneShot(_flickerSounds[soundIndex]);
        }

        private void EnableLamp() => SetIntensityAndEmission(_originalIntensity, true);

        private void DisableLamp() => SetIntensityAndEmission(0f, false);

        private void SetIntensityAndEmission(float intensity, bool emission)
        {
            _light.intensity = intensity;

            Color emissionColor = emission ? _originalEmissionColor : Color.black;
            _instancedMaterial.SetColor(EmissionPropertyName, emissionColor);
        }
    }
}