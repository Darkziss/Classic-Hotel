using System;
using UnityEngine;

namespace ClassicHotel
{
    public class PlayerCameraRotator : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;

        [SerializeField] private Transform _headTransform;

        [SerializeField] private float _sensitivity = 1f;

        private bool _canLook;

        private Vector2 _rotation;

        private Vector2 _lookInput;

        private void Update()
        {
            if (_canLook && _lookInput != Vector2.zero)
            {
                Look();
            }
        }

        public void UpdateLookInput(Vector2 delta)
        {
            if (!_canLook)
            {
                throw new InvalidOperationException(nameof(_canLook));
            }
            
            _lookInput = delta;
        }

        public void EnableLook()
        {
            if (_canLook)
            {
                throw new InvalidOperationException(nameof(_canLook));
            }

            _canLook = true;
        }

        public void DisableLook()
        {
            if (!_canLook)
            {
                throw new InvalidOperationException(nameof(_canLook));
            }

            _canLook = false;
        }

        private void Look()
        {
            Vector2 rotationDelta = new(_lookInput.y, _lookInput.x);
            rotationDelta *= _sensitivity * Time.deltaTime;
            _rotation += rotationDelta;

            _headTransform.localRotation = Quaternion.Euler(_rotation);
        }
    }
}