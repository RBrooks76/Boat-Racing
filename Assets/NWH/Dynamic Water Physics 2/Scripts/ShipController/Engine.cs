using System;
using NWH.DWP2.WaterData;
using UnityEngine;
using UnityEngine.Serialization;

namespace NWH.DWP2.ShipController
{
    /// <summary>
    ///     Engine object. Contains all the parameters related to ships's propulsion systems.
    /// </summary>
    [RequireComponent(typeof(AdvancedShipController))]
    [Serializable]
    public class Engine
    {

        public enum ThrottleBinding
        {
            Throttle, Throttle2, Throttle3, Throttle4
        }

        public ThrottleBinding throttleBinding = ThrottleBinding.Throttle;
        
        public enum RotationDirection
        {
            Left,
            Right,
        }

        public enum State
        {
            On,
            Off,
            Starting,
            Stopping,
        }

        public string name = "Engine";

        /// <summary>
        /// Should the engine start when the throttle is applied?
        /// </summary>
        [Tooltip("Should the engine start when the throttle is applied?")]
        public bool startOnThrottle = true;
        
        /// <summary>
        ///     Is the engine currently on?
        /// </summary>
        [FormerlySerializedAs("_isOn")]
        public bool isOn;

        /// <summary>
        ///     Min RPM of the engine.
        /// </summary>
        [FormerlySerializedAs("_minRPM")]
        [Tooltip("Min RPM of the engine.")]
        public float minRPM = 800;

        /// <summary>
        ///     Max RPM of the engine.
        /// </summary>
        [FormerlySerializedAs("_maxRPM")]
        [Tooltip("Max RPM of the engine.")]
        public float maxRPM = 6000;

        /// <summary>
        ///     Thrust at max RPM.
        /// </summary>
        [FormerlySerializedAs("_maxThrust")]
        [Tooltip("Thrust at max RPM.")]
        public float maxThrust = 5000;

        /// <summary>
        ///     Time needed to spin up the engines up to max RPM
        /// </summary>
        [FormerlySerializedAs("_spinUpTime")]
        [Tooltip("Time needed to spin up the engines up to max RPM")]
        public float spinUpTime = 2f;

        /// <summary>
        ///     Engine RPM when turning over. Used to determine starting sound pitch.
        /// </summary>
        [FormerlySerializedAs("startingRpm")]
        [Tooltip("Used to determine starting sound pitch. Engine RPM when turning over.")]
        public float startingRPM = 300f;

        /// <summary>
        ///     How long the engine starting phase take?
        /// </summary>
        [Tooltip("How long will the engine starting take?")]
        public float startDuration = 1.3f;

        /// <summary>
        ///     How long will the engine stopping phase take?
        /// </summary>
        [Tooltip("How long will the engine stopping take?")]
        public float stopDuration = 0.8f;

        /// <summary>
        ///     Local position where the thust is applied, relative to ship.
        /// </summary>
        [Tooltip("Local position at which the force will be applied.")]
        public Vector3 thrustPosition;

        /// <summary>
        ///     Direction in which the force will be applied.
        /// </summary>
        [Tooltip("Local direction in which the force will be applied.")]
        public Vector3 thrustDirection = Vector3.forward;

        /// <summary>
        ///     Should thrust be applied when above water?
        /// </summary>
        [Tooltip("Should thrust be applied when above water?")]
        public bool applyThrustWhenAboveWater;

        /// <summary>
        ///     Amount of thrust that will be applied if ship is reversing
        /// </summary>
        [Tooltip("Amount of thrust that will be applied if ship is reversing.")]
        public float reverseThrustCoefficient = 0.3f;

        /// <summary>
        ///     Ship peed at which propeller will reach it's maximum rotational speed.
        /// </summary>
        [Tooltip("Ship speed at which propeller will reach it's maximum rotational speed.")]
        public float maxSpeed = 20f;

