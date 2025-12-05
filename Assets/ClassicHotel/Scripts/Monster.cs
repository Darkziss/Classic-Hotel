using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class Monster : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private Transform _player;

        private readonly TweenSettings _chaseTweenSettings = new(ChaseDuration, ChaseEase);

        private const float ChaseDuration = 0.8f;
        private const Ease ChaseEase = Ease.InCirc;

        public void TriggerChase()
        {
            gameObject.SetActive(true);
            _audioSource.Play();

            Tween.Position(transform, _player.position, _chaseTweenSettings)
                .OnComplete(_audioSource.Stop);
        }
    }
}