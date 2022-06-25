#if UNITY_EDITOR
using NWH.DWP2.NUI;
using NWH.DWP2.WaterObjects;
using UnityEditor;

namespace NWH.DWP2
{
    [CustomEditor(typeof(MassFromChildren))]
    [CanEditMultipleObjects]
    public class MassFromChildrenEditor : DWP_NUIEditor
    {
        private MassFromChildren _massFromChildren;


        public override bool OnInspectorNUI()
        {
            _massFromChildren = (MassFromChildren) target;

            if (!base.OnInspectorNUI() || _massFromChildren == null)
            {
                return false;
            }

            drawer.Info("Sums mass of all 'MassFromVolume's attached to this and child objects.");

            if (drawer.Button("Calculate Mass From Children"))
            {
                _massFromChildren.Calculate();
            }

            drawer.EndEditor(this);
            return true;
        }
    }
}

#endif
