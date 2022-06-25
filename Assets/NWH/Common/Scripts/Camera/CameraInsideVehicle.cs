using System;
using UnityEngine;

namespace NWH.Common.Cameras
{
    /// <summary>
    ///     Empty component that should be attached to the cameras that are inside the vehicle if interior sound change is to
    ///     be used.
    /// </summary>
    public class CameraInsideVehicle : MonoBehaviour
    {
        /// <summary>
        ///     Is the camera inside vehicle?
        /// </summary>
        [Tooltip("    Is the camera inside vehicle?")]
        public bool isInsideVehicle = true;

        private Vehicle _vehicle;

        private void Awake()
        {
            _vehicle = GetComponentInParent<Vehicle>();
            Debug.Assert(_vehicle != null, "CameraInsideVehicle needs to be attached to an object containing a Vehicle script.");
        }

        private void Update()
        {
            _vehicle.cameraInsideVehicle = isInsideVehicle;
        }
    }
}