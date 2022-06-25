using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace NWH
{
    /// <summary>
    ///     Base class for all NWH vehicles.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Vehicle : MonoBehaviour
    {
        public enum MultiplayerInstanceType
        {
            Local,
            Remote
        }

        /// <summary>
        ///     Called when vehicle is put to sleep.
        /// </summary>
        [Tooltip("    Called when vehicle is put to sleep.")]
        public UnityEvent onSleep = new UnityEvent();

        /// <summary>
        ///     Called when vehicle is woken up.
        /// </summary>
        [Tooltip("    Called when vehicle is woken up.")]
        public UnityEvent onWake = new UnityEvent();

        /// <summary>
        ///     Cached value of vehicle rigidbody.
        /// </summary>
        [UnityEngine.Tooltip("    Cached value of vehicle rigidbody.")]
        public Rigidbody vehicleRigidbody;

        /// <summary>
        ///     Cached value of vehicle transform.
        /// </summary>
        [Tooltip("    Cached value of vehicle transform.")]
        public Transform vehicleTransform;

        /// <summary>
        ///     Should be true when camera is inside vehicle (cockpit, cabin, etc.).
        ///     Used for audio effects.
        /// </summary>
        [UnityEngine.Tooltip("    Should be true when camera is inside vehicle (cockpit, cabin, etc.).\r\n    Used for audio effects.")]
        public bool cameraInsideVehicle;

        public UnityEvent onSetMultiplayerInstanceType = new UnityEvent();

        /// <summary>
        /// Set to true to have the vehicle awake after Start().
        /// </summary>
        public bool awakeOnStart = true;
        
        /// <summary>
        ///     Determines if vehicle is running locally is synchronized over active multiplayer framework.
        /// </summary>
        [Tooltip("    Determines if vehicle is running locally is synchronized over active multiplayer framework.")]
        protected MultiplayerInstanceType _multiplayerInstanceType = MultiplayerInstanceType.Local;

        [FormerlySerializedAs("_isAwake")] 
        [SerializeField]
        protected bool isAwake = true;

        private Vector3 _prevLocalVelocity;

        /// <summary>
        ///     True if vehicle is awake. Different from disabled. Disable will deactivate the vehicle fully while putting the
        ///     vehicle to sleep will only force the highest lod so that some parts of the vehicle can remain working if configured
        ///     so.
        ///     Set to false if vehicle is parked and otherwise not in focus, but needs to function.
        ///     Call Wake() to wake or Sleep() to put to sleep.
        /// </summary>
        public bool IsAwake
        {
            get { return isAwake; }
        }

        public MultiplayerInstanceType VehicleMultiplayerInstanceType
        {
            get { return _multiplayerInstanceType; }
        }

        /// <summary>
        ///     Cached acceleration in local coordinates (z-forward)
        /// </summary>
        public Vector3 LocalAcceleration { get; private set; }

        /// <summary>
        ///     Cached acceleration in forward direction in local coordinates (z-forward).
        /// </summary>
        public float LocalForwardAcceleration { get; private set; }

        /// <summary>
        ///     Velocity in forward direction in local coordinates (z-forward).
        /// </summary>
        public float LocalForwardVelocity { get; private set; }

        /// <summary>
        ///     Velocity in m/s in local coordinates.
        /// </summary>
        public Vector3 LocalVelocity { get; private set; }

        /// <summary>
        ///     Speed of the vehicle in the forward direction. ALWAYS POSITIVE.
        ///     For positive/negative version use SpeedSigned.
        /// </summary>
        public float Speed
        {
            get { return LocalForwardVelocity < 0 ? -LocalForwardVelocity : LocalForwardVelocity; }
        }

        /// <summary>
        ///     Speed of the vehicle in the forward direction. Can be positive (forward) or negative (reverse).
        ///     Equal to LocalForwardVelocity.
        /// </summary>
        public float SpeedSigned
        {
            get { return LocalForwardVelocity; }
        }

        /// <summary>
        ///     Cached velocity of the vehicle in world coordinates.
        /// </summary>
        public Vector3 Velocity { get; protected set; }

        /// <summary>
        ///     Cached velocity magnitude of the vehicle in world coordinates.
        /// </summary>
        public float VelocityMagnitude { get; protected set; }

        public Vector3 AngularVelocity { get; protected set; }

        public float AngularVelocityMagnitude { get; protected set; }

        public virtual void Awake()
        {
            vehicleTransform = transform;
            vehicleRigidbody = GetComponent<Rigidbody>();

            if (onSetMultiplayerInstanceType == null)
            {
                onSetMultiplayerInstanceType = new UnityEvent();
            }

            if (onSleep == null)
            {
                onSleep = new UnityEvent();
            }

            if (onWake == null)
            {
                onWake = new UnityEvent();
            }
        }


        public virtual void FixedUpdate()
        {
            // Pre-calculate values
            _prevLocalVelocity       = LocalVelocity;
            Velocity                 = vehicleRigidbody.velocity;
            LocalVelocity            = transform.InverseTransformDirection(Velocity);
            LocalAcceleration        = (LocalVelocity - _prevLocalVelocity) / Time.fixedDeltaTime;
            LocalForwardVelocity     = LocalVelocity.z;
            LocalForwardAcceleration = LocalAcceleration.z;
            VelocityMagnitude        = Velocity.magnitude;
            AngularVelocity = vehicleRigidbody.angularVelocity;
            AngularVelocityMagnitude = AngularVelocity.magnitude;
        }


        public virtual void Sleep()
        {
            if (!isAwake) return;

            isAwake = false;
            onSleep.Invoke();
        }


        public virtual void Wake()
        {
            if (isAwake) return;

            isAwake = true;
            onWake.Invoke();
        }


        public virtual void SetMultiplayerInstanceType(MultiplayerInstanceType instanceType, bool isKinematic = true)
        {
            _multiplayerInstanceType = instanceType;
            onSetMultiplayerInstanceType.Invoke();
        }
    }
}