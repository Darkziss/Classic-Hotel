using System.Collections;
using System.Linq;
using UnityEngine;

namespace ClassicHotel
{
    public class Ending : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _endingTrigger;

        [SerializeField] private CorridorLight[] _lights;

        [SerializeField] private PlayerStateMachine _playerStateMachine;
        [SerializeField] private MusicPlayer _musicPlayer;
        [SerializeField] private AudioSource _lightSwitchAudioSource;

        private readonly WaitForSeconds _flickerDelay = new(FlickerDelay);

        private readonly WaitForSeconds _playerStopDelay = new(PlayerStopDelay);

        private readonly WaitForSeconds _musicPlayerRotateDelay = new(MusicPlayerRotateDelay);

        private const float FlickerDelay = 5f;

        private const float PlayerStopDelay = 0.5f;

        private const float MusicPlayerRotateDelay = 1.2f;

        private void Start()
        {
            _endingTrigger.TriggerEntered += StartEnding;
        }

        [ContextMenu(nameof(StartEnding))]
        private void StartEnding()
        {
            StartCoroutine(EndingSequence());
        }

        private IEnumerator EndingSequence()
        {
            FlickerCorridorLights();

            yield return _flickerDelay;

            CorridorBlackout();

            yield return _playerStopDelay;

            _playerStateMachine.SwitchToBlackoutMode();

            yield return _musicPlayerRotateDelay;

            _musicPlayer.SwitchToFlashlightMode();
        }

        private void FlickerCorridorLights()
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].TriggerFlicker();
            }
        }

        private void CorridorBlackout()
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].DisableLamp();
            }

            RenderSettings.ambientSkyColor = Color.black;
            RenderSettings.ambientEquatorColor = Color.black;
            RenderSettings.ambientGroundColor = Color.black;

            RenderSettings.fog = false;

            _lightSwitchAudioSource.Play();
        }

        [ContextMenu(nameof(FindAllCorridorLights))]
        private void FindAllCorridorLights()
        {
            _lights = FindObjectsByType<CorridorLight>(FindObjectsSortMode.None)
                .OrderBy(light => light.transform.position.z)
                .ToArray();
        }
    }
}