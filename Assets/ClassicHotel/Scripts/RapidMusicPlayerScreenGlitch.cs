using UnityEngine;

namespace ClassicHotel
{
    public class RapidMusicPlayerScreenGlitch : MonoBehaviour
    {
        [SerializeField] private MusicPlayer _musicPlayer;

        [SerializeField] private PlayerStateMachine _playerStateMachine;

        private void Start()
        {
            _musicPlayer.RapidScreenGlitchEnded += StopPlayer;
        }

        private void StopPlayer()
        {
            _playerStateMachine.ForceStop();
        }
    }
}