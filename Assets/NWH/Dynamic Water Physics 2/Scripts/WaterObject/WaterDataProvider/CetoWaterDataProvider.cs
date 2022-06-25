#if DWP_CETO

using NWH.DWP2.WaterObjects;
using UnityEngine;

namespace NWH.DWP2.WaterData
{
    public class CetoWaterDataProvider : WaterDataProvider
    {
        private Ceto.Ocean _ocean;
        private Vector3 _point;

        public override void Awake()
        {
            base.Awake();
            _ocean = GetComponent<Ceto.Ocean>();
            if (_ocean == null)
            {
                Debug.LogError($"{typeof(Ceto.Ocean)} not found. " +
                               $"{GetType()} needs to be attached to an object containing {typeof(Ceto.Ocean)}.");
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
                _point = points[i];
                waterHeights[i] = _ocean.QueryWaves(_point.x, _point.z);
            }
        }

        // Works but overly expensive for general usage.
        /*
        public override void GetWaterNormals(ref Vector3[] points, ref Vector3[] waterNormals)
        {
            Vector3 bOffset = Vector3.forward;
            Vector3 cOffset = Vector3.right;
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 a = points[i];
                Vector3 b = a + bOffset;
                Vector3 c = a + cOffset;
                a.y = _ocean.QueryWaves(a.x, a.z);
                b.y = _ocean.QueryWaves(b.x, b.z);
                c.y = _ocean.QueryWaves(c.x, c.z);
                waterNormals[i] = Vector3.Cross(b - a, c - a).normalized;
            }
        }
        */
    }
}

#endif
