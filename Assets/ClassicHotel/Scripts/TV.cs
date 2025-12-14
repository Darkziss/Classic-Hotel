using System.Collections;
using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(AudioSource))]
    public class TV : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _tvEnableTrigger;

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _switchSound;

        private const float ProgramStartDelay = 0.5f;

        private void OnValidate()
        {
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

            _audioSource.PlayDelayed(ProgramStartDelay);

            yield return disableDelay;

            PlaySwitchSound();

            void PlaySwitchSound()
            {
                _audioSource.PlayOneShot(_switchSound);
            }
        }
    }
}