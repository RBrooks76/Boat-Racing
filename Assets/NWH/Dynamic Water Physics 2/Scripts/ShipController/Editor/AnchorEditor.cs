#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;

namespace NWH.DWP2.WaterObjects
{
    [CustomEditor(typeof(Anchor))]
    [CanEditMultipleObjects]
    public class AnchorEditor : DWP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("dropOnStart");
            drawer.Field("forceCoefficient");
            drawer.Field("zeroForceRadius");
            drawer.Field("dragForce");
            drawer.Field("localAnchorPoint");

            drawer.EndEditor(this);
            return true;
        }
    }
}

#endif
