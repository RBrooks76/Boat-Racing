using System;
using NWH.DWP2.WaterObjects;
using UnityEngine;

namespace NWH.DWP2.WaterData
{
    /// <summary>
    ///     Class for providing water height and velocity data to WaterObjectManager.
    /// </summary>
    public abstract class WaterDataProvider : MonoBehaviour
    {
        protected Vector3[] _singlePointArray;
        protected float[]   _singleHeightArray;
        protected Collider _triggerCollider;
        
        
        private void OnTriggerEnter(Collider other)
        {
            // Assign the current water data provider
            Rigidbody targetRigidbody = other.attachedRigidbody;
            if (targetRigidbody != null)
            {
                WaterObject[] targetWaterObjects = targetRigidbody.GetComponentsInChildren<WaterObject>();
                for (int i = 0; i < targetWaterObjects.Length; i++)
                {
                    targetWaterObjects[i].OnEnterWaterDataProvider(this);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // Assign the current water data provider
            Rigidbody targetRigidbody = other.attachedRigidbody;
            if (targetRigidbody != null)
            {
                WaterObject[] targetWaterObjects = targetRigidbody.GetComponentsInChildren<WaterObject>();
                for (int i = 0; i < targetWaterObjects.Length; i++)
                {
                    targetWaterObjects[i].OnExitWaterDataProvider(this);
                }
            }
        }
        
        public virtual void Awake()
        {
            _singleHeightArray = new float[1];
            _singlePointArray  = new Vector3[1];

            _triggerCollider = GetComponent<Collider>();
            if (_triggerCollider == null)
            {
                // Debug.LogWarning("WaterDataProvider requires a Collider with 'Is Trigger' ticked to be present " +
                //                  "on the same GameObject to act as a trigger volume. Creating one.");
                _triggerCollider = gameObject.AddComponent<SphereCollider>();
                _triggerCollider.isTrigger = true;
                ((SphereCollider) _triggerCollider).radius = 1000000f;
            }

            if (!_triggerCollider.isTrigger)
            {
                Debug.LogWarning("WaterDataProvider Collider has to have 'Is Trigger' ticked. Fixing.");
                _triggerCollider.isTrigger = true;
            }
        }
        
        /// <summary>
        ///     Does this water system support water height queries?
        /// </summary>
        /// <returns>True if it does, false if it does not.</returns>
        public abstract bool SupportsWaterHeightQueries();


        /// <summary>
        ///     Does this water system support water normal queries?
        /// </summary>
        /// <returns>True if it does, false if it does not.</returns>
        public abstract bool SupportsWaterNormalQueries();


        /// <summary>
        ///     Does this water system support water velocity queries?
        /// </summary>
        /// <returns>True if it does, false if it does not.</returns>
        public abstract bool SupportsWaterFlowQueries();


        /// <summary>
        ///     Returns water height at each given point.
        /// </summary>
        /// <param name="points">Position array in world coordinates.</param>
        /// <param name="waterHeights">Water height array in world coordinates. Corresponds to positions.</param>
        public virtual void GetWaterHeights(WaterObject waterObject, ref Vector3[] points, ref float[] waterHeights)
        {
            // Do nothing. This will use the initial values of water heights (0).
        }


        /// <summary>
        ///     Returns water flow at each given point.
        ///     Water flow should be in world coordinates and relative to the world, not the WaterObject itself.
        ///     WaterObject velocity and angularVelocity are both accounted for inside WaterTriangleJob.
        /// </summary>
        /// <param name="points">Position array in world coordinates.</param>
        /// <param name="waterFlows">Water flow array in world coordinates. Corresponds to positions.</param>
        public virtual void GetWaterFlows(WaterObject waterObject, ref Vector3[] points, ref Vector3[] waterFlows)
        {
            // Do nothing. This will use the initial values of water velocities (0,0,0).
        }


        /// <summary>
        ///     Returns water normals at each given point.
        /// </summary>
        /// <param name="points">Position array in world coordinates.</param>
        /// <param name="waterNormals">Water normal array in world coordinates. Corresponds to positions.</param>
        public virtual void GetWaterNormals(WaterObject waterObject, ref Vector3[] points, ref Vector3[] waterNormals)
        {
            // Do nothing. This will use the initial values of water normals (0,0,0).
        }


        public void GetWaterHeightsFlowsNormals(WaterObject waterObject, ref Vector3[] points,     ref float[]   waterHeights,
            ref Vector3[] waterFlows, ref Vector3[] waterNormals, bool useWaterHeight, bool useWaterNormals, bool useWaterFlow)
        {
            if (useWaterHeight)
            {
                GetWaterHeights(waterObject, ref points, ref waterHeights);
            }
            
            if (useWaterFlow && SupportsWaterFlowQueries())
            {
                GetWaterFlows(waterObject, ref points, ref waterFlows);
            }

            if (useWaterNormals && SupportsWaterNormalQueries())
            {
                GetWaterNormals(waterObject, ref points, ref waterNormals);
            }
        }


        public virtual float GetWaterHeightSingle(WaterObject waterObject, Vector3 point)
        {
            _singlePointArray[0] = point;
            GetWaterHeights(waterObject, ref _singlePointArray, ref _singleHeightArray);
            return _singleHeightArray[0];
        }
        
        /// <summary>
        ///     Returns true if point is in water. Works with wavy water too.
        /// </summary>
        public bool PointInWater(WaterObject waterObject, Vector3 worldPoint)
        {
            return GetWaterHeight(waterObject, worldPoint) > worldPoint.y;
        }


        public float GetWaterHeight(WaterObject waterObject, Vector3 worldPoint)
        {
            return GetWaterHeightSingle(waterObject, worldPoint);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.1f);
        }
    }
}