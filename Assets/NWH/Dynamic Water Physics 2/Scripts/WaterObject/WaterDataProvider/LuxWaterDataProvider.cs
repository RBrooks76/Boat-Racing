#if DWP_LUX

using LuxWater;
using NWH.DWP2.WaterObjects;
using UnityEngine;

namespace NWH.DWP2.WaterData
{
    public class LuxWaterDataProvider : WaterDataProvider
    {
        public float timeOffset = 0.06f;

        private Material _waterMaterial;
        private LuxWater_WaterVolume _waterObject;
        private float _waterHeightOffset;
        private LuxWaterUtils.GersterWavesDescription _description;

        public override void Awake()
        {
            base.Awake();

            _waterObject = GetComponent<LuxWater_WaterVolume>();
            if (_waterObject == null)
            {
                Debug.LogError($"{typeof(LuxWater_WaterVolume)} not found. " +
                               $"{GetType()} needs to be attached to an object containing {typeof(LuxWater_WaterVolume)}.");
            }
            
            _waterMaterial = _waterObject.GetComponent<MeshRenderer>()?.sharedMaterial;
            if(_waterMaterial == null)
            {
                Debug.LogError("Lux water object does not contain a mesh renderer or material.");
            }
            LuxWaterUtils.GetGersterWavesDescription(ref _description, _waterMaterial);
            _waterHeightOffset = _waterObject.transform.position.y;
        }
        
        public override void GetWaterHeights(WaterObject waterObject, ref Vector3[] points, ref float[] waterHeights)
        {
            // Update wave description
            LuxWaterUtils.GetGersterWavesDescription(ref _description, _waterMaterial);
            for (int i = 0; i < points.Length; i++)
            {
                waterHeights[i] = LuxWaterUtils.GetGestnerDisplacement(points[i], _description, timeOffset).y 
                                  + _waterHeightOffset;
            }
        }

        public override bool SupportsWaterHeightQueries()
        {
            return true;
        }

        public override bool SupportsWaterNormalQueries()
        {
            return false;
        }

        public override bool SupportsWaterFlowQueries()
        {
            return false;
        }
    }
}

#endif

