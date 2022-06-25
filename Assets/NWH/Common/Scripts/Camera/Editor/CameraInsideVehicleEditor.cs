#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;

namespace NWH.Common.Cameras
{
    [CustomEditor(typeof(CameraInsideVehicle))]
    [CanEditMultipleObjects]
    public class CameraInsideVehicleEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            
            drawer.Field("isInsideVehicle");

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