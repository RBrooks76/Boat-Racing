using UnityEngine;

namespace NWH.Common.Input
{
    /// <summary>
    ///     Class for handling mobile user input via touch screen and sensors.
    /// </summary>
    public class MobileSceneInputProvider : SceneInputProviderBase
    {
        public MobileInputButton changeCameraButton;
        public MobileInputButton changeVehicleButton;


        public override bool ChangeCamera()
        {
            return changeCameraButton != null && changeCameraButton.hasBeenClicked;
        }


        public override bool ChangeVehicle()
        {
            return changeVehicleButton != null && changeVehicleButton.hasBeenClicked;
        }


        public override Vector2 CharacterMovement()
        {
            return Vector2.zero;
        }


        public override bool ToggleGUI()
        {
            return false;
        }


        public override Vector2 CameraRotation()
        {
            return Vector2.zero;
        }


        public override Vector2 CameraPanning()
        {
            return Vector2.zero;
        }


        public override bool CameraRotationModifier()
        {
            return false;
        }


        public override bool CameraPanningModifier()
        {
            return false;
        }


        public override float CameraZoom()
        {
            return 0;
        }
    }
}