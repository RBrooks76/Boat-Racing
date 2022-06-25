#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;

namespace NWH.DWP2.WaterObjects
{
    [CustomEditor(typeof(InputSystemShipInputProvider))]
    public class InputSystemShipInputProviderEditor : DWP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Info("Input settings for Unity's new input system can be changed by modifying 'ShipInputActions' " +
                        "file (double click on it to open).");

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
