using System.Collections;
using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(MeshRenderer), typeof(AudioSource))]
    public class TV : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _tvEnableTrigger;

        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private AudioSource _audioSource;

        [SerializeField, ColorUsage(true, true)] private Color _screenEnabledColor = Color.white;
        [SerializeField] private Color _screenDisabledColor = Color.black;

        [SerializeField] private AudioClip _switchSound;

        private Material _instancedScreenMaterial;

        private const float ProgramStartDelay = 0.5f;

        private const int ScreenMaterialIndex = 1;

        private const string EmissionColorPropertyName = "_EmissionColor";

        private void OnValidate()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
            }
            
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        private void Start()
        {
            _instancedScreenMaterial = _meshRenderer.materials[ScreenMaterialIndex];
        }

        private void OnEnable()
        {
            _tvEnableTrigger.TriggerEntered += TriggerEvent;
        }

        private void OnDisable()
        {
            _tvEnableTrigger.TriggerEntered -= TriggerEvent;
        }

        private void TriggerEvent()
        {
            StartCoroutine(TVEvent());
        }

        private IEnumerator TVEvent()
        {
            const float AdditionalDelay = 0.1f + ProgramStartDelay;

            WaitForSeconds disableDelay = new(_audioSource.clip.length + AdditionalDelay);

            PlaySwitchSound();

            SetColorOfScreen(_screenEnabledColor);

            _audioSource.PlayDelayed(ProgramStartDelay);

            yield return disableDelay;

            PlaySwitchSound();

            SetColorOfScreen(_screenDisabledColor);

            void PlaySwitchSound()
            {
                _audioSource.PlayOneShot(_switchSound);
            }
        }

        private void SetColorOfScreen(Color color)
        {
            _instancedScreenMaterial.SetColor(EmissionColorPropertyName, color);
        }
    }
}