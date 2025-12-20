using UnityEngine;

namespace ClassicHotel
{
    public class MirrorJumpscare : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _trigger;

        private void OnEnable()
        {
            _trigger.TriggerEntered += TriggerJumpscare;
        }

        private void OnDisable()
        {
            _trigger.TriggerEntered -= TriggerJumpscare;
        }

        private void TriggerJumpscare()
        {
            Debug.Log(nameof(TriggerJumpscare));
        }
    }
}