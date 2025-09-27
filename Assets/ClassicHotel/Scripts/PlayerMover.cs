using UnityEngine;

namespace ClassicHotel
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Transform _endPointTransform;

        [SerializeField] private float _speed = 1f;

        private Transform _transform;

        private bool _shouldMove;

        private const Space MoveSpace = Space.World;

        private const KeyCode MoveKeyCode = KeyCode.Space;

        private void OnValidate()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (Input.GetKeyDown(MoveKeyCode))
                _shouldMove = !_shouldMove;
            
            if (_shouldMove && _transform.position.z < _endPointTransform.position.z)
                Move();
        }

        private void Move()
        {
            Vector3 translation = _speed * Time.deltaTime * Vector3.forward;

            _transform.Translate(translation, MoveSpace);
        }
    }
}