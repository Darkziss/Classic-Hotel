using System;
using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoxTrigger : MonoBehaviour
    {
        private BoxCollider _collider;
        
        public event Action TriggerEntered;

        private void OnValidate()
        {
            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider>();
            }

            if (!_collider.isTrigger)
            {
                _collider.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            TriggerEntered?.Invoke();
        }
    }
}