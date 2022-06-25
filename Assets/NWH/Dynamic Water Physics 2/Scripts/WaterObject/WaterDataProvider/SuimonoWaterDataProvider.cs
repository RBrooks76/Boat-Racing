#if DWP_SUIMONO

using NWH.DWP2.WaterObjects;
using Suimono.Core;
using UnityEngine;

namespace NWH.DWP2.WaterData
{
    [RequireComponent(typeof(SuimonoModule))]
    public class SuimonoWaterDataProvider : WaterDataProvider
    {
        private SuimonoModule _suimono;

        public override void Awake()
        {
            base.Awake();

            _suimono = gameObject.GetComponent<SuimonoModule>();
            if (_suimono == null)
            {
                Debug.LogError($"{typeof(SuimonoModule)} not found. " +
                               $"{GetType()} needs to be attached to an object containing {typeof(SuimonoModule)}.");
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

        public override void GetWaterHeights(WaterObject waterObject, ref Vector3[] points, ref float[] waterHeights)
        {
            for (int i = 0; i < points.Length; i++)
            {
                waterHeights[i] = _suimono.SuimonoGetHeight(points[i], "height");
            }
        }
    }
}

#endif

