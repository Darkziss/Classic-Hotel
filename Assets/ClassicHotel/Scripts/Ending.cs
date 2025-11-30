using System;
using System.Linq;
using UnityEngine;

namespace ClassicHotel
{
    public class Ending : MonoBehaviour
    {
        [SerializeField] private BoxTrigger _endingTrigger;

        [SerializeField] private CorridorLight[] _lights;

        private void Start()
        {
            _endingTrigger.TriggerEntered += StartEnding;
        }

        [ContextMenu(nameof(StartEnding))]
        private void StartEnding()
        {
            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].TriggerFlicker();
            }
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