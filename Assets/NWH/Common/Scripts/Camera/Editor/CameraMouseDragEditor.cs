#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;

namespace NWH.Common.Cameras
{
    [CustomEditor(typeof(CameraMouseDrag))]
    [CanEditMultipleObjects]
    public class CameraMouseDragEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            CameraMouseDrag.POVType povType = ((CameraMouseDrag) target).povType;
            
            drawer.Field("target");
            
            drawer.BeginSubsection("POV");
            drawer.Field("povType");
            drawer.EndSubsection();

            if (povType == CameraMouseDrag.POVType.ThirdPerson)
            {
                drawer.BeginSubsection("Distance & Position");
                drawer.Field("distance");
                drawer.Field("minDistance");
                drawer.Field("maxDistance");
                drawer.Field("zoomSensitivity");
                drawer.Field("targetPositionOffset");
                drawer.EndSubsection();
            }

            drawer.BeginSubsection("Rotation");
            drawer.Field("allowRotation");
            drawer.Field("followTargetPitchAndYaw");
            drawer.Field("followTargetRoll");
            drawer.Field("rotationSensitivity");
            drawer.Field("verticalMaxAngle");
            drawer.Field("verticalMinAngle");
            drawer.Field("initXRotation");
            drawer.Field("initYRotation");
            drawer.Field("rotationSmoothing");
            drawer.EndSubsection();

            drawer.BeginSubsection("Panning");
            if (drawer.Field("allowPanning").boolValue)
            {
                drawer.Field("panningSensitivity");
            }
            drawer.EndSubsection();
            
            drawer.BeginSubsection("Camera Shake");
            drawer.Info("Movement introduced as a result of acceleration.");
            if (drawer.Field("useShake").boolValue)
            {
                drawer.Field("shakeMaxOffset");
                drawer.Field("shakeIntensity");
                drawer.Field("shakeSmoothing");
                drawer.Field("shakeAxisIntensity");
            }
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
