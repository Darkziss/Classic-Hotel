using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(AudioSource))]
    public class Ambience : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _normalAmbience;
        [SerializeField] private AudioClip _blackoutAmbience;

        private const float NormalVolume = 1f;
        private const float MuffledVolume = 0.3f;

        private void OnValidate()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        public void PlayNormalAmbience()
        {
            _audioSource.clip = _normalAmbience;
            _audioSource.Play();
        }

        public void PlayBlackoutAmbience()
        {
            if (_audioSource.isPlaying && _audioSource.clip != _blackoutAmbience)
            {
                Stop();
            }

            _audioSource.clip = _blackoutAmbience;
            _audioSource.Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void NormalizeVolume()
        {
            _audioSource.volume = NormalVolume;
        }

        public void MuffleVolume()
        {
            _audioSource.volume = MuffledVolume;
        }
    }
}