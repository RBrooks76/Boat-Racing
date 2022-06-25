using UnityEngine;

namespace NWH.Common.Cameras
{
    public class VehicleCamera : MonoBehaviour
    {
        /// <summary>
        ///     Transform that this script is targeting. Can be left empty if head movement is not being used.
        /// </summary>
        [Tooltip(
            "Transform that this script is targeting. Can be left empty if head movement is not being used.")]
        public Transform target;


        public virtual void Awake()
        {
            if (target == null)
            {
                target = GetComponentInParent<Rigidbody>()?.transform;
                if (target == null)
                {
                    Debug.LogError($"Make sure that the target object of camera {name} is assigned.");
                }
            }
        }
    }
}