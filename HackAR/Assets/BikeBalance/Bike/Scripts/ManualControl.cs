using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace VK.BikeLab
{
    /// <summary>
    /// ManualControl receives data from the BikeInput script and controls the BikeController script using appropriate methods.
    /// </summary>
    [RequireComponent(typeof(BikeInput))]
    public class ManualControl : MonoBehaviour
    {
        [Tooltip("BikeController")]
        public BikeController bike;
        [Tooltip("Sets the Time.timeScale.")]
        [Range(0.01f, 10)]
        public float timeScale;
        [Tooltip("Scale of X axis. If X axis = 1 velocity = maxVelocity.")]
        [Range(0, 200)]
        public float maxVelocity;
        [Space]
        [Tooltip("If true, balance is carried out automatically else the user must balance manually. In the last case, steering angle is calculated as a mix between user input and balanced steering angle + dumper.")]
        public bool fullAuto;
        [Tooltip("The interpolation value between user input and balanced steering angle.")]
        [Range(0, 1)]
        public float autoBalance;
        [Tooltip("Dumper factor.")]
        [Range(0, 1)]
        public float dumper;
        [Tooltip("These fields are calculated automatically at runtime.")]
        [Space]
        public Info info;

        public TextMeshPro speedoTMP;
        public GameObject speedoPlane, leanBalancePlane;

        private BikeInput bikeInput;
        private Rigidbody rb;
        private float goldVelocity;

        void Start()
        {
            bike.Init();
            rb = bike.GetRigidbody();
            bikeInput = GetComponent<BikeInput>();
            goldVelocity = goldV();
        }

        private void FixedUpdate()
        {
            Time.timeScale = timeScale;
            setVelo();
            setSteer();

            info.currentLean = bike.getLean();
            info.currentSteer = bike.frontCollider.steerAngle;
            info.currentVelocity = rb.velocity.magnitude;

            // Speedometer custom
            float angle = (info.targetVelocity / maxVelocity) * 270f;
            speedoPlane.transform.localEulerAngles = new Vector3(0f, angle, 0f); // update speedo

            speedoTMP.text = Mathf.FloorToInt(info.currentVelocity * 2).ToString();

            // lean indicator
            float leanRange = 16f;
            float maxLeanRange = 20f;
            float leanPercent = info.currentLean / bike.info.safeLean;

            float leanXPosition;

            if (leanPercent >= 0 && leanPercent <= 1)
            {
                leanXPosition = leanPercent * leanRange;
            }
            else if (leanPercent > 1)
            {
                leanXPosition = leanRange + (leanPercent - 1) * (maxLeanRange - leanRange);
            }
            else // leanPercent < 0
            {
                leanXPosition = leanPercent * leanRange;
            }

            // Clamp the leanXPosition to the maximum range
            if (leanXPosition > 0)
            {
                leanXPosition = Mathf.Clamp(leanXPosition, -maxLeanRange, maxLeanRange);
            }

            // Update the position of the leanBalancePlane
            leanBalancePlane.transform.localPosition = new Vector3(leanXPosition, leanBalancePlane.transform.localPosition.y, leanBalancePlane.transform.localPosition.z);

        }

        void Update()
        {
            bool pKey;
            bool rKey;
#if ENABLE_INPUT_SYSTEM
            pKey = Keyboard.current.pKey.wasPressedThisFrame;
            rKey = Keyboard.current.rKey.wasPressedThisFrame;
#else
            pKey = Input.GetKey(KeyCode.P);
            rKey = Input.GetKey(KeyCode.R);
#endif
            if (pKey)
                Debug.Break();
            if (rKey)
                bike.reset();
        }

        private void setSteer()
        {
            float steer = bikeInput.xAxis * bike.maxSteer;
            info.targetSteer = steer;
            if (fullAuto)
                setAutoSteer();
            else
                setMixedSteer();
        }

        private void setVelo()
        {
            info.targetVelocity = bikeInput.yAxis * maxVelocity;
            Vector3 localV = transform.InverseTransformVector(rb.velocity);
            float diff = info.targetVelocity - localV.z;
            float a = Mathf.Clamp(diff * 10.0f, -100f, 100f);

            if (a > 0)
            {
                bike.setAcceleration(a);
                bike.safeBrake(0);
            }
            else
            {
                bike.setAcceleration(0);
                bike.safeBrake(-a);
            }
        }

        private void setAutoSteer()
        {
            if (rb.velocity.magnitude > goldVelocity)
                bike.setSteerByLean(info.targetSteer);
            else if (rb.velocity.magnitude < 1)
                bike.setSteer(0);
            else
                bike.setSteer(info.targetSteer);
        }

        private float setMixedSteer()
        {
            float balanceSteer = bike.GetBalanceSteer();
            if (rb.velocity.magnitude < 1)
                balanceSteer = 0;
            float dmp = getDumper() * dumper;
            float mix = Mathf.Lerp(info.targetSteer, balanceSteer, autoBalance);
            bike.setSteerDirectly(mix + dmp);
            return balanceSteer;
        }

        private float getDumper()
        {
            Vector3 av = transform.InverseTransformVector(rb.angularVelocity);
            float veloFactor = 1 / (rb.velocity.magnitude + 1);
            float lean = bike.getLean();
            float damper = -(av.z * 100 + lean * 1.3f) * veloFactor;
            damper = Mathf.Clamp(damper, -20, 20);
            return damper;
        }

        private float goldV()
        {
            float minD = 1000;
            float gold = 0;
            for (int i = 30; i < 60; i++)
            {
                float v = (float)i * 0.1f;
                float d = 0;
                for (int j = 0; j < 30; j++)
                {
                    float lean = (float)j;
                    float steer = bike.geometry.getSteer(-lean, v);
                    d += Mathf.Abs(lean - steer);
                }
                if (d < minD)
                {
                    minD = d;
                    gold = v;
                }
            }
            return gold;
        }

        [System.Serializable]
        public class Info
        {
            [Space]
            [Range(-30, 30)] public float targetSteer;
            [Range(-30, 30)] public float currentSteer;
            [Space]
            [Range(-70, 70)] public float targetLean;
            [Range(-70, 70)] public float currentLean;
            [Tooltip("m/s")]
            [Space]
            [Range(0, 200)] public float targetVelocity;
            [Range(0, 200)] public float currentVelocity;
        }
    }
}
