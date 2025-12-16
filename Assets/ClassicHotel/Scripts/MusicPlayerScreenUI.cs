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

        private void OnEnable()
        {
            _musicPlayer.TrackChanged += UpdateTrackInfo;
        }

        private void OnDisable()
        {
            _musicPlayer.TrackChanged -= UpdateTrackInfo;
        }

        private void UpdateTrackInfo(TrackInfo track)
        {
            _trackNameText.text = track.Name;
            _trackAuthorText.text = track.AuthorName;

            _trackImage.sprite = track.Image;
        }
    }
}