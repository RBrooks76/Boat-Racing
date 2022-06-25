#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;

namespace NWH.Common.Input
{
    [CustomEditor(typeof(InputManagerSceneInputProvider))]
    public class InputManagerSceneInputProviderEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("requireCameraRotationModifier");
            drawer.Field("requireCameraPanningModifier");

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


