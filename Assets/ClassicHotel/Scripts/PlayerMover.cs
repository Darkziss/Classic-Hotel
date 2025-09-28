using UnityEngine;
using UnityEngine.InputSystem;

namespace ClassicHotel
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private Transform _endPointTransform;

        [SerializeField] private float _speed = 1f;

        private bool _shouldMove;

        private InputAction _moveAction;

        public bool ShouldMove => _shouldMove;

        private const Space MoveSpace = Space.World;

        private const string MoveActionName = "Move";

        private void OnValidate()
        {
            _transform = transform;
        }

        private void Start()
        {
            _moveAction = InputSystem.actions.FindAction(MoveActionName);
        }

        private void Update()
        {
            if (_moveAction.WasPressedThisFrame())
            {
                _shouldMove = !_shouldMove;
            }

            if (_shouldMove && _transform.position.z < _endPointTransform.position.z)
            {
                Move();
            }
        }

        private void Move()
        {
            Vector3 translation = _speed * Time.deltaTime * Vector3.forward;

            _transform.Translate(translation, MoveSpace);
        }
    }
}