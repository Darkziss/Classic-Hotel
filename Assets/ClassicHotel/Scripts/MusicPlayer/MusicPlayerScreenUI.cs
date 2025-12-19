using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PrimeTween;

namespace ClassicHotel
{
    public class MusicPlayerScreenUI : MonoBehaviour
    {
        [SerializeField] private MusicPlayer _musicPlayer;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private TextMeshProUGUI _trackNameText;
        [SerializeField] private TextMeshProUGUI _trackAuthorText;

        [SerializeField] private Image _trackImage;

        [SerializeField] private TextMeshProUGUI _positionText;

        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private ProgressBar _progressBar;

        private Coroutine _progressCoroutine;

        private readonly WaitForSeconds _trackProgressDelay = new(1f);

        private const Ease ProgressEase = Ease.Linear;

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

        public void SetCanvasEnabledState(bool isEnabled)
        {
            _canvas.enabled = isEnabled;
        }

        public void SetCanvasGroupAlpha(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        private void UpdateTrackInfo(TrackInfo track, int position, int maxPosition)
        {
            _trackNameText.text = track.Name;
            _trackAuthorText.text = track.AuthorName;

            _trackImage.sprite = track.Image;

            _positionText.text = $"{position} of {maxPosition}";
        }

        private void PlayProgressAnimation(float currentPlaytime, float targetPlaytime)
        {
            _progressCoroutine = StartCoroutine(TrackProgressTextRoutine(currentPlaytime, targetPlaytime));

            float duration = targetPlaytime - currentPlaytime;
            
            TweenSettings<float> progressBarTweenSettings = new(currentPlaytime, targetPlaytime, duration, ProgressEase);

            Tween.Custom(_progressBar, progressBarTweenSettings, (_, progress) => _progressBar.Set(progress, targetPlaytime));
        }

        private void StopProgressAnimation()
        {
            if (_progressCoroutine == null)
            {
                return;
            }
            
            StopCoroutine(_progressCoroutine);
            _progressCoroutine = null;

            Tween.StopAll(_progressBar);
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