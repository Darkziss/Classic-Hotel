using UnityEngine;

namespace ClassicHotel
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Transform _endPointTransform;

        [SerializeField] private float _speed = 1f;

        private Transform _transform;

        private const Space MoveSpace = Space.World;

        private void OnValidate()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (_transform.position.z >= _endPointTransform.position.z)
                return;
            
            Vector3 translation = _speed * Time.deltaTime * Vector3.forward;

            _transform.Translate(translation, MoveSpace);
        }
    }
}