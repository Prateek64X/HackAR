using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace VK.BikeLab
{
    public class Camera123 : MonoBehaviour
    {
        [System.Serializable]
        public enum CameraView { FirstPerson, SecondPerson, ThirdPerson }
        public Transform observationObject;
        public Transform camera1;
        public Transform camera2;
        public Transform camera3;
        //[SerializeField]
        public CameraView cameraView;
        /*
        public CameraView CameraV 
        {
            get { return cameraView; } 
            set { cameraView = value; } 
        }
        */
        [HideInInspector] public Vector3 localPos2 = new Vector3(0, 1.5f, -4);
        [HideInInspector] public float eulerX2 = 6;
        [HideInInspector] public float offsetY2 = 1.5f;
        [HideInInspector] public Vector3 wordOffset3 = new Vector3(5, 1.5f, 5);
        [HideInInspector] public float zoom1 = 1;
        [HideInInspector] public float zoom2 = 1;
        [HideInInspector] public float zoom3 = 1;
        private bool shift;
        private bool alt;

        void Start()
        {
            init();
        }
        public void init()
        {
            localPos2 = camera2.localPosition;
            eulerX2 = camera2.localEulerAngles.x;
            offsetY2 = (camera2.position - camera2.parent.position).y;

            wordOffset3 = camera3.position - camera3.parent.position;
            //wordOffset3 = camera3.localPosition;
            zoom1 = 1;
            zoom2 = 1;
            zoom3 = 1;
        }
        void Update()
        {
            bool vKey;
#if ENABLE_INPUT_SYSTEM
            vKey = Keyboard.current.vKey.wasPressedThisFrame;
#else
        vKey = Input.GetKeyDown(KeyCode.V);
#endif
            if (vKey)
                swithCamera();
            setView();
        }
        private void OnGUI()
        {
            shift = Event.current.shift;
            alt = Event.current.alt;
        }
        public void setView()
        {

            camera1.gameObject.SetActive(cameraView == CameraView.FirstPerson);
            camera2.gameObject.SetActive(cameraView == CameraView.SecondPerson);
            camera3.gameObject.SetActive(cameraView == CameraView.ThirdPerson);

            float zoom = 1;
            if (shift)
                zoom *= 1.03f;
            if (alt)
                zoom /= 1.03f;
            /*
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (shift)
                    zoom = 0.5f;
                else
                    zoom = 2f;
            }
            */
            switch (cameraView)
            {
                case CameraView.FirstPerson:
                    {
                        zoom1 *= zoom;
                        camera1.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                        break;
                    }
                case CameraView.SecondPerson:
                    {
                        zoom2 *= zoom;
                        camera2.localPosition = localPos2 * zoom2;

                        Vector3 pos = camera2.position;
                        pos.y = camera2.parent.position.y + offsetY2 * zoom2;
                        camera2.position = pos;

                        camera2.LookAt(camera2.parent.position + Vector3.up, Vector3.up);
                        camera2.eulerAngles = new Vector3(eulerX2, camera2.eulerAngles.y, 0);
                        break;
                    }
                case CameraView.ThirdPerson:
                    {
                        zoom3 *= zoom;
                        camera3.position = camera3.parent.position + wordOffset3 * zoom3;
                        camera3.LookAt(camera3.parent.position + Vector3.up * zoom3, Vector3.up);
                        //Vector3 e = camera3.eulerAngles;
                        //e.x = 0;
                        //camera3.eulerAngles = e;
                        break;
                    }
            }
        }
        private void swithCamera()
        {
            switch (cameraView)
            {
                case CameraView.FirstPerson: cameraView = CameraView.SecondPerson; break;
                case CameraView.SecondPerson: cameraView = CameraView.ThirdPerson; break;
                case CameraView.ThirdPerson: cameraView = CameraView.FirstPerson; break;
            }

        }
    }
}