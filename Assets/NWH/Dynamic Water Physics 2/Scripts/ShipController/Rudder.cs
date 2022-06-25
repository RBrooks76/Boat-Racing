using System;
using UnityEngine;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Represents a single rudder. If rudder has a floating object component it will also be used for steering and not be
    ///     visual-only.
    /// </summary>
    [Serializable]
    public class Rudder
    {
        /// <summary>
        ///     Name of the rudder. Can be any string.
        /// </summary>
        public string name = "Rudder";

        /// <summary>
        ///     Transform representing the rudder.
        /// </summary>
        public Transform rudderTransform;

        /// <summary>
        ///     Max angle in degrees rudder will be able to reach.
        /// </summary>
        public float maxAngle = 45f;

        /// <summary>
        ///     Rotation speed in degrees per second.
        /// </summary>
        public float rotationSpeed = 20f;

        /// <summary>
        ///     Axis around which the rudder will be rotated.
        /// </summary>
        public Vector3 localRotationAxis = new Vector3(0, 1, 0);

        private AdvancedShipController _sc;

        public float Angle { get; private set; }

        public float AnglePercent
        {
            get { return Angle / maxAngle; }
        }


        public void Initialize(AdvancedShipController sc)
        {
            _sc = sc;
        }


        public virtual void Update()
        {
            if (rudderTransform != null)
            {
                float targetAngle = -_sc.input.Steering * maxAngle;
                Angle = Mathf.MoveTowardsAngle(Angle, targetAngle, rotationSpeed * Time.fixedDeltaTime);
                rudderTransform.localRotation = Quaternion.Euler(Angle * localRotationAxis.x,
                                                                 Angle * localRotationAxis.y,
                                                                 Angle * localRotationAxis.z);
            }
        }
    }
}