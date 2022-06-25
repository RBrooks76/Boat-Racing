using NWH.Common.Utility;
using NWH.DWP2.WaterObjects;
using UnityEngine;

namespace NWH.DWP2.WaterData
{
    public class FlatWaterDataProvider : WaterDataProvider
    {
        public override bool SupportsWaterHeightQueries()
        {
            return false;
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
            float waterHeight = transform.position.y;

            waterHeights.Fill(waterHeight);
        }
    }
}