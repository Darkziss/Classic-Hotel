using UnityEngine;

namespace ClassicHotel
{
    public class RapidMusicPlayerScreenGlitch : MonoBehaviour
    {
        [SerializeField] private MusicPlayerScreen _musicPlayerScreen;

        [SerializeField] private PlayerStateMachine _playerStateMachine;

        private void Start()
        {
            _musicPlayerScreen.RapidScreenGlitchEnded += StopPlayer;
        }

        private void StopPlayer()
        {
            _playerStateMachine.ForceStop();
        }
    }
}