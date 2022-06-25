#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;

namespace NWH.DWP2.WaterObjects
{
    [CustomEditor(typeof(InputManagerShipInputProvider))]
    public class InputManagerShipInputProviderEditor : DWP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.EndEditor(this);
            return true;
        }


        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}

#endif
