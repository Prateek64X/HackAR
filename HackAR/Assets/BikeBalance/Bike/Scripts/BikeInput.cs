using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace VK.BikeLab
{
    public class BikeInput : MonoBehaviour
    {
        [Tooltip("User input sensitivity.")]
        [Range(0.001f, 0.1f)]
        public float sensitivityY = 0.01f;
        public float sensitivityX = 0.01f;
        [Tooltip("Determines the speed at which the return to zero occurs.")]
        [Range(0, 0.1f)]
        public float toZero = 0.01f;

        [Tooltip("Output of this script.")]
        public float xAxis;
        [Tooltip("Output of this script.")]
        public float yAxis;

        private void FixedUpdate()
        {
            // Read input from CrossPlatformInputManager
            xAxis += CrossPlatformInputManager.GetAxis("Horizontal") * sensitivityX;
            if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
            {
                yAxis += CrossPlatformInputManager.GetAxis("Vertical") * sensitivityY;
            }
            else
            {
                yAxis += CrossPlatformInputManager.GetAxis("Vertical") * sensitivityY * 2; //faster brake
            }

            // Apply damping to xAxis
            xAxis *= (1 - toZero);

            // Clamp the values
            xAxis = Mathf.Clamp(xAxis, -1, 1);
            yAxis = Mathf.Clamp(yAxis, 0, 1);
        }
    }
}
