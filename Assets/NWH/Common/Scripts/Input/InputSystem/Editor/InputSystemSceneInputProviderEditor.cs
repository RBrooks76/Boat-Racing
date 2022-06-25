#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;

namespace NWH.Common.Input
{
    [CustomEditor(typeof(InputSystemSceneInputProvider))]
    public class InputSystemSceneInputProviderEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Info("Input settings for Unity's new input system can be changed by modifying 'SceneInputActions' " +
                        "file (double click on it to open).");

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
