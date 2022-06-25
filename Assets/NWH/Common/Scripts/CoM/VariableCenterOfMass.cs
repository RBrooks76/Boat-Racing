using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using NWH.Common.Utility;

namespace NWH.Common.CoM
{
    /// <summary>
    /// Script used for adjusting Rigidbody properties at runtime based on
    /// attached IMassAffectors. This allows for vehicle center of mass and inertia changes
    /// as the fuel is depleted, cargo is added, etc. without the need of physically parenting Rigidbodies to the object.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class VariableCenterOfMass : MonoBehaviour, IMassAffector
    {
        /// <summary>
        /// Base mass of the object, without IMassAffectors.
        /// </summary>
        [UnityEngine.Tooltip("Base mass of the object, without IMassAffectors.")]
        public float baseMass = 1000f;

        /// <summary>
        /// Total mass of the object with masses of IMassAffectors counted in.
        /// </summary>
        [UnityEngine.Tooltip("Total mass of the object with masses of IMassAffectors counted in.")]
        public float totalMass = 1000f;
        
        /// <summary>
        /// Object dimensions in [m]. X - width, Y - height, Z - length.
        /// It is important to set the correct dimensions or otherwise inertia might be calculated incorrectly.
        /// </summary>
        [UnityEngine.Tooltip("Object dimensions in [m]. X - width, Y - height, Z - length.\r\nIt is important to set the correct dimensions or otherwise inertia might be calculated incorrectly.")]
        public Vector3 dimensions = new Vector3(1.5f, 1.5f, 4.6f);

        /// <summary>
        /// When enabled the Unity-calculated center of mass will be used.
        /// </summary>
        [Tooltip(
            "When enabled the Unity-calculated center of mass will be used.")]
        public bool useDefaultCenterOfMass = false;
        
        /// <summary>
        /// Center of mass of the object. Auto calculated. To adjust center of mass use centerOfMassOffset.
        /// </summary>
        [Tooltip(
            "Center of mass of the rigidbody. Needs to be readjusted when new colliders are added.")]
        public Vector3 centerOfMass = Vector3.zero;

        /// <summary>
        /// Used to adjust actual center of mass location in reference to the auto-calculated center of mass.
        /// </summary>
        [UnityEngine.Tooltip("Used to adjust actual center of mass location in reference to the auto-calculated center of mass.")]
        public Vector3 centerOfMassOffset = Vector3.zero;

        /// <summary>
        /// When true inertia settings will be ignored and default Rigidbody inertia tensor will be used.
        /// </summary>
        [UnityEngine.Tooltip("When true inertia settings will be ignored and default Rigidbody inertia tensor will be used.")]
        public bool useDefaultInertia = true;
        
        /// <summary>
        ///     Vector by which the inertia tensor of the rigidbody will be scaled on Start().
        ///     Due to the uniform density of the rigidbodies, versus the very non-uniform density of a vehicle, inertia can feel
        ///     off.
        ///     Use this to adjust inertia tensor values.
        /// </summary>
        [Tooltip(
            "    Vector by which the inertia tensor of the rigidbody will be scaled on Start().\r\n    Due to the unform density of the rigidbodies, versus the very non-uniform density of a vehicle, inertia can feel\r\n    off.\r\n    Use this to adjust inertia tensor values.")]
        public Vector3 inertiaTensor = new Vector3(170f, 1640f, 1350f);

        /// <summary>
        /// Used to adjust result given by automatic inertia calculation.
        /// </summary>
        [UnityEngine.Tooltip("Used to adjust result given by automatic inertia calculation.")]
        public Vector3 inertiaScale = new Vector3(1f, 1f, 1f);

        /// <summary>
        /// Update interval in seconds.
        /// On each update center of mass and inertia tensor will be updated based on values from IMassAffectors.
        /// </summary>
        [UnityEngine.Tooltip("Update interval in seconds.\r\nOn each update center of mass and inertia tensor will be updated based on values from IMassAffectors.")]
        public float updateInterval = 1f;

        /// <summary>
        /// Objects attached or part of the vehicle affecting its center of mass and inertia.
        /// </summary>
        [NonSerialized] public IMassAffector[] affectors;

        private Rigidbody _rigidbody;
        private float _timer = 999f;
        private Vector3 _initialRigidbodyLocalCoM;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            affectors = GetMassAffectors();
            
            _rigidbody.ResetCenterOfMass();
            _initialRigidbodyLocalCoM = _rigidbody.centerOfMass;
            
            UpdateMass();
            UpdateCoM();
            UpdateInertia();
        }

        private void OnValidate()
        {
            _rigidbody = GetComponent<Rigidbody>();
            affectors = GetMassAffectors();
        }

        private void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;
            
