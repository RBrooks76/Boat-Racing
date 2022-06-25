using NWH.Common.Input;
using NWH.Common.Utility;
using NWH.Common.CoM;
using NWH.DWP2.WaterData;
using NWH.DWP2.WaterObjects;
using UnityEngine;

namespace NWH.DWP2.ShipController
{
    [RequireComponent(typeof(AdvancedShipController))]
    [RequireComponent(typeof(VariableCenterOfMass))]
    public class Submarine : MonoBehaviour
    {
        public WaterObject ReferenceWaterObject;
        
        /// <summary>
        ///     Depth at which the submarine will aim to be at. This is achieved by changing the weight of the submarine's
        ///     rigidbody.
        /// </summary>
        [Tooltip(
            "Depth at which the submarine will aim to be at. This is achieved by changing the weight of the submarine's rigidbody.")]
        public float requestedDepth;

        /// <summary>
        ///     Speed of depth change in m/s when a key is used to change depth.
        /// </summary>
        [Tooltip("Speed of depth change in m/s when a key is used to change depth.")]
        public float depthInputSensitivity = 10f;

        /// <summary>
        ///     Proportional term of the depth PID controller.
        /// </summary>
        [Tooltip("Integral term of the depth PID controller.")]
        [Range(0, 10)]
        public float depthPID_Kp = 3f;
        
        /// <summary>
        ///     Integral term of the depth PID controller.
        /// </summary>
        [Tooltip("Integral term of the depth PID controller.")]
        [Range(0, 5)]
        public float depthPID_Ki = 0.3f;
        
        /// <summary>
        ///     Derivative term term of the depth PID controller.
        /// </summary>
        [Tooltip("Derivative term of the depth PID controller.")]
        [Range(0, 5)]
        public float depthPID_Kd = 0.05f;

        /// <summary>
        ///     Maximum additional mass that can be added (taking on water) to the base mass of the rigidbody to make submarine
        ///     sink.
        /// </summary>
        [Tooltip(
            "Maximum additional mass that can be added (taking on water) to the base mass of the rigidbody to make submarine sink.")]
        [Range(1, 5)]
        public float maxMassFactor = 3f;

        /// <summary>
        ///     If enabled submarine will try to keep horizontal by shifting the center of mass.
        /// </summary>
        [Tooltip("If enabled submarine will try to keep horizontal by shifting the center of mass.")]
        public bool keepHorizontal = false;

        /// <summary>
        ///     Sensitivity of calculation trying to keep the submarine horizontal. Higher number will mean faster reaction.
        /// </summary>
        [Tooltip(
            "Sensitivity of calculation trying to keep the submarine horizontal. Higher number will mean faster reaction.")]
        public float keepHorizontalSensitivity = 1f;

        /// <summary>
        ///     Maximum rigidbody center of mass offset that can be used to keep the submarine level.
        /// </summary>
        [Tooltip("Maximum rigidbody center of mass offset that can be used to keep the submarine level.")]
        public float maxMassOffset = 5f;

        private Rigidbody              _rb;
        private VariableCenterOfMass   _com;
        private float                  _initialMass;
        private Vector3                _initialCom;
        private float                  _depth;
        private float                  _mass;
        private float                  _zOffset;
        private AdvancedShipController _asc;

        private PIDController _depthController;

        [HideInInspector] [SerializeField] private float depthInput;

        public float DepthInput
        {
            get { return depthInput; }
            set { depthInput = Mathf.Clamp(value, -1f, 1f); }
        }

        private void Awake()
        {
            _depthController = new PIDController(depthPID_Kp, depthPID_Ki, depthPID_Kd, 0f, maxMassFactor);
            
            if (ReferenceWaterObject == null)
            {
                ReferenceWaterObject = GetComponentInChildren<WaterObject>();
            }
        }

        private void Start()
        {
            _rb          = GetComponent<Rigidbody>();
            _com         = GetComponent<VariableCenterOfMass>();
            if (_com == null)
            {
                Debug.LogError($"VariableCenterOfMass script not found on object {name}. If updating from older versions" +
                               $"of DWP2 replace CenterOfMass [deprecated] script with VariableCenterOfMass [new] script.");
            }
            
            _asc         = GetComponent<AdvancedShipController>();
            _initialMass = _rb.mass;
            _initialCom  = _com.centerOfMassOffset;
        }


        private void FixedUpdate()
        {
            Debug.Assert(ReferenceWaterObject != null, "ReferenceWaterObject not set.");
            
            DepthInput     =  InputProvider.CombinedInput<ShipInputProvider>(i => i.SubmarineDepth());
            
            requestedDepth -= DepthInput * depthInputSensitivity * Time.fixedDeltaTime;
            _depth = ReferenceWaterObject.GetWaterHeightSingle(transform.position) - transform.position.y;

            _depthController.SetPoint = requestedDepth;
            _depthController.ProcessVariable = _depth;
            _depthController.GainProportional = depthPID_Kp;
            _depthController.GainDerivative = depthPID_Kd;
            _depthController.GainIntegral = depthPID_Ki;
            _depthController.minValue = 0f;
            _depthController.maxValue = maxMassFactor;
            float pidOutput = _depthController.ControlVariable(Time.fixedDeltaTime);
            _mass       = Mathf.Clamp(1f + pidOutput, 1f, maxMassFactor) * _initialMass;
            _rb.mass    = _mass; // TODO - move to variable com

            if (keepHorizontal)
            {
                float angle = Vector3.SignedAngle(transform.up, Vector3.up, transform.right);
                _zOffset = Mathf.Clamp(Mathf.Sign(angle) * Mathf.Pow(angle * 0.2f, 2f) * keepHorizontalSensitivity,
                                      -maxMassOffset, maxMassOffset);
                _com.centerOfMassOffset = new Vector3(_initialCom.x, _initialCom.y, _initialCom.z + _zOffset);
            }
        }
    }
}