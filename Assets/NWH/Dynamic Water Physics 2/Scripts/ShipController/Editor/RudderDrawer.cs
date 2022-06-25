#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    [CustomPropertyDrawer(typeof(Rudder))]
    public class RudderDrawer : DWP_NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("name");
            drawer.Field("rudderTransform");
            drawer.Field("maxAngle");
            drawer.Field("rotationSpeed");
            drawer.Field("localRotationAxis");

            drawer.EndSubsection();

            drawer.EndProperty();
            return true;
        }
    }
}

#endif
