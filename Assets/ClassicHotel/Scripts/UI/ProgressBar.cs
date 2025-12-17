using UnityEngine;
using UnityEngine.UI;

namespace ClassicHotel
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;

        public void Set(float current, float target)
        {
            _fillImage.fillAmount = current / target;
        }
    }
}