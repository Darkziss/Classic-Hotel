using System.Collections;
using System.Linq;
using UnityEngine;
using PrimeTween;

namespace ClassicHotel
{
    public class Ending : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _endingTrigger;

        [SerializeField] private CorridorLight[] _lights;

        [SerializeField] private PlayerStateMachine _playerStateMachine;
        [SerializeField] private MusicPlayer _musicPlayer;
        [SerializeField] private AudioSource _lightSwitchAudioSource;
        [SerializeField] private Ambience _ambience;
        [SerializeField] private BoxTrigger _endPoint;
        [SerializeField] private Transform _endDoor;
        [SerializeField] private AudioSource _endDoorAudioSource;
        [SerializeField] private Canvas _canvas;

        private readonly WaitForSeconds _flickerDelay = new(FlickerDelay);

        private readonly WaitForSeconds _playerStopDelay = new(PlayerStopDelay);

        private readonly WaitForSeconds _musicPlayerRotateDelay = new(MusicPlayerRotateDelay);

        private readonly WaitForSeconds _ambienceChangeDelay = new(AmbienceChangeDelay);

        private readonly WaitForSeconds _playerWalkEnableDelay = new(PlayerWalkEnableDelay);

        private readonly WaitForSeconds _doorOpenDelay = new(DoorOpenDelay);

        private readonly WaitForSeconds _endingScreenShowDelay = new(EndingScreenShowDelay);

        private const float FlickerDelay = 5f;

        private const float PlayerStopDelay = 0.5f;

        private const float MusicPlayerRotateDelay = 1.2f;

        private const float AmbienceChangeDelay = 1f;

        private const float PlayerWalkEnableDelay = 2f;

        private const float DoorOpenDelay = 1.2f;

        private const float EndingScreenShowDelay = 0.5f;

        private void Start()
        {
            _endingTrigger.TriggerEntered += StartEnding;

            _endPoint.TriggerEntered += _playerStateMachine.ParalyzePlayer;
            _endPoint.TriggerEntered += StartDoorEnding;
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

            yield return _ambienceChangeDelay;

            _ambience.PlayBlackoutAmbience();

            yield return _playerWalkEnableDelay;

            _playerStateMachine.EnableWalkInBlackoutMode();
        }

        private void StartDoorEnding()
        {
            StartCoroutine(DoorEndingSequence());
        }

        private IEnumerator DoorEndingSequence()
        {
            yield return _doorOpenDelay;

            const float DoorRotationY = 125f;
            float duration = _endDoorAudioSource.clip.length;
            const Ease DoorEase = Ease.OutBack;

            Sequence.Create()
                .Chain(Tween.LocalRotation(_endDoor, Vector3.up * DoorRotationY, duration, DoorEase))
                .ChainDelay(EndingScreenShowDelay)
                .ChainCallback(() => _canvas.gameObject.SetActive(true));
            
            _endDoorAudioSource.Play();
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