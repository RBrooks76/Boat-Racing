using NWH.DWP2.WaterObjects;
using UnityEngine;

namespace NWH.DWP2.DemoContent
{
    /// <summary>
    ///     An example on how to add WaterObject to an existing object at runtime.
    /// </summary>
    public class WaterObjectFromScript : MonoBehaviour
    {
        private void Start()
        {
            WaterObject waterObject = gameObject.AddComponent<WaterObject>();
            waterObject.convexifyMesh       = true;
            waterObject.simplifyMesh        = true;
            waterObject.targetTriangleCount = 64;
            waterObject.GenerateSimMesh();

            MassFromVolume massFromVolume = gameObject.AddComponent<MassFromVolume>();
            massFromVolume.SetDefaultAsMaterial(); // Use massFromVolume.SetMaterial() to use a different material
            // instead of the default one.

            // Important. Without running Synchronize() WaterObject will not be registered by the WaterObjectManager and 
            // the physics will not work. Just note that running Synchronize() can be 
        }
    }
}