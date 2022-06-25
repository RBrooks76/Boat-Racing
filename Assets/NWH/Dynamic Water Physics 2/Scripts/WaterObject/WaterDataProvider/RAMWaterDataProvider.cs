#if DWP_RAM

using NWH.Common.Utility;
using NWH.DWP2.WaterObjects;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace NWH.DWP2.WaterData
{
    /// <summary>
    /// Version of RaycastWaterDataProvider with support for R.A.M. flow data.
    /// </summary>
    public class RAMWaterDataProvider : RaycastWaterDataProvider
    {
        public bool riverFlow = true;
        public bool lakeFlow = false;
        
        private RamSpline          _ramSpline;
        private LakePolygon        _ramPolygon;
        private LakePolygon        _prevRamPolygon;
        private RamSpline          _prevRamSpline;

        public override void Awake()
        {
            base.Awake();
            Physics.IgnoreLayerCollision(waterLayer, objectLayer);
            _rayDirection = -Vector3.up;
            _rayStartOffset = -_rayDirection * raycastDistance * 0.5f;
            _prevDataSize = -1;
        }

        public override bool SupportsWaterFlowQueries()
        {
            return true;
        }

        public override void GetWaterFlows(WaterObject waterObject, ref Vector3[] points, ref Vector3[] waterFlows)
        {
            _flow = Vector3.zero;
            
            bool queriesHitBackfaces = Physics.queriesHitBackfaces;
            Physics.queriesHitBackfaces = false;
            
            _ray.origin = waterObject.transform.position + _rayStartOffset;
            _ray.direction = _rayDirection;
            if (Physics.Raycast(_ray, out _hit, raycastDistance, _layerMask, QueryTriggerInteraction.Ignore) && _hit.collider != null)
            {
                _ramSpline = _hit.collider.GetComponent<RamSpline>();
                if (riverFlow && _ramSpline != null)
                {
                    if (_ramSpline != _prevRamSpline)
                    {
                        _mesh = _ramSpline.meshfilter.sharedMesh;
                        _vertIndex = _mesh.triangles[_hit.triangleIndex * 3];
                        _vertDir = _ramSpline.verticeDirection[_vertIndex];
                        _uv4 = _mesh.uv4[_vertIndex];
                        _tmp.x = _vertDir.z;
                        _tmp.y = _vertDir.y;
                        _tmp.z = -_vertDir.x;
                        _vertDir = _vertDir * _uv4.y - _tmp * _uv4.x;
                
                        _flow.x = _vertDir.x * _ramSpline.floatSpeed;
                        _flow.y = 0;
                        _flow.z = _vertDir.z * _ramSpline.floatSpeed;
                    }
                }
                else if (lakeFlow)
                {
                    _ramPolygon = _hit.collider.GetComponent<LakePolygon>();
                    if (_ramPolygon != null)
                    {
                        if (_ramPolygon != _prevRamPolygon)
                        {
                            _mesh = _ramPolygon.meshfilter.sharedMesh;
                            _vertIndex = _mesh.triangles[_hit.triangleIndex * 3];
                            _uv4 = -_mesh.uv4[_vertIndex];
                            _vertDir.x = _uv4.x;
                            _vertDir.y = 0;
                            _vertDir.z = _uv4.y;
                    
                            _flow.x = _vertDir.x * _ramPolygon.floatSpeed;
                            _flow.y = 0;
                            _flow.z = _vertDir.z * _ramPolygon.floatSpeed;
                        }
                    }
                }

                _prevRamPolygon = _ramPolygon;
                _prevRamSpline = _ramSpline;
                
                for (int d = 0; d < waterFlows.Length; d++)
                {
                    waterFlows[d] = _flow;
                }
            }

            _prevRamPolygon = null;
            _prevRamSpline = null;
            
            Physics.queriesHitBackfaces = queriesHitBackfaces;
        }
    }
}

#endif