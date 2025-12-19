using UnityEngine;

namespace ClassicHotel
{
    public class LightGroup : MonoBehaviour
    {
        [SerializeField] private Light[] _lights;

        public void Enable()
        {
            SetEnabledState(true);
        }

        public void Disable()
        {
            SetEnabledState(false);
        }

        private void SetEnabledState(bool isEnabled)
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].enabled = isEnabled;
            }
        }
    }
}