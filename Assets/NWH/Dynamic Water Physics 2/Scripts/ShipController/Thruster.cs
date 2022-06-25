using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Bow or stern thrusters.
    ///     Can be multiple of each.
    /// </summary>
    [Serializable]
    public class Thruster
    {
        public enum RotationDirection
        {
            Left,
            Right,
        }

        public enum ThrusterPosition
        {
            BowThruster,
            SternThruster,
        }

        public ThrusterPosition thrusterPosition = ThrusterPosition.BowThruster;

        [Tooltip("Name of the thruster - can be any string.")]
        public string name = "Thruster";

        [Tooltip("Relative force application position.")]
        public Vector3 position;

        [Tooltip("Max thrust in [N].")]
        public float maxThrust;

        [Tooltip("Time needed to reach maxThrust.")]
        public float spinUpSpeed = 1f;

        [Tooltip("Optional. Transform representing a propeller. Visual only.")]
        public Transform propellerTransform;

        [FormerlySerializedAs("rotationDirection")]
        [Tooltip("Rotation direction of the propeller. Visual only.")]
        public RotationDirection propellerRotationDirection = RotationDirection.Right;

        [Tooltip("Rotation speed of the propeller if assigned. Visual only.")]
        public float propellerRotationSpeed = 1000f;

        private AdvancedShipController sc;

        private float thrust;

        public Vector3 WorldPosition
        {
            get { return sc.transform.TransformPoint(position); }
        }

        public float Input
        {
            get
            {
                float input = 0;
                if (thrusterPosition == ThrusterPosition.BowThruster)
                {
                    input = -sc.input.BowThruster;
                }
                else
                {
                    input = -sc.input.SternThruster;
                }

                return input;
            }
        }


        public void Initialize(AdvancedShipController sc)
        {
            this.sc = sc;
        }


        public virtual void Update()
        {
            float newThurst = maxThrust * -Input;
            thrust = Mathf.MoveTowards(thrust, newThurst, spinUpSpeed * maxThrust * Time.fixedDeltaTime);
            if (sc.VehicleMultiplayerInstanceType == Vehicle.MultiplayerInstanceType.Local) sc.vehicleRigidbody.AddForceAtPosition(thrust * sc.transform.right, WorldPosition);

            if (propellerTransform != null)
            {
                float zRotation = Input * propellerRotationSpeed * Time.fixedDeltaTime;
                if (propellerRotationDirection == RotationDirection.Right)
                {
                    zRotation = -zRotation;
                }

                propellerTransform.RotateAround(propellerTransform.position, propellerTransform.forward, zRotation);
            }
        }
    }
}