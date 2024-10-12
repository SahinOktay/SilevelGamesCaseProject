using System;
using UnityEngine;

namespace SilevelGames
{
    public class InputManager : MonoBehaviour
    {
        private float swerveMaxPixel;
        private Vector3 lastFrameTouch;

        public event Action<float> Swerve;
        public event Action Tap;

        [Range(0, 1)]
        public float swerveScreenRatio = 0.5f;

        private void Start() => swerveMaxPixel = (Screen.width * swerveScreenRatio);

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Tap?.Invoke();
                lastFrameTouch = Input.mousePosition;
                return;
            }

            if (Input.GetMouseButton(0))
            {
                Swerve?.Invoke((Input.mousePosition.x - lastFrameTouch.x) / swerveMaxPixel);
                lastFrameTouch = Input.mousePosition;
                return;
            }
            else Swerve?.Invoke(0);
        }
    }
}