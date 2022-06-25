#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    /// <summary>
    ///     Property drawer for InputStates.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShipInputStates))]
    public class InputStatesDrawer : DWP_NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("steering");
            drawer.Field("throttle");
            drawer.Field("throttle2");
            drawer.Field("throttle3");
            drawer.Field("throttle4");
            drawer.Field("bowThruster");
            drawer.Field("sternThruster");
            drawer.Field("submarineDepth");
            drawer.Field("engineStartStop");
            drawer.Field("anchor");
            EditorGUI.EndDisabledGroup();

            drawer.EndProperty();
            return true;
        }
    }
}

#endif
