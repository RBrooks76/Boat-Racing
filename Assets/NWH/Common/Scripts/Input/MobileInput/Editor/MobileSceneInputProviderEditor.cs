#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;

namespace NWH.Common.Input
{
    /// <summary>
    ///     Editor for MobileInputProvider.
    /// </summary>
    [CustomEditor(typeof(MobileSceneInputProvider))]
    public class MobileSceneInputProviderEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.BeginSubsection("Scene Buttons");
            drawer.Field("changeVehicleButton");
            drawer.Field("changeCameraButton");
            drawer.EndSubsection();

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
