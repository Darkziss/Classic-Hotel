using UnityEngine;

namespace ClassicHotel
{
    [CreateAssetMenu(fileName = DefaultFileName)]
    public class TrackInfo : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private string _author;

        [SerializeField] private Sprite _image;

        [SerializeField] private AudioClip _clip;

        public string Name => _name;

        public string AuthorName => _author;

        public Sprite Image => _image;

        public AudioClip Clip => _clip;

        private const string DefaultFileName = "Track";
    }
}