#if UNITY_EDITOR
using NWH.Common.AssetInfo;
using NWH.Common.WelcomeMessage;
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    [CustomEditor(typeof(AdvancedShipController))]
    [CanEditMultipleObjects]
    public class AdvancedShipControllerEditor : DWP_NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            // Draw logo texture
            Rect logoRect = drawer.positionRect;
            logoRect.height = 60f;
            drawer.DrawEditorTexture(logoRect, "DWP2/Logos/AdvancedShipControllerLogo");
            drawer.AdvancePosition(logoRect.height);

            // Draw menu
            int categoryTab = drawer.HorizontalToolbar("categoryTab",
                                                       new[] {"Input", "Engines", "Rudders", "Thrusters", "Settings", "About"});

            switch (categoryTab)
            {
                case 0:
                    DrawInput();
                    break;
                case 1:
                    DrawEngines();
                    break;
                case 2:
                    DrawRudders();
                    break;
                case 3:
                    DrawThrusters();
                    break;
                case 4:
                    DrawSettings();
                    break;
                case 5:
                    DrawAboutTab();
                    break;
                default:
                    DrawInput();
                    break;
            }

            drawer.EndEditor(this);
            return true;
        }


        private void DrawInput()
        {
            drawer.Property("input");
        }


        private void DrawAboutTab()
        {
            AssetInfo assetInfo = Resources.Load("DWP2/Dynamic Water Physics 2 AssetInfo") as AssetInfo;
            if (assetInfo == null)
            {
                return;
            }
        
            GUILayout.Space(drawer.positionRect.y - 20f);
            WelcomeMessageWindow.DrawWelcomeMessage(assetInfo, drawer.positionRect.width - 20f);
        }


        private void DrawEngines()
        {
            drawer.ReorderableList("engines");
        }


        private void DrawThrusters()
        {
            drawer.ReorderableList("thrusters");
        }


        private void DrawRudders()
        {
            drawer.ReorderableList("rudders");
        }


        private void DrawSettings()
        {
            drawer.BeginSubsection("Settings");
            drawer.Field("dropAnchorWhenInactive");
            drawer.Field("weighAnchorWhenActive");
            drawer.EndSubsection();

            drawer.BeginSubsection("Stabilization");
            if (drawer.Field("stabilizeRoll").boolValue)
            {
                drawer.Field("rollStabilizationMaxTorque");
            }

            if (drawer.Field("stabilizePitch").boolValue)
            {
                drawer.Field("pitchStabilizationMaxTorque");
            }

            drawer.EndSubsection();
        }


        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}

#endif
