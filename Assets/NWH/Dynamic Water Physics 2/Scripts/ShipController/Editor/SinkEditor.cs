#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;

namespace NWH.DWP2.WaterObjects
{
    [CustomEditor(typeof(Sink))]
    [CanEditMultipleObjects]
    public class SinkEditor : DWP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("floodedCenterOfMass");
            drawer.Field("centerOfMassDriftPercent");
            drawer.Field("addedMassPercentPerSecond");
            drawer.Field("maxMassPercent");
            drawer.Field("sink");

            drawer.EndEditor(this);
            return true;
        }
    }
}

#endif
