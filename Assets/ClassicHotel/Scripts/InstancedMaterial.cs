using UnityEngine;

namespace ClassicHotel
{
    [RequireComponent(typeof(MeshRenderer))]
    public class InstancedMaterial : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private int _materialIndex;

        private Material _material;

        private const string BaseColorPropertyName = "_BaseColor";

        private const string EmissionColorPropertyName = "_EmissionColor";

        private void OnValidate()
        {
            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
            }
        }

        private void Start()
        {
            _material = _meshRenderer.materials[_materialIndex];
        }

        public void SetBaseColor(Color color)
        {
            _material.SetColor(BaseColorPropertyName, color);
        }

        public void SetEmissionColor(Color color)
        {
            _material.SetColor(EmissionColorPropertyName, color);
        }
    }
}