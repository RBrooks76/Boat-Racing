#if DWP_KWS

using System;
using KWS;
using NWH.DWP2.WaterObjects;
using UnityEngine;


namespace NWH.DWP2.WaterData
{
    [DefaultExecutionOrder(-50)]
    public class KWSWaterDataProvider : WaterDataProvider
    {
        private WaterSystem _water;
        private Vector3     _point;


        private void OnEnable()
        {
            //Following is a workaround and should not be required in future KWS versions
            Rigidbody waterRigidbody = gameObject.AddComponent<Rigidbody>();
            waterRigidbody.isKinematic = true;
            
            
            SphereCollider sphereCollider = gameObject.GetComponent<SphereCollider>();
            if (sphereCollider == null)
            {
                sphereCollider = gameObject.AddComponent<SphereCollider>();
                sphereCollider.isTrigger = true;
                sphereCollider.radius = 100000f;
            }

            KW_Buoyancy kwBuoyancy = gameObject.AddComponent<KW_Buoyancy>();
            kwBuoyancy.WaterInstance = GetComponent<WaterSystem>();
            kwBuoyancy.VolumeSource = KW_Buoyancy.ModelSourceEnum.Collider;
            kwBuoyancy.enabled = false;
            kwBuoyancy.enabled = true;
        }


        public override void Awake()
        {
            base.Awake();
            _water = GetComponent<WaterSystem>();
            if (_water == null)
            {
                Debug.LogError($"{typeof(WaterSystem)} not found. " +
                               $"{GetType()} needs to be attached to an object containing {typeof(WaterSystem)}.");
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
                Vector3 localQueryPoint = _water.transform.TransformPoint(_point);
                KW_WaterSurfaceData waterSurfaceData = _water.GetWaterSurfaceData(localQueryPoint);
                if (waterSurfaceData.IsActualDataReady)
                {
                    waterHeights[i] = waterSurfaceData.Position.y;
                }
            }
        }
    }
}

#endif