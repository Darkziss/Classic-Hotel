using System.Collections;
using System.Linq;
using UnityEngine;

namespace ClassicHotel
{
    public class Ending : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _endingTrigger;

        [SerializeField] private CorridorLight[] _lights;

        private readonly WaitForSeconds _flickerDelay = new(FlickerDelay);

        private const float FlickerDelay = 5f;

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