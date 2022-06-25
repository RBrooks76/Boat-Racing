using System;
using System.Collections.Generic;
using NWH.DWP2.WaterObjects;
using UnityEngine;
using UnityEngine.Events;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Script for controling ships, boats and other vessels.
    /// </summary>
    [RequireComponent(typeof(Anchor))]
    [Serializable]
    public class AdvancedShipController : Vehicle
    {
        public WaterObject ReferenceWaterObject;
        
        /// <summary>
        ///     Class that handles all of the user input.
        /// </summary>
        [Tooltip("Handles all of the user input.")]
        [SerializeField]
        public ShipInputHandler input = new ShipInputHandler();

        /// <summary>
        ///     Ship's engines.
        /// </summary>
        [Tooltip(
            "List of engines. Each engine is a propulsion system in itself consisting of the engine and the propeller.")]
        [SerializeField]
        public List<Engine> engines = new List<Engine>();

        /// <summary>
        ///     Ship's rudders.
        /// </summary>
        [Tooltip("List of rudders.")]
        [SerializeField]
        public List<Rudder> rudders = new List<Rudder>();

        /// <summary>
        ///     Bow or stern thrusters that a ship has.
        /// </summary>
        [Tooltip("List of either bow or stern thrusters.")]
        [SerializeField]
        public List<Thruster> thrusters = new List<Thruster>();

        /// <summary>
        ///     Should the anchor be dropped when the ship is deactivated?
        /// </summary>
        public bool dropAnchorWhenInactive = true;

        /// <summary>
        ///     Should the anchor be weighed/lifted when the ship is activated?
        /// </summary>
        public bool weighAnchorWhenActive = true;

        /// <summary>
        ///     Should the ship roll be stabilized?
        /// </summary>
        public bool stabilizeRoll;

        /// <summary>
        ///     Should the ship pitch be stabilized?
        /// </summary>
        public bool stabilizePitch;

        /// <summary>
        ///     Angle at which roll stabilization torque reaches maximum.
        /// </summary>
        public float maxStabilizationTorqueAngle = 20f;

        /// <summary>
        ///     Torque that will be applied to stabilize roll when the ship roll angle reaches maxStabilizationTorqueAngle.
        /// </summary>
        public float rollStabilizationMaxTorque = 3000f;

        /// <summary>
        ///     Torque that will be applied to stabilize pitch when the ship pitch angle reaches maxStabilizationTorqueAngle.
        /// </summary>
        public float pitchStabilizationMaxTorque;

        private Vector3 _stabilizationTorque = Vector3.zero;

        /// <summary>
        ///     Anchor script.
        /// </summary>
        public Anchor Anchor { get; private set; }

        /// <summary>
        /// Called after ship initialization.
        /// </summary>
        public UnityEvent onShipInitialized = new UnityEvent();

        /// <summary>
        ///     Speed in knots.
        /// </summary>
        public float SpeedKnots
        {
            get { return Speed * 1.944f; }
        }


        public void Start()
        {
            if (ReferenceWaterObject == null)
            {
                ReferenceWaterObject = GetComponentInChildren<WaterObject>();
                if (ReferenceWaterObject == null)
                {
                    Debug.LogWarning("ShipController has no child WaterObjects.");
                }
            }
            
            foreach (Thruster thruster in thrusters)
            {
                thruster.Initialize(this);
            }

            foreach (Rudder rudder in rudders)
            {
                rudder.Initialize(this);
            }

            foreach (Engine engine in engines)
            {
                engine.Initialize(this);
            }

            Anchor = GetComponent<Anchor>();
            if (Anchor == null)
            {
                Debug.LogWarning(
                    $"Object {name} is missing 'Anchor' component which is required for AdvancedShipController to work properly.");
                Anchor = gameObject.AddComponent<Anchor>();
            }
            
            //SetMultiplayerInstanceType(_multiplayerInstanceType);
            
            onShipInitialized.Invoke();
        }


        public void Update()
        {
            if (_multiplayerInstanceType == MultiplayerInstanceType.Local) input.Update();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!isAwake)
            {
                return;
            }

            foreach (Engine engine in engines)
            {
                engine.Update();
            }

            foreach (Rudder rudder in rudders)
            {
                rudder.Update();
            }

            foreach (Thruster thruster in thrusters)
            {
                thruster.Update();
            }

            if (_multiplayerInstanceType == MultiplayerInstanceType.Local)
            {
                if (input.Anchor)
                {
                    if (Anchor.Dropped)
                    {
                        Anchor.Weigh();
                    }
                    else
                    {
                        Anchor.Drop();
                    }
                }

                // Reset bool inputs
                input.Anchor          = false;
                input.EngineStartStop = false;

                if (stabilizePitch || stabilizeRoll)
                {
                    _stabilizationTorque = Vector3.zero;
                
                    if (stabilizeRoll)
                    {
                        _stabilizationTorque.z = -Mathf.Clamp(GetRollAngle() / maxStabilizationTorqueAngle, -1f, 1f) *
                                                 rollStabilizationMaxTorque;
                    }

                    if (stabilizePitch)
                    {
                        _stabilizationTorque.x = Mathf.Clamp(GetPitchAngle() / maxStabilizationTorqueAngle, -1f, 1f) *
                                                 pitchStabilizationMaxTorque;
                    }

                    vehicleRigidbody.AddTorque(transform.TransformVector(_stabilizationTorque));
                }
            }
        }


        public override void Sleep()
        {
            base.Sleep();
            foreach (Engine e in engines)
            {
                e.StopEngine();
            }

            if (dropAnchorWhenInactive && Anchor != null)
            {
                Anchor.Drop();
            }
        }


        public override void Wake()
        {
            base.Wake();
            foreach (Engine e in engines)
            {
                e.StartEngine();
            }

            if (weighAnchorWhenActive && Anchor != null)
            {
                Anchor.Weigh();
            }
        }


        private void OnDrawGizmos()
        {
            Start();

            foreach (Rudder rudder in rudders)
            {
                Gizmos.color = Color.magenta;
            }

            foreach (Engine e in engines)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(e.ThrustPosition, 0.2f);
                Gizmos.DrawRay(new Ray(e.ThrustPosition, e.ThrustDirection));
            }

            foreach (Thruster thruster in thrusters)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(transform.TransformPoint(thruster.position), 0.2f);
                Gizmos.DrawRay(new Ray(thruster.WorldPosition, transform.right));
            }
        }


        /// <summary>
        ///     Returns pitch angle of the ship in degrees.
        /// </summary>
        /// <returns></returns>
        public float GetPitchAngle()
        {
            Vector3 right = transform.right;
            right.y =  0;
            right   *= Mathf.Sign(transform.up.y);
            Vector3 fwd = Vector3.Cross(right, Vector3.up).normalized;
            return Vector3.Angle(fwd, transform.forward) * Mathf.Sign(transform.forward.y);
        }


        /// <summary>
        ///     Returns roll angle of the ship in degrees.
        /// </summary>
        /// <returns></returns>
        public float GetRollAngle()
        {
            Vector3 fwd = transform.forward;
            fwd.y =  0;
            fwd   *= Mathf.Sign(transform.up.y);
            Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized;
            return Vector3.Angle(right, transform.right) * Mathf.Sign(transform.right.y);
        }


        private float WrapAngle(float angle)
        {
            return angle > 180 ? angle - 360 : angle;
        }
    }
}