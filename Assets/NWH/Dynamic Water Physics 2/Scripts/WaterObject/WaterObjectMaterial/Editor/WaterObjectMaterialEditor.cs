#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.WaterObjects;
using UnityEditor;

namespace NWH.DWP2
{
    [CustomEditor(typeof(WaterObjectMaterial))]
    [CanEditMultipleObjects]
    public class WaterObjectMaterialEditor : DWP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("density");

            drawer.EndEditor(this);
            return true;
        }
    }
}

#endif
