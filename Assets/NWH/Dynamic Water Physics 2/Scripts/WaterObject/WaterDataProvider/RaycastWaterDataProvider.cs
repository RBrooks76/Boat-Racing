using NWH.DWP2.WaterObjects;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace NWH.DWP2.WaterData
{
    /// <summary>
    ///     WaterDataProvider that can be used with any object that has a Collider.
    /// </summary>
    public class RaycastWaterDataProvider : WaterDataProvider
    {
        /// <summary>
        ///     Layer the water is on. Required to be able to disable physics collisions between water and object.
        /// </summary>
        [UnityEngine.Tooltip("    Layer the water is on. Required to be able to disable physics collisions between water and object.")]
        public int waterLayer = 4;

        /// <summary>
        ///     Layer the water object(s) are on. Required to be able to disable physics collisions between water and object.
        /// </summary>
        [UnityEngine.Tooltip("    Layer the water object(s) are on. Required to be able to disable physics collisions between water and object.")]
        public int objectLayer = 12;

        /// <summary>
        ///     Raycasts will start at this distance above the point and extend this distance below the point. This means
        ///     that if the water surface is raycastDistance below or above the point, it will not be detected.
        ///     Using lower value will slightly improve performance of Raycasts.
        /// </summary>
        [UnityEngine.Tooltip("    Raycasts will start at this distance above the point and extend this distance below the point. This means\r\n    that if the water surface is raycastDistance below or above the point, it will not be detected.\r\n    Using lower value will slightly improve performance of Raycasts.")]
        public float raycastDistance = 100f;

        /// <summary>
        ///     Minimum number of RaycastCommands per job.
        /// </summary>
        [UnityEngine.Tooltip("    Minimum number of RaycastCommands per job.")]
        public int commandsPerJob = 16;

        protected Vector3[]          _normals;
        protected Vector3            _flow;
        protected LayerMask          _layerMask;
        protected int                _prevDataSize;
        protected Vector3            _rayDirection;
        protected Vector3            _rayStartOffset;
        protected Ray                _ray;
        protected RaycastHit         _hit;
        protected Mesh               _mesh;
        protected Vector2            _uv4;
        protected int                _vertIndex;
        protected Vector3            _vertDir;
        protected Vector3            _tmp;

        protected NativeArray<RaycastCommand> _raycastCommands;
        protected NativeArray<RaycastHit>     _raycastHits;
        protected RaycastCommand              _tmpCommand;
        protected JobHandle                   _raycastJobHandle;
        protected Vector3                     _zeroVector;
        protected Vector3                     _upVector;


        public override void Awake()
        {
            base.Awake();
            Physics.IgnoreLayerCollision(waterLayer, objectLayer);
            _rayDirection   = -Vector3.up;
            _rayStartOffset = -_rayDirection * raycastDistance * 0.5f;
            _prevDataSize   = -1;
            _zeroVector     = Vector3.zero;
            _upVector       = Vector3.up;
        }


        public override bool SupportsWaterHeightQueries()
        {
            return true;
        }


        public override bool SupportsWaterNormalQueries()
        {
            return true;
        }


        public override bool SupportsWaterFlowQueries()
        {
            return false;
        }


        public override void GetWaterHeights(WaterObject waterObject, ref Vector3[] points, ref float[] waterHeights)
        {
            int n = points.Length;

            bool queriesHitBackfaces = Physics.queriesHitBackfaces;
            bool queriesHitTriggers  = Physics.queriesHitTriggers;
            Physics.queriesHitBackfaces = false;
            Physics.queriesHitTriggers  = false;

            if (n != _prevDataSize)
            {
                _normals = new Vector3[n];
                Deallocate();
                _raycastCommands = new NativeArray<RaycastCommand>(n, Allocator.Persistent);
                _raycastHits     = new NativeArray<RaycastHit>(n, Allocator.Persistent);
            }

            _layerMask = 1 << waterLayer;
            for (int i = 0; i < n; i++)
            {
                _tmpCommand.from      = points[i] + _rayStartOffset;
                _tmpCommand.direction = _rayDirection;
                _tmpCommand.distance  = raycastDistance;
                _tmpCommand.maxHits   = 1;
                _tmpCommand.layerMask = _layerMask;
                _raycastCommands[i]   = _tmpCommand;
            }

            _raycastJobHandle = RaycastCommand.ScheduleBatch(_raycastCommands, _raycastHits, 16);
            _raycastJobHandle.Complete();

            Vector3 hitNormal;
            for (int i = 0; i < n; i++)
            {
                hitNormal       = _hit.normal;
                waterHeights[i] = _raycastHits[i].point.y;
                _normals[i]     = hitNormal == _zeroVector ? _upVector : hitNormal;
            }

            Physics.queriesHitBackfaces = queriesHitBackfaces;
            Physics.queriesHitTriggers  = queriesHitTriggers;

            _prevDataSize = n;
        }


        public override void GetWaterNormals(WaterObject waterObject, ref Vector3[] points, ref Vector3[] waterNormals)
        {
            waterNormals = _normals;
        }


        public virtual void OnDisable()
        {
            Deallocate();
        }


        public virtual void OnDestroy()
        {
            Deallocate();
        }


        public virtual void Deallocate()
        {
            _raycastJobHandle.Complete();
            if (_raycastCommands.IsCreated)
            {
                _raycastCommands.Dispose();
            }

            if (_raycastHits.IsCreated)
            {
                _raycastHits.Dispose();
            }
        }
    }
}