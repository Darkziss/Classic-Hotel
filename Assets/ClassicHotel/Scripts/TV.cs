using UnityEngine;

namespace ClassicHotel
{
    public class TV : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _tvEnableTrigger;

        private void OnEnable()
        {
            _tvEnableTrigger.TriggerEntered += TriggerEvent;
        }

        private void OnDisable()
        {
            _tvEnableTrigger.TriggerEntered -= TriggerEvent;
        }

        private void TriggerEvent()
        {
            Debug.Log("TV Enabled");
        }
    }
}