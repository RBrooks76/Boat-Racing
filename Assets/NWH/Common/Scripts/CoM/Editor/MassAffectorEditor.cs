#if UNITY_EDITOR
using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.Common.CoM
{
    [CustomEditor(typeof(MassAffector))]
    [CanEditMultipleObjects]
    public class MassAffectorEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI())
            {
                return false;
            }

            drawer.Field("mass", true, "kg");

            drawer.EndEditor(this);
            return true;
        }
    }
}

#endif
