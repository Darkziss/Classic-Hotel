using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(AudioSource))]
    public class SingleOneShotAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _audioClip;

        [SerializeField] private float _volumeScale = DefaultVolumeScale;

        private const float DefaultVolumeScale = 1f;

        private void OnValidate()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        public void PlayOneShot()
        {
            _audioSource.PlayOneShot(_audioClip, _volumeScale);
        }

        public void SetPitch(float pitch)
        {
            _audioSource.pitch = pitch;
        }
    }
}