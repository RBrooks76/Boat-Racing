using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    [CreateAssetMenu(fileName = "WaterObjectMaterial", menuName = "Dynamic Water Physics 2/Water Object Material",
                     order    = 0)]
    public class WaterObjectMaterial : ScriptableObject
    {
        public float density = 600;
    }
}