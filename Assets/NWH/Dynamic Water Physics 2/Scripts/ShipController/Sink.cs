using NWH.Common.CoM;
using UnityEngine;

namespace NWH.DWP2.ShipController
{
    [RequireComponent(typeof(VariableCenterOfMass))]
    public class Sink : MonoBehaviour
    {
        [Tooltip("Ingress point in local coordinates. Indicated by a red sphere gizmo.")]
        public Vector3 floodedCenterOfMass = Vector3.zero;

        [Tooltip(
            "How much should center of mass drift due to water ingress. 0 = none, 1 = center of mass will move to ingress point, 0-1 = in-between.")]
        [Range(0, 1)]
        public float centerOfMassDriftPercent = 0.4f;

        [Tooltip("Percentage of initial mass that will be added each second to imitate water ingress")]
        public float addedMassPercentPerSecond = 0.1f;

        [Tooltip("Maximum added mass after water ingress. 1f = 100% of orginal mass, 2f = 200% of original mass, etc.")]
        public float maxMassPercent = 3f;

        [Tooltip("Should the ship sink? Call Begin() to initialize sinking from script.")]
        [SerializeField] private bool sink;

        private float     _initialMass;
        private Vector3   _initialCoMOffset;
        private VariableCenterOfMass _variableCenterOfMass;

        ///
        public Vector3 FloodedCenterOfMass
        {
            get { return transform.TransformPoint(floodedCenterOfMass); }
            set { floodedCenterOfMass = transform.InverseTransformPoint(value); }
        }


        private void Start()
        {
            _variableCenterOfMass = GetComponent<VariableCenterOfMass>();
            _initialCoMOffset = _variableCenterOfMass.centerOfMass;
            _initialMass     = _variableCenterOfMass.baseMass;
        }


        private void FixedUpdate()
        {
            if (sink)
            {
                _variableCenterOfMass.baseMass += _initialMass * addedMassPercentPerSecond * Time.fixedDeltaTime;
                _variableCenterOfMass.centerOfMassOffset = Mathf.Clamp01((_variableCenterOfMass.baseMass - _initialMass) / (maxMassPercent * _initialMass)) *
                                                           centerOfMassDriftPercent * floodedCenterOfMass;
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(FloodedCenterOfMass, 0.2f);
        }


        public void Begin()
        {
            sink = true;
        }


        public void Stop()
        {
            sink = false;
        }


        public void Reset()
        {
            Stop();
            _variableCenterOfMass.baseMass         = _initialMass;
            _variableCenterOfMass.centerOfMassOffset = _initialCoMOffset;
        }
    }
}