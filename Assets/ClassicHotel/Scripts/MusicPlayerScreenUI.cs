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
    }
}