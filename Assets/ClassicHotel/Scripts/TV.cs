using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(AudioSource))]
    public class TV : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _tvEnableTrigger;

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _switchSound;

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
            _audioSource.PlayOneShot(_switchSound);
        }
    }
}