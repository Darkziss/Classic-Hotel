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

        private Color _originalEmissionColor;

        public Material LightMaterial => _renderer.materials[LightMaterialIndex];

        private const int MinFlickerCount = 3;
        private const int MaxFlickerCount = 7;

        private const float MinFlickerStartDelay = 0.05f;
        private const float MaxFlickerStartDelay = 0.3f;

        private const float MinFlickerDelay = 0.05f;
        private const float MaxFlickerDelay = 0.3f;

        private const int LightMaterialIndex = 0;
        private const string EmissionPropertyName = "_EmissionColor";

        public void TriggerFlicker()
        {
            StartCoroutine(FlickerAnimation());
        }

        private IEnumerator FlickerAnimation()
        {
            float originalIntensity = _light.intensity;
            _originalEmissionColor = LightMaterial.GetColor(EmissionPropertyName);

            int flickerCount = UnityRandom.Range(MinFlickerCount, MaxFlickerCount);
            float startDelay = UnityRandom.Range(MinFlickerStartDelay, MaxFlickerStartDelay);

            MutableWaitForSeconds delay = new();

            delay.SetSeconds(startDelay);

            for (int i = 0; i < flickerCount; i++)
            {
                bool isDisabled = i % 2 == 0;
                float intensity = isDisabled ? 0f : originalIntensity;

                SetIntensityAndEmission(intensity, isDisabled);

                float flickerDelay = UnityRandom.Range(MinFlickerDelay, MaxFlickerDelay);
                delay.SetSeconds(flickerDelay);

                yield return delay;
            }

            SetIntensityAndEmission(originalIntensity, true);
        }

        private void SetIntensityAndEmission(float intensity, bool emission)
        {
            _light.intensity = intensity;

            Color emissionColor = emission ? _originalEmissionColor : Color.black;
            LightMaterial.SetColor(EmissionPropertyName, emissionColor);
        }
    }
}