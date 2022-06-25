#if DWP_OCEAN_NEXT_GEN

using NWH.DWP2.WaterObjects;
using UnityEngine;

namespace NWH.DWP2.WaterData
{
    public class OceanNextGenWaterDataProvider : WaterDataProvider
    {
        private Ocean _ocean;
        private Vector3 _point;

        public override void Awake()
        {
            base.Awake();

            _ocean = gameObject.GetComponent<Ocean>();
            if (_ocean == null)
            {
                Debug.LogError($"{typeof(Ocean)} not found. " +
                               $"{GetType()} needs to be attached to an object containing {typeof(Ocean)}.");
            }

        }
        
        public override void GetWaterHeights(WaterObject waterObject, ref Vector3[] points, ref float[] waterHeights)
        {
            if (_ocean.canCheckBuoyancyNow[0] != 1) return;

            float choppyOffset = 0;
            for (int i = 0; i < points.Length; i++)
            {
                _point = points[i];
                if (_ocean.choppy_scale > 0)
                {
                    choppyOffset = _ocean.GetChoppyAtLocation2(_point.x, _point.z);
                }

                waterHeights[i] = _ocean.GetWaterHeightAtLocation2(_point.x - choppyOffset, _point.z);
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