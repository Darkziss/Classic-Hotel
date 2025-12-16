using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClassicHotel
{
    public class MusicPlayerScreenUI : MonoBehaviour
    {
        [SerializeField] private MusicPlayer _musicPlayer;

        [SerializeField] private TextMeshProUGUI _trackNameText;
        [SerializeField] private TextMeshProUGUI _trackAuthorText;

        [SerializeField] private Image _trackImage;

        [SerializeField] private TextMeshProUGUI _progressText;

        private Coroutine _progressCoroutine;

        private readonly WaitForSeconds _trackProgressDelay = new(1f);

        private void OnEnable()
        {
            _musicPlayer.TrackResumed += PlayProgressAnimation;
            _musicPlayer.TrackPaused += StopProgressAnimation;

            _musicPlayer.TrackChanged += UpdateTrackInfo;
        }

        private void OnDisable()
        {
            _musicPlayer.TrackResumed -= PlayProgressAnimation;
            _musicPlayer.TrackPaused -= StopProgressAnimation;

            _musicPlayer.TrackChanged -= UpdateTrackInfo;
        }

        private void UpdateTrackInfo(TrackInfo track, float duration)
        {
            _trackNameText.text = track.Name;
            _trackAuthorText.text = track.AuthorName;

            _trackImage.sprite = track.Image;
        }

        private void PlayProgressAnimation(float currentPlaytime, float targetPlaytime)
        {
            _progressCoroutine = StartCoroutine(TrackProgressTextRoutine(currentPlaytime, targetPlaytime));
        }

        private void StopProgressAnimation()
        {
            if (_progressCoroutine == null)
            {
                return;
            }
            
            StopCoroutine(_progressCoroutine);
            _progressCoroutine = null;
        }

        private void SetProgressText(int second)
        {
            _progressText.text = $"0:{second:00}";
        }

        private IEnumerator TrackProgressTextRoutine(float currentPlaytime, float targetPlaytime)
        {
            for (int i = (int)currentPlaytime; i < targetPlaytime; i++)
            {
                yield return _trackProgressDelay;

                SetProgressText(i);
            }
        }
    }
}