        /// <summary>
        ///     Thrust curve of the propeller. X axis is speed in m/s and y axis is efficiency.
        /// </summary>
        [Tooltip("Thrust curve of the propeller. X axis is speed in m/s and y axis is efficiency.")]
        public AnimationCurve thrustCurve =
            new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.5f, 0.95f), new Keyframe(1f, 0f));

        /// <summary>
        ///     Optional. Only use if you vessel has propeller mounted to the rudder (as in outboard engines). Propuslion force
        ///     direction will be rotated with rudder if assigned.
        /// </summary>
        [Tooltip(
            "Optional. Only use if you vessel has propeller mounted to the rudder (as in outboard engines). Propuslion force direction will be rotated with rudder if assigned.")]
        public Transform rudderTransform;

        /// <summary>
        ///     Optional. Propeller transform. Visual rotation only, does not affect physics.
        /// </summary>
        [Tooltip("Optional. Propeller transform. Visual rotation only, does not affect physics.")]
        public Transform propellerTransform;

        /// <summary>
        ///     Engine RPM will be multiplied by this value to get rotation speed of the propeller. Animation only.
        /// </summary>
        [Tooltip("Engine RPM will be multiplied by this value to get rotation speed of the propeller. Animation only.")]
        public float propellerRpmRatio = 0.1f;

        /// <summary>
        ///     Direction of propeller rotation. Affects animation only.
        /// </summary>
        [Tooltip("Direction of propeller rotation. Animation only.")]
        public RotationDirection rotationDirection = RotationDirection.Right;

        /// <summary>
        ///     Engine running audio source.
        /// </summary>
        [Tooltip("Engine running audio source.")]
        public AudioSource runningSource;

        /// <summary>
        ///     Engine starting source.
        /// </summary>
        [Tooltip("[Optional] Sound of engine starting. If left empty fade-in will be used.")]
        public AudioSource startingSource;

        /// <summary>
        ///     Engine stopping source.
        /// </summary>
        [Tooltip("[Optional] Sound of engine stopping. If left empty cut-out will be used.")]
        public AudioSource stoppingSource;

        /// <summary>
        ///     Base volume of the engine
        /// </summary>
        [Tooltip("Base (idle) volume of the engine.")]
        [Range(0, 2)]
        public float volume = 0.2f;

        /// <summary>
        ///     Idle pitch of the engine
        /// </summary>
        [Tooltip("Base (idle) pitch of the engine.")]
        [Range(0, 2)]
        public float pitch = 0.5f;

        /// <summary>
        ///     Volume range of the engine.
        /// </summary>
        [Tooltip("Volume range of the engine.")]
        [Range(0, 2)]
        public float volumeRange = 0.8f;

        /// <summary>
        ///     Pitch range of the engine.
        /// </summary>
        [Tooltip("Pitch range of the engine.")]
        [Range(0, 2)]
        public float pitchRange = 1f;
        
        private float _rpm;
        private float _spinVelocity;
        private State _engineState;
        private float _startTime;
        private float _stopTime;
        private bool  _wasOn;

        private AdvancedShipController _sc;

        public State EngineState
        {
            get => _engineState;
            set => _engineState = value;
        }

        /// <summary>
        ///     Current RPM of the engine
        /// </summary>
        public float RPM
        {
            get { return Mathf.Clamp(_rpm, minRPM, maxRPM); }
        }

        /// <summary>
        ///     Percentage of rpm range engine is currently on
        /// </summary>
        public float RpmPercent
        {
            get { return Mathf.Clamp01((RPM - minRPM) / maxRPM); }
        }

        /// <summary>
        ///     Current thrust generated by the engine / propeller
        /// </summary>
        public float Thrust { get; private set; }

        /// <summary>
        ///     True if engine's thrust postion is under water.
        /// </summary>
        public bool Submerged
        {
            get
            {
                return _sc.ReferenceWaterObject == null || _sc.ReferenceWaterObject.GetWaterHeightSingle(ThrustPosition) > ThrustPosition.y;
            }
        }

        /// <summary>
        ///     Point at which thrust will be applied to the Rigidbody, in world coordinates.
        /// </summary>
        public Vector3 ThrustPosition
        {
            get { return _sc.transform.TransformPoint(thrustPosition); }
        }

        /// <summary>
        ///     Direction of thrust force in world coordinates.
        /// </summary>
        public Vector3 ThrustDirection
        {
            get
            {
                if (rudderTransform == null)
                {
                    return _sc.transform.TransformDirection(thrustDirection).normalized;
                }

                return rudderTransform.TransformDirection(thrustDirection).normalized;
            }
        }


        public void Initialize(AdvancedShipController sc)
        {
            _sc = sc;

            if (isOn)
            {
                _engineState = State.On;
                _wasOn       = true;
            }
            else
            {
                _engineState = State.Off;
                _wasOn       = false;
            }

            // Init sound
            SoundInit();
        }


        public virtual void Update()
        {
            if (_sc.input.EngineStartStop)
            {
                if (isOn)
                {
                    StopEngine();
                }
                else
                {
                    StartEngine();
                }
            }
            
            // Get throttle
            float throttleInput = 0;
            switch (throttleBinding)
            {
                case ThrottleBinding.Throttle:
                    throttleInput = _sc.input.Throttle;
                    break;
                case ThrottleBinding.Throttle2:
                    throttleInput = _sc.input.Throttle2;
                    break;
                case ThrottleBinding.Throttle3:
                    throttleInput = _sc.input.Throttle3;
                    break;
                case ThrottleBinding.Throttle4:
                    throttleInput = _sc.input.Throttle4;
                    break;
            }


            // Start on throttle
            if (!isOn && startOnThrottle && throttleInput > 0.5f)
            {
                StartEngine();
            }

            // Check engine state
            if (_engineState == State.Starting && !isOn)
            {
                _engineState = State.Off;
            }
            else if (isOn && !_wasOn)
            {
                _engineState = State.Starting;
                _startTime   = Time.realtimeSinceStartup;
            }
            else if (!isOn && _wasOn)
            {
                _engineState = State.Stopping;
                _stopTime    = Time.realtimeSinceStartup;
            }

            // Run timer starting or stopping
            if (_engineState == State.Starting)
            {
                if (Time.realtimeSinceStartup > _startTime + startDuration)
                {
                    _engineState = State.On;
                }
            }
            else if (_engineState == State.Stopping)
            {
                if (Time.realtimeSinceStartup > _stopTime + startDuration)
                {
                    _engineState = State.Off;
                }
            }

            // RPM
            float newRpm = 0f;
            switch (_engineState)
            {
                case State.On:
                    newRpm = (0.7f + 0.3f * (_sc.Speed / maxSpeed)) * Mathf.Abs(throttleInput) * maxRPM;
                    newRpm = Mathf.Clamp(newRpm, minRPM, maxRPM);
                    if (!Submerged)
                    {
                        newRpm = maxRPM;
                    }

                    break;
                case State.Off:
                    newRpm = 0;
                    break;
                case State.Starting:
                    newRpm = startingRPM;
                    break;
                case State.Stopping:
                    newRpm = 0f;
                    break;
            }

            _rpm = Mathf.SmoothDamp(_rpm, newRpm, ref _spinVelocity, spinUpTime);

            if (_engineState == State.On)
            {
                // Check if propeller under water
                bool applyForce = Submerged || applyThrustWhenAboveWater;

                // Check if thrust can be applied
                Thrust = 0;
                if (applyForce && maxRPM != 0 && maxSpeed != 0 && RPM > minRPM + 1f && throttleInput != 0)
                {
                    Thrust = Mathf.Sign(throttleInput) * (_rpm / maxRPM) *
                             thrustCurve.Evaluate(Mathf.Abs(_sc.Speed) / maxSpeed) * maxThrust;
                    Thrust = Mathf.Sign(throttleInput) == 1 ? Thrust : Thrust * reverseThrustCoefficient;
                    if (_sc.VehicleMultiplayerInstanceType == Vehicle.MultiplayerInstanceType.Local) _sc.vehicleRigidbody.AddForceAtPosition(Thrust * ThrustDirection, ThrustPosition);
                }

                if (propellerTransform != null)
                {
                    float zRotation = _rpm * propellerRpmRatio * 6.0012f * Time.fixedDeltaTime;
                    if (rotationDirection == RotationDirection.Right)
                    {
                        zRotation = -zRotation;
                    }

                    propellerTransform.RotateAround(propellerTransform.position, propellerTransform.forward, zRotation);
                }
            }

            SoundUpdate();

            _wasOn = isOn;
        }


        /// <summary>
        ///     Starts the ship engine.
        /// </summary>
        public void StartEngine()
        {
            isOn = true;
        }


        /// <summary>
        ///     Stops the ship engine.
        /// </summary>
        public void StopEngine()
        {
            isOn = false;
            StopAll();
        }


        public virtual void SoundInit()
        {
            if (runningSource != null)
            {
                runningSource.loop        = true;
                runningSource.playOnAwake = false;
            }

            if (startingSource != null)
            {
                startingSource.loop        = false;
                startingSource.playOnAwake = false;
            }

            if (stoppingSource != null)
            {
                stoppingSource.loop        = false;
                stoppingSource.playOnAwake = false;
            }
        }


        public virtual void SoundUpdate()
        {
            if (runningSource == null)
            {
                //Debug.LogWarning($"No AudioSource assigned to Running Source field of object {_sc.name}");
                return;
            }

            // Pitch
            runningSource.pitch = pitch + RpmPercent * pitchRange;

            // Volume
            runningSource.volume = volume + RpmPercent * volumeRange;

            if (_engineState == State.On)
            {
                PlayRunning();
            }
            else if (_engineState == State.Off)
            {
                StopAll();
            }
            else if (_engineState == State.Starting)
            {
                PlayStarting();
            }
            else if (_engineState == State.Stopping)
            {
                PlayStopping();
            }
        }


        private void PlayStarting()
        {
            if (startingSource == null)
            {
                if (!runningSource.isPlaying)
                {
                    runningSource.Play();
                }

                runningSource.volume = Mathf.Lerp(0f, volume, (Time.realtimeSinceStartup - _startTime) / startDuration);
            }

            if (stoppingSource != null)
            {
                stoppingSource.Stop();
            }

            if (startingSource != null && runningSource != null)
            {
                runningSource.Stop();
            }

            if (startingSource != null)
            {
                startingSource.Play();
            }
        }


        private void PlayRunning()
        {
            //if (startingSource != null) startingSource.Stop();
            if (stoppingSource != null)
            {
                stoppingSource.Stop();
            }

            if (runningSource != null)
            {
                if (!runningSource.isPlaying)
                {
                    runningSource.Play();
                }
            }
        }


        private void PlayStopping()
        {
            if (startingSource != null)
            {
                startingSource.Stop();
            }

            if (runningSource != null)
            {
                runningSource.Stop();
            }

            if (stoppingSource != null)
            {
                stoppingSource.Play();
            }
        }


        private void StopAll()
        {
            if (startingSource != null)
            {
                startingSource.Stop();
            }

            if (runningSource != null)
            {
                runningSource.Stop();
            }

            if (stoppingSource != null)
            {
                stoppingSource.Stop();
            }
        }


        public void SetDefaults()
        {
            name            = "Engine";
            throttleBinding = ThrottleBinding.Throttle;
            minRPM          = 500;
            maxRPM          = 3000;
            maxThrust       = 8000;
            spinUpTime      = 2;
            startingRPM     = 300;
            startDuration   = 1.3f;
            stopDuration    = 0.8f;

            thrustPosition           = Vector3.zero;
            thrustDirection          = Vector3.forward;
            reverseThrustCoefficient = 0.3f;
            maxSpeed                 = 20f;
            thrustCurve = AnimationCurve.Constant(0, 1f, 1f);

            volume      = 0.2f;
            volumeRange = 0.4f;
            pitch       = 0.4f;
            pitchRange  = 0.6f;
        }
    }
}