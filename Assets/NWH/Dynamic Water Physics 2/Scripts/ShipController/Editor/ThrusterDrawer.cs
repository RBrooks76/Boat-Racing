#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    [CustomPropertyDrawer(typeof(Thruster))]
    public class ThrusterDrawer : DWP_NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("name");
            drawer.Field("position");
            drawer.Field("maxThrust");
            drawer.Field("spinUpSpeed");
            drawer.Field("thrusterPosition");

            drawer.BeginSubsection("Propeller");
            drawer.Field("propellerTransform");
            drawer.Field("propellerRotationDirection");
            drawer.Field("propellerRotationSpeed");
            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}

#endif