            if (_timer > updateInterval && !_rigidbody.isKinematic)
            {
                UpdateAllProperties();
                _timer = 0;
            }
        }

        public void UpdateAllProperties()
        {
            UpdateMass();
            UpdateCoM();
            UpdateInertia();
        }

        public void UpdateMass()
        {
            totalMass = CalculateMass();
            _rigidbody.mass = totalMass;
        }

        /// <summary>
        /// Calculates and applies the CoM to the Rigidbody.
        /// </summary>
        public void UpdateCoM()
        {
            centerOfMass = CalculateCenterOfMass();
            _rigidbody.centerOfMass = centerOfMass;
        }
        
        /// <summary>
        /// Calculates and applies the inertia tensor to the Rigidbody.
        /// </summary>
        public void UpdateInertia()
        {
            inertiaTensor = CalculateInertiaTensor(dimensions);
            
            // Inertia tensor of constrained rigidbody will be 0 which causes errors when trying to set.
            if (!useDefaultInertia && inertiaTensor.x > 0 && inertiaTensor.y > 0 && inertiaTensor.z > 0)
            {
                _rigidbody.inertiaTensor = inertiaTensor;
                _rigidbody.inertiaTensorRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Updates list of IMassAffectors attached to this object.
        /// Call after IMassAffector has been added or removed from the object.
        /// </summary>
        public IMassAffector[] GetMassAffectors()
        {
            return GetComponentsInChildren<IMassAffector>(true);
        }
        
        /// <summary>
        /// Calculates the mass of the Rigidbody and attached mass affectors.
        /// </summary>
        public float CalculateMass()
        {
            float   massSum = 0;
            foreach (IMassAffector affector in affectors)
            {
                if (affector.GetTransform().gameObject.activeInHierarchy)
                {
                    massSum += affector.GetMass();
                }
            }
            
            return massSum;
        }

        /// <summary>
        /// Calculates the center of mass of the Rigidbody and attached mass affectors.
        /// </summary>
        public Vector3 CalculateCenterOfMass()
        {
            _rigidbody.ResetCenterOfMass();
            _initialRigidbodyLocalCoM = _rigidbody.centerOfMass;

            Vector3 newCoM  = Vector3.zero;
            float massSum = CalculateMass();
            
            for(int i = 0; i < affectors.Length; i++) 
            {
                IMassAffector affector = affectors[i];
                if (affector.GetTransform().gameObject.activeInHierarchy)
                {
                    float affectorMass = affector.GetMass();
                    newCoM += transform.InverseTransformPoint(affector.GetWorldCenterOfMass()) * (affectorMass / massSum);
                }
            }
            
            return newCoM + centerOfMassOffset;
        }
        
        /// <summary>
        /// Calculates the inertia tensor of the Rigidbody and attached mass affectors.
        /// </summary>
        public Vector3 CalculateInertiaTensor(Vector3 dimensions)
        {
            if (useDefaultInertia)
            {
                _rigidbody.ResetInertiaTensor();
                return _rigidbody.inertiaTensor;
            }
            
            Vector3 inertiaTensor = new Vector3
            (
                (dimensions.y * dimensions.y + dimensions.z * dimensions.z) * 0.5f * totalMass,
                (dimensions.z * dimensions.z + dimensions.x * dimensions.x) * 0.5f * totalMass,
                (dimensions.x * dimensions.x + dimensions.y * dimensions.y) * 0.5f * totalMass
            );

            Vector3 affectorInertiaSum = Vector3.zero;
            for(int i = 1; i < affectors.Length; i++) // Skip first (this)
            {
                IMassAffector affector = affectors[i];
                if (affector.GetTransform().gameObject.activeInHierarchy)
                {
                    float mass = affector.GetMass();
                    Vector3 affectorLocalPos = transform.InverseTransformPoint(affector.GetTransform().position);
                    float x = Vector3.ProjectOnPlane(affectorLocalPos, Vector3.right).magnitude * mass;
                    float y = Vector3.ProjectOnPlane(affectorLocalPos, Vector3.up).magnitude * mass;
                    float z = Vector3.ProjectOnPlane(affectorLocalPos, Vector3.forward).magnitude * mass;
                    affectorInertiaSum.x += x * x;
                    affectorInertiaSum.y += y * y;
                    affectorInertiaSum.z += z * z;
                }
            }

            return Vector3.Scale(inertiaTensor + affectorInertiaSum, inertiaScale * 0.1f);
        }

        private void OnDrawGizmos()
        {
            // CoM
            Gizmos.color = Color.red;
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
            Gizmos.DrawSphere(_rigidbody.worldCenterOfMass, 0.1f);
            
            // Mass Affectors
            Gizmos.color = Color.cyan;

            if (affectors == null) return;
            for(int i = 0; i < affectors.Length; i++)
            {
                try // For some reason still getting MissingReferenceException, even after a null check.
                {
                    Gizmos.DrawSphere(affectors[i].GetTransform().position, 0.05f);
                }
                catch
                {
                }
            }

            // Dimensions
            Transform t = transform;
            Vector3 tPosition = t.position;
            Vector3 fwdOffset   = t.forward * dimensions.z * 0.5f;
            Vector3 rightOffset = t.right * dimensions.x * 0.5f;
            Vector3 upOffset    = t.up * dimensions.y * 0.5f;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(tPosition + fwdOffset, tPosition - fwdOffset);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(tPosition + rightOffset, tPosition - rightOffset);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(tPosition + upOffset, tPosition - upOffset);
        }

        private void Reset()
        {
            _rigidbody = GetComponent<Rigidbody>();
            Bounds bounds = gameObject.FindBoundsIncludeChildren();
            dimensions = new Vector3(bounds.extents.x * 2f, bounds.extents.y * 2f, bounds.extents.z * 2f);
            if (dimensions.x < 0.001f) dimensions.x = 0.001f;
            if (dimensions.y < 0.001f) dimensions.y = 0.001f;
            if (dimensions.z < 0.001f) dimensions.z = 0.001f;
            centerOfMass = _rigidbody.centerOfMass;
            baseMass     = dimensions.x * dimensions.y * dimensions.z * 1.2f;
        }

        public float GetMass()
        {
            return baseMass;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public Vector3 GetWorldCenterOfMass()
        {
            return transform.TransformPoint(_initialRigidbodyLocalCoM);
        }
    }
}