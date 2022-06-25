using UnityEngine;

namespace NWH.Common.Input
{
    public class InputManagerSceneInputProvider : SceneInputProviderBase
    {
        public override bool ChangeCamera()
        {
            return InputUtils.TryGetButtonDown("ChangeCamera", KeyCode.C);
        }


        public override Vector2 CameraRotation()
        {
            return new Vector2(InputUtils.TryGetAxis("CameraRotationX"), InputUtils.TryGetAxis("CameraRotationY"));
        }


        public override Vector2 CameraPanning()
        {
            return new Vector2(InputUtils.TryGetAxis("CameraPanningX"), InputUtils.TryGetAxis("CameraPanningY"));
        }


        public override bool CameraRotationModifier()
        {
            return InputUtils.TryGetButton("CameraRotationModifier", KeyCode.Mouse0) || !requireCameraRotationModifier;
        }


        public override bool CameraPanningModifier()
        {
            return InputUtils.TryGetButton("CameraPanningModifier", KeyCode.Mouse1) || !requireCameraPanningModifier;
        }


        public override float CameraZoom()
        {
            return InputUtils.TryGetAxis("CameraZoom");
        }


        public override bool ChangeVehicle()
        {
            return InputUtils.TryGetButtonDown("ChangeVehicle", KeyCode.V);
        }


        public override Vector2 CharacterMovement()
        {
            return new Vector2(InputUtils.TryGetAxis("FPSMovementX"), InputUtils.TryGetAxis("FPSMovementY"));
        }


        public override bool ToggleGUI()
        {
            return InputUtils.TryGetButtonDown("ToggleGUI", KeyCode.Tab);
        }
    }
}