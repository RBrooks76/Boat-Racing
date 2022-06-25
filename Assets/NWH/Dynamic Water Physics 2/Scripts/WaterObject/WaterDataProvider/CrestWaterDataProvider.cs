#if DWP_CREST

using System;
using System.Collections.Generic;
using UnityEngine;
using Crest;
using NWH.DWP2.WaterObjects;
using Random = UnityEngine.Random;

namespace NWH.DWP2.WaterData
{
    public class CrestWaterDataProvider : WaterDataProvider
    {
        private class CrestWOData
        {
            public int prevArraySize;
            public Vector3[] normals;
            public int hash;
        }
        
        private OceanRenderer _oceanRenderer;
        private ICollProvider _collProvider;
        private IFlowProvider _flowProvider;
        private Dictionary<int, CrestWOData> _woDataDictionary = new Dictionary<int, CrestWOData>();
        private CrestWOData _woDataTmp;

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
            return true;
        }
        
        public override void Awake()
        {
            base.Awake();
            
            _oceanRenderer = GetComponent<OceanRenderer>();
            if (_oceanRenderer == null)
            {
                Debug.LogError($"{typeof(OceanRenderer)} not found. " +
                               $"{GetType()} needs to be attached to an object containing {typeof(OceanRenderer)}.");
            }

            _woDataDictionary = new Dictionary<int, CrestWOData>();
        }

        public override void GetWaterHeights(WaterObject waterObject, ref Vector3[] points, ref float[] waterHeights)
        {
            if (!_woDataDictionary.ContainsKey(waterObject.instanceID))
            {
                CrestWOData woData = new CrestWOData();
                woData.hash = -1;
                woData.prevArraySize = -1;
                _woDataDictionary.Add(waterObject.instanceID, woData);
            }
            
            int n = points.Length;

            _collProvider = _oceanRenderer.CollisionProvider;
            _flowProvider = _oceanRenderer.FlowProvider;
            
            // Resize hash array if data size changed

            _woDataTmp = _woDataDictionary[waterObject.instanceID];
            if (n != _woDataTmp.prevArraySize)
            {
                _woDataTmp.normals = new Vector3[n];
                _woDataTmp.prevArraySize = n;
            }

            _collProvider.Query(waterObject.instanceID, 0, points, waterHeights, _woDataTmp.normals, null);

            _woDataTmp.prevArraySize = n;
        }

        public override void GetWaterNormals(WaterObject waterObject, ref Vector3[] points, ref Vector3[] waterNormals)
        {
            waterNormals = _woDataTmp.normals; // Already queried in GetWaterHeights
        }

        public override void GetWaterFlows(WaterObject waterObject, ref Vector3[] points, ref Vector3[] waterFlows)
        {
            _flowProvider.Query(_woDataTmp.hash, 0, points, waterFlows);
        }

        public override float GetWaterHeightSingle(WaterObject waterObject, Vector3 point)
        {
            _singlePointArray[0] = point;
            _oceanRenderer.CollisionProvider.Query(_oceanRenderer.GetHashCode(), 0, 
                _singlePointArray, _singleHeightArray,null, null);
            return _singleHeightArray[0];
        }
    }
}

#endif