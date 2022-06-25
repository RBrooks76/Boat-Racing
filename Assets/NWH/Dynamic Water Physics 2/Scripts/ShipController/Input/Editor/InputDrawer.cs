#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.ShipController;
using UnityEditor;
using UnityEngine;

namespace NWH.DWP2.WaterObjects
{
    /// <summary>
    ///     Property drawer for Input.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShipInputHandler))]
    public class InputDrawer : DWP_NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label))
            {
                return false;
            }

            drawer.Field("autoSetInput");
            drawer.Field("states");

            drawer.EndProperty();
            return true;
        }
    }
}

#endif
