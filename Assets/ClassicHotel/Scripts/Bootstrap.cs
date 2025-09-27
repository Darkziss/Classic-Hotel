using UnityEngine;

namespace ClassicHotel
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private int _targetFps = DefaultTargetFps;

        [SerializeField] private bool _isCursorVisible = true;
        [SerializeField] private CursorLockMode _cursorLockMode;

        private const int DefaultTargetFps = 60;

        private void Awake()
        {
            Application.targetFrameRate = _targetFps;

            Cursor.visible = _isCursorVisible;
            Cursor.lockState = _cursorLockMode;
        }
    }
}