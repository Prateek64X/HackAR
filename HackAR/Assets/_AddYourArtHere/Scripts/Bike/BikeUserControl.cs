using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Bike
{
    [RequireComponent(typeof (BikeController))]
    public class BikeUserControl : MonoBehaviour
    {
        private BikeController m_Bike; // the car controller we want to use


        private void Awake()
        {
            // get the car controller
            m_Bike = GetComponent<BikeController>();
        }


        private void FixedUpdate()
        {
            // pass the input to the car!
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Bike.Move(h, v, v, handbrake);
#else
            m_Bike.Move(h, v, v, 0f);
#endif
        }
    }
}
