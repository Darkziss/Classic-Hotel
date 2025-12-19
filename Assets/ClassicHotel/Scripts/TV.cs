using System.Collections;
using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(InstancedMaterial), typeof(AudioSource))]
    public class TV : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _tvEnableTrigger;

        [SerializeField] private InstancedMaterial _screenMaterial;
        [SerializeField] private AudioSource _audioSource;

        [SerializeField, ColorUsage(true, true)] private Color _screenEnabledColor = Color.white;
        [SerializeField] private Color _screenDisabledColor = Color.black;

        [SerializeField] private AudioClip _switchSound;

        private const float ProgramStartDelay = 0.5f;

        private void OnValidate()
        {
            if (_screenMaterial == null)
            {
                _screenMaterial = GetComponent<InstancedMaterial>();
            }
            
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
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

            _screenMaterial.SetEmissionColor(_screenEnabledColor);

            _audioSource.PlayDelayed(ProgramStartDelay);

            yield return disableDelay;

            PlaySwitchSound();

            _screenMaterial.SetEmissionColor(_screenDisabledColor);

            void PlaySwitchSound()
            {
                _audioSource.PlayOneShot(_switchSound);
            }
        }
    }
}