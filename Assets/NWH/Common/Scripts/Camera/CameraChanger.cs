using System.Collections.Generic;
using NWH.Common.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.Common.Cameras
{
    /// <summary>
    ///     Switches between the camera objects that are children to this object and contain camera tag,
    ///     in order they appear in the hierarchy or in order they are added to the vehicle cameras list.
    /// </summary>
    public class CameraChanger : MonoBehaviour
    {
        /// <summary>
        ///     If true vehicleCameras list will be filled through cameraTag.
        /// </summary>
        [Tooltip("    If true vehicleCameras list will be filled through cameraTag.")]
        public bool autoFindCameras = true;

        /// <summary>
        ///     Index of the camera from vehicle cameras list that will be active first.
        /// </summary>
        [Tooltip("    Index of the camera from vehicle cameras list that will be active first.")]
        public int currentCameraIndex;

        /// <summary>
        ///     List of cameras that the changer will cycle through. Leave empty if you want cameras to be automatically detected.
        ///     To be detected cameras need to have camera tag and be children of the object this script is attached to.
        /// </summary>
        [FormerlySerializedAs("vehicleCameras")]
        [Tooltip(
            "List of cameras that the changer will cycle through. Leave empty if you want cameras to be automatically detected." +
            " To be detected cameras need to have camera tag and be children of the object this script is attached to.")]
        public List<GameObject> cameras = new List<GameObject>();

        private int     _previousCamera;
        private Vehicle _vehicle;


        private void Awake()
        {
            _vehicle = GetComponentInParent<Vehicle>();
            if (_vehicle == null)
            {
                Debug.LogError("None of the parent objects of CameraChanger contain VehicleController.");
            }

            _vehicle.onWake.AddListener(OnVehicleWake);
            _vehicle.onSleep.AddListener(OnVehicleSleep);

            if (_vehicle == null)
            {
                Debug.Log("None of the parents of camera changer contain VehicleController component. " +
                          "Make sure that the camera changer is amongst the children of VehicleController object.");
            }

            if (autoFindCameras)
            {
                cameras = new List<GameObject>();
                foreach (Camera cam in GetComponentsInChildren<Camera>(true))
                {
                    cameras.Add(cam.gameObject);
                }
            }

            if (cameras.Count == 0)
            {
                Debug.LogWarning("No cameras could be found by CameraChanger. Either add cameras manually or " +
                                 "add them as children to the game object this script is attached to.");
            }

            OnMultiplayerInstanceTypeChanged();

            _vehicle.onSetMultiplayerInstanceType.AddListener(OnMultiplayerInstanceTypeChanged);
        }


        private void Update()
        {
            if (_vehicle.IsAwake &&
                _vehicle.VehicleMultiplayerInstanceType == Vehicle.MultiplayerInstanceType.Local &&
                InputProvider.Instances.Count > 0)
            {
                bool changeCamera = InputProvider.CombinedInput<SceneInputProviderBase>(i => i.ChangeCamera());

                if (changeCamera)
                {
                    NextCamera();
                    CheckIfInside();
                }
            }
        }


        private void OnMultiplayerInstanceTypeChanged()
        {
            if (!_vehicle.IsAwake ||
                _vehicle.VehicleMultiplayerInstanceType == Vehicle.MultiplayerInstanceType.Remote)
            {
                DisableAllCameras();
            }
            else
            {
                EnableCurrentDisableOthers();
                CheckIfInside();
            }
        }


        private void EnableCurrentDisableOthers()
        {
            int cameraCount = cameras.Count;
            for (int i = 0; i < cameraCount; i++)
            {
                if (cameras[i] == null)
                {
                    continue;
                }

                if (i == currentCameraIndex)
                {
                    cameras[i].SetActive(true);
                    AudioListener al = cameras[i].GetComponent<AudioListener>();
                    if (al != null)
                    {
                        al.enabled = true;
                    }
                }
                else
                {
                    cameras[i].SetActive(false);
                    AudioListener al = cameras[i].GetComponent<AudioListener>();
                    if (al != null)
                    {
                        al.enabled = false;
                    }
                }
            }
        }


        private void DisableAllCameras()
        {
            int cameraCount = cameras.Count;
            for (int i = 0; i < cameraCount; i++)
            {
                cameras[i].SetActive(false);
                AudioListener al = cameras[i].GetComponent<AudioListener>();
                if (al != null)
                {
                    al.enabled = true;
                }
            }
        }


        /// <summary>
        ///     Activates next camera in order the camera scripts are attached to the camera object.
        /// </summary>
        public void NextCamera()
        {
            if (cameras.Count <= 0)
            {
                return;
            }

            currentCameraIndex++;
            if (currentCameraIndex >= cameras.Count)
            {
                currentCameraIndex = 0;
            }

            EnableCurrentDisableOthers();
        }


        public void PreviousCamera()
        {
            if (cameras.Count <= 0)
            {
                return;
            }

            currentCameraIndex--;
            if (currentCameraIndex < 0)
            {
                currentCameraIndex = cameras.Count - 1;
            }

            EnableCurrentDisableOthers();
        }


        private void CheckIfInside()
        {
            if (cameras.Count == 0 || cameras[currentCameraIndex] == null)
            {
                return;
            }

            CameraInsideVehicle civ = cameras[currentCameraIndex]?.GetComponent<CameraInsideVehicle>();
            if (civ != null)
            {
                _vehicle.cameraInsideVehicle = civ.isInsideVehicle;
            }
            else
            {
                _vehicle.cameraInsideVehicle = false;
            }
        }


        private void OnVehicleWake()
        {
            EnableCurrentDisableOthers();
        }


        private void OnVehicleSleep()
        {
            DisableAllCameras();
        }
    }
}