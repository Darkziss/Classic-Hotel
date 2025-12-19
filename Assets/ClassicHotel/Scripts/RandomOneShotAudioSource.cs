using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace ClassicHotel
{
    [RequireComponent(typeof(AudioSource))]
    public class RandomOneShotAudioSource : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip[] _clips;

        private void OnValidate()
        {
            if (_audioSource)
            {
                _audioSource = GetComponent<AudioSource>();
            }
        }

        public void PlayOneShot()
        {
            int index = UnityRandom.Range(0, _clips.Length);

            _audioSource.PlayOneShot(_clips[index]);
        }
    }
}